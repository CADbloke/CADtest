// CADtest.NET by CAD bloke. http://CADbloke.com - See License.txt for license details

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices.Core;


namespace CADtest.Helpers
{
  /// <summary> Static Helper methods to work with Block Definitions aka <see cref="BlockTableRecord"/>s. </summary>
  public class BlockDefinition
  {
    /// <summary> The X Position of the next Entity to be Inserted. </summary>
    public static double Xpos = 0;
    /// <summary> The Y Position of the next Entity to be Inserted. </summary>
    public static double Ypos = 0;
    /// <summary> The Z Position of the next Entity to be Inserted. Zero is good for me. </summary>
    public static double Zpos = 0;
    /// <summary> How much to increment the X Position by after each insertion. Default is 0. </summary>
    public static double Xincrement = 0;
    /// <summary> How much to increment the Y Position by after each insertion. Default is Y. </summary>
    public static double Yincrement = 5;


    /// <summary> Adds Entities (text, Attribute Definitions etc)to the block definition. </summary>
    /// <param name="blockDefinitionObjectId"> <see cref="ObjectId"/> for the block definition object. </param>
    /// <param name="entities">                The entities to add to the block. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    /// <exception cref="ArgumentNullException">The value of 'blockDefinitionObjectId' cannot be null. </exception>
    public static bool AddEntitiesToBlockDefinition<T>(ObjectId blockDefinitionObjectId, ICollection<T> entities) where T: Entity
    {
      if (blockDefinitionObjectId == null) { throw new ArgumentNullException("blockDefinitionObjectId"); }
      if (!blockDefinitionObjectId.IsValid) { return false;}
      if (entities == null) { throw new ArgumentNullException("entities"); }
      if (entities.Count<1) { return false; }
      
      Xpos = 0;
      Ypos = 0;
      bool workedOk = false;
      Database database = blockDefinitionObjectId.Database;
      TransactionManager transactionManager = database.TransactionManager;

      using (Transaction transaction = transactionManager.StartTransaction())
      {
        if (blockDefinitionObjectId.ObjectClass == (RXObject.GetClass(typeof (BlockTableRecord))))
        {
          BlockTableRecord blockDefinition =
            (BlockTableRecord) transactionManager.GetObject(blockDefinitionObjectId, OpenMode.ForWrite, false);
          if (blockDefinition != null)
          {
            foreach (T entity in entities)
            {
              DBText text = entity as DBText;
              if (text != null)   { text.Position = new Point3d(Xpos, Ypos, Zpos); incrementXY(); }
              else
              {
              MText mText = entity as MText;
              if (mText != null) { mText.Location = new Point3d(Xpos, Ypos, Zpos); incrementXY(); }
              }

              blockDefinition.AppendEntity(entity); // todo: vertical spacing ??

              transactionManager.AddNewlyCreatedDBObject(entity, true);
            }
            workedOk = true;
          }
        }
        transaction.Commit();
      }
      return workedOk;
    }

    /// <summary> Increment the X and Y Insertion points for the next entity. </summary>
    private static void incrementXY()
    {
      Xpos += Xincrement;
      Ypos += Yincrement;
    }


    /// <summary> Adds a single Entity (text, Attribute Definition etc)to the block definition. </summary>
    /// <param name="blockDefinitionObjectId"> <see cref="ObjectId"/> for the block definition object. </param>
    /// <param name="entity">                  The entity. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    public static bool AddEntityToBlockDefinition<T>(ObjectId blockDefinitionObjectId, T entity) where T: Entity
    {
      return AddEntitiesToBlockDefinition<T>(blockDefinitionObjectId, new List<T> {entity});
    }



    /// <summary> Adds a new block defintion by name. If it already exists then it returns <c>ObjectId.Null</c>. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when blockDefinitionName is null or empty. </exception>
    /// <param name="blockDefinitionName"> Name of the block definition. </param>
    /// <returns> An ObjectId - new if created or existing if the block already exists. </returns>
    public static ObjectId AddNewBlockDefintion(string blockDefinitionName)
    {
      if (string.IsNullOrEmpty(blockDefinitionName)) { throw new ArgumentNullException("blockDefinitionName"); }
      Database database = Application.DocumentManager.MdiActiveDocument.Database;
      TransactionManager transactionManager = database.TransactionManager;
      ObjectId newBlockId;

      using (Transaction transaction = transactionManager.StartTransaction())
      {
        BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForWrite) as BlockTable;
        if (blockTable != null && blockTable.Has(blockDefinitionName)) { return ObjectId.Null; }

        using (BlockTableRecord blockTableRecord = new BlockTableRecord())
        {
          blockTableRecord.Name = blockDefinitionName;
          blockTableRecord.Origin = new Point3d(0, 0, 0);
          using (Circle circle = new Circle())
          {
            circle.Center = new Point3d(0, 0, 0);
            circle.Radius = 2;
            blockTableRecord.AppendEntity(circle);
            if (blockTable != null) { blockTable.Add(blockTableRecord); }
            transaction.AddNewlyCreatedDBObject(blockTableRecord, true);
            newBlockId = blockTableRecord.Id;
          }
        }
        transaction.Commit();
      }
      return newBlockId;
    }


    /// <summary> Gets existing block defintion by name. If it doesn't exist then it returns <c>ObjectId.Null</c> </summary>
    /// <exception cref="ArgumentNullException"> Thrown when blockDefinitionName is null or empty. </exception>
    /// <param name="blockDefinitionName"> Name of the block definition. </param>
    /// <returns> An ObjectId - if the block already exists. . If it doesn't exist then it returns <c>ObjectId.Null</c>. </returns>
    public static ObjectId GetExistingBlockDefintion(string blockDefinitionName)
    {
      if (string.IsNullOrEmpty(blockDefinitionName)) { throw new ArgumentNullException("blockDefinitionName"); }
      Database database = Application.DocumentManager.MdiActiveDocument.Database;
      TransactionManager transactionManager = database.TransactionManager;

      using (Transaction transaction = transactionManager.StartTransaction())
      {
        BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForWrite) as BlockTable;
        if (blockTable != null && blockTable.Has(blockDefinitionName)) { return blockTable[blockDefinitionName]; }
      }
      return ObjectId.Null;
    }


    /// <summary> Adds a new or gets existing block defintion by name. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when blockDefinitionName is null or empty. </exception>
    /// <param name="blockDefinitionName"> Name of the block definition. </param>
    /// <returns> An ObjectId - new if created or existing if the block already exists. </returns>
    public static ObjectId AddNewOrGetExistingBlockDefintion(string blockDefinitionName)
    {
      if (string.IsNullOrEmpty(blockDefinitionName)) { throw new ArgumentNullException("blockDefinitionName"); }

      ObjectId newBlockId = GetExistingBlockDefintion(blockDefinitionName);
      if (newBlockId != ObjectId.Null) return newBlockId;

      return AddNewBlockDefintion(blockDefinitionName);
    }
  }
}
