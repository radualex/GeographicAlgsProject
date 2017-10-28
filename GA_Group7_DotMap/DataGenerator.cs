using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace GA_Group7_DotMap
{
    // Generate fake data.
    // return a List<Dot>, where a dot means a person.
    // and for each dot, it contains position and region.
    public class DataGenerator
    {
        /// <summary>
        /// This method generates positions.
        /// basically, for each position, it can possibly contians 1 person.
        /// total number of people is defined by Setting.Propotion * Setting.Propotion
        /// Note that for each position, it is not necessary to contains a person.
        /// For each position, it has its own possibility that it contains a person.
        /// </summary>
        public void GenerateData(List<Dot> dots, MapPainter mapPainter)
        {
            var random = new Random();
            // If there is already raw data, then read the raw data from the disk.
            if (File.Exists("Data.txt"))
            {
                string data = File.ReadAllText("Data.txt");
                var splits = data.Split('#');
                var splitsList = splits.ToList();
                splitsList.RemoveAt(splitsList.Count - 1);
                foreach (string coordination in splitsList)
                {
                    var xy = coordination.Split(',');
                    GeneratePeople(dots, random, Convert.ToInt32(xy[0]), Convert.ToInt32(xy[1]), Convert.ToInt32(xy[2]));
                }
            }
            // Otherwise create raw data.
            else
            {
                // Raw data, Propotion * Propotion positions, each position can possibly contains 1 person.
                for (int i = 0; i < Setting.Propotion; i++)
                {
                    for (int j = 0; j < Setting.Propotion; j++)
                    {
                        for (int k = 0; k < mapPainter.Blocks.Count; k++)
                        {
                            bool inSideEindhoven = false;
                            bool inSideSomeCityBlock = false;
                            var polygonPoints = new List<PointF>();
                            foreach (PointF point in mapPainter.Blocks[k].Coordinates)
                            {
                                polygonPoints.Add(new PointF(point.X * Setting.Propotion, point.Y * Setting.Propotion));
                            }
                            var checkPoint = new PointF(i, j);
                            // for now, we only give 2 different values, 1 or 0, to indicate whether this position is in the busy area.
                            // 1 means this dot is in busy area, 0 means not.
                            if (IsInPolygon(checkPoint, polygonPoints))
                            {
                                if (k != 0) inSideSomeCityBlock = true;
                                else inSideEindhoven = true;
                                if (inSideSomeCityBlock)
                                {
                                    File.AppendAllText("Data.txt", i + "," + j + "," + 1 + "#");
                                    GeneratePeople(dots, random, i, j, 1);
                                }
                                else if (inSideEindhoven)
                                {
                                    File.AppendAllText("Data.txt", i + "," + j + "," + 0 + "#");
                                    GeneratePeople(dots, random, i, j, 0);
                                }
                            }
                        }
                    }
                }
            }
        }

        // This method generates people based on the positions.
        private void GeneratePeople(List<Dot> dots, Random random, int i, int j, int inSideCityBlockss)
        {
            // if this position is not in the busy area.
            // we give 2% possibility that there is a person.
            if ((random.Next(0, 200) > 100 && inSideCityBlockss == 1) || (inSideCityBlockss == 0 && random.Next(0, 100) > 98))
            {
                int region = 1;
                int r = random.Next(0, 100);
                if (r < Setting.WestPecrentage) region = 1;
                else if (r < Setting.WestPecrentage + Setting.NorthPercentage) region = 2;
                else if (r < Setting.WestPecrentage + Setting.SouthPercentage + Setting.NorthPercentage) region = 3;
                else if (r < Setting.WestPecrentage + Setting.SouthPercentage + Setting.NorthPercentage + Setting.EastPercentage) region = 4;
                else region = 5;
                dots.Add(new Dot((Region)region, new PointF((float)i / Setting.Propotion, (float)j / Setting.Propotion)));
            }
        }

        // To determine whether a dot is inside a polygon or not.
        private static bool IsInPolygon(PointF checkPoint, List<PointF> polygonPoints)
        {
            int counter = 0;
            int i;
            double xinters;
            PointF p1, p2;
            int pointCount = polygonPoints.Count;
            p1 = polygonPoints[0];
            for (i = 1; i <= pointCount; i++)
            {
                p2 = polygonPoints[i % pointCount];
                if (checkPoint.Y > Math.Min(p1.Y, p2.Y) && checkPoint.Y <= Math.Max(p1.Y, p2.Y))
                {
                    if (checkPoint.X <= Math.Max(p1.X, p2.X))
                    {
                        if (p1.Y != p2.Y)
                        {
                            xinters = (checkPoint.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;
                            if (p1.X == p2.X || checkPoint.X <= xinters)
                            {
                                counter++;
                            }
                        }
                    }
                }
                p1 = p2;
            }

            if (counter % 2 == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}