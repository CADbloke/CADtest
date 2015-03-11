' CADtest.NET by CAD bloke. http://CADbloke.com - See License.txt for license details

Imports System.Runtime.InteropServices
Imports Autodesk.AutoCAD.Runtime


<Assembly: ExtensionApplication(GetType(CADTestRunner.NUnitRunnerApp))>

Namespace CADTestRunner
	''' <summary> AutoCAD extension application implementation. </summary>
	''' <seealso cref="T:Autodesk.AutoCAD.Runtime.IExtensionApplication"/>
	Public Class NUnitRunnerApp
		Implements IExtensionApplication
		Public Sub Initialize()
#If Not CoreConsole Then
			AllocConsole()
#End If
    End Sub

    Public Sub IExtensionApplication_Terminate() Implements IExtensionApplication.Terminate
#If Not CoreConsole Then
      FreeConsole()
#End If
    End Sub


    Public Sub IExtensionApplication_Initialize() Implements IExtensionApplication.Initialize
#If Not CoreConsole Then
      AllocConsole()
#End If
    End Sub

    Public Sub Terminate()
#If Not CoreConsole Then
      FreeConsole()
#End If
    End Sub

		' http://stackoverflow.com/questions/3917202/how-do-i-include-a-console-in-winforms/3917353#3917353
		' http://web.archive.org/web/20100815055904/http://www.thereforesystems.com/output-to-console-in-windows-forms-application
		' http://www.codeproject.com/Questions/229000/Will-Console-writeline-work-in-Windows-Application

		''' <summary> Allocates a new console for current process. </summary>
		<DllImport("kernel32.dll")> _
		Public Shared Function AllocConsole() As [Boolean]
		End Function


		''' <summary> Frees the console. </summary>
		<DllImport("kernel32.dll")> _
		Public Shared Function FreeConsole() As [Boolean]
		End Function
	End Class
End Namespace
