﻿Imports System.ComponentModel
Imports System.IO
Imports System.Security.Permissions
Imports System.Threading
Imports System.Resources
Imports MetroFramework.Controls
Imports NHLGames.Controls
Imports NHLGames.My.Resources
Imports NHLGames.Objects
Imports NHLGames.Objects.Modules
Imports NHLGames.Utilities

Public Class NHLGamesMetro

    Public Shared ServerIp As String
    Public Const DomainName As String = "mf.svc.nhl.com"
    Public Shared HostName As String
    Public Shared HostNameResolved As Boolean = False
    Public Shared FormInstance As NHLGamesMetro = Nothing
    Public Shared StreamStarted As Boolean = False
    Public Shared SpnLoadingValue As Integer = 0
    Public Shared SpnLoadingMaxValue As Integer = 1000
    Public Shared SpnLoadingVisible As Boolean = False
    Public Shared SpnStreamingValue As Integer = 0
    Public Shared SpnStreamingMaxValue As Integer = 1000
    Public Shared SpnStreamingVisible As Boolean = False
    Public Shared FlpCalendar As FlowLayoutPanel
    Public Shared GamesDownloadedTime As Date
    Public Shared LabelDate As Label
    Public Shared DownloadLink As String = "https://www.reddit.com/r/nhl_games/"
    Public Shared GameDate As Date = DateHelper.GetPacificTime()
    Private _resizeDirection As Integer = -1
    Private Const ResizeBorderWidth As Integer = 8
    Public Shared RmText As ResourceManager = English.ResourceManager
    Public Shared LstTasks As List(Of Task) = New List(Of Task)()
    Public Shared FormLoaded As Boolean = False
    Private Shared _adDetectionEngine As AdDetection

    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.ControlAppDomain)>
    Public Shared Sub Main()
        AddHandler Application.ThreadException, AddressOf Form1_UIThreadException
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException

        Dim form As New NHLGamesMetro()
        FormInstance = form

        Dim writer = New ConsoleRedirectStreamWriter(form.txtConsole)
        Console.SetOut(writer)
        Application.Run(form)
    End Sub

    Private Shared Sub Form1_UIThreadException(ByVal sender As Object, ByVal t As ThreadExceptionEventArgs)
        Console.WriteLine(English.errorGeneral, $"Running main thread", t.Exception.ToString())
    End Sub

    Private Shared Sub CurrentDomain_UnhandledException(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Console.WriteLine(e.ExceptionObject.ToString())
    End Sub

    Public Sub HandleException(e As Exception)
        Console.WriteLine(e.ToString())
    End Sub

    Private Sub NHLGames_Load(sender As Object, e As EventArgs) Handles Me.Load
        SuspendLayout()
        Common.GetLanguage()
        Common.CheckAppCanRun()

        tabMenu.SelectedIndex = 0
        FlpCalendar = flpCalendarPanel
        InitializeForm.SetSettings()
        InitializeForm.VersionCheck()
        ResumeLayout()
        FormLoaded = True
    End Sub

    Private Shared Sub _writeToConsoleSettingsChanged(key As String, value As String)
        If FormLoaded Then Console.WriteLine(String.Format(English.msgSettingUpdated, key, value))
    End Sub

    Private Shared Sub tmrAnimate_Tick(sender As Object, e As EventArgs) Handles tmr.Tick
        If StreamStarted Then
            GameFetcher.StreamingProgress()
        Else
            GameFetcher.LoadingProgress()
        End If
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        flpCalendarPanel.Visible = False
        InvokeElement.LoadGamesAsync(GameDate, True)
        flpGames.Focus()
    End Sub

    Private Sub RichTextBox_TextChanged(sender As Object, e As EventArgs) Handles txtConsole.TextChanged
        txtConsole.SelectionStart = txtConsole.Text.Length
        txtConsole.ScrollToCaret()
    End Sub

    Private Sub btnVLCPath_Click(sender As Object, e As EventArgs) Handles btnVLCPath.Click 
        ofd.Filter = $"VLC|vlc.exe|All files (*.*)|*.*"
        ofd.Multiselect = False
        ofd.InitialDirectory = If(txtVLCPath.Text.Equals(String.Empty), "C:\", Path.GetDirectoryName(txtVLCPath.Text))

        If ofd.ShowDialog() = DialogResult.OK Then
            If String.IsNullOrEmpty(ofd.FileName) = False And txtVLCPath.Text <> ofd.FileName Then
                ApplicationSettings.SetValue(SettingsEnum.VlcPath, ofd.FileName)
                txtVLCPath.Text = ofd.FileName
            End If
        End If
    End Sub

    Private Sub btnMPCPath_Click(sender As Object, e As EventArgs) Handles btnMPCPath.Click 
        ofd.Filter = $"MPC|mpc-hc64.exe;mpc-hc.exe|All files (*.*)|*.*"
        ofd.Multiselect = False
        ofd.InitialDirectory = If(txtMPCPath.Text.Equals(String.Empty), "C:\", Path.GetDirectoryName(txtMPCPath.Text))

        If ofd.ShowDialog() = DialogResult.OK Then

            If String.IsNullOrEmpty(ofd.FileName) = False And txtMPCPath.Text <> ofd.FileName Then
                ApplicationSettings.SetValue(SettingsEnum.MpcPath, ofd.FileName)
                txtMPCPath.Text = ofd.FileName
            End If

        End If
    End Sub

    Private Sub btnMpvPath_Click(sender As Object, e As EventArgs) Handles btnMpvPath.Click 
        ofd.Filter = $"mpv|mpv.exe|All files (*.*)|*.*"
        ofd.Multiselect = False
        ofd.InitialDirectory = If(txtMpvPath.Text.Equals(String.Empty), "C:\", Path.GetDirectoryName(txtMpvPath.Text))

        If ofd.ShowDialog() = DialogResult.OK Then

            If String.IsNullOrEmpty(ofd.FileName) = False And txtMpvPath.Text <> ofd.FileName Then
                ApplicationSettings.SetValue(SettingsEnum.MpvPath, ofd.FileName)
                txtMpvPath.Text = ofd.FileName
            End If

        End If
    End Sub

    Private Sub btnstreamerPath_Click(sender As Object, e As EventArgs) Handles btnStreamerPath.Click 
        ofd.Filter = $"streamer|streamlink.exe;livestreamer.exe|All files (*.*)|*.*"
        ofd.Multiselect = False
        ofd.InitialDirectory = If(txtStreamerPath.Text.Equals(String.Empty), "C:\", Path.GetDirectoryName(txtStreamerPath.Text))

        If ofd.ShowDialog() = DialogResult.OK Then

            If String.IsNullOrEmpty(ofd.FileName) = False And txtStreamerPath.Text <> ofd.FileName Then
                ApplicationSettings.SetValue(SettingsEnum.StreamerPath, ofd.FileName)
                txtStreamerPath.Text = ofd.FileName
            End If

        End If
    End Sub

    Private Sub tgShowFinalScores_CheckedChanged(sender As Object, e As EventArgs) Handles tgShowFinalScores.CheckedChanged
        ApplicationSettings.SetValue(SettingsEnum.ShowScores, tgShowFinalScores.Checked)
        For each game As GameControl In flpGames.Controls
            game.UpdateGame(GameManager.GamesDict(game.GameId), 
                            tgShowFinalScores.Checked,
                            tgShowLiveScores.Checked,
                            tgShowSeriesRecord.Checked,
                            tgShowTeamCityAbr.Checked)
        Next
    End Sub

    Private Sub btnClearConsole_Click(sender As Object, e As EventArgs) Handles btnClearConsole.Click
        txtConsole.Clear()
    End Sub

    Private Sub txtVLCPath_TextChanged(sender As Object, e As EventArgs) Handles txtVLCPath.TextChanged 
        If Not rbVLC.Enabled Then rbVLC.Enabled = True
        Player.RenewArgs()
    End Sub

    Private Sub txtMPCPath_TextChanged(sender As Object, e As EventArgs) Handles txtMPCPath.TextChanged 
        If Not rbMPC.Enabled Then rbMPC.Enabled = True
        Player.RenewArgs()
    End Sub

    Private Sub txtMpvPath_TextChanged(sender As Object, e As EventArgs) Handles txtMpvPath.TextChanged 
        If Not rbMPV.Enabled Then rbMPV.Enabled = True
        Player.RenewArgs()
    End Sub

    Private Sub txtStreamerPath_TextChanged(sender As Object, e As EventArgs) Handles txtStreamerPath.TextChanged 
        Player.RenewArgs()
    End Sub

    Private Sub player_CheckedChanged(sender As Object, e As EventArgs) Handles rbVLC.CheckedChanged, rbMPV.CheckedChanged, rbMPC.CheckedChanged  
        Dim rb As RadioButton = sender
        If (rb.Checked) Then 
            Player.RenewArgs()
            _writeToConsoleSettingsChanged(lblPlayer.Text, rb.Text)
        End If
    End Sub

    Private Sub tgAlternateCdn_CheckedChanged(sender As Object, e As EventArgs) Handles tgAlternateCdn.CheckedChanged
        Dim cdn = If(tgAlternateCdn.Checked, CdnType.L3C, CdnType.Akc)
        Player.RenewArgs()
        _writeToConsoleSettingsChanged(lblCdn.Text, cdn.ToString())
        If NHLGamesMetro.FormLoaded Then InvokeElement.LoadGamesAsync(GameDate)
    End Sub

    Private Sub txtOutputPath_TextChanged(sender As Object, e As EventArgs) Handles txtOutputArgs.TextChanged 
        Player.RenewArgs()
        _writeToConsoleSettingsChanged(lblOutput.Text, txtOutputArgs.Text)
    End Sub

    Private Sub txtPlayerArgs_TextChanged(sender As Object, e As EventArgs) Handles txtPlayerArgs.TextChanged 
        Player.RenewArgs()
        _writeToConsoleSettingsChanged(lblPlayerArgs.Text, txtPlayerArgs.Text)
    End Sub

    Private Sub txtStreamerArgs_TextChanged(sender As Object, e As EventArgs) Handles txtStreamerArgs.TextChanged 
        Player.RenewArgs()
        _writeToConsoleSettingsChanged(lblStreamerArgs.Text, txtStreamerArgs.Text)
    End Sub

    Private Sub btnOuput_Click(sender As Object, e As EventArgs) Handles btnOuput.Click
        fbd.SelectedPath = If (txtOutputArgs.Text <> String.Empty, 
                                            Path.GetDirectoryName(txtOutputArgs.Text), 
                                            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
                                            )
        fbd.ShowDialog()
        If fbd.SelectedPath <> txtOutputArgs.Text Then
            txtOutputArgs.Text = fbd.SelectedPath & $"\(DATE)_(HOME)_vs_(AWAY)_(TYPE)_(NETWORK).mp4"
            Player.RenewArgs()
        End If
    End Sub

    Private Sub btnYesterday_Click(sender As Object, e As EventArgs) Handles btnYesterday.Click
        GameDate = GameDate.AddDays(-1)
        lblDate.Text = DateHelper.GetFormattedDate(GameDate)
    End Sub

    Private Sub btnTomorrow_Click(sender As Object, e As EventArgs) Handles btnTomorrow.Click
        GameDate = GameDate.AddDays(1)
        lblDate.Text = DateHelper.GetFormattedDate(GameDate)
    End Sub

    Private Sub lnkVLCDownload_Click(sender As Object, e As EventArgs) Handles lnkGetVlc.Click 
        Dim sInfo As ProcessStartInfo = New ProcessStartInfo("http://www.videolan.org/vlc/download-windows.html")
        Process.Start(sInfo)
    End Sub

    Private Sub lnkMPCDownload_Click(sender As Object, e As EventArgs) Handles lnkGetMpc.Click 
        Dim sInfo As ProcessStartInfo = New ProcessStartInfo("https://mpc-hc.org/downloads/")
        Process.Start(sInfo)
    End Sub

    Private Sub btnDate_Click(sender As Object, e As EventArgs) Handles btnDate.Click
        Dim val = Not flpCalendarPanel.Visible
        flpCalendarPanel.Visible = val
    End Sub

    Private Sub lblDate_TextChanged(sender As Object, e As EventArgs) Handles lblDate.TextChanged
        flpCalendarPanel.Visible = False
        InvokeElement.LoadGamesAsync(GameDate)
        flpGames.Focus()
    End Sub

    Private Sub chkShowLiveScores_CheckedChanged(sender As Object, e As EventArgs) Handles tgShowLiveScores.CheckedChanged
        ApplicationSettings.SetValue(SettingsEnum.ShowLiveScores, tgShowLiveScores.Checked)
        For each game As GameControl In flpGames.Controls
            game.UpdateGame(GameManager.GamesDict(game.GameId),
                            tgShowFinalScores.Checked,
                            tgShowLiveScores.Checked,
                            tgShowSeriesRecord.Checked,
                            tgShowTeamCityAbr.Checked)
        Next
    End Sub

    Private Sub lnkDownload_Click(sender As Object, e As EventArgs) Handles lnkDownload.Click
        Dim sInfo As ProcessStartInfo = New ProcessStartInfo(DownloadLink)
        Process.Start(sInfo)
    End Sub

    Private Sub tgStreamer_CheckedChanged(sender As Object, e As EventArgs) Handles tgStreamer.CheckedChanged 
        txtStreamerArgs.Enabled = tgStreamer.Checked
        Player.RenewArgs()
        _writeToConsoleSettingsChanged(String.Format(English.msgThisEnable, lblStreamerArgs.Text), 
                                       if(tgStreamer.Checked, English.msgOn, English.msgOff))
    End Sub

    Private Sub tgPlayer_CheckedChanged(sender As Object, e As EventArgs) Handles tgPlayer.CheckedChanged 
        txtPlayerArgs.Enabled = tgPlayer.Checked
        Player.RenewArgs()
        _writeToConsoleSettingsChanged(String.Format(English.msgThisEnable,lblPlayerArgs.Text), 
                                       if(tgPlayer.Checked, English.msgOn, English.msgOff))
    End Sub

    Private Sub tgOutput_CheckedChanged(sender As Object, e As EventArgs) Handles tgOutput.CheckedChanged 
        txtOutputArgs.Enabled = tgOutput.Checked
        If txtOutputArgs.Text = String.Empty Then
            txtOutputArgs.Text = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)}\(DATE)_(HOME)_vs_(AWAY)_(TYPE)_(NETWORK).mp4"
        End If
        Player.RenewArgs()
        _writeToConsoleSettingsChanged(String.Format(English.msgThisEnable,lblOutput.Text),
                                       if(tgOutput.Checked, English.msgOn, English.msgOff))
    End Sub

    Private Sub chkShowSeriesRecord_CheckedChanged(sender As Object, e As EventArgs) Handles tgShowSeriesRecord.CheckedChanged
        ApplicationSettings.SetValue(SettingsEnum.ShowSeriesRecord, tgShowSeriesRecord.Checked)
        For each game As GameControl In flpGames.Controls
            game.UpdateGame(GameManager.GamesDict(game.GameId),
                            tgShowFinalScores.Checked,
                            tgShowLiveScores.Checked,
                            tgShowSeriesRecord.Checked,
                            tgShowTeamCityAbr.Checked)
        Next
    End Sub

    Private Shared Sub btnHelp_Click(sender As Object, e As EventArgs) Handles btnHelp.Click
        Dim sInfo As ProcessStartInfo = New ProcessStartInfo("https://github.com/NHLGames/NHLGames/wiki")
        Process.Start(sInfo)
    End Sub

    Private Sub btnMaximize_Click(sender As Object, e As EventArgs) Handles btnMaximize.Click
        WindowState = FormWindowState.Maximized
        btnMaximize.Visible = False
        btnNormal.Visible = True
        RefreshFocus()
    End Sub

    Private Sub btnMinimize_Click(sender As Object, e As EventArgs) Handles btnMinimize.Click
        WindowState = FormWindowState.Minimized
        RefreshFocus()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Close()
    End Sub

    Private Sub NHLGamesMetro_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        Refresh()
        RefreshFocus()
    End Sub

    Private Sub btnNormal_Click(sender As Object, e As EventArgs) Handles btnNormal.Click
        WindowState = FormWindowState.Normal
        btnMaximize.Visible = True
        btnNormal.Visible = False
        RefreshFocus()
    End Sub

    Private Sub RefreshFocus()
        If tabGames.Visible Then 
            flpGames.Focus()
        ElseIf tabSettings.Visible Then
            tlpSettings.Focus()
        ElseIf tabConsole.Visible Then
            txtConsole.Focus()
        Else 
            tabMenu.Focus()
        End If
    End Sub

    Private Sub TabControl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tabMenu.SelectedIndexChanged
        RefreshFocus()
    End Sub

    Private Sub NHLGamesMetro_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove
        _resizeDirection = -1
        If e.Location.X < ResizeBorderWidth And e.Location.Y < ResizeBorderWidth Then
            Cursor = Cursors.SizeNWSE
            _resizeDirection = WindowsCode.HTTOPLEFT
        ElseIf e.Location.X < ResizeBorderWidth And e.Location.Y > Height - ResizeBorderWidth Then
            Cursor = Cursors.SizeNESW
            _resizeDirection = WindowsCode.HTBOTTOMLEFT
        ElseIf e.Location.X > Width - ResizeBorderWidth And e.Location.Y > Height - ResizeBorderWidth Then
            Cursor = Cursors.SizeNWSE
            _resizeDirection = WindowsCode.HTBOTTOMRIGHT
        ElseIf e.Location.X > Width - ResizeBorderWidth And e.Location.Y < ResizeBorderWidth Then
            Cursor = Cursors.SizeNESW
            _resizeDirection = WindowsCode.HTTOPRIGHT
        ElseIf e.Location.X < ResizeBorderWidth Then
            Cursor = Cursors.SizeWE
            _resizeDirection = WindowsCode.HTLEFT
        ElseIf e.Location.X > Width - ResizeBorderWidth Then
            Cursor = Cursors.SizeWE
            _resizeDirection = WindowsCode.HTRIGHT
        ElseIf e.Location.Y < ResizeBorderWidth Then
            Cursor = Cursors.SizeNS
            _resizeDirection = WindowsCode.HTTOP
        ElseIf e.Location.Y > Height - ResizeBorderWidth Then
            Cursor = Cursors.SizeNS
            _resizeDirection = WindowsCode.HTBOTTOM
        Else
            Cursor = Cursors.Default
        End If
    End Sub

    Private Sub NHLGamesMetro_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        If e.Button = MouseButtons.Left And WindowState <> FormWindowState.Maximized Then
            ResizeForm()
        End If
    End Sub

    Private Sub ResizeForm()
        If _resizeDirection <> -1 Then
            NativeMethods.ReleaseCaptureOfForm()
            NativeMethods.SendMessageToHandle(Handle, WindowsCode.WM_NCLBUTTONDOWN, _resizeDirection, 0)
        End If
    End Sub

    Private Sub NHLGamesMetro_MouseLeave(sender As Object, e As EventArgs) Handles MyBase.MouseLeave
        Cursor = Cursors.Default
    End Sub

    Private Sub cbServers_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbServers.SelectedIndexChanged 
        tlpSettings.Focus()
        HostName = cbServers.SelectedItem.ToString()
        ServerIp = Net.Dns.GetHostEntry(HostName).AddressList.First.ToString()
        HostNameResolved = HostsFile.TestEntry(DomainName, ServerIp)
        ApplicationSettings.SetValue(SettingsEnum.SelectedServer, cbServers.SelectedItem.ToString())
        If FormLoaded Then InvokeElement.LoadGamesAsync(GameDate)
    End Sub

    Private Sub btnCopyConsole_Click(sender As Object, e As EventArgs) Handles btnCopyConsole.Click
        dim player As String = If (rbMpv.Checked,"MPV",If(rbMPC.Checked,"MPC",If(rbVLC.Checked,"VLC","none")))
        Dim x64 As String = if(Environment.Is64BitOperatingSystem,"64 Bits","32 Bits")
        Dim streamerPath = ApplicationSettings.Read(Of String)(SettingsEnum.StreamerPath, String.Empty).ToString()
        Dim vlcPath = ApplicationSettings.Read(Of String)(SettingsEnum.VlcPath, String.Empty).ToString()
        Dim mpcPath = ApplicationSettings.Read(Of String)(SettingsEnum.MpcPath, String.Empty).ToString()
        Dim mpvPath = ApplicationSettings.Read(Of String)(SettingsEnum.MpvPath, String.Empty).ToString()
        Dim streamerExists = streamerPath <> "" AndAlso File.Exists(streamerPath)
        Dim vlcExists = vlcPath <> "" AndAlso File.Exists(vlcPath)
        Dim mpcExists = mpcPath <> "" AndAlso File.Exists(mpcPath)
        Dim mpvExists = mpvPath <> "" AndAlso File.Exists(mpvPath)
        Dim report = $"NHLGames Bug Report {lblVersion.Text}{vbCrLf}{vbCrLf}" &
            $"Operating system: {My.Computer.Info.OSFullName.ToString()} {x64.ToString()}{vbCrLf}{vbCrLf}" &
            $"Form: {If (Not String.IsNullOrEmpty(lblDate.Text), "loaded", "not loaded")}, " &
            $"{flpGames.Controls.Count} games currently on form, " &
            $"Spinner (games) {If (SpnLoadingVisible, "visible", "invisible")} {SpnLoadingValue.ToString()}/{SpnLoadingMaxValue.ToString()}, " &
            $"Spinner (stream) {If (SpnStreamingVisible, "visible", "invisible")} {SpnStreamingValue.ToString()}/{SpnStreamingMaxValue.ToString()}{vbCrLf}{vbCrLf}" &
            $"Servers: NHLGames IP {If (My.Computer.Network.Ping(ServerIp), "found", "not found")} ({cbServers.SelectedItem.ToString()}), " &
            $"NHL.TV redirection is{If (HostsFile.TestEntry(DomainName,ServerIp), " working", "n't working")} (Hosts file tested){vbCrLf}{vbCrLf}" &
            $"Streams fetcher: {LstTasks.Count} tasks still running{vbCrLf}{vbCrLf}" &
            $"Selected player: {player.ToString()}{vbCrLf}{vbCrLf}" &
            $"Streamer path: {streamerPath.ToString()} [{If (streamerPath.Equals(txtStreamerPath.Text), "on form", "not on form")}] [{If (streamerExists, "exe found", "exe not found")}]{vbCrLf}{vbCrLf}" &
            $"VLC path: {vlcPath.ToString()} [{If (vlcPath.Equals(txtVLCPath.Text), "on form", "not on form")}] [{If (vlcExists, "exe found", "exe not found")}]{vbCrLf}{vbCrLf}" &
            $"MPC path: {mpcPath.ToString()} [{If (mpcPath.Equals(txtMPCPath.Text), "on form", "not on form")}] [{If (mpcExists, "exe found", "exe not found")}]{vbCrLf}{vbCrLf}" &
            $"MPV path: {mpvPath.ToString()} [{If (mpvPath.Equals(txtMpvPath.Text), "on form", "not on form")}] [{If (mpvExists, "exe found", "exe not found")}]{vbCrLf}{vbCrLf}" &
            $"Console log: {vbCrLf}{vbCrLf}{txtConsole.Text.ToString()}"
        Clipboard.SetText(report)
    End Sub

    Private Sub cbLanguage_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbLanguage.SelectedIndexChanged 
        tlpSettings.Focus()
        ApplicationSettings.SetValue(SettingsEnum.SelectedLanguage, cbLanguage.SelectedItem.ToString())
        Common.GetLanguage()
        InitializeForm.SetLanguage()
        For each game As GameControl In flpGames.Controls
            game.UpdateGame(GameManager.GamesDict(game.GameId),
                            tgShowFinalScores.Checked,
                            tgShowLiveScores.Checked,
                            tgShowSeriesRecord.Checked,
                            tgShowTeamCityAbr.Checked)
        Next
    End Sub

    Private Sub NHLGamesMetro_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            Task.WaitAll(LstTasks.ToArray())
            LstTasks.Clear()
        Catch
        End Try
    End Sub

    Private Sub tgModules_Click(sender As Object, e As EventArgs) Handles tgModules.CheckedChanged 
        Dim tg As MetroToggle = sender

        If tg.Checked Then
            _adDetectionEngine = New AdDetection
        Else
            tgSpotify.Checked = False
            tgOBS.Checked = False
        End If

        tgOBS.Enabled = tg.Checked AndAlso txtAdKey.Text <> String.Empty AndAlso txtGameKey.Text <> String.Empty
        tgSpotify.Enabled = tg.Checked
        tlpOBSSettings.Enabled = tg.Checked
        flpSpotifyParameters.Enabled = tg.Checked

        _adDetectionEngine.IsEnabled = tg.checked
        If tg.Checked Then _adDetectionEngine.Start()
        AdDetection.Renew()
        _writeToConsoleSettingsChanged(String.Format(English.msgThisEnable,lblModules.Text), 
                                       if(tgModules.Checked, English.msgOn, English.msgOff))
    End Sub

    Private Sub tgOBS_CheckedChanged(sender As Object, e As EventArgs) Handles tgOBS.CheckedChanged  
        Dim tg As MetroToggle = sender
        Dim obs As New Obs

        tlpOBSSettings.Enabled = Not tg.Checked

        If tg.Checked Then
            obs.HotkeyAd.Key = txtAdKey.Text
            obs.HotkeyAd.Ctrl = chkAdCtrl.Checked
            obs.HotkeyAd.Alt = chkAdAlt.Checked
            obs.HotkeyAd.Shift = chkAdShift.Checked

            obs.HotkeyGame.Key = txtGameKey.Text
            obs.HotkeyGame.Ctrl = chkGameCtrl.Checked
            obs.HotkeyGame.Alt = chkGameAlt.Checked
            obs.HotkeyGame.Shift = chkGameShift.Checked

            _adDetectionEngine.AddModule(obs)
        Else 
            If _adDetectionEngine.IsInAdModulesList(obs.Title) Then
                _adDetectionEngine.RemoveModule(obs.Title)
            End If
        End If

        AdDetection.Renew()
        _writeToConsoleSettingsChanged(String.Format(English.msgThisEnable,lblOBS.Text), 
                                       if(tgOBS.Checked, English.msgOn, English.msgOff))
    End Sub

    Private Sub tgSpotify_CheckedChanged(sender As Object, e As EventArgs) Handles tgSpotify.CheckedChanged  
        Dim tg As MetroToggle = sender
        Dim spotify As New Spotify

        flpSpotifyParameters.Enabled = Not tg.Checked

        If tg.Checked Then
            spotify.ForceToOpen = chkSpotifyForceToStart.Checked
            spotify.PlayNextSong = chkSpotifyPlayNextSong.Checked
            spotify.AnyMediaPlayer = chkSpotifyAnyMediaPlayer.Checked
            _adDetectionEngine.AddModule(spotify)
        Else 
            If _adDetectionEngine.IsInAdModulesList(spotify.Title) Then
                _adDetectionEngine.RemoveModule(spotify.Title)
            End If
        End If

        AdDetection.Renew()
        _writeToConsoleSettingsChanged(String.Format(English.msgThisEnable,lblSpotify.Text), 
                                       if(tgSpotify.Checked, English.msgOn, English.msgOff))
    End Sub

    Private Sub cbHostsFileActions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbHostsFileActions.SelectedIndexChanged
        tlpSettings.Focus()
    End Sub

    Private Sub btnHostsFileActions_Click(sender As Object, e As EventArgs) Handles btnHostsFileActions.Click
        If cbHostsFileActions.SelectedIndex = 0 Then
            If HostsFile.TestEntry(DomainName, ServerIp) Then
                InvokeElement.MsgBoxBlue(RmText.GetString("msgHostsSuccess"), RmText.GetString("msgSuccess"), MessageBoxButtons.OK)
            Else
                InvokeElement.MsgBoxBlue(RmText.GetString("msgHostsFailure"), RmText.GetString("msgFailure"), MessageBoxButtons.OK)
            End If
        ElseIf cbHostsFileActions.SelectedIndex = 1 Then
            HostsFile.AddEntry(ServerIp, DomainName)
        ElseIf cbHostsFileActions.SelectedIndex = 2 Then
            HostsFile.CleanHosts(DomainName)
        ElseIf cbHostsFileActions.SelectedIndex = 3 Then
            HostsFile.OpenHostsFile()
        ElseIf cbHostsFileActions.SelectedIndex = 4 Then
            Clipboard.SetText(ServerIp & vbTab & DomainName)
            InvokeElement.MsgBoxBlue(String.Format(RmText.GetString("msgHostsCopyEntry"), ServerIp & " " & DomainName), RmText.GetString("msgSuccess"), MessageBoxButtons.OK)
        Else 
            HostsFile.OpenHostsFile(false)
        End If
    End Sub

    Private Sub cbStreamQuality_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbStreamQuality.SelectedIndexChanged
        Player.RenewArgs()
        _writeToConsoleSettingsChanged(lblQuality.Text, cbStreamQuality.SelectedItem)
        tlpSettings.Focus()
    End Sub

    Private Sub txtGameKey_TextChanged(sender As Object, e As EventArgs) Handles txtGameKey.TextChanged
        txtGameKey.Text = txtGameKey.Text.ToUpper()
        If txtGameKey.Text = String.Empty Then
            tgOBS.Enabled = False
        End If
    End Sub

    Private Sub txtAdKey_TextChanged(sender As Object, e As EventArgs) Handles txtAdKey.TextChanged
        txtAdKey.Text = txtAdKey.Text.ToUpper()
        If txtAdKey.Text = String.Empty Then
            tgOBS.Enabled = False
        End If
    End Sub

    Private Sub tgTeamNamesAbr_CheckedChanged(sender As Object, e As EventArgs) Handles tgShowTeamCityAbr.CheckedChanged
        ApplicationSettings.SetValue(SettingsEnum.ShowTeamCityAbr, tgShowTeamCityAbr.Checked)
        For each game As GameControl In flpGames.Controls
            game.UpdateGame(GameManager.GamesDict(game.GameId), tgShowFinalScores.Checked, tgShowLiveScores.Checked, tgShowSeriesRecord.Checked, tgShowTeamCityAbr.Checked)
        Next
    End Sub

    Private Sub txtObsKey_TextChanged(sender As Object, e As EventArgs) Handles txtGameKey.TextChanged, txtAdKey.TextChanged
        tgOBS.Enabled = txtAdKey.Text <> String.Empty AndAlso txtGameKey.Text <> String.Empty AndAlso tgModules.Checked
    End Sub

    Private Sub chkSpotifyAnyMediaPlayer_CheckedChanged(sender As Object, e As EventArgs) Handles chkSpotifyAnyMediaPlayer.CheckedChanged
        If chkSpotifyAnyMediaPlayer.Checked Then
            chkSpotifyForceToStart.Checked = False
        End If
        chkSpotifyForceToStart.Enabled = Not chkSpotifyAnyMediaPlayer.Checked
    End Sub

    Private Sub pnlCalendar_MouseLeave(sender As Object, e As EventArgs) Handles flpCalendarPanel.MouseLeave
        flpCalendarPanel.Visible = flpCalendarPanel.ClientRectangle.Contains(flpCalendarPanel.PointToClient(Cursor.Position))
    End Sub

    Private Sub flpCalendarPanel_VisibleChanged(sender As Object, e As EventArgs) Handles flpCalendarPanel.VisibleChanged
        If flpCalendarPanel.Visible Then 
            btnDate.BackColor = Color.FromArgb(0, 170, 210)
        Else
            btnDate.BackColor = Color.FromArgb(64, 64, 64)
        End If
    End Sub
End Class
