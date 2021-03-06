﻿Imports System.IO
Imports System.Net
Imports NHLGames.My.Resources

Namespace Utilities
    Public Class Common

        Public Const UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, Like Gecko) Chrome/48.0.2564.82 Safari/537.36 Edge/14.14316"
        Private Const Http = "http"

        Public Shared Function GetRandomString(ByVal intLength As Integer)
            Const s As String = "abcdefghijklmnopqrstuvwxyz0123456789"
            Dim r As New Random
            Dim sb As New Text.StringBuilder
            For i = 1 To intLength
                Dim idx As Integer = r.Next(0, 35)
                sb.Append(s.Substring(idx, 1))
            Next

            Return sb.ToString()
        End Function

        Public Shared Async Function SendWebRequest(ByVal address As String, Optional httpWebRequest As HttpWebRequest = Nothing) As Task(Of Boolean)
            Try
                Dim myHttpWebRequest As HttpWebRequest
                If httpWebRequest Is Nothing Then
                    myHttpWebRequest = CType(WebRequest.Create(address), HttpWebRequest)
                    myHttpWebRequest.UserAgent = UserAgent
                    myHttpWebRequest.Timeout = 1000
                Else 
                    myHttpWebRequest = httpWebRequest
                End If
                Dim myHttpWebResponse As HttpWebResponse = CType(Await myHttpWebRequest.GetResponseAsync(), HttpWebResponse)
                myHttpWebResponse.Close()

                If myHttpWebResponse.StatusCode = HttpStatusCode.OK Then
                    Return True
                End If
            Catch e As Exception
                Return False
            End Try
            Return False
        End Function

        Public Shared Async Function SendWebRequestForStream(ByVal address As String, ByVal legacyAddress As String, ByVal gameTitle As String, ByVal gameDate As DateTime) As Task(Of String)
            Dim myHttpWebRequest As HttpWebRequest
            Dim resp As StreamReader
            Dim myHttpWebResponse As HttpWebResponse
            Dim gameUrl As String = Await Task.FromResult(Of String)(String.Empty)

            myHttpWebRequest = CType(WebRequest.Create(address), HttpWebRequest)
            myHttpWebRequest.UserAgent = UserAgent
            myHttpWebRequest.Timeout = 2000
            Try
                myHttpWebResponse = CType(myHttpWebRequest.GetResponse(), HttpWebResponse)
                If myHttpWebResponse.StatusCode = Httpstatuscode.OK Then
                    resp = New StreamReader(myHttpWebResponse.GetResponseStream())
                    gameUrl = Await resp.ReadToEndAsync()
                    If Not gameUrl.StartsWith(Http) Then
                        gameUrl = Await Task.FromResult(Of String)(String.Empty)
                    End If
                Else 
                    myHttpWebRequest = CType(WebRequest.Create(legacyAddress), HttpWebRequest)
                    myHttpWebResponse = CType(myHttpWebRequest.GetResponse(), HttpWebResponse)
                    If myHttpWebResponse.StatusCode = Httpstatuscode.OK Then
                        resp = New StreamReader(myHttpWebResponse.GetResponseStream())
                        gameUrl = Await resp.ReadToEndAsync()
                        If Not gameUrl.StartsWith(Http) Then
                            gameUrl = Await Task.FromResult(Of String)(String.Empty)
                        End If
                    Else 
                        If DateHelper.IsGameTimePast(gameDate) Then
                            Console.WriteLine(String.Format(English.errorGettingStream, gameTitle))
                        End If
                    End If
                End If
                myHttpWebResponse.Close()
                Return gameUrl
            Catch
            End Try
            'If first Web request fails with an exception, it will try the second request with the legacy address
            Try
                myHttpWebRequest = CType(WebRequest.Create(legacyAddress), HttpWebRequest)
                myHttpWebResponse = CType(myHttpWebRequest.GetResponse(), HttpWebResponse)
                If myHttpWebResponse.StatusCode = Httpstatuscode.OK Then
                    resp = New StreamReader(myHttpWebResponse.GetResponseStream())
                    gameUrl = Await resp.ReadToEndAsync()
                    If Not gameUrl.StartsWith(Http) Then
                        gameUrl = Await Task.FromResult(Of String)(String.Empty)
                    End If
                Else 
                    If DateHelper.IsGameTimePast(gameDate) Then
                        Console.WriteLine(String.Format(English.errorGettingStream, gameTitle))
                    End If
                End If
                myHttpWebResponse.Close()
                Return gameUrl
            Catch ex As Exception
                If DateHelper.IsGameTimePast(gameDate) Then
                    Console.WriteLine(String.Format(English.errorGettingStreamWithEx, gameTitle, ex.Message))
                End If
            End Try

            Return gameUrl
        End Function

        Public Shared Sub GetLanguage()
            Dim lang = ApplicationSettings.Read(Of String)(SettingsEnum.SelectedLanguage, String.Empty)
            If String.IsNullOrEmpty(lang) OrElse lang = NHLGamesMetro.RmText.GetString("lblEnglish") Then
                NHLGamesMetro.RmText = English.ResourceManager
            ElseIf lang = NHLGamesMetro.RmText.GetString("lblFrench") Then
                NHLGamesMetro.RmText = French.ResourceManager
            End If
        End Sub

        Public Shared Async Sub CheckAppCanRun()
            If Not File.Exists("NHLGames.exe.Config") then
                FatalError(NHLGamesMetro.RmText.GetString("noConfigFile"))
            Else If Not (Await SendWebRequest("http://www.google.com")) Then
                FatalError(NHLGamesMetro.RmText.GetString("noWebAccess"))
            End If
        End Sub

        Private Shared Sub FatalError(message As String)
            If InvokeElement.MsgBoxRed(message, NHLGamesMetro.RmText.GetString("msgFailure"), MessageBoxButtons.OK) = DialogResult.OK Then
                NHLGamesMetro.FormInstance.Close
            End If
        End Sub

    End Class
End Namespace
