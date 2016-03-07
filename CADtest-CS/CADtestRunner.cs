// CADtest.NET by CAD bloke. http://CADbloke.com - See License.txt for license details

#if CoreConsole
#endif
using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.AutoCAD.Runtime;
using CADTestRunner;
using NUnitLite;
using System.Text;
using System.Diagnostics;
using System.Reflection;

[assembly: CommandClass(typeof (NUnitLiteTestRunner))]

namespace CADTestRunner
{
    public class NUnitLiteTestRunner
    {
        /// <summary> This command runs the NUnit tests in this assembly. </summary>
        [CommandMethod("RunCADtests", CommandFlags.Session)]
        public void RunCADtests()
        {
            //string directoryName = Path.GetTempPath(); //Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string directoryPlugin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string directoryReportUnit = Path.Combine(directoryPlugin, @"ReportUnit");
            string fileInputXML = Path.Combine(directoryReportUnit, @"Report-NUnit.xml");
            string fileOutputHTML = Path.Combine(directoryReportUnit, @"Report-NUnit.html");
            string generatorReportUnit = Path.Combine(directoryPlugin, @"ReportUnit", @"ReportUnit.exe");

            string[] nunitArgs = new List<string>
                {
                    // for details of options see  http://www.nunit.com/index.php?p=nunitliteOptions&r=3.0
                    "--verbose" // Tell me everything
                   ,"--result=" + fileInputXML
                    // , "--work=" + directoryName // save TestResults.xml to the build folder
                    // , "--wait" // Wait for input before closing console window (PAUSE). Comment this out for batch operations.
                }.ToArray();


            new AutoRun().Execute(nunitArgs);
            // https://github.com/nunit/nunit/commit/6331e7e694439f8cbf000156f138a3e10370ec40

            CreateHTMLReport(fileInputXML, fileOutputHTML, generatorReportUnit);
        }

        private void CreateHTMLReport(string pFileInputXML, string pFileOutputHTML, string pGeneratorReportUnit)
        {
            if (!File.Exists(pFileInputXML))
                return;

            if (File.Exists(pFileOutputHTML))
                File.Delete(pFileOutputHTML);

            string output = string.Empty;
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;

                    process.StartInfo.FileName = pGeneratorReportUnit;

                    StringBuilder param = new StringBuilder();
                    param.AppendFormat(" \"{0}\"", pFileInputXML);
                    param.AppendFormat(" \"{0}\"", pFileOutputHTML);
                    process.StartInfo.Arguments = param.ToString();

                    process.Start();

                    // read the output to return
                    // this will stop this execute until AutoCAD exits
                    StreamReader outputStream = process.StandardOutput;
                    output = outputStream.ReadToEnd();
                    outputStream.Close();
                }

            }
            catch (System.Exception ex)
            {
                output = ex.Message;
            }

        }
    }
}