﻿Imports System.Text
Imports FastColoredTextBoxNS
Imports System.Text.RegularExpressions
Imports System.IO

Public Class TriggerViewerForm
    Dim savestatus As Boolean = True
    Dim formname As String = "트리거 뷰어"


    Dim viewcomment As Boolean = True
    Dim issetto As Boolean
    Dim playernum As Integer

    'Private Declare Function LockWindowUpdate Lib "user32" Alias "LockWindowUpdate" (ByVal hWnd As IntPtr) As Integer

    Dim loadS As Boolean = False



    Friend WithEvents FCTB As New FastColoredTextBox
    Private Sub TriggerViewerForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MsgBox(Lan.GetText("Msgbox", "NotArrow"), MsgBoxStyle.Critical, ProgramSet.ErrorFormMessage)


        Me.Close()
        Exit Sub

        FCTB.Dock = DockStyle.Fill
        FCTB.Language = Language.Lua

        TableLayoutPanel1.Controls.Add(FCTB, 0, 0)







        ComboBox1.SelectedIndex = 0
        CheckBox1.Checked = ProjectSet.TriggerSetTouse
        'CheckBox2.Checked = True
        issetto = ProjectSet.TriggerSetTouse
        'viewcomment = True
        ComboBox1.SelectedIndex = ProjectSet.TriggerPlayer
        playernum = ProjectSet.TriggerPlayer

        FCTB.Text = RedrawText()

        FCTB.ExpandAllFoldingBlocks()
        loadS = True
        savestatus = True
        updatame_text()
    End Sub


    'Public DafacultStyle As Style = New TextStyle(Brushes.Black, Nothing, FontStyle.Italic)
    Private Sub updatame_text()
        Me.Text = Me.Text.Replace(" (수정됨)", "")
        If savestatus = False Then
            Me.Text = Me.Text & " (수정됨)"
        End If
    End Sub

    Private Sub FCTB_TextChanged(sender As Object, e As TextChangedEventArgs) Handles FCTB.TextChanged
        savestatus = False
        ProjectSet.saveStatus = False

        updatame_text()

        '주석 처리
        'e.ChangedRange.SetStyle(GreenStyle, "--.*$", RegexOptions.Multiline)

        '문자열 처리
        'e.ChangedRange.SetStyle(PurpleStyle, """.*.""", RegexOptions.Multiline)

        '숫자 처리
        'e.ChangedRange.SetStyle(BumberStyle, "\b\d+\b", RegexOptions.Multiline)

        'e.ChangedRange.SetStyle(BumberStyle, "\b0x.\w+\b", RegexOptions.Multiline)




        e.ChangedRange.ClearFoldingMarkers()

        'e.ChangedRange.SetFoldingMarkers("conditions = {", "}," & vbCrLf)
        'e.ChangedRange.SetFoldingMarkers("actions = {", "}," & vbCrLf)

        e.ChangedRange.SetFoldingMarkers("{", "}")
        'e.ChangedRange.SetFoldingMarkers("--start", "--end")
    End Sub




    '50C000 ~ 6EBFFF
    Public Function RedrawText() As String
        Dim LasttriggerCount As Integer = 0

        Dim stringb As New StringBuilder

        stringb.AppendLine("-- This text is generated by EUD Editor")


        'stringb.AppendLine("--start SCMLoader")

        'If MPQlib.ReadListfile.Count > 1 Then
        '    Dim filestream As New FileStream(My.Application.Info.DirectoryPath & "\Data\SCMLoaderText.txt", FileMode.Open)
        '    Dim streamread As New StreamReader(filestream)

        '    stringb.AppendLine(streamread.ReadToEnd())
        '    stringb.Replace("PLAYER", "P" & playernum + 1)


        '    streamread.Close()
        '    filestream.Close()
        'End If


        'stringb.AppendLine("--end" & vbCrLf)









        stringb.AppendLine("Trigger { -- EUD Editor")
        stringb.AppendLine("	players = {P" & playernum + 1 & "},")


        stringb.AppendLine("	conditions = {")
        stringb.AppendLine("		Always();")
        stringb.AppendLine("	},")
        stringb.AppendLine("	actions = {")

        Dim triggercount As Byte = 0
        For k = 0 To DatEditDATA.Count - 1
            For i = 0 To DatEditDATA(k).projectdata.Count - 1
                For j = 0 To DatEditDATA(k).projectdata(i).Count - 1
                    If DatEditDATA(k).projectdata(i)(j) <> 0 Then ' 하나라도 0이 아니라면. 즉 하나라도 수정되어있다면.
                        Dim Offsetname As String = DatEditDATA(k).typeName & "_" & DatEditDATA(k).keyDic.Keys.ToList(i)
                        Dim startoffset As Long
                        Dim endoffset As Long

                        Dim typeName As String = DatEditDATA(k).keyDic.Keys.ToList(i)

                        Dim _size As Integer = DatEditDATA(k).keyINFO(i).realSize

                        Dim _value As Long = DatEditDATA(k).projectdata(i)(j)
                        Dim _oldvalue As Long = DatEditDATA(k).data(i)(j)

                        Dim _lastvalue As Long = 0

                        Dim _offsetNum As Long = Val("&H" & ReadOffset(Offsetname)) + _size * j



                        Dim _byte2 As Long = (j * _size) Mod 4
                        Dim _byte As Long = _offsetNum Mod 4
                        _offsetNum = _offsetNum - _byte

                        Dim _offset As String = Hex(_offsetNum)
                        Dim _modifier As String

                        If InStr(stringb.ToString, _offset) <> 0 Then
                            Dim oldaction As String
                            'Dim temp As String = Mid(TriggerText.Text, InStr(TriggerText.Text, _offset) - 2)
                            'Dim temptext As String
                            'temp = Replace(temp, ");", "")




                            'InStr(stringb.ToString, _offset)
                            'temptext = temp.Split(",")(2).Trim

                            'TriggerText.Select(InStr(TriggerText.Text, _offset) + temp.LastIndexOf(",") - 1, temptext.Length)
                            '오프셋 부분 찾기.
                            startoffset = InStr(stringb.ToString, "SetMemory(0x" & _offset) - 1
                            endoffset = InStr(Mid(stringb.ToString, startoffset), vbCrLf)
                            oldaction = Mid(stringb.ToString, startoffset, endoffset).Trim
                            If issetto = True Then 'SetTo
                                Dim _index As Integer

                                '================================================
                                Select Case _size
                                    Case 1
                                        _index = j - (j Mod 4)
                                    Case 2
                                        _index = j - (j Mod 2)
                                    Case 4
                                        _index = j
                                End Select
                                '================================================

                                Dim values(3) As Long

                                Dim _maxcount As Integer
                                Select Case _size
                                    Case 1
                                        _maxcount = 3
                                    Case 2
                                        _maxcount = 1
                                    Case 4
                                        _maxcount = 0
                                End Select

                                For p = 0 To _maxcount
                                    values(p) = DatEditDATA(k).projectdata(i)(_index + p) + DatEditDATA(k).data(i)(_index + p) + DatEditDATA(k).mapdata(i)(_index + p)
                                Next

                                Select Case _size
                                    Case 1
                                        _lastvalue += values(0) * 256 ^ 0
                                        _lastvalue += values(1) * 256 ^ 1
                                        _lastvalue += values(2) * 256 ^ 2
                                        _lastvalue += values(3) * 256 ^ 3
                                    Case 2
                                        _lastvalue += values(0) * 256 ^ 0
                                        _lastvalue += values(1) * 256 ^ 2
                                    Case 4
                                        _lastvalue += values(0) * 256 ^ 0
                                End Select


                                If Offsetname = "units_Unit Size Left" Or Offsetname = "units_Unit Size Up" Then
                                    _index = j
                                    Dim v1, v2 As Long
                                    v1 = DatEditDATA(k).projectdata(40)(_index) + DatEditDATA(k).data(40)(_index) + DatEditDATA(k).mapdata(40)(_index)
                                    v2 = DatEditDATA(k).projectdata(41)(_index) + DatEditDATA(k).data(41)(_index) + DatEditDATA(k).mapdata(41)(_index)

                                    _lastvalue = 0
                                    _lastvalue += v1 * 256 ^ 0
                                    _lastvalue += v2 * 256 ^ 2
                                ElseIf Offsetname = "units_Unit Size Right" Or Offsetname = "units_Unit Size Down" Then
                                    _index = j
                                    Dim v1, v2 As Long
                                    v1 = DatEditDATA(k).projectdata(42)(_index) + DatEditDATA(k).data(42)(_index) + DatEditDATA(k).mapdata(42)(_index)
                                    v2 = DatEditDATA(k).projectdata(43)(_index) + DatEditDATA(k).data(43)(_index) + DatEditDATA(k).mapdata(43)(_index)

                                    _lastvalue = 0
                                    _lastvalue += v1 * 256 ^ 0
                                    _lastvalue += v2 * 256 ^ 2
                                ElseIf Offsetname = "units_StarEdit Placement Box Width" Or Offsetname = "units_StarEdit Placement Box Height" Then
                                    _index = j
                                    Dim v1, v2 As Long
                                    v1 = DatEditDATA(k).projectdata(36)(_index) + DatEditDATA(k).data(36)(_index) + DatEditDATA(k).mapdata(36)(_index)
                                    v2 = DatEditDATA(k).projectdata(37)(_index) + DatEditDATA(k).data(37)(_index) + DatEditDATA(k).mapdata(37)(_index)

                                    _lastvalue = 0
                                    _lastvalue += v1 * 256 ^ 0
                                    _lastvalue += v2 * 256 ^ 2
                                ElseIf Offsetname = "units_Addon Horizontal(X) Position" Or Offsetname = "units_Addon Vertical(Y) Position" Then
                                    _index = j
                                    Dim v1, v2 As Long
                                    v1 = DatEditDATA(k).projectdata(38)(_index) + DatEditDATA(k).data(38)(_index) + DatEditDATA(k).mapdata(38)(_index)
                                    v2 = DatEditDATA(k).projectdata(39)(_index) + DatEditDATA(k).data(39)(_index) + DatEditDATA(k).mapdata(39)(_index)

                                    _lastvalue = 0
                                    _lastvalue += v1 * 256 ^ 0
                                    _lastvalue += v2 * 256 ^ 2
                                End If

                                _modifier = "SetTo"
                                'TriggerText.SelectedText = _lastvalue
                                'MsgBox(values(0) & "," & values(1) & "," & values(2) & "," & values(3))
                            Else 'setto가 아닐 경우
                                Dim oldtriggervalue As String

                                oldtriggervalue = Replace(oldaction.Split(",")(2), ");", "")


                                _lastvalue = _value * 256 ^ _byte
                                If oldaction.Split(",")(1).Trim <> "Add" Then

                                    oldtriggervalue *= -1
                                    'isadd = "Subtract"
                                End If


                                If _lastvalue > 0 Then
                                    'TriggerText.SelectedText = temptext + _lastvalue

                                    'TriggerText.Select(InStr(TriggerText.Text, _offset) + temp.IndexOf(",") - 1, isadd.Length)
                                    'TriggerText.SelectedText = "Add"
                                    _lastvalue += oldtriggervalue
                                    _modifier = "Add"
                                Else
                                    _lastvalue = (_lastvalue + oldtriggervalue) * -1
                                    'TriggerText.SelectedText = (temptext + _lastvalue) * -1

                                    'TriggerText.Select(InStr(TriggerText.Text, _offset) + temp.IndexOf(",") - 1, isadd.Length)
                                    'TriggerText.SelectedText = "Subtract"
                                    _modifier = "Subtract"
                                End If


                            End If




                            'stringb.Remove(startoffset, stringb.Length - startoffset)

                            Dim outputtext As New StringBuilder



                            If viewcomment = True Then
                                outputtext.AppendLine("		-- " & Offsetname &
                                          ", ObjectID : " & j &
                                          ", Value : " & _value &
                                          ", OldValue : " & _oldvalue &
                                          ", Size : " & _size &
                                          ", Byte : " & _byte)
                            End If

                            outputtext.Append("		SetMemory(" & "0x" & _offset & ", " & _modifier & ", " & _lastvalue & ");")
                            stringb.Replace("		" & oldaction, outputtext.ToString)
                            'stringb.AppendLine("		)
                        Else '포함하고 있지 않을 경우. 즉 처음 쓰는 거면
                            If triggercount > 62 Then
                                triggercount = 0
                                stringb.AppendLine("		Comment(""EUD Editor"");")
                                stringb.AppendLine("	},")
                                stringb.AppendLine("}")


                                stringb.AppendLine("Trigger { -- EUD Editor")
                                stringb.AppendLine("	players = {P" & playernum + 1 & "},")

                                stringb.AppendLine("	conditions = {")
                                stringb.AppendLine("		Always();")
                                stringb.AppendLine("	},")
                                stringb.AppendLine("	actions = {")
                            End If


                            LasttriggerCount += 1
                            triggercount += 1




                            If viewcomment = True Then
                                stringb.AppendLine("		-- " & Offsetname &
                                      ", ObjectID : " & j &
                                      ", Value : " & _value &
                                      ", OldValue : " & _oldvalue &
                                      ", Size : " & _size &
                                      ", Byte : " & _byte)
                            End If



                            'stringb.Append("		SetMemory(" & "0x" & _offset & ", ")
                            If issetto = True Then
                                _modifier = "SetTo"


                                Dim _index As Integer

                                '================================================
                                Select Case _size
                                    Case 1
                                        _index = j - (j Mod 4)
                                    Case 2
                                        _index = j - (j Mod 2)
                                    Case 4
                                        _index = j
                                End Select
                                '================================================


                                Dim values(3) As Long

                                Dim _maxcount As Integer
                                Select Case _size
                                    Case 1
                                        _maxcount = 3
                                    Case 2
                                        _maxcount = 1
                                    Case 4
                                        _maxcount = 0
                                End Select
                                For p = 0 To _maxcount
                                    values(p) = DatEditDATA(k).projectdata(i)(_index + p) + DatEditDATA(k).data(i)(_index + p) + DatEditDATA(k).mapdata(i)(_index + p)
                                    'values(p) = DatEditDATA(k).projectdata(i)(_index + p) + DatEditDATA(k).data(i)(_index + p) + DatEditDATA(k).mapdata(i)(_index + p)


                                    'Try

                                    'Catch ex As Exception
                                    '    MsgBox(DatEditDATA(k).typeName & "," & DatEditDATA(k).keyDic.Keys(i) & "," & _index)
                                    'End Try
                                Next



                                Select Case _size
                                    Case 1
                                        _lastvalue += values(0) * 256 ^ 0
                                        _lastvalue += values(1) * 256 ^ 1
                                        _lastvalue += values(2) * 256 ^ 2
                                        _lastvalue += values(3) * 256 ^ 3
                                    Case 2
                                        _lastvalue += values(0) * 256 ^ 0
                                        _lastvalue += values(1) * 256 ^ 2
                                    Case 4
                                        _lastvalue += values(0) * 256 ^ 0
                                End Select

                                'units_StarEdit Placement Box Width=0x
                                'units_StarEdit Placement Box Height=0x
                                'units_Addon Horizontal(X) Position=0x
                                'units_Addon Vertical(Y) Position=0x
                                'units_Unit Size Left=0x6617C8
                                'units_Unit Size Up=0x6617CA
                                'units_Unit Size Right=0x6617CC
                                'units_Unit Size Down=0x6617CE

                                If Offsetname = "units_Unit Size Left" Or Offsetname = "units_Unit Size Up" Then
                                    _index = j
                                    Dim v1, v2 As Long
                                    v1 = DatEditDATA(k).projectdata(40)(_index) + DatEditDATA(k).data(40)(_index) + DatEditDATA(k).mapdata(40)(_index)
                                    v2 = DatEditDATA(k).projectdata(41)(_index) + DatEditDATA(k).data(41)(_index) + DatEditDATA(k).mapdata(41)(_index)

                                    _lastvalue = 0
                                    _lastvalue += v1 * 256 ^ 0
                                    _lastvalue += v2 * 256 ^ 2
                                ElseIf Offsetname = "units_Unit Size Right" Or Offsetname = "units_Unit Size Down" Then
                                    _index = j
                                    Dim v1, v2 As Long
                                    v1 = DatEditDATA(k).projectdata(42)(_index) + DatEditDATA(k).data(42)(_index) + DatEditDATA(k).mapdata(42)(_index)
                                    v2 = DatEditDATA(k).projectdata(43)(_index) + DatEditDATA(k).data(43)(_index) + DatEditDATA(k).mapdata(43)(_index)

                                    _lastvalue = 0
                                    _lastvalue += v1 * 256 ^ 0
                                    _lastvalue += v2 * 256 ^ 2
                                ElseIf Offsetname = "units_StarEdit Placement Box Width" Or Offsetname = "units_StarEdit Placement Box Height" Then
                                    _index = j
                                    Dim v1, v2 As Long
                                    v1 = DatEditDATA(k).projectdata(36)(_index) + DatEditDATA(k).data(36)(_index) + DatEditDATA(k).mapdata(36)(_index)
                                    v2 = DatEditDATA(k).projectdata(37)(_index) + DatEditDATA(k).data(37)(_index) + DatEditDATA(k).mapdata(37)(_index)

                                    _lastvalue = 0
                                    _lastvalue += v1 * 256 ^ 0
                                    _lastvalue += v2 * 256 ^ 2
                                ElseIf Offsetname = "units_Addon Horizontal(X) Position" Or Offsetname = "units_Addon Vertical(Y) Position" Then
                                    _index = j
                                    Dim v1, v2 As Long
                                    v1 = DatEditDATA(k).projectdata(38)(_index) + DatEditDATA(k).data(38)(_index) + DatEditDATA(k).mapdata(38)(_index)
                                    v2 = DatEditDATA(k).projectdata(39)(_index) + DatEditDATA(k).data(39)(_index) + DatEditDATA(k).mapdata(39)(_index)

                                    _lastvalue = 0
                                    _lastvalue += v1 * 256 ^ 0
                                    _lastvalue += v2 * 256 ^ 2
                                End If
                                'MsgBox(values(0) & "," & values(1) & "," & values(2) & "," & values(3))
                            Else
                                _lastvalue = _value * 256 ^ _byte
                                If _lastvalue > 0 Then
                                    _modifier = "Add"
                                Else
                                    _modifier = "Subtract"
                                    _lastvalue = _lastvalue * -1
                                End If
                            End If



                            stringb.AppendLine("		SetMemory(" & "0x" & _offset & ", " & _modifier & ", " & _lastvalue & ");")
                        End If
                    End If
                Next
            Next
        Next


        'For i = 0 To ProjectUnitStatusFn1.Count - 1
        '    Dim narr() As String = {"FG_Debug", "FG_Status", "FG_Display"}

        '    For sname = 0 To 2
        '        Dim checkvalue As Long
        '        Dim _lastvalue As Long
        '        If issetto = True Then
        '            Select Case sname
        '                Case 0
        '                    checkvalue = ProjectDebugID(i)
        '                    _lastvalue = ProjectDebugID(i) + DebugID(i)
        '                Case 1
        '                    checkvalue = ProjectUnitStatusFn1(i)
        '                    _lastvalue = statusFn1(ProjectUnitStatusFn1(i) + UnitStatusFn1(i))
        '                Case 2
        '                    checkvalue = ProjectUnitStatusFn2(i)
        '                    _lastvalue = statusFn2(ProjectUnitStatusFn2(i) + UnitStatusFn2(i))
        '            End Select
        '        Else
        '            Select Case sname
        '                Case 0
        '                    checkvalue = ProjectDebugID(i)
        '                    _lastvalue = ProjectDebugID(i)
        '                Case 1
        '                    checkvalue = ProjectUnitStatusFn1(i)
        '                    _lastvalue = CLng(statusFn1(ProjectUnitStatusFn1(i) + UnitStatusFn1(i))) - statusFn1(UnitStatusFn1(i))

        '                Case 2
        '                    checkvalue = ProjectUnitStatusFn2(i)
        '                    _lastvalue = CLng(statusFn2(ProjectUnitStatusFn2(i) + UnitStatusFn2(i))) - statusFn2(UnitStatusFn2(i))
        '            End Select
        '        End If



        '        Dim _offsetNum As Long = Val("&H" & ReadOffset(narr(sname))) + 12 * i
        '        Dim _offset As String = Hex(_offsetNum)


        '        Dim _modifier As String

        '        If issetto = True Then
        '            _modifier = "SetTo"
        '        Else
        '            If _lastvalue > 0 Then
        '                _modifier = "Add"
        '            Else
        '                _modifier = "Subtract"
        '                _lastvalue = _lastvalue * -1
        '            End If
        '        End If


        '        If checkvalue <> 0 Then

        '            If triggercount > 62 Then
        '                triggercount = 0
        '                stringb.AppendLine("		Comment(""EUD Editor"");")
        '                stringb.AppendLine("	},")
        '                stringb.AppendLine("}")


        '                stringb.AppendLine("Trigger { -- EUD Editor")
        '                stringb.AppendLine("	players = {P" & playernum + 1 & "},")

        '                stringb.AppendLine("	conditions = {")
        '                stringb.AppendLine("		Always();")
        '                stringb.AppendLine("	},")
        '                stringb.AppendLine("	actions = {")
        '            End If

        '            LasttriggerCount += 1
        '            triggercount += 1



        '            stringb.AppendLine("		SetMemory(" & "0x" & _offset & ", " & _modifier & ", " & _lastvalue & ");")

        '        End If
        '    Next

        'Next



        stringb.AppendLine("		Comment(""EUD Editor"");")
        stringb.AppendLine("	},")
        stringb.AppendLine("}")












        Me.Text = formname & "    액션 수 : " & LasttriggerCount

        Return stringb.ToString
    End Function
    Private Sub AddAction()

    End Sub












    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If loadS = True Then
            issetto = CheckBox1.Checked
            ProjectSet.TriggerSetTouse = issetto
            FCTB.Text = RedrawText()

            'FCTB.CollapseFoldingBlock(1)
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If loadS = True Then
            viewcomment = CheckBox2.Checked
            FCTB.Text = RedrawText()

            'FCTB.CollapseFoldingBlock(1)
        End If
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If loadS = True Then
            playernum = ComboBox1.SelectedIndex
            ProjectSet.TriggerPlayer = playernum
            FCTB.Text = RedrawText()

            'FCTB.CollapseFoldingBlock(1)
        End If
    End Sub


    Private Sub Compile()
        Dim _strBuilder As New StringBuilder
        Dim startoffset As Long
        Dim endoffset As Long
        Dim oldaction As String

        '--start ~ --end 까지 우선 삭제를 한다.
        Try
            FCTB.Text = Mid(FCTB.Text, InStr(FCTB.Text, "--end"))
        Catch ex As Exception

        End Try



        _strBuilder.Append(FCTB.Text)

        For k = 0 To DatEditDATA.Count - 1
            DatEditDATA(k).projectReset()
        Next

        While InStr(_strBuilder.ToString, "SetMemory") <> 0
            startoffset = InStr(_strBuilder.ToString, "SetMemory") - 1
            endoffset = InStr(Mid(_strBuilder.ToString, startoffset), vbCrLf)
            oldaction = Mid(_strBuilder.ToString, startoffset, endoffset).Trim

            _strBuilder.Remove(1, startoffset)

            Dim Textoffset As String = oldaction.Split(",")(0).Replace("SetMemory(", "").Trim
            Dim Textmodifi As String = oldaction.Split(",")(1).Trim
            Dim Textvalue As Long = oldaction.Split(",")(2).Replace(");", "").Trim



            Dim value As Long
            value = Textvalue
            If Textmodifi = "Subtract" Then
                value = Textvalue * -1
            End If


            Dim offsetuint As UInteger = Val("&H" & Textoffset.Replace("0x", ""))

            For k = 0 To DatEditDATA.Count - 1
                For i = 0 To DatEditDATA(k).keyINFO.Count - 1
                    Dim Offsetname As String = DatEditDATA(k).typeName & "_" & DatEditDATA(k).keyDic.Keys.ToList(i)

                    Dim _offsetNum As Long = Val("&H" & ReadOffset(Offsetname))

                    Dim objectcount As UInteger = DatEditDATA(k).data(i).Count - 1

                    Dim size As Byte = DatEditDATA(k).keyINFO(i).realSize
                    Dim _maxvalue As Long = _offsetNum + size * objectcount
                    Dim _minvalue As Long = _offsetNum

                    If _minvalue <= offsetuint And offsetuint <= _maxvalue Then
                        Dim index As Integer = (offsetuint - _minvalue) \ size

                        If k = 0 And (i = 36 Or i = 38 Or i = 40) Then
                            If i = 40 Then
                                Dim unitindex As Byte = ((offsetuint - _minvalue) \ 2) Mod 4
                                If unitindex = 2 Then
                                    i = 42
                                End If

                            End If


                            Dim v1, v2 As Integer
                            Dim pv1, pv2 As Integer



                            pv2 = DatEditDATA(k).data(i + 1)(index) + DatEditDATA(k).mapdata(i + 1)(index)
                            pv1 = DatEditDATA(k).data(i)(index) + DatEditDATA(k).mapdata(i)(index)

                            Dim resultvalue As UInteger
                            resultvalue = pv1 + pv2 * 65536

                            resultvalue += value

                            v2 = (resultvalue >> 16) '- pv2
                            v1 = (resultvalue - (v2 << 16)) ' - pv1

                            v2 -= pv2
                            v1 -= pv1

                            If Textmodifi = "SetTo" Then
                                DatEditDATA(k).projectdata(i)(index) = v1 - pv1
                                DatEditDATA(k).projectdata(i + 1)(index) = v2 - pv2
                            Else
                                DatEditDATA(k).projectdata(i)(index) += v1
                                DatEditDATA(k).projectdata(i + 1)(index) += v2
                            End If
                        Else
                            Select Case size
                                Case 1
                                    Dim v1, v2, v3, v4 As Short
                                    Dim pv1, pv2, pv3, pv4 As Short



                                    pv4 = DatEditDATA(k).data(i)(index + 3) + DatEditDATA(k).mapdata(i)(index + 3)
                                    pv3 = DatEditDATA(k).data(i)(index + 2) + DatEditDATA(k).mapdata(i)(index + 2)
                                    pv2 = DatEditDATA(k).data(i)(index + 1) + DatEditDATA(k).mapdata(i)(index + 1)
                                    pv1 = DatEditDATA(k).data(i)(index) + DatEditDATA(k).mapdata(i)(index)
                                    Dim resultvalue As ULong

                                    resultvalue = pv1 + (pv2 * 256 ^ 1) + (pv3 * 256 ^ 2) + (pv4 * 256 ^ 3)

                                    resultvalue += value


                                    v4 = (resultvalue And &HFF000000) / 16777216 ' - pv4
                                    v3 = (resultvalue And &HFF0000) / 65536 '- pv3
                                    v2 = (resultvalue And &HFF00) / 256 '- pv2
                                    v1 = (resultvalue And &HFF) '- pv1

                                    v4 -= pv4
                                    v3 -= pv3
                                    v2 -= pv2
                                    v1 -= pv1


                                    If Textmodifi = "SetTo" Then
                                        DatEditDATA(k).projectdata(i)(index) = (value And &HFF) - pv1
                                        index += 1
                                        DatEditDATA(k).projectdata(i)(index) = (value And &HFF00) / 256 - pv2
                                        index += 1
                                        DatEditDATA(k).projectdata(i)(index) = (value And &HFF0000) / 65536 - pv3
                                        index += 1
                                        DatEditDATA(k).projectdata(i)(index) = (value And &HFF000000) / 16777216 - pv4
                                    Else
                                        DatEditDATA(k).projectdata(i)(index) += v1
                                        index += 1
                                        DatEditDATA(k).projectdata(i)(index) += v2
                                        index += 1
                                        DatEditDATA(k).projectdata(i)(index) += v3
                                        index += 1
                                        DatEditDATA(k).projectdata(i)(index) += v4
                                    End If
                                Case 2
                                    Dim v1, v2 As Integer
                                    Dim pv1, pv2 As Integer



                                    pv2 = DatEditDATA(k).data(i)(index + 1) + DatEditDATA(k).mapdata(i)(index + 1)
                                    pv1 = DatEditDATA(k).data(i)(index) + DatEditDATA(k).mapdata(i)(index)

                                    Dim resultvalue As ULong
                                    resultvalue = pv1 + pv2 * 65536

                                    resultvalue += value

                                    v2 = (resultvalue >> 16) '- pv2
                                    v1 = (resultvalue - (v2 << 16)) ' - pv1

                                    v2 -= pv2
                                    v1 -= pv1

                                    If Textmodifi = "SetTo" Then
                                        DatEditDATA(k).projectdata(i)(index) = v1 - pv1
                                        index += 1
                                        DatEditDATA(k).projectdata(i)(index) = v2 - pv2
                                    Else
                                        DatEditDATA(k).projectdata(i)(index) += v1
                                        index += 1
                                        DatEditDATA(k).projectdata(i)(index) += v2
                                    End If
                                Case 4
                                    Dim v1 As Long = value
                                    Dim pv1 As Long

                                    pv1 = DatEditDATA(k).data(i)(index) + DatEditDATA(k).mapdata(i)(index)
                                    If Textmodifi = "SetTo" Then
                                        DatEditDATA(k).projectdata(i)(index) = v1 - pv1
                                    Else
                                        DatEditDATA(k).projectdata(i)(index) += v1
                                    End If
                            End Select
                        End If

                        Exit For
                    End If


                    'DatEditDATA(k).keyINFO(i).VarStart
                    'DatEditDATA(k).keyINFO(i).VarEnd

                    'DatEditDATA(k).keyINFO(i).realSize


                Next
            Next





            'MsgBox(offset & ", " & modifi & ", " & value)
        End While

        FCTB.Text = RedrawText()
        'FCTB.CollapseFoldingBlock(1)
        savestatus = True
        updatame_text()


    End Sub

    Private Sub ComplieButton_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Compile()
    End Sub

    Private Sub 컴파일ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 컴파일ToolStripMenuItem.Click
        Compile()
    End Sub
End Class