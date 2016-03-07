' CADtest.NET by CAD bloke. http://CADbloke.com - See License.txt for license details
Imports System.Runtime.InteropServices
Imports Autodesk.AutoCAD.ApplicationServices.Core
Imports Autodesk.AutoCAD.Runtime
Imports CADtest.CADTestRunner

<Assembly: ExtensionApplication(GetType(NUnitRunnerApp))>

Namespace CADTestRunner
    ''' <summary> AutoCAD extension application implementation. </summary>
    ''' <seealso cref="T:Autodesk.AutoCAD.Runtime.IExtensionApplication" />
    Public Class NUnitRunnerApp
        Implements IExtensionApplication

        Public Sub Initialize() Implements IExtensionApplication.Initialize
#If Not CoreConsole Then
            If Not AttachConsole(-1) Then ' Attach to a parent process console
        AllocConsole()              ' Alloc a new console if none available
            End If

#Else
            ' http://forums.autodesk.com/t5/net/accoreconsole-exe-in-2015-doesn-t-do-system-console-writeline/m-p/5551652
            ' This code is so you can see the test results in the console window, as per the above forum thread.
            If Application.Version.Major * 10 + Application.Version.Minor > 190 Then ' >v2013
                ' http://stackoverflow.com/a/15960495/492
                Dim defaultStdout As New IntPtr(7)  ' stdout's handle seems to always be equal to 7
                Dim currentStdout As IntPtr = GetStdHandle(StdOutputHandle)
                If currentStdout <> defaultStdout Then
                    SetStdHandle(StdOutputHandle, defaultStdout)
                End If

                ' http://forums.autodesk.com/t5/net/accoreconsole-exe-in-2015-doesn-t-do-system-console-writeline/m-p/5551652#M43708
                FixStdOut()
            End If
#End If
        End Sub

        Public Sub Terminate() Implements IExtensionApplication.Terminate
#If Not CoreConsole Then
            FreeConsole()
#End If
        End Sub


        ' http://stackoverflow.com/questions/3917202/how-do-i-include-a-console-in-winforms/3917353#3917353
        ''' <summary> Allocates a new console for current process. </summary>
        <DllImport("kernel32.dll")>
        Public Shared Function AllocConsole() As [Boolean]
        End Function

        ''' <summary> Frees the console. </summary>
        <DllImport("kernel32.dll")>
        Public Shared Function FreeConsole() As [Boolean]
        End Function

        ' http://stackoverflow.com/a/8048269/492
        <DllImport("kernel32.dll")>
        Private Shared Function AttachConsole(pid As Integer) As Boolean
        End Function


#If CoreConsole Then
        ' http://forums.autodesk.com/t5/net/accoreconsole-exe-in-2015-doesn-t-do-system-console-writeline/m-p/5551652#M43708
        ' trying to fix the telex-like line output from Core Console in versions > 2013. This didn't work for me. :(
        <DllImport("msvcr110.dll")>
        Private Shared Function _setmode(fileno As Integer, mode As Integer) As Integer
        End Function

        Public Sub FixStdOut()
            _setmode(3, &H4000)
        End Sub


        ' http://stackoverflow.com/a/15960495/492
        Private Const StdOutputHandle As UInt32 = &HFFFFFFF5UI

        <DllImport("kernel32.dll")>
        Private Shared Function GetStdHandle(nStdHandle As UInt32) As IntPtr
        End Function

        <DllImport("kernel32.dll")>
        Private Shared Sub SetStdHandle(nStdHandle As UInt32, handle As IntPtr)
        End Sub

#End If
    End Class
End Namespace
