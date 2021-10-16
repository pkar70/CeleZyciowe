' operacje:
' usuniecie, rename, przeniesienie pomiedzy itemami

Public NotInheritable Class EditTree
    Inherits Page

    Private Function TimeSpentForPrint(oTimeSpan As TimeSpan) As String
        Dim sTime = ""
        If oTimeSpan.TotalMinutes < 600 Then Return CInt(oTimeSpan.TotalMinutes).ToString & " m"
        If oTimeSpan.TotalMinutes < 240 Then Return CInt(oTimeSpan.TotalHours).ToString & " h"
        Return CInt(oTimeSpan.TotalDays).ToString & " d"
    End Function

    Dim obGray = New SolidColorBrush(Windows.UI.Colors.Gray)

    Public Sub ShowList(iParent As Integer, sPrefix As String)

        'For Each oItem In App.gGoalList.GetChilds(iParent)

        '    Dim sTxt = ""
        '    sTxt = sPrefix
        '    If oItem.HasChilds Then
        '        If oItem.IsCollapsed Then
        '            sTxt &= "■"
        '        Else
        '            sTxt &= "□"
        '        End If
        '    End If

        '    Dim sTxt2 = " " & oItem.sNazwa & " (" & TimeSpentForPrint(oItem.sTotalTimeSpent) & ")"

        '    Dim oText = New TextBlock
        '    oText.Text = sTxt
        '    Dim oText2 = New TextBlock
        '    If oItem.HasChilds Then oText2.FontWeight = Windows.UI.Text.FontWeights.Bold
        '    ' If oItem.DoneTime IsNot Nothing Then oText2.Foreground = obGray

        '    Dim oHorStack = New StackPanel
        '    oHorStack.HorizontalAlignment = True
        '    oHorStack.Children.Add(oText)
        '    oHorStack.Children.Add(oText2)
        '    ListaCeli.Children.Add(oHorStack)

        '    If Not oItem.IsCollapsed Then
        '        ShowList(oItem.ID, sPrefix & " ꜕—")
        '    End If

        'Next
    End Sub

    Private Sub uiPage_Loaded(sender As Object, e As RoutedEventArgs)
        ShowList(0, " ")
    End Sub

    ' kolory w zaleznosci od typu:
    ' haschilds: bold
    ' donetime: gray

    ' reakcja na Click: select, oraz zwija/rozwija
    ' reakcja na Right: menu: edit, open/close, addchild

End Class
