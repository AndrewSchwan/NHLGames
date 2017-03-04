Public Class Common

    Public Shared Function GetRandomString(ByVal intLength As Integer)
        Dim s As String = "abcdefghijklmnopqrstuvwxyz0123456789"
        Dim r As New Random
        Dim sb As New System.Text.StringBuilder
        For i As Integer = 1 To intLength
            Dim idx As Integer = r.Next(0, 35)
            sb.Append(s.Substring(idx, 1))
        Next

        Return sb.ToString()
    End Function

End Class
