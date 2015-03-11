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
using CADtest.Helpers;
using NUnit.Framework;

namespace NUnitAutoCADTestRunner

{
  [TestFixture, Apartment(ApartmentState.STA)]
  public class TestClass1
  {
    [Test]
    public void Test_method_should_pass()
    {
      Assert.Pass("Test that should pass did not pass");
    }


    [Test]
    public void Test_method_should_fail()
    {
      Assert.Fail("Test was supposed to fail.");
    }
    
    [Test]
    public void Test_method_name()
    {
// Arrange
      Database db = HostApplicationServices.WorkingDatabase;
      Document doc = Application.DocumentManager.GetDocument(db);
      DBText dbText = new DBText {TextString = "cat"};
      string testMe;
      ObjectId dbTextObjectID; 
// Act
      using (doc.LockDocument())
      { 
        using (Transaction transaction = db.TransactionManager.StartTransaction())
        {
          dbTextObjectID = DbEntity.AddToModelSpace(dbText, db);
          dbText.TextString = "dog";
          transaction.Commit();
        }
#pragma warning disable 618
        DBText testText =  dbTextObjectID.Open(OpenMode.ForRead, false) as DBText;
#pragma warning restore 618
        testMe = testText != null ? testText.TextString : string.Empty;
// Assert
        StringAssert.AreEqualIgnoringCase("dog", testMe, "Failed String Assertion");
      }
    }
  }
}