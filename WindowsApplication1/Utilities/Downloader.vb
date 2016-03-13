﻿Imports System.Globalization
Imports Newtonsoft.Json.Linq


Public Class Downloader

    Private Shared GamesTxtURL = "http://showtimes.ninja/games.txt"
    Private Shared ScheduleAPIURL = "http://statsapi.web.nhl.com/api/v1/schedule?startDate={0}&endDate={1}&expand=schedule.teams,schedule.game.content.media.epg"
    Private Shared ApplicationVersionURL = "http://showtimes.ninja/version.txt"

    Private Shared ApplicationVersionFileName As String = "version.txt"
    Private Shared GamesTextFileName As String = "games.txt"

    Private Shared LocalFileDirectory = Application.StartupPath

    Private Shared Sub DownloadFile(URL As String, fileName As String, Optional checkIfExists As Boolean = False, Optional overwrite As Boolean = True)
        Dim fullPath As String = LocalFileDirectory & fileName

        If (checkIfExists = False) OrElse (checkIfExists AndAlso My.Computer.FileSystem.FileExists(fullPath) = False) Then
            My.Computer.Network.DownloadFile(URL, LocalFileDirectory & fileName, "", "", False, 10000, overwrite)
        End If

    End Sub

    Private Shared Function ReadFileContents(fileName As String) As String

        Dim returnValue As String = ""
        Dim filePath As String = LocalFileDirectory & fileName

        Using streamReader As IO.StreamReader = New IO.StreamReader(filePath)
            returnValue = streamReader.ReadToEnd()
        End Using

        Return returnValue
    End Function

    Public Shared Function DownloadApplicationVersion() As String

        DownloadFile(ApplicationVersionURL, ApplicationVersionFileName)
        Return ReadFileContents(ApplicationVersionFileName).Trim()

    End Function

    Public Shared Function DownloadAvailableGames() As List(Of String)

        DownloadFile(GamesTxtURL, GamesTextFileName)
        Return ReadFileContents(GamesTextFileName).Split(New Char() {vbLf}).ToList()

    End Function



    Public Shared Function DownloadJSONSchedule(startDate As DateTime) As JObject

        Dim returnValue As New JObject
        Dim dateTimeString As String = startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        Dim fileName As String = dateTimeString & ".json"
        Dim URL As String = String.Format(ScheduleAPIURL, dateTimeString, dateTimeString)

        DownloadFile(URL, fileName, True)

        Dim fileContent As String = ReadFileContents(fileName)
        returnValue = JObject.Parse(fileContent)

        Return returnValue

    End Function

End Class
