using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace GA_Group7_DotMap
{
    /// <summary>
    /// This class determines how persons are distributed.
    /// This class also generates some fake data.
    /// This is also the class where aggregation algorithm is applied.
    /// </summary>
    public class DotDistrubutor
    {
        private MapPainter _mapPainter;

        private List<Dot> _dots = new List<Dot>();
        private List<AggregatedDot> _dotsAfterAggregation = new List<AggregatedDot>();

        private List<Dot> Dots { get { return _dots; } }

        private int _radius = Setting.BaseRadius;
        private int _width = 0;
        private int _height = 0;
        private float _ratio = 1;
        
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
                else if (r < Setting.WestPecrentage + Setting.SouthPercentage + Setting.NorthPercentage + Setting.EastPercentage) region = 3;
                else region = 4;
                _dots.Add(new Dot((Region)region, new PointF((float)i / Setting.Propotion, (float)j / Setting.Propotion)));
            }
        }

        #endregion

        private void ApplyAggregationAlgorithm(int width, int height, float ratio)
        {
            _width = width;
            _height = height;
            _ratio = ratio;
            _radius = Math.Max(Setting.BaseRadius, Convert.ToInt32(Math.Min(_width, _height) / (Setting.BaseNumberOfGroupsPerLine * _ratio * 5))); // divided by 5 for better display.
            int newNumberOfDotsPerGroup = Convert.ToInt32(_dots.Count / ((Setting.BaseNumberOfGroupsPerLine * (_ratio < 1 ? _ratio * _ratio : _ratio)) * (Setting.BaseNumberOfGroupsPerLine * (_ratio < 1 ? _ratio * _ratio : _ratio)))) + 1;
            newNumberOfDotsPerGroup = RoundNumberOfDotsPerGroup(newNumberOfDotsPerGroup);
            if (NumberOfDotsPerGroup == newNumberOfDotsPerGroup) return;
            NumberOfDotsPerGroup = newNumberOfDotsPerGroup;
            _dotsAfterAggregation = new List<AggregatedDot>();
            if (NumberOfDotsPerGroup == 1) // if it has zoomed in a lot, then we do not need to apply the aggreagation algorithm.
            {
                foreach (Dot dot in _dots)
                {
                    _dotsAfterAggregation.Add(new AggregatedDot(dot, Math.Max(Setting.BaseRadius, _radius)));
                }
                return;
            }
            ApplyAggregationAlgorithm(_dots, 0, 1, 0, 1);
        }

        private void ApplyAggregationAlgorithm(List<Dot> dots, double x1, double x2, double y1, double y2)
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
                ApplyAggregationAlgorithm(dotsArray[0], x1, xmid, y1, ymid);
                ApplyAggregationAlgorithm(dotsArray[1], xmid, x2, y1, ymid);
                ApplyAggregationAlgorithm(dotsArray[2], x1, xmid, ymid, y2);
                ApplyAggregationAlgorithm(dotsArray[3], xmid, x2, ymid, y2);
            }
            else
            {
                var dotsInOneGroup = ApplyAggregationAlgorithm(dots);
                if (dotsInOneGroup != null) _dotsAfterAggregation.AddRange(dotsInOneGroup);
            }
        }

        // Apply aggregation algoithm to this group of dots.
        private List<AggregatedDot> ApplyAggregationAlgorithm(List<Dot> dots)
        {
            var returnDots = new List<AggregatedDot>();
            if (dots.Count > NumberOfDotsPerGroup * Setting.MinimumPercentageToShow)
            {
                returnDots.AddRange(DetermineMainGroupSolution1(dots));
                return returnDots;
            }
            return null;
        }

        private List<AggregatedDot> DetermineMainGroupSolution1(List<Dot> dots)
        {
            List<AggregatedDot> result = new List<AggregatedDot>();
            Region mainGroup = Region.Error;

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
            double westpercentage = Convert.ToDouble(west) / dots.Count / Math.Sqrt(Setting.WestPecrentage); // somehow this root makes the result more realistic.
            double eastpercentage = Convert.ToDouble(east) / dots.Count / Math.Sqrt(Setting.EastPercentage);
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
            if (maxNumber == westpercentage) mainGroup = Region.West;
            else if (maxNumber == eastpercentage) mainGroup = Region.East;
            else if (maxNumber == southpercentage) mainGroup = Region.South;
            else if (maxNumber == northpercentage) mainGroup = Region.North;
            else if (maxNumber == noneupercentage) mainGroup = Region.NonEU;

            result.Add(new AggregatedDot(new Dot(mainGroup, ResolvePossibleOverLap(dots[dots.Count / 2].Position)), _radius));
            //result.Add(new AggregatedDot(new Dot(mainGroup, dots[dots.Count / 2].Position), _radius));

            return result;
        }

        // Slow wrost case. O(n)
        private PointF ResolvePossibleOverLap(PointF poition)
        {
            var tempPosition = poition;
            foreach (AggregatedDot dot in _dotsAfterAggregation)
            {
                float distanceBetweenCenter = (float)Math.Sqrt(Math.Pow(dot.Dot.Position.X * _ratio * _width - tempPosition.X * _ratio * _width, 2) + Math.Pow(dot.Dot.Position.Y * _ratio * _height - tempPosition.Y * _ratio * _height, 2));
                distanceBetweenCenter = (float)Math.Round(distanceBetweenCenter, 0);
                if (distanceBetweenCenter < dot.Raduis)
                {
                    float propotion = dot.Raduis / distanceBetweenCenter;
                    tempPosition = new PointF(dot.Dot.Position.X + (tempPosition.X - dot.Dot.Position.X) * propotion, dot.Dot.Position.Y + (tempPosition.Y - dot.Dot.Position.Y) * propotion);
                }
            }
            return tempPosition;
        }

        // for instance, numberOfDotsPerGroup = 10345, return value will be 10000.
        // numberOfDotsPerGroup = 1245, return value will be 1000.
        // numberOfDotsPerGroup = 6, return value will be 6.
        private int RoundNumberOfDotsPerGroup(int numberOfDotsPerGroup)
        {
            if (numberOfDotsPerGroup < 10) return numberOfDotsPerGroup;
            return Convert.ToInt32(Convert.ToInt32(numberOfDotsPerGroup.ToString().Substring(0, 1)) * Math.Pow(10, numberOfDotsPerGroup.ToString().Length - 1));
        }

        private PointF CalculateMiddlePosition(List<Dot> dots)
        {
            PointF position = new PointF(0, 0);
            foreach (Dot dot in dots)
            {
                position.X += dot.Position.X;
                position.Y += dot.Position.Y;
            }
            return new PointF(position.X / dots.Count, position.Y / dots.Count);
        }

        private List<AggregatedDot> DetermineMainGroupSolution2(List<Dot> dots)
        {
            List<AggregatedDot> result = new List<AggregatedDot>();
            List<Dot> westDots = new List<Dot>();
            List<Dot> northDots = new List<Dot>();
            List<Dot> southDots = new List<Dot>();
            List<Dot> eastDots = new List<Dot>();
            List<Dot> nonEuDots = new List<Dot>();

            Region mainGroup = Region.Error;
            Region secondaryGroup = Region.Error;
            int west = 0;
            int east = 0;
            int south = 0;
            int north = 0;
            int noneu = 0;

            foreach (Dot dot in dots)
            {
                if (dot.Region == Region.West) { west++; westDots.Add(dot); }
                else if (dot.Region == Region.East) { east++; eastDots.Add(dot); }
                else if (dot.Region == Region.North) { north++; northDots.Add(dot); }
                else if (dot.Region == Region.South) { south++; southDots.Add(dot); }
                else if (dot.Region == Region.NonEU) { noneu++; nonEuDots.Add(dot); }
            }

            var numbers = new List<double>();
            numbers.Add(west);
            numbers.Add(east);
            numbers.Add(north);
            numbers.Add(south);
            numbers.Add(noneu);

            var maxNumber = GetTheMaximumNumber(numbers);
            List<Dot> mainGroupDots = new List<Dot>();
            if (maxNumber == west) { mainGroup = Region.West; mainGroupDots = westDots; }
            else if (maxNumber == east) { mainGroup = Region.East; mainGroupDots = eastDots; }
            else if (maxNumber == north) { mainGroup = Region.North; mainGroupDots = northDots; }
            else if (maxNumber == south) { mainGroup = Region.South; mainGroupDots = southDots; }
            else if (maxNumber == noneu) { mainGroup = Region.NonEU; mainGroupDots = nonEuDots; }

            numbers.Remove(maxNumber);
            maxNumber = GetTheMaximumNumber(numbers);
            List<Dot> secondaryGroupDots = new List<Dot>();
            if (maxNumber == west) { secondaryGroup = Region.West; secondaryGroupDots = westDots; }
            else if (maxNumber == east) { secondaryGroup = Region.East; secondaryGroupDots = eastDots; }
            else if (maxNumber == north) { secondaryGroup = Region.North; secondaryGroupDots = northDots; }
            else if (maxNumber == south) { secondaryGroup = Region.South; secondaryGroupDots = southDots; }
            else if (maxNumber == noneu) { secondaryGroup = Region.NonEU; secondaryGroupDots = nonEuDots; }

            result.Add(new AggregatedDot(new Dot(mainGroup, CalculateMiddlePosition(mainGroupDots)), _radius));
            result.Add(new AggregatedDot(new Dot(secondaryGroup, CalculateMiddlePosition(secondaryGroupDots)), _radius / 2));

            return result;
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
            ApplyAggregationAlgorithm(width, height, ratio);
            for (int i = 0; i < _dotsAfterAggregation.Count; i++)
            {
                _dotsAfterAggregation[i].DrawDot(width, height, graph, ratio);
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
