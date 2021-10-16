Public Class GoalLista
    Public lLista As List(Of GoalItem) = New List(Of GoalItem)

    Private Const msFileName As String = "goals.json"

    Public Function Count() As Integer
        Return lLista.Count
    End Function

    Public Async Function SaveItemsAsync() As Task(Of Boolean)
        If lLista.Count < 1 Then Return False

        Dim oFold As Windows.Storage.StorageFolder = Windows.Storage.ApplicationData.Current.RoamingFolder
        Dim sTxt As String = Newtonsoft.Json.JsonConvert.SerializeObject(lLista, Newtonsoft.Json.Formatting.Indented)

        Await oFold.WriteAllTextToFileAsync(msFileName, sTxt, Windows.Storage.CreationCollisionOption.ReplaceExisting)

        Return True
    End Function

    Private Function MakeRootItem() As GoalItem
        Dim oGoal = New GoalItem
        oGoal.dCreateDate = Date.Now
        oGoal.ID = 0
        oGoal.sInfo = ""
        oGoal.sNazwa = GetSettingsString("rootname", GetLangString("txtRoot"))
        Return oGoal
    End Function


    Public Async Function LoadItemsAsync(bForce As Boolean) As Task(Of Boolean)
        If Not bForce AndAlso lLista.Count > 1 Then Return True

        Dim sTxt As String = Await Windows.Storage.ApplicationData.Current.RoamingFolder.ReadAllTextFromFileAsync(msFileName)
        If sTxt Is Nothing OrElse sTxt.Length < 5 Then
            lLista = New List(Of GoalItem)
            lLista.Add(MakeRootItem())
            Return False
        End If
        lLista = Newtonsoft.Json.JsonConvert.DeserializeObject(sTxt, GetType(List(Of GoalItem)))
        'UzupelnijMinutes()
        'UzupelnijChilds()
        Return True
    End Function

    'Private Sub UzupelnijMinutes()
    '    For Each oItem As GoalItem In lLista
    '        Dim iSecs As Integer = 0
    '        For Each oHist As GoalHistoryItem In oItem.lHistory
    '            iSecs += (oHist.dDateStop - oHist.dDateStart).TotalSeconds
    '        Next

    '        oItem.iMinutesSpent = iSecs / 60
    '    Next
    'End Sub

    Private Sub UzupelnijMinutesWithSubs()
        ' idziemy tylko po jednym level za kazdym razem, zmieniajac -1 na konkretną wartość
    End Sub


    Private Sub UzupelnijChilds()
        'For Each oItem As GoalItem In lLista
        '    For Each oItemChild As GoalItem In lLista
        '        iSecs += (oHist.dDateStop - oHist.dDateStart).TotalSeconds
        '    Next

        '    oItem.iMinutesSpent = iSecs / 60
        'Next
    End Sub

    Public Function GetItem(iItem As Integer) As GoalItem
        For Each oItem As GoalItem In lLista
            If oItem.ID = iItem Then Return oItem
        Next

        Return Nothing
    End Function

    Public Function GetChilds(oItem As GoalItem) As List(Of GoalItem)
        Return GetChilds(oItem.ID)
    End Function

    Public Function GetChilds(iItem As Integer) As List(Of GoalItem)
        Dim lRet As List(Of GoalItem) = New List(Of GoalItem)
        For Each oGoal As GoalItem In lLista
            For Each oParent As Integer In oGoal.aiParents
                If oParent = iItem Then
                    lRet.Add(oGoal)
                    Exit For
                End If
            Next
        Next

        Return lRet
    End Function

    Public Async Function LogGoalActivity(oItem As GoalItem, bStart As Boolean) As Task(Of Boolean)
        Dim oFile As Windows.Storage.StorageFile = Await GetLogFileMonthlyAsync("", True)
        If oFile Is Nothing Then Return False

        Dim sTxt As String = DateTime.Now.ToString("yyyy.MM.dd HH:mm") & vbTab
        If bStart Then
            sTxt &= "START"
        Else
            sTxt &= "stop"
        End If

        sTxt = sTxt & vbTab & oItem.ID & vbTab & " (" & oItem.sNazwa & ")"

        Await oFile.AppendLineAsync(sTxt)
        Return True
    End Function


End Class

Public Class GoalItem
    Property ID As Integer
    Property sNazwa As String
    Property aiParents As List(Of Integer) = New List(Of Integer)
    ' Property aiChilds As List(Of Integer) = New List(Of Integer)

    Property sInfo As String = ""    ' info / opis

    Property dCreateDate As DateTime = DateTime.Now
    Property dDoneTime As DateTime = New DateTime(1970, 1, 1)

    ' Property bSuspended As Boolean = False ' zawiessony - idzie w dół w sortowaniu
    Property dLastModDate As DateTime = DateTime.Now ' do sortowania 

    Property dStartDate As DateTime = New DateTime(1970, 1, 1)
    'Property lHistory As List(Of GoalHistoryItem) = New List(Of GoalHistoryItem)
    Property lComments As List(Of GoalCommentItem) = New List(Of GoalCommentItem)
    Property lLinks As List(Of GoalLinkItem) = New List(Of GoalLinkItem)

    Property iMinutesPlanned As Integer
    Property iSecondsSpent As Integer = -1      ' bezposrednio tego GoalItem
    ' pomocnicze ekranowe, bez zapisu w pliku
    <Newtonsoft.Json.JsonIgnore>
    Property iMinutesSpentWithSubs As Integer = -1  ' ten node i wszystkie wgłąb
    'Property IsVisible As Boolean = False   ' moze niepotrzebne?
    'Property LevelSpaces As String = ""     ' wyliczane w trakcie Load, emulacja wcięcia TreeView, ꜕ —
    'Property IsCollapsed As Boolean = True
End Class

Public Class GoalHistoryItem
    Property dDateStart As DateTime
    Property dDateStop As DateTime
    Property sOpis As String
End Class

Public Class GoalCommentItem
    Property dCreateDate As DateTime
    Property sOpis As String
End Class

Public Class GoalLinkItem
    Property dCreateDate As DateTime
    Property sNazwa As String
    Property sLink As String
End Class