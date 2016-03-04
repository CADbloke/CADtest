// CADtest.NET by CAD bloke. http://CADbloke.com - See License.txt for license details

#if CoreConsole
#endif
using System;
using Autodesk.AutoCAD.Runtime;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CADTestRunner;

[assembly: CommandClass(typeof (NUnitLiteTestRunner))]
namespace CADTestRunner
{
  public class NUnitLiteTestRunner
  {
     /// <summary> This command runs the NUnit tests in this assembly. </summary>
    [CommandMethod("RunCADtests", CommandFlags.Session)]
    public void RunCADtests()
     {
       string directoryName = Path.GetTempPath();//Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
       string[] nunitArgs = new List<string>
       {
         // for details of options see  http://www.nunit.com/index.php?p=nunitliteOptions&r=3.0
         "--verbose"                 // Tell me everything
         // , "--work=" + directoryName // save TestResults.xml to the build folder
         , "--wait"                  // Wait for input before closing console window (PAUSE). Comment this out for batch operations.
       }.ToArray();
      
      
       new NUnitLite.AutoRun().Execute(nunitArgs); 
      // https://github.com/nunit/nunit/commit/6331e7e694439f8cbf000156f138a3e10370ec40

       Console.ReadLine();
     }
  }
}