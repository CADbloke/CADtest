using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
#if (ACAD2006 || ACAD2007 || ACAD2008 || ACAD2009|| ACAD2010|| ACAD2011 || ACAD2012)
    using Autodesk.AutoCAD.ApplicationServices;
    using Application = Autodesk.AutoCAD.ApplicationServices.Application;
#else
using Autodesk.AutoCAD.ApplicationServices.Core;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
#endif

namespace CADtest.Helpers
{
    internal sealed class WorkingDatabaseSwitcher : IDisposable
    {
        Database _oldDb = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="db">Target database.</param>
        public WorkingDatabaseSwitcher(Database db)
        {
            _oldDb = HostApplicationServices.WorkingDatabase;
            HostApplicationServices.WorkingDatabase = db;
        }

        public void Dispose()
        {
            HostApplicationServices.WorkingDatabase = _oldDb;
        }
    }
}
