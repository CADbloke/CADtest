' CADtest.NET by CAD bloke. http://CADbloke.com - See License.txt for license details
Imports Autodesk.AutoCAD.ApplicationServices.Core
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.Runtime

Namespace Helpers
    ''' <summary> Static Helper methods to work with Block Definitions aka
    '''     <see cref="Autodesk.AutoCAD.DatabaseServices.BlockTableRecord" />s. </summary>
    Public Class BlockDefinition
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


        ''' <summary> Adds Entities (text, Attribute Definitions etc)to the block definition. </summary>
        ''' <param name="blockDefinitionObjectId"> <see cref="Autodesk.AutoCAD.DatabaseServices.ObjectId" /> for the block
        '''     definition object. </param>
        ''' <param name="entities"> The entities to add to the block. </param>
        ''' <returns> true if it succeeds, false if it fails. </returns>
        ''' <exception cref="ArgumentNullException"> The value of 'blockDefinitionObjectId' cannot be null. </exception>
        Public Shared Function AddEntitiesToBlockDefinition (Of T As Entity)(blockDefinitionObjectId As ObjectId,
                                                                             entities As ICollection(Of T)) As Boolean
            If blockDefinitionObjectId = ObjectId.Null Then
                Throw New ArgumentNullException("blockDefinitionObjectId")
            End If
            If Not blockDefinitionObjectId.IsValid Then
                Return False
            End If
            If entities Is Nothing Then
                Throw New ArgumentNullException("entities")
            End If
            If entities.Count < 1 Then
                Return False
            End If

            Xpos = 0
            Ypos = 0
            Dim workedOk = False
            Dim database As Database = blockDefinitionObjectId.Database
            Dim transactionManager As TransactionManager = database.TransactionManager

            Using transaction As Transaction = transactionManager.StartTransaction()
                If blockDefinitionObjectId.ObjectClass = (RXObject.GetClass(GetType(BlockTableRecord))) Then
                    Dim blockDefinition = DirectCast(transactionManager.GetObject(blockDefinitionObjectId,
                                                                                  OpenMode.ForWrite,
                                                                                  False),
                                                                                  BlockTableRecord)
                    If blockDefinition IsNot Nothing Then
                        For Each entity As T In entities
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

                            blockDefinition.AppendEntity(entity)
                            ' todo: vertical spacing ??
                            transactionManager.AddNewlyCreatedDBObject(entity, True)
                        Next
                        workedOk = True
                    End If
                End If
                transaction.Commit()
            End Using
            Return workedOk
        End Function

        ''' <summary> Increment the X and Y Insertion points for the next entity. </summary>
        Private Shared Sub incrementXY()
            Xpos += Xincrement
            Ypos += Yincrement
        End Sub


        ''' <summary> Adds a single Entity (text, Attribute Definition etc)to the block definition. </summary>
        ''' <param name="blockDefinitionObjectId"> <see cref="ObjectId" /> for the block definition object. </param>
        ''' <param name="entity"> The entity. </param>
        ''' <returns> true if it succeeds, false if it fails. </returns>
        Public Shared Function AddEntityToBlockDefinition (Of T As Entity)(blockDefinitionObjectId As ObjectId,
                                                                           entity As T) As Boolean
            Return AddEntitiesToBlockDefinition (Of T)(blockDefinitionObjectId,
                                                       New List(Of T)() From { _
                                                          entity _
                                                          })
        End Function


        ''' <summary> Adds a new block defintion by name. If it already exists then it returns <c> ObjectId.Null </c>. </summary>
        ''' <exception cref="ArgumentNullException"> Thrown when blockDefinitionName is null or empty. </exception>
        ''' <param name="blockDefinitionName"> Name of the block definition. </param>
        ''' <returns> An ObjectId - new if created or existing if the block already exists. </returns>
        Public Shared Function AddNewBlockDefintion(blockDefinitionName As String) As ObjectId
            If String.IsNullOrEmpty(blockDefinitionName) Then
                Throw New ArgumentNullException("blockDefinitionName")
            End If
            Dim database As Database = Application.DocumentManager.MdiActiveDocument.Database
            Dim transactionManager As TransactionManager = database.TransactionManager
            Dim newBlockId As ObjectId

            Using transaction As Transaction = transactionManager.StartTransaction()
                Dim blockTable = TryCast(transaction.GetObject(database.BlockTableId, OpenMode.ForWrite), BlockTable)
                If blockTable IsNot Nothing AndAlso blockTable.Has(blockDefinitionName) Then
                    Return ObjectId.Null
                End If

                Using blockTableRecord As New BlockTableRecord()
                    blockTableRecord.Name = blockDefinitionName
                    blockTableRecord.Origin = New Point3d(0, 0, 0)
                    Using circle As New Circle()
                        circle.Center = New Point3d(0, 0, 0)
                        circle.Radius = 2
                        blockTableRecord.AppendEntity(circle)
                        If blockTable IsNot Nothing Then
                            blockTable.Add(blockTableRecord)
                        End If
                        transaction.AddNewlyCreatedDBObject(blockTableRecord, True)
                        newBlockId = blockTableRecord.Id
                    End Using
                End Using
                transaction.Commit()
            End Using
            Return newBlockId
        End Function


        ''' <summary> Gets existing block defintion by name. If it doesn't exist then it returns <c> ObjectId.Null </c> </summary>
        ''' <exception cref="ArgumentNullException"> Thrown when blockDefinitionName is null or empty. </exception>
        ''' <param name="blockDefinitionName"> Name of the block definition. </param>
        ''' <returns> An ObjectId - if the block already exists. . If it doesn't exist then it returns <c> ObjectId.Null </c>. </returns>
        Public Shared Function GetExistingBlockDefintion(blockDefinitionName As String) As ObjectId
            If String.IsNullOrEmpty(blockDefinitionName) Then
                Throw New ArgumentNullException("blockDefinitionName")
            End If
            Dim database As Database = Application.DocumentManager.MdiActiveDocument.Database
            Dim transactionManager As TransactionManager = database.TransactionManager

            Using transaction As Transaction = transactionManager.StartTransaction()
                Dim blockTable = TryCast(transaction.GetObject(database.BlockTableId, OpenMode.ForWrite), BlockTable)
                If blockTable IsNot Nothing AndAlso blockTable.Has(blockDefinitionName) Then
                    Return blockTable(blockDefinitionName)
                End If
            End Using
            Return ObjectId.Null
        End Function


        ''' <summary> Adds a new or gets existing block defintion by name. </summary>
        ''' <exception cref="ArgumentNullException"> Thrown when blockDefinitionName is null or empty. </exception>
        ''' <param name="blockDefinitionName"> Name of the block definition. </param>
        ''' <returns> An ObjectId - new if created or existing if the block already exists. </returns>
        Public Shared Function AddNewOrGetExistingBlockDefintion(blockDefinitionName As String) As ObjectId
            If String.IsNullOrEmpty(blockDefinitionName) Then
                Throw New ArgumentNullException("blockDefinitionName")
            End If

            Dim newBlockId As ObjectId = GetExistingBlockDefintion(blockDefinitionName)
            If newBlockId <> ObjectId.Null Then
                Return newBlockId
            End If

            Return AddNewBlockDefintion(blockDefinitionName)
        End Function
    End Class
End Namespace
