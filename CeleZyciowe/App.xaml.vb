﻿Imports Windows.Storage
Imports Windows.UI.Popups

NotInheritable Class App
    Inherits Application

    Public Shared gGoalList As GoalLista

    Protected Function OnLaunchFragment(e As ApplicationExecutionState) As Frame
        Dim mRootFrame As Frame = OnLaunchFragment(e)

        If mRootFrame Is Nothing Then
            AddHandler mRootFrame.NavigationFailed, AddressOf OnNavigationFailed

            ' PKAR added wedle https://stackoverflow.com/questions/39262926/uwp-hardware-back-press-work-correctly-in-mobile-but-error-with-pc
            AddHandler mRootFrame.Navigated, AddressOf OnNavigatedAddBackButton
            AddHandler Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf OnBackButtonPressed

            ' Place the frame in the current Window
            Window.Current.Content = mRootFrame
        End If

        Return mRootFrame
    End Function


    ''' <summary>
    ''' Invoked when the application is launched normally by the end user.  Other entry points
    ''' will be used when the application is launched to open a specific file, to display
    ''' search results, and so forth.
    ''' </summary>
    ''' <param name="e">Details about the launch request and process.</param>
    Protected Overrides Sub OnLaunched(e As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)
        Dim RootFrame As Frame = OnLaunchFragment(e.PreviousExecutionState)

        If e.PrelaunchActivated = False Then
            If rootFrame.Content Is Nothing Then
                ' When the navigation stack isn't restored navigate to the first page,
                ' configuring the new page by passing required information as a navigation
                ' parameter
                rootFrame.Navigate(GetType(MainPage), e.Arguments)
            End If

            ' Ensure the current window is active
            Window.Current.Activate()
        End If
    End Sub

    ''' <summary>
    ''' Invoked when Navigation to a certain page fails
    ''' </summary>
    ''' <param name="sender">The Frame which failed navigation</param>
    ''' <param name="e">Details about the navigation failure</param>
    Private Sub OnNavigationFailed(sender As Object, e As NavigationFailedEventArgs)
        Throw New Exception("Failed to load Page " + e.SourcePageType.FullName)
    End Sub

    ''' <summary>
    ''' Invoked when application execution is being suspended.  Application state is saved
    ''' without knowing whether the application will be terminated or resumed with the contents
    ''' of memory still intact.
    ''' </summary>
    ''' <param name="sender">The source of the suspend request.</param>
    ''' <param name="e">Details about the suspend request.</param>
    Private Sub OnSuspending(sender As Object, e As SuspendingEventArgs) Handles Me.Suspending
        Dim deferral As SuspendingDeferral = e.SuspendingOperation.GetDeferral()
        ' TODO: Save application state and stop any background activity
        deferral.Complete()
    End Sub

    Private Async Function AppServiceLocalCommand(sCommand As String) As Task(Of String)
        Return ""
    End Function

    ' RemoteSystems, Timer
    Protected Overrides Async Sub OnBackgroundActivated(args As BackgroundActivatedEventArgs)
        moTaskDeferal = args.TaskInstance.GetDeferral() ' w pkarmodule.App

        Dim bNoComplete As Boolean = False
        Dim bObsluzone As Boolean = False

        'If args.TaskInstance.Task.Name = "InstaMonitorTimer" Then
        '    Await GetAllFeedsAsync(Nothing)
        '    bObsluzone = True
        'End If

        ' lista komend danej aplikacji
        Dim sLocalCmds As String = ""

        ' zwroci false gdy to nie jest RemoteSystem; gdy true, to zainicjalizowało odbieranie
        If Not bObsluzone Then bNoComplete = RemSysInit(args, sLocalCmds)

        If Not bNoComplete Then moTaskDeferal.Complete()

    End Sub


    ' CommandLine, Toasts
    Protected Overrides Async Sub OnActivated(args As IActivatedEventArgs)
        ' to jest m.in. dla Toast i tak dalej?

        ' próba czy to commandline
        If args.Kind = ActivationKind.CommandLineLaunch Then

            Dim commandLine As CommandLineActivatedEventArgs = TryCast(args, CommandLineActivatedEventArgs)
            Dim operation As CommandLineActivationOperation = commandLine?.Operation
            Dim strArgs As String = operation?.Arguments

            If Not String.IsNullOrEmpty(strArgs) Then
                Await ObsluzCommandLine(strArgs)
                Window.Current.Close()
                Return
            End If
        End If

        ' jesli nie cmdline (a np. toast), albo cmdline bez parametrow, to pokazujemy okno
        Dim rootFrame As Frame = OnLaunchFragment(args.PreviousExecutionState)

        If args.Kind = ActivationKind.ToastNotification Then
            rootFrame.Navigate(GetType(MainPage))
        End If

        Window.Current.Activate()

    End Sub


    Public Shared Async Function GetRoamingFile(sName As String, bCreate As Boolean) As Task(Of StorageFile)
        Dim oFold = Windows.Storage.ApplicationData.Current.RoamingFolder
        If oFold Is Nothing Then
            DialogBoxRes("errNoRoamFolder")
            Return Nothing
        End If

        Dim bErr = False
        Dim oFile = Nothing
        Try
            If bCreate Then
                oFile = Await oFold.CreateFileAsync(sName, CreationCollisionOption.ReplaceExisting)
            Else
                oFile = Await oFold.GetFileAsync(sName)
            End If
        Catch ex As Exception
            bErr = True
        End Try
        If bErr Then
            Return Nothing
        End If

        Return oFile
    End Function


End Class
