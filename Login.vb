Public Class Login
    Private adminUsername As String = "admin"
    Private adminPassword As String = "admin123"

    Private Sub lblExit_Click(sender As Object, e As EventArgs) Handles lblExit.Click
        Application.Exit()
    End Sub

    Private Sub PictureBox7_Click(sender As Object, e As EventArgs) Handles PictureBox7.Click
        If txtUser.Text = adminUsername AndAlso txtPass.Text = adminPassword Then
            Form1.Show()
            Me.Hide()
        Else
            MsgBox("Invalid username or password. Please try again.", vbOKOnly + vbExclamation, "Login Failed")
            txtPass.Clear()
        End If
    End Sub
End Class