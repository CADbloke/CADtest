// CADtest.NET by CAD bloke. http://CADbloke.com - See License.txt for license details

using System;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.Runtime;


[assembly: ExtensionApplication(typeof (CADTestRunner.NUnitRunnerApp))]

namespace CADTestRunner
{
  /// <summary> AutoCAD extension application implementation. </summary>
  /// <seealso cref="T:Autodesk.AutoCAD.Runtime.IExtensionApplication"/>
  public class NUnitRunnerApp : IExtensionApplication
  {
    public void Initialize()
    {
#if !CoreConsole
      AllocConsole();
#endif
    }


    public void Terminate()
    {
#if !CoreConsole
      FreeConsole();
#endif
    }



// http://stackoverflow.com/questions/3917202/how-do-i-include-a-console-in-winforms/3917353#3917353
// http://web.archive.org/web/20100815055904/http://www.thereforesystems.com/output-to-console-in-windows-forms-application
// http://www.codeproject.com/Questions/229000/Will-Console-writeline-work-in-Windows-Application

    /// <summary> Allocates a new console for current process. </summary>
    [DllImport("kernel32.dll")]
    public static extern Boolean AllocConsole();


    /// <summary> Frees the console. </summary>
    [DllImport("kernel32.dll")]
    public static extern Boolean FreeConsole();
  }
}