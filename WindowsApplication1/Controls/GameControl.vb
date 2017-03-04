Imports NHLGames.Game

Public Class GameControl

    Private _Game As Game
    Private _selectedDate As Date

    Public Sub New(Game As Game, showScores As Boolean, selectedDate As Date)
        InitializeComponent()

        _Game = Game
        _selectedDate = selectedDate

        SetInitialProperties(Game)
        UpdateGameStatusProperties(Game)

        lblHomeScore.Visible = showScores
        lblAwayScore.Visible = showScores

        AddHandler _Game.GameUpdated, AddressOf GameUpdatedHandler

    End Sub

    Private Sub UpdateGameStatusProperties(Game As Game)
        lblHomeScore.Text = Game.HomeScore
        lblHomeTeam.Text = Game.HomeAbbrev

        lblAwayScore.Text = Game.AwayScore
        lblAwayTeam.Text = Game.AwayAbbrev

        'btnWatch.Enabled = Game.Date <= DateTime.Now 'AndAlso Game.GameIsLive

        If Game.Date <= DateTime.Now.ToUniversalTime() AndAlso Game.GameIsLive Then
            BorderPanel1.BorderColour = Color.Green
            'lblVS.Visible = True
        ElseIf Game.Date <= DateTime.Now.ToUniversalTime() Then
            BorderPanel1.BorderColour = Color.LightGray
            'lblVS.Visible = True
        Else
            BorderPanel1.BorderColour = Color.DarkGray
            'lblVS.Visible = False
        End If
    End Sub

    Private Sub SetInitialProperties(Game As Game)
        picAway.SizeMode = PictureBoxSizeMode.StretchImage
        If String.IsNullOrEmpty(Game.HomeTeamLogo) = False Then
            picAway.Image = ImageFetcher.GetEmbeddedImage(Game.AwayTeamLogo)
            AwayTeamToolTip.SetToolTip(picAway, "Away Team: " & Game.AwayTeam)
        End If


        picHome.SizeMode = PictureBoxSizeMode.StretchImage
        If String.IsNullOrEmpty(Game.AwayTeamLogo) = False Then
            picHome.Image = ImageFetcher.GetEmbeddedImage(Game.HomeTeamLogo)
            HomeTeamToolTip.SetToolTip(picHome, "Home Team: " & Game.HomeTeam)
        End If

        lblTime.Text = Game.Date.ToLocalTime().ToString("h:mm tt")

        lblAwayStream.Visible = Game.AwayStream.IsAvailable
        If Game.AwayStream.Network <> String.Empty Then
            lblAwayStream.Text = lblAwayStream.Text & " (" & Game.AwayStream.Network & ")"
        End If
        lblHomeStream.Visible = Game.HomeStream.IsAvailable
        If Game.HomeStream.Network <> String.Empty Then
            lblHomeStream.Text = lblHomeStream.Text & " (" & Game.HomeStream.Network & ")"
        End If
        lblFrenchStream.Visible = Game.FrenchStream.IsAvailable
        If Game.FrenchStream.Network <> String.Empty Then
            lblFrenchStream.Text = lblFrenchStream.Text & " (" & Game.FrenchStream.Network & ")"
        End If
        lblNationalStream.Visible = Game.NationalStream.IsAvailable
        If Game.NationalStream.Network <> String.Empty Then
            lblNationalStream.Text = lblNationalStream.Text & " (" & Game.NationalStream.Network & ")"
        End If
        lblMultiCam1.Visible = Game.MultiCam1Stream.IsAvailable

        lblMultiCam2.Visible = Game.MultiCam2Stream.IsAvailable

        lblRefCam.Visible = Game.RefCamStream.IsAvailable

        lblEndzoneCam1.Visible = Game.EndzoneCam1Stream.IsAvailable

        lblEndzoneCam2.Visible = Game.EndzoneCam2Stream.IsAvailable
    End Sub

    Private Sub btnWatch_Click(sender As Object, e As EventArgs)
        Dim newForm As New WatchGameForm(_Game, New GameWatchArguments())
        newForm.ShowDialog(Me)
    End Sub

    Private Sub GameUpdatedHandler(game As Game)
        _Game = game
        UpdateGameStatusProperties(game)
    End Sub

    Private Sub lblHomeStream_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblHomeStream.LinkClicked
        Dim args = WatchArgs()

        If DateHelper.GetPacificTime(_Game.Date).ToShortDateString <> DateHelper.GetPacificTime().ToShortDateString() Then
            _Game.HomeStream.CheckVOD(args.CDN)
            args.IsVOD = _Game.HomeStream.IsVOD
        End If

        'If DateHelper.GetPacificTime(_Game.Date).ToShortDateString = _selectedDate.ToShortDateString() Then
        '    _Game.HomeStream.CheckDuplicate(args.CDN)
        'End If

        args.Stream = _Game.HomeStream
        _Game.Watch(args)
    End Sub

    Private Sub lblAwayStream_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblAwayStream.LinkClicked
        Dim args = WatchArgs()

        If DateHelper.GetPacificTime(_Game.Date).ToShortDateString <> DateHelper.GetPacificTime().ToShortDateString() Then
            _Game.AwayStream.CheckVOD(args.CDN)
            args.IsVOD = _Game.AwayStream.IsVOD
        End If

        'If DateHelper.GetPacificTime(_Game.Date).ToShortDateString = _selectedDate.ToShortDateString() Then
        '    _Game.AwayStream.CheckDuplicate(args.CDN)
        'End If

        args.Stream = _Game.AwayStream
        _Game.Watch(args)
    End Sub

    Private Sub lblFrenchStream_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblFrenchStream.LinkClicked
        Dim args = WatchArgs()

        If DateHelper.GetPacificTime(_Game.Date).ToShortDateString <> DateHelper.GetPacificTime().ToShortDateString() Then
            _Game.FrenchStream.CheckVOD(args.CDN)
            args.IsVOD = _Game.FrenchStream.IsVOD
        End If

        'If DateHelper.GetPacificTime(_Game.Date).ToShortDateString = _selectedDate.ToShortDateString() Then
        '    _Game.FrenchStream.CheckDuplicate(args.CDN)
        'End If

        args.Stream = _Game.FrenchStream
        _Game.Watch(args)
    End Sub

    Private Sub lblNationalStream_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblNationalStream.LinkClicked
        Dim args = WatchArgs()

        If DateHelper.GetPacificTime(_Game.Date).ToShortDateString <> DateHelper.GetPacificTime().ToShortDateString() Then
            _Game.NationalStream.CheckVOD(args.CDN)
            args.IsVOD = _Game.NationalStream.IsVOD
        End If

        'If DateHelper.GetPacificTime(_Game.Date).ToShortDateString = _selectedDate.ToShortDateString() Then
        '    _Game.NationalStream.CheckDuplicate(args.CDN)
        'End If

        args.Stream = _Game.NationalStream
        _Game.Watch(args)
    End Sub

    Private Sub lblMultiCam1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblMultiCam1.LinkClicked
        Dim args = WatchArgs()

        If DateHelper.GetPacificTime(_Game.Date).ToShortDateString <> DateHelper.GetPacificTime().ToShortDateString() Then
            _Game.MultiCam1Stream.CheckVOD(args.CDN)
            args.IsVOD = _Game.MultiCam1Stream.IsVOD
        End If

        'If DateHelper.GetPacificTime(_Game.Date).ToShortDateString = _selectedDate.ToShortDateString() Then
        '    _Game.NationalStream.CheckDuplicate(args.CDN)
        'End If

        args.Stream = _Game.MultiCam1Stream
        _Game.Watch(args)
    End Sub

    Private Sub lblMultiCam2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblMultiCam2.LinkClicked
        Dim args = WatchArgs()

        If DateHelper.GetPacificTime(_Game.Date).ToShortDateString <> DateHelper.GetPacificTime().ToShortDateString() Then
            _Game.MultiCam2Stream.CheckVOD(args.CDN)
            args.IsVOD = _Game.MultiCam2Stream.IsVOD
        End If

        'If DateHelper.GetPacificTime(_Game.Date).ToShortDateString = _selectedDate.ToShortDateString() Then
        '    _Game.NationalStream.CheckDuplicate(args.CDN)
        'End If

        args.Stream = _Game.MultiCam2Stream
        _Game.Watch(args)
    End Sub

    Private Sub lblRefCam_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblRefCam.LinkClicked
        Dim args = WatchArgs()

        If DateHelper.GetPacificTime(_Game.Date).ToShortDateString <> DateHelper.GetPacificTime().ToShortDateString() Then
            _Game.RefCamStream.CheckVOD(args.CDN)
            args.IsVOD = _Game.RefCamStream.IsVOD
        End If

        'If DateHelper.GetPacificTime(_Game.Date).ToShortDateString = _selectedDate.ToShortDateString() Then
        '    _Game.NationalStream.CheckDuplicate(args.CDN)
        'End If

        args.Stream = _Game.RefCamStream
        _Game.Watch(args)
    End Sub

    Private Sub lblEndzoneCam1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblEndzoneCam1.LinkClicked
        Dim args = WatchArgs()

        If DateHelper.GetPacificTime(_Game.Date).ToShortDateString <> DateHelper.GetPacificTime().ToShortDateString() Then
            _Game.EndzoneCam1Stream.CheckVOD(args.CDN)
            args.IsVOD = _Game.EndzoneCam1Stream.IsVOD
        End If

        'If DateHelper.GetPacificTime(_Game.Date).ToShortDateString = _selectedDate.ToShortDateString() Then
        '    _Game.NationalStream.CheckDuplicate(args.CDN)
        'End If

        args.Stream = _Game.EndzoneCam1Stream
        _Game.Watch(args)
    End Sub
    Private Sub lblEndzoneCam2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblEndzoneCam2.LinkClicked
        Dim args = WatchArgs()

        If DateHelper.GetPacificTime(_Game.Date).ToShortDateString <> DateHelper.GetPacificTime().ToShortDateString() Then
            _Game.EndzoneCam2Stream.CheckVOD(args.CDN)
            args.IsVOD = _Game.EndzoneCam2Stream.IsVOD
        End If

        'If DateHelper.GetPacificTime(_Game.Date).ToShortDateString = _selectedDate.ToShortDateString() Then
        '    _Game.NationalStream.CheckDuplicate(args.CDN)
        'End If

        args.Stream = _Game.EndzoneCam2Stream
        _Game.Watch(args)
    End Sub

    Private Function WatchArgs() As Game.GameWatchArguments
        Dim args = ApplicationSettings.Read(Of Game.GameWatchArguments)(ApplicationSettings.Settings.DefaultWatchArgs)
        args.GameTitle = _Game.AwayAbbrev & " @ " & _Game.HomeAbbrev
        Return args
    End Function
End Class

