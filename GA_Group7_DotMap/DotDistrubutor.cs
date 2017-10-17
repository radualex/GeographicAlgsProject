using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace GA_Group7_DotMap
{
    public class DotDistrubutor
    {
        private MapPainter _mapPainter;

        private List<Dot> _dots = new List<Dot>();
        private List<Dot> Dots { get { return _dots; } }
        private int _radius = Setting.BaseRaduis;

        public int NumberOfDotsPerGroup { get;private set; }

        public DotDistrubutor(MapPainter mapPainter)
        {
            _mapPainter = mapPainter;
            GenerateData();
        }

        #region generate the fake data.
        public void GenerateData()
        {
            _dots = new List<Dot>();
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
                    GenerateData(random, Convert.ToInt32(xy[0]), Convert.ToInt32(xy[1]), Convert.ToInt32(xy[2]));
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
                        for (int k = 0; k < _mapPainter.Blocks.Count; k++)
                        {
                            bool inSideEindhoven = false;
                            bool inSideSomeCityBlock = false;
                            var polygonPoints = new List<PointF>();
                            foreach (PointF point in _mapPainter.Blocks[k].Coordinates)
                            {
                                polygonPoints.Add(new PointF(point.X * Setting.Propotion, point.Y * Setting.Propotion));
                            }
                            var checkPoint = new PointF(i, j);
                            if (IsInPolygon(checkPoint, polygonPoints))
                            {
                                if (k != 0) inSideSomeCityBlock = true;
                                else inSideEindhoven = true;
                                if (inSideSomeCityBlock)
                                {
                                    File.AppendAllText("Data.txt", i + "," + j + "," + 1 + "#");
                                    GenerateData(random, i, j, 1);
                                }
                                else if(inSideEindhoven)
                                {
                                    File.AppendAllText("Data.txt", i + "," + j + "," + 0 + "#");
                                    GenerateData(random, i, j, 0);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void GenerateData(Random random, int i, int j, int inSideCityBlockss)
        {
            if ((random.Next(0, 200) > 100 && inSideCityBlockss == 1) || (inSideCityBlockss == 0 && random.Next(0, 102) > 100))
            {
                int region = 0;
                int r = random.Next(0, 100);
                if (r < Setting.WestPecrentage) region = 0;
                else if (r < Setting.WestPecrentage + Setting.NorthPercentage) region = 1;
                else if (r < Setting.WestPecrentage + Setting.SouthPercentage + Setting.NorthPercentage) region = 2;
                else if (r < Setting.WestPecrentage + Setting.SouthPercentage + Setting.NorthPercentage + Setting.Eastecerntage) region = 3;
                else region = 4;
                _dots.Add(new Dot((Region)region, new PointF((float)i / Setting.Propotion, (float)j / Setting.Propotion)));
            }
        }
        #endregion

        private List<AggregatedDot> ApplyAggregationAlgorithm(int width, int height, float ratio)
        {
            var dotsAfterAggregation = new List<AggregatedDot>();

            _radius = Math.Max(Setting.BaseRaduis, Math.Min(Setting.MaximumRaduis, Convert.ToInt32(Math.Min(width, height) / (Setting.BaseNumberOfGroupsPerLine * ratio * 4))));
            NumberOfDotsPerGroup = Convert.ToInt32(_dots.Count / ((Setting.BaseNumberOfGroupsPerLine * (ratio < 1 ? ratio * ratio : ratio)) * (Setting.BaseNumberOfGroupsPerLine * (ratio < 1 ? ratio * ratio : ratio)))) + 1;
            if (NumberOfDotsPerGroup == 1)
            {
                foreach (Dot dot in _dots)
                {
                    dotsAfterAggregation.Add(new AggregatedDot(dot, Math.Max(Setting.BaseRaduis, _radius)));
                }
                return dotsAfterAggregation; // if it has zoomed in a lot, then we do not need to apply the aggreagation algorithm.
            }

            ApplyAggregationAlgorithm(_dots, dotsAfterAggregation, 0, 1, 0, 1); // +1 = upper bound.

            return dotsAfterAggregation;
        }

        private void ApplyAggregationAlgorithm(List<Dot> dots, List<AggregatedDot> dotsAfterAggregation, double x1, double x2, double y1, double y2)
        {
            if (dots.Count > NumberOfDotsPerGroup)
            {
                List<List<Dot>> dotsArray = new List<List<Dot>>();
                for (int i = 0; i < 4; i++)
                {
                    dotsArray.Add(new List<Dot>());
                }
                double xmid = (x1 + x2) / 2;
                double ymid = (y1 + y2) / 2;
                foreach (Dot dot in dots)
                {
                    if (dot.Position.X >= x1 && dot.Position.X <= xmid && dot.Position.Y >= y1 && dot.Position.Y <= ymid) dotsArray[0].Add(dot);
                    else if (dot.Position.X >= xmid && dot.Position.X <= x2 && dot.Position.Y >= y1 && dot.Position.Y <= ymid) dotsArray[1].Add(dot);
                    else if (dot.Position.X >= x1 && dot.Position.X <= xmid && dot.Position.Y >= ymid && dot.Position.Y <= y2) dotsArray[2].Add(dot);
                    else if (dot.Position.X >= xmid && dot.Position.X <= x2 && dot.Position.Y >= ymid && dot.Position.Y <= y2) dotsArray[3].Add(dot);
                }
                ApplyAggregationAlgorithm(dotsArray[0], dotsAfterAggregation, x1, xmid, y1, ymid);
                ApplyAggregationAlgorithm(dotsArray[1], dotsAfterAggregation, xmid, x2, y1, ymid);
                ApplyAggregationAlgorithm(dotsArray[2], dotsAfterAggregation, x1, xmid, ymid, y2);
                ApplyAggregationAlgorithm(dotsArray[3], dotsAfterAggregation, xmid, x2, ymid, y2);
            }
            else
            {
                var dot = ApplyAggregationAlgorithm(dots);
                if (dot != null) dotsAfterAggregation.Add(dot);
            }
        }

        // Apply aggregation algoithm to this group of dots.
        private AggregatedDot ApplyAggregationAlgorithm(List<Dot> dots)
        {
            if (dots.Count > 1)
                return new AggregatedDot(new Dot(DetermineMainGroup(dots), dots[dots.Count / 2].Position), _radius);
            return null; 
        }

        private Region DetermineMainGroup(List<Dot> dots)
        {
            int west = 0;
            int east = 0;
            int south = 0;
            int north = 0;
            int noneu = 0;
            foreach (Dot dot in dots)
            {
                if (dot.Region == Region.West) west++;
                else if (dot.Region == Region.East) east++;
                else if (dot.Region == Region.North) north++;
                else if (dot.Region == Region.South) south++;
                else if (dot.Region == Region.NonEU) noneu++;
            }
            double westpercentage = Convert.ToDouble(west) / dots.Count / Math.Sqrt(Setting.WestPecrentage); // somehow this root makes the visulization more realistic.
            double eastpercentage = Convert.ToDouble(east) / dots.Count / Math.Sqrt(Setting.Eastecerntage);
            double southpercentage = Convert.ToDouble(south) / dots.Count / Math.Sqrt(Setting.SouthPercentage);
            double northpercentage = Convert.ToDouble(north) / dots.Count / Math.Sqrt(Setting.NorthPercentage);
            double noneupercentage = Convert.ToDouble(noneu) / dots.Count / Math.Sqrt(Setting.NonEuPercentage);

            var numbers = new List<double>();
            numbers.Add(westpercentage);
            numbers.Add(eastpercentage);
            numbers.Add(southpercentage);
            numbers.Add(northpercentage);
            numbers.Add(noneupercentage);

            var maxNumber = GetTheMaximumNumber(numbers);
            if (maxNumber == westpercentage) return Region.West;
            else if (maxNumber == eastpercentage) return Region.East;
            else if (maxNumber == southpercentage) return Region.South;
            else if (maxNumber == northpercentage) return Region.North;
            else if (maxNumber == noneupercentage) return Region.NonEU;
            else return Region.Error;
        }

        private double GetTheMaximumNumber(List<double> numbers)
        {
            double result = 0;
            foreach (double number in numbers)
            {
                if (number >= result) result = number;
            }
            return result;
        }

        public void DrawDots(int width, int height, Graphics graph, float ratio)
        {
            var dotsAfterAggregation = ApplyAggregationAlgorithm(width, height, ratio);
            for (int i = 0; i < dotsAfterAggregation.Count; i++)
            {
                dotsAfterAggregation[i].DrawDot(width, height, graph, ratio);
            }
        }
        
        public bool IsInPolygon(PointF checkPoint, List<PointF> polygonPoints)
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

    public class AggregatedDot
    {
        public Dot Dot { get;private set; }
        public int Raduis { get;private set; }

        public AggregatedDot(Dot dot, int raduis)
        {
            Dot = dot;
            Raduis = raduis;
        }
        
        public void DrawDot(int width, int height, Graphics graph, float ratio)
        {
            Dot.DrawDot(width, height, Raduis, graph, ratio);
        }
    }
}
