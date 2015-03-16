// CADtest.NET by CAD bloke. http://CADbloke.com - See License.txt for license details

#if CoreConsole
using System;
#endif
using Autodesk.AutoCAD.Runtime;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CADTestRunner;
using NUnitLite.Runner;

[assembly: CommandClass(typeof (NUnitLiteTestRunner))]
namespace CADTestRunner
{
  public class NUnitLiteTestRunner
  {
     /// <summary> This command runs the NUnit tests in this assembly. </summary>
    [CommandMethod("RunCADtests", CommandFlags.Session)]
    public void RunCADtests()
     {
      string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
       string[] nunitArgs = new List<string>
       {
         // for details of options see  http://www.nunit.com/index.php?p=nunitliteOptions&r=3.0
         "--verbose",
         "--work=" + directoryName // save TestResults.xml to the build folder
  
       }.ToArray();
      
      new NUnitLite.Runner.TextUI().Execute(nunitArgs); 
      // NOTE: BREAKING CHANGE
      // new NUnitLite.Runner.AutoRun().Execute(nunitArgs); todo: Coming soon in NUnit V3 Beta 1. 
      // https://github.com/nunit/nunit/commit/6331e7e694439f8cbf000156f138a3e10370ec40
#if CoreConsole
      Console.WriteLine("Press Enter to Close this");
      Console.ReadLine();
#endif
     }
  }
}