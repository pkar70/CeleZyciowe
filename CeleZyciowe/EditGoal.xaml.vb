Public NotInheritable Class EditGoal
    Inherits Page

    Private mEditId As Integer = 0

    Protected Overrides Sub onNavigatedTo(e As NavigationEventArgs)
        If e.Parameter Is Nothing Then Return
        If Not Integer.TryParse(e.Parameter.ToString, mEditId) Then mEditId = 0
    End Sub

    Private Sub PokazZamkniecie(dDate As DateTime)
        If dDate.Year > 1970 Then
            uiDone.Text = "Finished @ " & dDate.ToString("dd.MM.yyyy HH:mm")
            uiDone.Visibility = Visibility.Visible
            uiDoneButton.Visibility = Visibility.Collapsed
        Else
            uiDone.Visibility = Visibility.Collapsed
            uiDoneButton.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub PokazComments(oItem As GoalItem)
        uiCommentsList.ItemsSource = oItem.lComments
    End Sub

    Private Sub PokazLinki(oItem As GoalItem)
        uiLinksList.ItemsSource = oItem.lLinks
    End Sub

    Private Function TimeSpanString(oTimeSpan As TimeSpan) As String
        If oTimeSpan.TotalDays() < 1 Then
            Return oTimeSpan.ToString("hh\:mm")
        Else
            Return oTimeSpan.ToString("d\,\ hh\:mm")
        End If
    End Function

    Private Sub PokazTiming(oItem As GoalItem)
        Dim iAddSecs As Integer = 0
        If oItem.dStartDate.Year > 1970 Then
            uiTimerStartStop.IsChecked = True
            uiTimerStartStop.Content = "Ticking..."
            iAddSecs = (DateTime.Now - oItem.dStartDate).TotalSeconds
        Else
            uiTimerStartStop.IsChecked = False
            uiTimerStartStop.Content = "Start!"
        End If
        'uiTimerIcon

        uiTimeSpent.Text = TimeSpanString(TimeSpan.FromSeconds(oItem.iSecondsSpent + iAddSecs))
        uiTimePlanned.Text = TimeSpanString(TimeSpan.FromMinutes(oItem.iMinutesPlanned))

        If oItem.iMinutesSpentWithSubs < 0 Then
            uiTimeWithSub.Text = "(niepoliczone)"
        Else
            uiTimeWithSub.Text = TimeSpanString(TimeSpan.FromMinutes(oItem.iMinutesSpentWithSubs))
        End If

    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Dim oItem As GoalItem = App.gGoalList.GetItem(mEditId)
        If oItem Is Nothing Then oItem = App.gGoalList.GetItem(0)
        If oItem Is Nothing Then
            DialogBox("Nie mogę znaleźć celu do pokazania!")
            Return
        End If

        '    ' teraz pokaż po kolei wszystko, jako EDIT - tylko jak...
        'Property ID As Integer - tego nie pokazujemy, bo po co
        uiNazwa.Text = oItem.sNazwa
        Dim sTmp As String = "Created @ " & oItem.dCreateDate.ToString("dd.MM.yyyy HH:mm") &
            ", modified @ " & oItem.dLastModDate.ToString("dd.MM.yyyy HH:mm")

        'Property aiParents As List(Of Integer) = New List(Of Integer)
        '' Property aiChilds As List(Of Integer) = New List(Of Integer)

        PokazZamkniecie(oItem.dDoneTime)

        uiOpis.Text = oItem.sInfo

        ' History: tworzone automatycznie z guziczka start/stop (zegarka), pozwala dopisac komentarz
        PokazTiming(oItem)

        ' Comments: dodawanie wpisow datowanych (EditBox, <enter> - i robi sie wpis)
        PokazComments(oItem)

        ' Links: dodawanie (EditBox, <enter> - i robi sie wpis)
        PokazLinki(oItem)


        '' pomocnicze ekranowe, bez zapisu w pliku
        '<Newtonsoft.Json.JsonIgnore>
        'Property iMinutesSpent As Integer = -1      ' bezposrednio tego GoalItem, suma po historii
        '<Newtonsoft.Json.JsonIgnore>
        'Property iMinutesSpentWithSubs As Integer = -1  ' ten node i wszystkie wgłąb


        ' poniżej, jako CommandBar:
        ' Add child (do aktualnie otwartego - czyli utworzenie Empty, i przeskoczenie do jego edycji)
        ' Save (że niby po zmianach)

    End Sub

    Private Async Sub uiDoneButon_Click(sender As Object, e As RoutedEventArgs)

        If Not Await DialogBoxYNAsync("Na pewno zamknąć to zadanie?") Then Return


        Dim bAllClosed As Boolean = True
        Dim lLista As List(Of GoalItem) = App.gGoalList.GetChilds(mEditId)
        For Each oItem As GoalItem In lLista
            If oItem.dDoneTime.Year > 1970 Then
                bAllClosed = False
                Await DialogBoxAsync("Ale są pod-zadania otwarte!")
                Return
            End If
        Next

        App.gGoalList.GetItem(mEditId).dDoneTime = DateTime.Now
        PokazZamkniecie(DateTime.Now)
        Await App.gGoalList.SaveItemsAsync
    End Sub


    Private Async Sub uiSaveComment_Click(sender As Object, e As RoutedEventArgs)
        Dim sComment As String = uiComment.Text
        If sComment.Length < 2 Then
            DialogBox("Za krótki tekst, nie uznaję takich")
            Return
        End If

        uiComment.Text = ""

        Dim oComm As GoalCommentItem = New GoalCommentItem
        oComm.dCreateDate = DateTime.Now
        oComm.sOpis = sComment

        Dim oItem As GoalItem = App.gGoalList.GetItem(mEditId)
        oItem.lComments.Add(oComm)
        PokazComments(oItem)
        Await App.gGoalList.SaveItemsAsync

    End Sub

    Private Async Sub uiSaveLink_Click(sender As Object, e As RoutedEventArgs)
        Dim oLink As GoalLinkItem = New GoalLinkItem
        oLink.dCreateDate = DateTime.Now

        oLink.sNazwa = uiLinkNazwa.Text
        If oLink.sNazwa.Length < 2 Then
            DialogBox("Za krótka nazwa linku, nie uznaję takich")
            Return
        End If

        oLink.sLink = uiLinkUri.Text
        If oLink.sLink.Length < 5 Then
            DialogBox("Za krótki link, nie uznaję takich")
            Return
        End If
        If (Not oLink.sLink.Contains("://")) AndAlso (Not oLink.sLink.Contains(":\")) Then
            DialogBox("Link bez '://' ani ':\', nie uznaję takich")
            Return
        End If

        uiLinkNazwa.Text = ""
        uiLinkUri.Text = ""

        Dim oItem As GoalItem = App.gGoalList.GetItem(mEditId)
        oItem.lLinks.Add(oLink)
        PokazLinki(oItem)
        Await App.gGoalList.SaveItemsAsync

    End Sub


    Private Sub uiOpenChild_Click(sender As Object, e As RoutedEventArgs)
        Dim oMFI As MenuFlyoutItem = TryCast(sender, MenuFlyoutItem)
        If oMFI Is Nothing Then Return

        If oMFI.DataContext Is Nothing Then Return

        Dim oItem As GoalItem = TryCast(oMFI.DataContext, GoalItem)
        If oItem Is Nothing Then Return

        ' jesli jest historia, to robimy tak:
        'Me.Frame.Navigate(GetType(EditGoal), oItem.ID.ToString)
        ' ale z drugiej strony, wtedy powrót do MAIN bedzie uciazliwy

        ' a jesli nie ma historii (parametr wywolania, nie tylko przeciez konkretny item, to tak:
        mEditId = oItem.ID
        Page_Loaded(Nothing, Nothing)

    End Sub

    Private Async Sub uiTimerStartStop_Click(sender As Object, e As RoutedEventArgs)
        ' przełączenie timera
        Dim oItem As GoalItem = App.gGoalList.GetItem(mEditId)
        If oItem.dStartDate.Year > 1970 Then
            If uiTimerStartStop.IsChecked Then
                DialogBox("Coś nie tak, niezgodność danych oItem i stanu guzika")
                Return
            End If

            Dim iAddSecs As Integer = (DateTime.Now - oItem.dStartDate).TotalSeconds
            oItem.iSecondsSpent += iAddSecs
            oItem.dStartDate = New DateTime(1970, 1, 1)
        Else
            If Not uiTimerStartStop.IsChecked Then
                DialogBox("Coś nie tak, niezgodność danych oItem i stanu guzika")
                Return
            End If
            oItem.dStartDate = DateTime.Now
        End If

        Await App.gGoalList.SaveItemsAsync
        PokazTiming(oItem)
    End Sub
End Class

Public Class KonwersjaDaty
    Implements IValueConverter

    ' Define the Convert method to change a DateTime object to
    ' a month string.
    Public Function Convert(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.Convert

        Dim dTemp As DateTime = CType(value, DateTime)

        Return dTemp.ToString("dd.MM.yyyy HH:mm")

    End Function

    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class

