<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Login
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lblExit = New System.Windows.Forms.Label()
        Me.picPaw = New System.Windows.Forms.PictureBox()
        Me.lblSystem = New System.Windows.Forms.Label()
        Me.lblNomVC = New System.Windows.Forms.Label()
        Me.txtPass = New System.Windows.Forms.TextBox()
        Me.txtUser = New System.Windows.Forms.TextBox()
        Me.picRoundedBarOverlay4 = New System.Windows.Forms.PictureBox()
        Me.picRoundedBarOverlay3 = New System.Windows.Forms.PictureBox()
        Me.PictureBox7 = New System.Windows.Forms.PictureBox()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.lblPass = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        CType(Me.picPaw, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picRoundedBarOverlay4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picRoundedBarOverlay3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox7, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.Transparent
        Me.Panel1.BackgroundImage = Global.WMA3ANOMVC.My.Resources.Resources.Rectangle_4
        Me.Panel1.Controls.Add(Me.lblExit)
        Me.Panel1.Controls.Add(Me.picPaw)
        Me.Panel1.Controls.Add(Me.lblSystem)
        Me.Panel1.Controls.Add(Me.lblNomVC)
        Me.Panel1.Location = New System.Drawing.Point(-2, -6)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(561, 146)
        Me.Panel1.TabIndex = 0
        '
        'lblExit
        '
        Me.lblExit.AutoSize = True
        Me.lblExit.Font = New System.Drawing.Font("Segoe UI Variable Text", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblExit.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.lblExit.Location = New System.Drawing.Point(496, 13)
        Me.lblExit.Name = "lblExit"
        Me.lblExit.Size = New System.Drawing.Size(45, 22)
        Me.lblExit.TabIndex = 7
        Me.lblExit.Text = "EXIT"
        '
        'picPaw
        '
        Me.picPaw.Image = Global.WMA3ANOMVC.My.Resources.Resources.paw
        Me.picPaw.Location = New System.Drawing.Point(-38, 22)
        Me.picPaw.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.picPaw.Name = "picPaw"
        Me.picPaw.Size = New System.Drawing.Size(124, 95)
        Me.picPaw.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picPaw.TabIndex = 6
        Me.picPaw.TabStop = False
        '
        'lblSystem
        '
        Me.lblSystem.AutoSize = True
        Me.lblSystem.Font = New System.Drawing.Font("Segoe UI Variable Small", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSystem.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.lblSystem.Location = New System.Drawing.Point(140, 93)
        Me.lblSystem.Name = "lblSystem"
        Me.lblSystem.Size = New System.Drawing.Size(302, 22)
        Me.lblSystem.TabIndex = 5
        Me.lblSystem.Text = "Information System and Appointment"
        '
        'lblNomVC
        '
        Me.lblNomVC.AutoSize = True
        Me.lblNomVC.Font = New System.Drawing.Font("Segoe UI Variable Display", 22.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNomVC.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.lblNomVC.Location = New System.Drawing.Point(100, 46)
        Me.lblNomVC.Name = "lblNomVC"
        Me.lblNomVC.Size = New System.Drawing.Size(405, 49)
        Me.lblNomVC.TabIndex = 4
        Me.lblNomVC.Text = "Nom Veterinary Clinic"
        '
        'txtPass
        '
        Me.txtPass.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtPass.Font = New System.Drawing.Font("Segoe UI Variable Small", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtPass.Location = New System.Drawing.Point(92, 289)
        Me.txtPass.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.txtPass.Multiline = True
        Me.txtPass.Name = "txtPass"
        Me.txtPass.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPass.Size = New System.Drawing.Size(364, 23)
        Me.txtPass.TabIndex = 21
        '
        'txtUser
        '
        Me.txtUser.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtUser.Font = New System.Drawing.Font("Segoe UI Variable Small", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtUser.Location = New System.Drawing.Point(92, 215)
        Me.txtUser.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.txtUser.Multiline = True
        Me.txtUser.Name = "txtUser"
        Me.txtUser.Size = New System.Drawing.Size(364, 24)
        Me.txtUser.TabIndex = 20
        '
        'picRoundedBarOverlay4
        '
        Me.picRoundedBarOverlay4.Image = Global.WMA3ANOMVC.My.Resources.Resources.Rectangle_2__7_
        Me.picRoundedBarOverlay4.Location = New System.Drawing.Point(76, 279)
        Me.picRoundedBarOverlay4.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.picRoundedBarOverlay4.Name = "picRoundedBarOverlay4"
        Me.picRoundedBarOverlay4.Size = New System.Drawing.Size(408, 40)
        Me.picRoundedBarOverlay4.TabIndex = 19
        Me.picRoundedBarOverlay4.TabStop = False
        '
        'picRoundedBarOverlay3
        '
        Me.picRoundedBarOverlay3.Image = Global.WMA3ANOMVC.My.Resources.Resources.Rectangle_2__7_
        Me.picRoundedBarOverlay3.Location = New System.Drawing.Point(78, 206)
        Me.picRoundedBarOverlay3.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.picRoundedBarOverlay3.Name = "picRoundedBarOverlay3"
        Me.picRoundedBarOverlay3.Size = New System.Drawing.Size(408, 40)
        Me.picRoundedBarOverlay3.TabIndex = 18
        Me.picRoundedBarOverlay3.TabStop = False
        '
        'PictureBox7
        '
        Me.PictureBox7.Image = Global.WMA3ANOMVC.My.Resources.Resources.Group_1__6_
        Me.PictureBox7.Location = New System.Drawing.Point(162, 338)
        Me.PictureBox7.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PictureBox7.Name = "PictureBox7"
        Me.PictureBox7.Size = New System.Drawing.Size(226, 55)
        Me.PictureBox7.TabIndex = 22
        Me.PictureBox7.TabStop = False
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("Segoe UI Variable Small", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUser.ForeColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.lblUser.Location = New System.Drawing.Point(75, 185)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(79, 20)
        Me.lblUser.TabIndex = 25
        Me.lblUser.Text = "Username"
        '
        'lblPass
        '
        Me.lblPass.AutoSize = True
        Me.lblPass.Font = New System.Drawing.Font("Segoe UI Variable Small", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPass.ForeColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.lblPass.Location = New System.Drawing.Point(75, 258)
        Me.lblPass.Name = "lblPass"
        Me.lblPass.Size = New System.Drawing.Size(74, 20)
        Me.lblPass.TabIndex = 26
        Me.lblPass.Text = "Password"
        '
        'Login
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(553, 446)
        Me.Controls.Add(Me.lblPass)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.PictureBox7)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.txtPass)
        Me.Controls.Add(Me.picRoundedBarOverlay4)
        Me.Controls.Add(Me.txtUser)
        Me.Controls.Add(Me.picRoundedBarOverlay3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Name = "Login"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Login"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.picPaw, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picRoundedBarOverlay4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picRoundedBarOverlay3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox7, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents picPaw As PictureBox
    Friend WithEvents lblSystem As Label
    Friend WithEvents lblNomVC As Label
    Friend WithEvents txtPass As TextBox
    Friend WithEvents txtUser As TextBox
    Friend WithEvents picRoundedBarOverlay4 As PictureBox
    Friend WithEvents picRoundedBarOverlay3 As PictureBox
    Friend WithEvents PictureBox7 As PictureBox
    Friend WithEvents lblExit As Label
    Friend WithEvents lblUser As Label
    Friend WithEvents lblPass As Label
End Class
