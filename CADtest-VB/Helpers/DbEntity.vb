' CADtest.NET by CAD bloke. http://CADbloke.com - See License.txt for license details
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry


Namespace Helpers
    Public Class DbEntity
        ' source unknown but probably http://www.theswamp.org/index.php?action=profile;u=935
        ''' <summary> The X Position of the next Entity to be Inserted. </summary>
        Public Shared Xpos As Double = 0

        ''' <summary> The Y Position of the next Entity to be Inserted. </summary>
        Public Shared Ypos As Double = 0

        ''' <summary> The Z Position of the next Entity to be Inserted. Zero is good for me. </summary>
        Public Shared Zpos As Double = 0

        ''' <summary> How much to increment the X Position by after each insertion. Default is 0. </summary>
        Public Shared Xincrement As Double = 0

        ''' <summary> How much to increment the Y Position by after each insertion. Default is Y. </summary>
        Public Shared Yincrement As Double = 5


        ''' <summary> Adds a single AutoCAD <see cref="Autodesk.AutoCAD.DatabaseServices.Entity" /> to Model space. </summary>
        ''' <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        ''' <param name="entity"> The <see cref="Autodesk.AutoCAD.DatabaseServices.Entity" />. </param>
        ''' <param name="database"> The <see cref="Autodesk.AutoCAD.DatabaseServices.Database" /> you are adding the
        '''     <see cref="Autodesk.AutoCAD.DatabaseServices.Entity" /> to. </param>
        ''' <returns> the ObjectId of the <see cref="Autodesk.AutoCAD.DatabaseServices.Entity" /> you just added. </returns>
        Public Shared Function AddToModelSpace (Of T As Entity)(entity As T, database As Database) As ObjectId
            Dim objectIdCollection As ObjectIdCollection = AddToModelSpace(New List(Of T)() From { _
                                                                              entity _
                                                                              },
                                                                           database)
            Return objectIdCollection(0)
        End Function


        ''' <summary> Adds a collection of AutoCAD <see cref="Entity" />s to Model space. </summary>
        ''' <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        ''' <param name="entities"> The entities. </param>
        ''' <param name="database"> The <see cref="Database" /> you are adding the <see cref="Entity" />s to. </param>
        ''' <returns> a collection of ObjectId of the <see cref="Entity" />s you just added. </returns>
        Public Shared Function AddToModelSpace (Of T As Entity)(entities As List(Of T), database As Database) _
            As ObjectIdCollection
            Dim objIdCollection As New ObjectIdCollection()
            If entities Is Nothing Then
                Throw New ArgumentNullException("entities")
            End If
            If database Is Nothing Then
                Throw New ArgumentNullException("database")
            End If

            Dim transactionManager As TransactionManager = database.TransactionManager
            Using transaction As Transaction = transactionManager.StartTransaction()
                Dim blockTable = DirectCast(transactionManager.GetObject(database.BlockTableId, OpenMode.ForRead, False), BlockTable)
                Dim modelSpace = DirectCast(transactionManager.GetObject(blockTable(BlockTableRecord.ModelSpace),
                                                                         OpenMode.ForWrite,
                                                                         False),
                                            BlockTableRecord)
                For Each entity As T In entities
                    objIdCollection.Add(modelSpace.AppendEntity(entity))

                    Dim text = TryCast(entity, DBText)
                    If text IsNot Nothing Then
                        text.Position = New Point3d(Xpos, Ypos, Zpos)
                        incrementXY()
                    Else
                        Dim mText = TryCast(entity, MText)
                        If mText IsNot Nothing Then
                            mText.Location = New Point3d(Xpos, Ypos, Zpos)
                            incrementXY()
                        End If
                    End If

                    transactionManager.AddNewlyCreatedDBObject(entity, True)
                Next
                transaction.Commit()
            End Using
            Return objIdCollection
        End Function


        ''' <summary> Increment the X and Y Insertion points for the next entity. </summary>
        Private Shared Sub incrementXY()
            Xpos += Xincrement
            Ypos += Yincrement
        End Sub
    End Class
End Namespace
