// CADtest.NET by CAD bloke. http://CADbloke.com - See License.txt for license details

using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

// 

namespace CADtest.Helpers
{
    public class DbEntity // source unknown but probably http://www.theswamp.org/index.php?action=profile;u=935
    {
        /// <summary> The X Position of the next Entity to be Inserted. </summary>
        public static double Xpos;

        /// <summary> The Y Position of the next Entity to be Inserted. </summary>
        public static double Ypos;

        /// <summary> The Z Position of the next Entity to be Inserted. Zero is good for me. </summary>
        public static double Zpos = 0;

        /// <summary> How much to increment the X Position by after each insertion. Default is 0. </summary>
        public static double Xincrement = 0;

        /// <summary> How much to increment the Y Position by after each insertion. Default is Y. </summary>
        public static double Yincrement = 5;

        /// <summary> Adds a single AutoCAD <see cref="Entity" /> to Model space. </summary>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        /// <param name="entity"> The <see cref="Entity" />. </param>
        /// <param name="database"> The <see cref="Database" /> you are adding the <see cref="Entity" /> to. </param>
        /// <returns> the ObjectId of the <see cref="Entity" /> you just added. </returns>
        public static ObjectId AddToModelSpace <T>(T entity, Database database) where T: Entity
        {
            ObjectIdCollection objectIdCollection = AddToModelSpace(new List<T> { entity }, database);
            return objectIdCollection[0];
        }

        /// <summary> Adds a collection of AutoCAD <see cref="Entity" />s to Model space. </summary>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        /// <param name="entities"> The entities. </param>
        /// <param name="database"> The <see cref="Database" /> you are adding the <see cref="Entity" />s to. </param>
        /// <returns> a collection of ObjectId of the <see cref="Entity" />s you just added. </returns>
        public static ObjectIdCollection AddToModelSpace <T>(List<T> entities, Database database) where T: Entity
        {
            var objIdCollection = new ObjectIdCollection();
            if (entities == null)
                throw new ArgumentNullException("entities");
            if (database == null)
                throw new ArgumentNullException("database");

            TransactionManager transactionManager = database.TransactionManager;
            using (Transaction transaction = transactionManager.StartTransaction())
            {
                var blockTable = (BlockTable)transactionManager.GetObject(database.BlockTableId, OpenMode.ForRead, false);
                var modelSpace = (BlockTableRecord)transactionManager.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite, false);
                foreach (T entity in entities)
                {
                    objIdCollection.Add(modelSpace.AppendEntity(entity));

                    var text = entity as DBText;
                    if (text != null)
                    {
                        text.Position = new Point3d(Xpos, Ypos, Zpos);
                        incrementXY();
                    }
                    else
                    {
                        var mText = entity as MText;
                        if (mText != null)
                        {
                            mText.Location = new Point3d(Xpos, Ypos, Zpos);
                            incrementXY();
                        }
                    }

                    transactionManager.AddNewlyCreatedDBObject(entity, true);
                }
                transaction.Commit();
            }
            return objIdCollection;
        }

        /// <summary> Increment the X and Y Insertion points for the next entity. </summary>
        private static void incrementXY()
        {
            Xpos += Xincrement;
            Ypos += Yincrement;
        }
    }
}