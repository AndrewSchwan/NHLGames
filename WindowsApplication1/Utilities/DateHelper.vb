Public Class DateHelper

    Public Shared Function GetPacificTime() As DateTime
        Return TimeZoneInfo.ConvertTime(DateTime.Now(), TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
    End Function

    Public Shared Function GetPacificTime(ByVal utcDate As DateTime) As DateTime
        Return TimeZoneInfo.ConvertTime(utcDate, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
    End Function

End Class
