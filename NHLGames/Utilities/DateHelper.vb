﻿Imports System.Globalization

Namespace Utilities
    Public Class DateHelper

        Public Shared Function GetPacificTime() As DateTime
            Return TimeZoneInfo.ConvertTime(DateTime.Now(), TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
        End Function

        Public Shared Function GetPacificTime(ByVal utcDate As DateTime) As DateTime
            Return TimeZoneInfo.ConvertTime(utcDate, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
        End Function

        Public Shared Function GetFormattedDate(dt As Date) As String
            Return String.Format(NHLGamesMetro.RmText.GetString("lblFormatWeekMonthDayYear"), dt.ToString("ddd"), dt.ToString("MMM"), dt.Day.ToString, dt.Year.ToString)
        End Function

        Public Shared Function GetFormattedWeek(number As DayOfWeek) As String
           Return If(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(number).Length >=2,
                    CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(number).Substring(0,2),
                    CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(number).ToString())
        End Function

        Public Shared Function IsGameTimePast(gameDate As DateTime) As Boolean
            Return DateTime.Now() > gameDate.ToLocalTime()
        End Function

    End Class
End Namespace
