Imports System
Imports System.Text
Imports System.Diagnostics
Imports System.Windows.Forms
Imports System.Runtime.InteropServices


Public Class Memory
    Public Class ManageMemory
        Public Shared m_Process As Process
        Public Shared m_pProcessHandle As IntPtr
        Public Shared m_iNumberOfBytesRead As Integer = 0
        Public Shared m_iNumberOfBytesWritten As Integer = 0

        Public Shared Sub Initialize(ByVal ProcessName As String)
            If Process.GetProcessesByName(ProcessName).Length > 0 Then
                m_Process = Process.GetProcessesByName(ProcessName)(0)
            Else
                MessageBox.Show("Couldn't find Counter-Strike. Please start it first!", "Process not found!", MessageBoxButtons.OK)
                Environment.[Exit](1)
            End If

            m_pProcessHandle = OpenProcess(PROCESS_VM_OPERATION Or PROCESS_VM_READ Or PROCESS_VM_WRITE, False, m_Process.Id)
        End Sub

        Public Shared Function GetModuleAdress(ByVal ModuleName As String) As Integer
            Try

                For Each ProcMod As ProcessModule In m_Process.Modules
                    If Not ModuleName.Contains(".dll") Then ModuleName = ModuleName.Insert(ModuleName.Length, ".dll")

                    If ModuleName = ProcMod.ModuleName Then
                        Return CInt(ProcMod.BaseAddress)
                    End If
                Next

            Catch
            End Try

            Return -1
        End Function

        Public Shared Function ReadMemory(Of T As Structure)(ByVal Adress As Integer) As T
            Dim ByteSize As Integer = Marshal.SizeOf(GetType(T))
            Dim buffer As Byte() = New Byte(ByteSize - 1) {}
            ReadProcessMemory(CInt(m_pProcessHandle), Adress, buffer, buffer.Length, m_iNumberOfBytesRead)
            Return ByteArrayToStructure(Of T)(buffer)
        End Function

        Public Shared Function ReadMatrix(Of T As Structure)(ByVal Adress As Integer, ByVal MatrixSize As Integer) As Single()
            Dim ByteSize As Integer = Marshal.SizeOf(GetType(T))
            Dim buffer As Byte() = New Byte(ByteSize * MatrixSize - 1) {}
            ReadProcessMemory(CInt(m_pProcessHandle), Adress, buffer, buffer.Length, m_iNumberOfBytesRead)
            Return ConvertToFloatArray(buffer)
        End Function

        Public Shared Sub WriteMemory(Of T)(ByVal Adress As Integer, ByVal Value As Object)
            Dim buffer As Byte() = StructureToByteArray(Value)
            WriteProcessMemory(CInt(m_pProcessHandle), Adress, buffer, buffer.Length, m_iNumberOfBytesWritten)
        End Sub

        Public Shared Sub WriteMemory(Of T)(ByVal Adress As Integer, ByVal Value As Char())
            Dim buffer As Byte() = Encoding.UTF8.GetBytes(Value)
            WriteProcessMemory(CInt(m_pProcessHandle), Adress, buffer, buffer.Length, m_iNumberOfBytesWritten)
        End Sub

        Public Shared Function ConvertToFloatArray(ByVal bytes As Byte()) As Single()
            If bytes.Length Mod 4 <> 0 Then Throw New ArgumentException()
            Dim floats As Single() = New Single(bytes.Length / 4 - 1) {}

            For i As Integer = 0 To floats.Length - 1
                floats(i) = BitConverter.ToSingle(bytes, i * 4)
            Next

            Return floats
        End Function

        Private Shared Function ByteArrayToStructure(Of T As Structure)(ByVal bytes As Byte()) As T
            Dim handle = GCHandle.Alloc(bytes, GCHandleType.Pinned)

            Try
                Return CType(Marshal.PtrToStructure(handle.AddrOfPinnedObject(), GetType(T)), T)
            Finally
                handle.Free()
            End Try
        End Function

        Private Shared Function StructureToByteArray(ByVal obj As Object) As Byte()
            Dim len As Integer = Marshal.SizeOf(obj)
            Dim arr As Byte() = New Byte(len - 1) {}
            Dim ptr As IntPtr = Marshal.AllocHGlobal(len)
            Marshal.StructureToPtr(obj, ptr, True)
            Marshal.Copy(ptr, arr, 0, len)
            Marshal.FreeHGlobal(ptr)
            Return arr
        End Function

        <DllImport("kernel32.dll")>
        Private Shared Function OpenProcess(ByVal dwDesiredAccess As Integer, ByVal bInheritHandle As Boolean, ByVal dwProcessId As Integer) As IntPtr

        End Function
        <DllImport("kernel32.dll")>
        Private Shared Function ReadProcessMemory(ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByVal buffer As Byte(), ByVal size As Integer, ByRef lpNumberOfBytesRead As Integer) As Boolean

        End Function
        <DllImport("kernel32.dll")>
        Private Shared Function WriteProcessMemory(ByVal hProcess As Integer, ByVal lpBaseAddress As Integer, ByVal buffer As Byte(), ByVal size As Integer, <Out> ByRef lpNumberOfBytesWritten As Integer) As Boolean

        End Function

        Const PROCESS_VM_OPERATION As Integer = &H8
        Const PROCESS_VM_READ As Integer = &H10
        Const PROCESS_VM_WRITE As Integer = &H20

    End Class
End Class
