Namespace WinformsExample
    Partial Class CaptureForm
        Private components As System.ComponentModel.IContainer = Nothing

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If

            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.toolStrip1 = New System.Windows.Forms.ToolStrip()
            Me.startStopToolStripButton = New System.Windows.Forms.ToolStripButton()
            Me.statusStrip1 = New System.Windows.Forms.StatusStrip()
            Me.captureStatisticsToolStripStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
            Me.splitContainer1 = New System.Windows.Forms.SplitContainer()
            Me.dataGridView = New System.Windows.Forms.DataGridView()
            Me.packetInfoTextbox = New System.Windows.Forms.RichTextBox()
            Me.toolStrip1.SuspendLayout()
            Me.statusStrip1.SuspendLayout()
            Me.splitContainer1.Panel1.SuspendLayout()
            Me.splitContainer1.Panel2.SuspendLayout()
            Me.splitContainer1.SuspendLayout()
            (CType((Me.dataGridView), System.ComponentModel.ISupportInitialize)).BeginInit()
            Me.SuspendLayout()
            Me.toolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.startStopToolStripButton})
            Me.toolStrip1.Location = New System.Drawing.Point(0, 0)
            Me.toolStrip1.Name = "toolStrip1"
            Me.toolStrip1.Size = New System.Drawing.Size(741, 25)
            Me.toolStrip1.TabIndex = 1
            Me.toolStrip1.Text = "toolStrip1"
            Me.startStopToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.startStopToolStripButton.Image = [global].WinformsExample.Properties.Resources.stop_icon_disabled
            Me.startStopToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.startStopToolStripButton.Name = "startStopToolStripButton"
            Me.startStopToolStripButton.Size = New System.Drawing.Size(23, 22)
            AddHandler Me.startStopToolStripButton.Click, New System.EventHandler(Me.toolStripButton1_Click)
            Me.statusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.captureStatisticsToolStripStatusLabel})
            Me.statusStrip1.Location = New System.Drawing.Point(0, 498)
            Me.statusStrip1.Name = "statusStrip1"
            Me.statusStrip1.Size = New System.Drawing.Size(741, 22)
            Me.statusStrip1.TabIndex = 2
            Me.statusStrip1.Text = "statusStrip1"
            Me.captureStatisticsToolStripStatusLabel.Name = "captureStatisticsToolStripStatusLabel"
            Me.captureStatisticsToolStripStatusLabel.Size = New System.Drawing.Size(0, 17)
            Me.splitContainer1.Location = New System.Drawing.Point(12, 28)
            Me.splitContainer1.Name = "splitContainer1"
            Me.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
            Me.splitContainer1.Panel1.Controls.Add(Me.dataGridView)
            Me.splitContainer1.Panel1.Paint += New System.Windows.Forms.PaintEventHandler(Me.splitContainer1_Panel1_Paint)
            Me.splitContainer1.Panel2.Controls.Add(Me.packetInfoTextbox)
            Me.splitContainer1.Size = New System.Drawing.Size(717, 467)
            Me.splitContainer1.SplitterDistance = 262
            Me.splitContainer1.TabIndex = 3
            Me.dataGridView.AllowUserToAddRows = False
            Me.dataGridView.AllowUserToDeleteRows = False
            Me.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dataGridView.Location = New System.Drawing.Point(0, 0)
            Me.dataGridView.Name = "dataGridView"
            Me.dataGridView.[ReadOnly] = True
            Me.dataGridView.Size = New System.Drawing.Size(717, 262)
            Me.dataGridView.TabIndex = 0
            AddHandler Me.dataGridView.SelectionChanged, New System.EventHandler(Me.dataGridView_SelectionChanged)
            Me.packetInfoTextbox.Dock = System.Windows.Forms.DockStyle.Fill
            Me.packetInfoTextbox.Location = New System.Drawing.Point(0, 0)
            Me.packetInfoTextbox.Name = "packetInfoTextbox"
            Me.packetInfoTextbox.Size = New System.Drawing.Size(717, 201)
            Me.packetInfoTextbox.TabIndex = 1
            Me.packetInfoTextbox.Text = ""
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(741, 520)
            Me.Controls.Add(Me.splitContainer1)
            Me.Controls.Add(Me.statusStrip1)
            Me.Controls.Add(Me.toolStrip1)
            Me.Name = "CaptureForm"
            Me.Text = "CaptureForm"
            Me.FormClosing += New System.Windows.Forms.FormClosingEventHandler(Me.CaptureForm_FormClosing)
            AddHandler Me.Load, New System.EventHandler(Me.CaptureForm_Load)
            AddHandler Me.Shown, New System.EventHandler(Me.CaptureForm_Shown)
            Me.toolStrip1.ResumeLayout(False)
            Me.toolStrip1.PerformLayout()
            Me.statusStrip1.ResumeLayout(False)
            Me.statusStrip1.PerformLayout()
            Me.splitContainer1.Panel1.ResumeLayout(False)
            Me.splitContainer1.Panel2.ResumeLayout(False)
            Me.splitContainer1.ResumeLayout(False)
            (CType((Me.dataGridView), System.ComponentModel.ISupportInitialize)).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Private toolStrip1 As System.Windows.Forms.ToolStrip
        Private startStopToolStripButton As System.Windows.Forms.ToolStripButton
        Private statusStrip1 As System.Windows.Forms.StatusStrip
        Private captureStatisticsToolStripStatusLabel As System.Windows.Forms.ToolStripStatusLabel
        Private splitContainer1 As System.Windows.Forms.SplitContainer
        Private dataGridView As System.Windows.Forms.DataGridView
        Private packetInfoTextbox As System.Windows.Forms.RichTextBox
    End Class
End Namespace
