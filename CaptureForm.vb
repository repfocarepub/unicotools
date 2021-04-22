Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Forms
Imports SharpPcap
Imports PacketDotNet

Namespace WinformsExample
    Public Partial Class CaptureForm
        Inherits Form

        Private BackgroundThreadStop As Boolean
        Private QueueLock As Object = New Object()
        Private PacketQueue As List(Of RawCapture) = New List(Of RawCapture)()
        Private LastStatisticsOutput As DateTime
        Private LastStatisticsInterval As TimeSpan = New TimeSpan(0, 0, 2)
        Private backgroundThread As System.Threading.Thread
        Private deviceListForm As DeviceListForm
        Private device As ICaptureDevice

        Public Sub New()
            InitializeComponent()
            AddHandler Application.ApplicationExit, New EventHandler(AddressOf Application_ApplicationExit)
        End Sub

        Private Sub Application_ApplicationExit(ByVal sender As Object, ByVal e As EventArgs)
        End Sub

        Private Sub CaptureForm_Load(ByVal sender As Object, ByVal e As EventArgs)
            deviceListForm = New DeviceListForm()
            deviceListForm.OnItemSelected += New DeviceListForm.OnItemSelectedDelegate(AddressOf deviceListForm_OnItemSelected)
            deviceListForm.OnCancel += New DeviceListForm.OnCancelDelegate(AddressOf deviceListForm_OnCancel)
        End Sub

        Private Sub deviceListForm_OnItemSelected(ByVal itemIndex As Integer)
            deviceListForm.Hide()
            StartCapture(itemIndex)
        End Sub

        Private Sub deviceListForm_OnCancel()
            Application.[Exit]()
        End Sub

        Public Class PacketWrapper
            Public p As RawCapture
            Public Property Count As Integer

            Public ReadOnly Property Timeval As PosixTimeval
                Get
                    Return p.Timeval
                End Get
            End Property

            Public ReadOnly Property LinkLayerType As LinkLayers
                Get
                    Return p.LinkLayerType
                End Get
            End Property

            Public ReadOnly Property Length As Integer
                Get
                    Return p.Data.Length
                End Get
            End Property

            Public Sub New(ByVal count As Integer, ByVal p As RawCapture)
                Me.Count = count
                Me.p = p
            End Sub
        End Class

        Private arrivalEventHandler As PacketArrivalEventHandler
        Private captureStoppedEventHandler As CaptureStoppedEventHandler

        Private Sub Shutdown()
            If device IsNot Nothing Then
                device.StopCapture()
                device.Close()
                device.OnPacketArrival -= arrivalEventHandler
                device.OnCaptureStopped -= captureStoppedEventHandler
                device = Nothing
                BackgroundThreadStop = True
                backgroundThread.Join()
                startStopToolStripButton.Image = [global].WinformsExample.Properties.Resources.play_icon_enabled
                startStopToolStripButton.ToolTipText = "Select device to capture from"
            End If
        End Sub

        Private Sub StartCapture(ByVal itemIndex As Integer)
            packetCount = 0
            device = CaptureDeviceList.Instance(itemIndex)
            packetStrings = New Queue(Of PacketWrapper)()
            bs = New BindingSource()
            dataGridView.DataSource = bs
            LastStatisticsOutput = DateTime.Now
            BackgroundThreadStop = False
            backgroundThread = New System.Threading.Thread(AddressOf BackgroundThread)
            backgroundThread.Start()
            arrivalEventHandler = New PacketArrivalEventHandler(AddressOf device_OnPacketArrival)
            device.OnPacketArrival += arrivalEventHandler
            captureStoppedEventHandler = New CaptureStoppedEventHandler(AddressOf device_OnCaptureStopped)
            device.OnCaptureStopped += captureStoppedEventHandler
            device.Open()
            captureStatistics = device.Statistics
            UpdateCaptureStatistics()
            device.StartCapture()
            startStopToolStripButton.Image = [global].WinformsExample.Properties.Resources.stop_icon_enabled
            startStopToolStripButton.ToolTipText = "Stop capture"
        End Sub

        Private Sub device_OnCaptureStopped(ByVal sender As Object, ByVal status As CaptureStoppedEventStatus)
            If status <> CaptureStoppedEventStatus.CompletedWithoutError Then
                MessageBox.Show("Error stopping capture", "Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            End If
        End Sub

        Private packetStrings As Queue(Of PacketWrapper)
        Private packetCount As Integer
        Private bs As BindingSource
        Private captureStatistics As ICaptureStatistics
        Private statisticsUiNeedsUpdate As Boolean = False

        Private Sub device_OnPacketArrival(ByVal sender As Object, ByVal e As CaptureEventArgs)
            Dim Now = DateTime.Now
            Dim interval = Now - LastStatisticsOutput

            If interval > LastStatisticsInterval Then
                Console.WriteLine("device_OnPacketArrival: " & e.Device.Statistics)
                captureStatistics = e.Device.Statistics
                statisticsUiNeedsUpdate = True
                LastStatisticsOutput = Now
            End If

            SyncLock QueueLock
                PacketQueue.Add(e.Packet)
            End SyncLock
        End Sub

        Private Sub CaptureForm_Shown(ByVal sender As Object, ByVal e As EventArgs)
            deviceListForm.Show()
        End Sub

        Private Sub toolStripButton1_Click(ByVal sender As Object, ByVal e As EventArgs)
            If device Is Nothing Then
                deviceListForm.Show()
            Else
                Shutdown()
            End If
        End Sub

        Private Sub BackgroundThread()
            While Not BackgroundThreadStop
                Dim shouldSleep As Boolean = True

                SyncLock QueueLock

                    If PacketQueue.Count <> 0 Then
                        shouldSleep = False
                    End If
                End SyncLock

                If shouldSleep Then
                    System.Threading.Thread.Sleep(250)
                Else
                    Dim ourQueue As List(Of RawCapture)

                    SyncLock QueueLock
                        ourQueue = PacketQueue
                        PacketQueue = New List(Of RawCapture)()
                    End SyncLock

                    Console.WriteLine("BackgroundThread: ourQueue.Count is {0}", ourQueue.Count)

                    For Each packet In ourQueue
                        Dim packetWrapper = New PacketWrapper(packetCount, packet)
                        Me.BeginInvoke(New MethodInvoker(Function()
                                                             packetStrings.Enqueue(packetWrapper)
                                                         End Function))
                        packetCount += 1
                        Dim time = packet.Timeval.Date
                        Dim len = packet.Data.Length
                        Console.WriteLine("BackgroundThread: {0}:{1}:{2},{3} Len={4}", time.Hour, time.Minute, time.Second, time.Millisecond, len)
                    Next

                    Me.BeginInvoke(New MethodInvoker(Function()
                                                         bs.DataSource = packetStrings.Reverse()
                                                     End Function))

                    If statisticsUiNeedsUpdate Then
                        UpdateCaptureStatistics()
                        statisticsUiNeedsUpdate = False
                    End If
                End If
            End While
        End Sub

        Private Sub UpdateCaptureStatistics()
            captureStatisticsToolStripStatusLabel.Text = String.Format("Received packets: {0}, Dropped packets: {1}, Interface dropped packets: {2}", captureStatistics.ReceivedPackets, captureStatistics.DroppedPackets, captureStatistics.InterfaceDroppedPackets)
        End Sub

        Private Sub CaptureForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs)
            Shutdown()
        End Sub

        Private Sub splitContainer1_Panel1_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)
        End Sub

        Private Sub dataGridView_SelectionChanged(ByVal sender As Object, ByVal e As EventArgs)
            If dataGridView.SelectedCells.Count = 0 Then Return
            Dim packetWrapper = CType(dataGridView.Rows(dataGridView.SelectedCells(0).RowIndex).DataBoundItem, PacketWrapper)
            Dim packet = Packet.ParsePacket(packetWrapper.p.LinkLayerType, packetWrapper.p.Data)
            packetInfoTextbox.Text = packet.ToString(StringOutputType.VerboseColored)
        End Sub
    End Class
End Namespace
