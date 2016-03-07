' CADtest.NET by CAD bloke. http://CADbloke.com - See License.txt for license details

Imports System.Threading
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports CADtest.Helpers
Imports NUnit.Framework
#If (ACAD2006 OrElse ACAD2007 OrElse ACAD2008 OrElse ACAD2009 OrElse ACAD2010 OrElse ACAD2011 OrElse ACAD2012) Then
Imports Autodesk.AutoCAD.ApplicationServices
Imports Application = Autodesk.AutoCAD.ApplicationServices.Application
#Else
Imports Application = Autodesk.AutoCAD.ApplicationServices.Core.Application
#End If

Namespace NUnitAutoCADTestRunner
    <TestFixture, Apartment(ApartmentState.STA)>
    Public Class TestClass1
        <Test>
        Public Sub Test_method_should_pass()
            Assert.Pass("Test that should pass did not pass")
        End Sub


        <Test>
        Public Sub Test_method_should_fail()
            Assert.Fail("Test was supposed to fail.")
        End Sub

        <Test>
        Public Sub Test_method_name()
            ' Arrange
            Dim db As Database = HostApplicationServices.WorkingDatabase
            Dim doc As Document = Application.DocumentManager.GetDocument(db)
            Dim dbText As New DBText()
            dbText.TextString = "cat"

            Dim testMe As String
            Dim dbTextObjectID As ObjectId
            ' Act
            Using doc.LockDocument()
                Using transaction As Transaction = db.TransactionManager.StartTransaction()
                    dbTextObjectID = DbEntity.AddToModelSpace(dbText, db)
                    dbText.TextString = "dog"
                    transaction.Commit()
                End Using

                Dim testText = TryCast(dbTextObjectID.Open(OpenMode.ForRead, False), DBText)
                testMe = If(testText IsNot Nothing, testText.TextString, String.Empty)
            End Using
            ' Assert
            StringAssert.AreEqualIgnoringCase("dog", testMe, "DBText string was not changed to dog.")
            StringAssert.AreNotEqualIgnoringCase("cat", testMe, "DBText string was not changed.")
        End Sub
    End Class
End Namespace
