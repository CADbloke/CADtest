// CADtest.NET by CAD bloke. http://CADbloke.com - See License.txt for license details

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
#if (ACAD2006 || ACAD2007 || ACAD2008 || ACAD2009|| ACAD2010|| ACAD2011 || ACAD2012)
    using Autodesk.AutoCAD.ApplicationServices;
    using Application = Autodesk.AutoCAD.ApplicationServices.Application;
#else
using Autodesk.AutoCAD.ApplicationServices.Core;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
#endif
using Autodesk.AutoCAD.Geometry;

using CADtest.Helpers;
using NUnit.Framework;
using CADTestRunner;
using System.IO;
using System.Reflection;
using System.Collections;

namespace NUnitAutoCADTestRunner
{
    public class SampleTestsData
    {
        public static IEnumerable ParametersTest
        {
            get
            {
                string description = "Test the function Average Point";
                string category = "Geometry Functions";

                yield return new TestCaseData(
                 new Point3d(0, 0, 0),
                 new Point3d(1000, 1000, 1000),
                 new Point3d(500, 500, 500))
                 .SetDescription(description)
                 .SetCategory(category);

                yield return new TestCaseData(
                new Point3d(0, 0, 0),
                new Point3d(300, 300, 0),
                new Point3d(150, 150, 0))
                .SetDescription(description)
                .SetCategory(category);
            }
        }

    }
}
