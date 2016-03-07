using System;
using System.Collections.Generic;
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
using CADtest.Helpers;
using NUnit.Framework;
using System.IO;

namespace CADTestRunner
{
    public abstract class BaseTests
    {
        protected void ExecuteActionDWG(String pDrawingPath, params Action<Database, Transaction>[] pActionList)
        {
            bool buildDefaultDrawing;

            if (String.IsNullOrEmpty(pDrawingPath))
            {
                buildDefaultDrawing = true;
            }
            else
            {
                buildDefaultDrawing = false;

                if (!File.Exists(pDrawingPath))
                {
                    Assert.Fail("The file '{0}' doesn't exist", pDrawingPath);
                    return;
                }
            }

            Exception exceptionToThrown = null;

            Document doc = Application.DocumentManager.MdiActiveDocument;
            using (doc.LockDocument())
            {
                using (Database db = new Database(buildDefaultDrawing, false))
                {
                    if (!String.IsNullOrEmpty(pDrawingPath))
                        db.ReadDwgFile(pDrawingPath, FileOpenMode.OpenForReadAndWriteNoShare, true, null);

                    using (new WorkingDatabaseSwitcher(db))
                    {
                        foreach (var item in pActionList)
                        {
                            using (Transaction tr = db.TransactionManager.StartTransaction())
                            {
                                try
                                {
                                    item(db, tr);
                                }
                                catch (Exception ex)
                                {
                                    exceptionToThrown = ex;
                                    tr.Commit();

                                    //stop execution of actions
                                    break;
                                }

                                tr.Commit();

                            }//transaction

                        }//foreach

                    }//change WorkingDatabase

                }//database

            }//lock document

            //throw exception outside of transaction
            //Sometimes Autocad crashes when exception throws
            if (exceptionToThrown != null)
            { 
                throw exceptionToThrown;
            }
        }

    }
}
