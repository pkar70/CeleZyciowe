' początek: maj 2018, ale praktycznie nic nie zrobione - tylko wstepnie struktura danych
' wstepna działająca struktura danych: sierpien 2021



Public NotInheritable Class MainPage
    Inherits Page


    'Private Sub Recount()
    '    ' uzupelnienie zmiennych niewrzucanych do pliku

    '    For Each oGoal In App.mGoalList
    '        oGoal.IsCollapsed = True
    '        oGoal.TotalTimeSpent = oGoal.TimeSpent
    '        oGoal.HasChilds = False

    '        For Each oGoalSrodek In App.mGoalList
    '            If oGoalSrodek.Parent = oGoal.ID Then
    '                oGoal.HasChilds = True
    '                oGoal.TotalTimeSpent += oGoalSrodek.TimeSpent
    '            End If
    '        Next
    '    Next

    'End Sub

    Private Sub ShowTasks()
        ' albo wlasna iteracja, albo przez jego DetailsTemplate
    End Sub

    Private Async Sub uiPage_Loaded(sender As Object, e As RoutedEventArgs)
        Await App.gGoalList.LoadItemsAsync(False)
        ' Recount()
    End Sub

    Private Sub uiEditGoals_Click(sender As Object, e As RoutedEventArgs)
        ' można tam pójść wstawiając string z ID jako parametr - wtedy pokaże od tego elementu
        Me.Frame.Navigate(GetType(EditGoal))
    End Sub
End Class
