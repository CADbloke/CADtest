' CADtest.NET by CAD bloke. http://CADbloke.com - See License.txt for license details

#If CoreConsole Then
Imports System
#End If
Imports CADtest.CADTestRunner
Imports Autodesk.AutoCAD.Runtime
Imports System.Collections.Generic
Imports System.IO
Imports System.Reflection
Imports NUnitLite.Runner


<Assembly: CommandClass(GetType(NUnitLiteTestRunner))> 
Namespace CADTestRunner
	Public Class NUnitLiteTestRunner
		''' <summary> This command runs the NUnit tests in this assembly. </summary>
    <CommandMethod("RunCADtests", CommandFlags.Session)> _
    Public Sub RunCADtests()
      Dim directoryName As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
      ' for details of options see  http://www.nunit.com/index.php?p=nunitliteOptions&r=3.0
      ' save TestResults.xml to the build folder
      Dim nunitArgs As String() = New List(Of String)() From { _
        "--verbose", _
        Convert.ToString("--work=") & directoryName _
      }.ToArray()

      Call New NUnitLite.Runner.TextUI().Execute(nunitArgs)
#If CoreConsole Then
			Console.WriteLine("Press Enter to Close this")
			Console.ReadLine()
#End If
    End Sub
	End Class
End Namespace
