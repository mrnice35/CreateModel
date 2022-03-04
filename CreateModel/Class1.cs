using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateModel


{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CreationModel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            var level1 = FoundLevel(doc, "Уровень 1");
            var level2 = FoundLevel(doc, "Уровень 2");
            CreateWall(doc, level1, level2);
            return Result.Succeeded;
        }


        public Level FoundLevel(Document doc, string str)   //метод поиска уровня
        {
            List<Level> listLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            var level = listLevel
                .Where(x => x.Name.Equals(str))
                .FirstOrDefault();

            return level;
        }

        public List<Wall> CreateWall(Document doc, Level lev1, Level lev2) //метод ссоздания стены
        {

            var level1 = lev1;
            var level2 = lev2;
            double width = UnitUtils.ConvertToInternalUnits(10000, UnitTypeId.Millimeters);
            double depth = UnitUtils.ConvertToInternalUnits(5000, UnitTypeId.Millimeters);
            double dx = width / 2;
            double dy = depth / 2;

            List<XYZ> points = new List<XYZ>();
            points.Add(new XYZ(-dx, -dy, 0));
            points.Add(new XYZ(dx, -dy, 0));
            points.Add(new XYZ(dx, dy, 0));
            points.Add(new XYZ(-dx, dy, 0));
            points.Add(new XYZ(-dx, -dy, 0));

            List<Wall> wallList = new List<Wall>();

            Transaction transaction = new Transaction(doc, "Создание стен");
            transaction.Start();
            for (int i = 0; i < 4; i++)
            {
                Line line = Line.CreateBound(points[i], points[i + 1]);
                Wall wall = Wall.Create(doc, line, level1.Id, false);
                wallList.Add(wall);
                wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level2.Id);
            }
            transaction.Commit();
            return wallList;
        }
    }
}