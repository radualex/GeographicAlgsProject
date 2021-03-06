﻿using System;
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
        private List<AggregatedDot> _aggregatedDots = new List<AggregatedDot>();
        private List<AggregatedDot> _beforeOverlapFix;

        private List<Dot> Dots { get { return _dots; } }

        private int _width = 0;
        private int _height = 0;
        private float _ratio = 1;
        public int NumberOfDotsPerGroup { get; private set; }


        private List<double> _groupEuclideans = new List<double>();
        public DotDistrubutor(MapPainter mapPainter)
        {
            _mapPainter = mapPainter;
            new DataGenerator().GenerateData(_dots,_mapPainter);
        }

        private void ApplyAggregationAlgorithm(int width, int height, float ratio)
        {
            _width = width;
            _height = height;
            _ratio = ratio;
            int newNumberOfDotsPerGroup = CalculateNumberOfDotsPerGroup();
            newNumberOfDotsPerGroup = RoundNumberOfDotsPerGroup(newNumberOfDotsPerGroup);

            // if the aggregation algorithm does not need to be applied again.
            if (NumberOfDotsPerGroup == newNumberOfDotsPerGroup) return;

            NumberOfDotsPerGroup = newNumberOfDotsPerGroup;
            _aggregatedDots = new List<AggregatedDot>();

            // if it has zoomed in a lot, then we do not need to apply the aggreagation algorithm.
            if (NumberOfDotsPerGroup == 1)
            {
                foreach (Dot dot in _dots)
                {
                    _aggregatedDots.Add(new AggregatedDot(dot, Setting.MinimumAggregationDotRadius));
                }
                return;
            }
            _groupEuclideans = new List<double>();
            SplitDotsIntoSmallGroups(_dots, 0, 1, 0, 1);
            _beforeOverlapFix = new List<AggregatedDot>(_aggregatedDots);
            //ResolvePossibleOverLap();
        }

        private int CalculateNumberOfDotsPerGroup()
        {
            return Convert.ToInt32(_dots.Count / ((Setting.BaseNumberOfGroupsPerLine * _ratio) * (Setting.BaseNumberOfGroupsPerLine * _ratio))) + 1;
        }

        // for instance, numberOfDotsPerGroup = 10345, return value will be 10000.
        // numberOfDotsPerGroup = 1245, return value will be 1000.
        // numberOfDotsPerGroup = 6, return value will be 6.
        private int RoundNumberOfDotsPerGroup(int numberOfDotsPerGroup)
        {
            if (numberOfDotsPerGroup < 10) return numberOfDotsPerGroup;
            return Convert.ToInt32(Convert.ToInt32(numberOfDotsPerGroup.ToString().Substring(0, 1)) * Math.Pow(10, numberOfDotsPerGroup.ToString().Length - 1));
        }

        // The aggregation algorithm.
        #region Aggregation algorithm.

        // Split the whole map in to 4 sub pieces (left top, right top, left down, right down) recursively,
        // until the number of people in that area meets its criteria (less than NumberOfDotsPerGroup).
        private void SplitDotsIntoSmallGroups(List<Dot> dots, double x1, double x2, double y1, double y2)
        {
            if (dots.Count > NumberOfDotsPerGroup * 1.5) // Let the tresh hold become 1.5 * NumberOfDotsPerGroup
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
                SplitDotsIntoSmallGroups(dotsArray[0], x1, xmid, y1, ymid);
                SplitDotsIntoSmallGroups(dotsArray[1], xmid, x2, y1, ymid);
                SplitDotsIntoSmallGroups(dotsArray[2], x1, xmid, ymid, y2);
                SplitDotsIntoSmallGroups(dotsArray[3], xmid, x2, ymid, y2);
            }
            else
            {
                // The return list is design for alternative solutions.
                // For this solution, the return list always contains exactly 1 aggregated Dot.
                var aggregatedDots = GetAggregatedDot(dots);
                if (aggregatedDots != null)
                {
                    _aggregatedDots.AddRange(aggregatedDots);
                    var averageEuclidean = Metrics.CalculateLocation(dots, aggregatedDots.First());
                    _groupEuclideans.Add(averageEuclidean);
                }
            }
        }

        // Apply aggregation algoithm to a specific group.
        // The return list is design for alternative solutions.
        // For this solution, the return list always contains exactly 1 aggregated Dot.
        private List<AggregatedDot> GetAggregatedDot(List<Dot> dots)
        {
            var returnDots = new List<AggregatedDot>();
            if (dots.Count > NumberOfDotsPerGroup * Setting.MinimumPercentageToShow)
            {
                returnDots.AddRange(DetermineMainGroupSolution1(dots));
                return returnDots;
            }
            return null;
        }

        // this determines the main region of people inside a group.
        // in other words, this decides the color of this group.
        // The return list is design for alternative solutions.
        // For this solution, the return list always contains exactly 1 aggregated Dot.
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
            double westpercentage = Convert.ToDouble(west) / dots.Count / Math.Sqrt(Setting.WestPecrentage);
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

            var maxNumber = numbers.Max();
            if (maxNumber == westpercentage) mainGroup = Region.West;
            else if (maxNumber == eastpercentage) mainGroup = Region.East;
            else if (maxNumber == southpercentage) mainGroup = Region.South;
            else if (maxNumber == northpercentage) mainGroup = Region.North;
            else if (maxNumber == noneupercentage) mainGroup = Region.NonEU;

            Circle circle = MinimumCoverCircle.GetMinimumCoverCircle(dots);
            result.Add(new AggregatedDot(new Dot(mainGroup, new PointF((float)circle.c.x, (float)circle.c.y)), Math.Max((int)(circle.r * _ratio * Math.Min(_width, _height)), Setting.MinimumAggregationDotRadius)));

            return result;
        }

        // To enable resolve overlap, uncomment line 63.
        // It is not enabled, because it takes O(n^2) time, which is slow.
        // We use a greedy implementation.
        // 1. Try to add as much points as possible in the first time. i.e ignore all overlap dots.
        // 2. Try to add the overlap dots without effecting others.
        // When resolving the overlap, we ignore the minimum radius requirement, which is set to 4.
        // The minimum raduis will be 1 if overlap occurs.
        // Note that there will be no full overlap (same center), becasue we use the center of the minimum cover circle as the position of the dot.
        // hence, they are all unique and raduis = 1 would be the value which will not cause any overlap with others.
        private void ResolvePossibleOverLap()
        {
            float distanceBetweenCenter = 0;
            List<AggregatedDot> tempAggregatedDots = new List<AggregatedDot>(_aggregatedDots);
            _aggregatedDots.Clear();
            List<AggregatedDot> overlapDots = new List<AggregatedDot>();
            for (int i = 0; i < tempAggregatedDots.Count; i++)
            {
                var tempDot = tempAggregatedDots[i];
                bool containsOverlap = false;
                foreach (AggregatedDot dot in _aggregatedDots)
                {
                    double xdiffer = tempDot.Dot.Position.X * _ratio * _width - dot.Dot.Position.X * _ratio * _width;
                    double ydiffer = tempDot.Dot.Position.Y * _ratio * _height - dot.Dot.Position.Y * _ratio * _height;
                    distanceBetweenCenter = (float)Math.Sqrt(Math.Pow(xdiffer, 2) + Math.Pow(ydiffer, 2));
                    if (distanceBetweenCenter < (tempDot.Raduis + dot.Raduis) / 2)
                    {
                        containsOverlap = true;
                        overlapDots.Add(tempDot);
                        break;
                    }
                }
                if (!containsOverlap) _aggregatedDots.Add(tempDot);
            }

            foreach (AggregatedDot overlapDot in overlapDots)
            {
                List<AggregatedDot> allDotsOverlapWithThisDot = new List<AggregatedDot>();
                List<float> distances = new List<float>();
                foreach (AggregatedDot d in _aggregatedDots)
                {
                    double xdiffer = overlapDot.Dot.Position.X * _ratio * _width - d.Dot.Position.X * _ratio * _width;
                    double ydiffer = overlapDot.Dot.Position.Y * _ratio * _height - d.Dot.Position.Y * _ratio * _height;
                    distanceBetweenCenter = (float)Math.Sqrt(Math.Pow(xdiffer, 2) + Math.Pow(ydiffer, 2));
                    if (distanceBetweenCenter < (overlapDot.Raduis + d.Raduis) / 2)
                    {
                        distances.Add(distanceBetweenCenter);
                        allDotsOverlapWithThisDot.Add(d);
                    }
                }

                // When resolving the overlap, we ignore the minimum radius requirement, which is set to 4 in Setting.cs
                for (int i = 0; i < distances.Count; i++)
                {
                    AggregatedDot criticalDot = allDotsOverlapWithThisDot[i];
                    double xdiffer = overlapDot.Dot.Position.X * _ratio * _width - criticalDot.Dot.Position.X * _ratio * _width;
                    double ydiffer = overlapDot.Dot.Position.Y * _ratio * _height - criticalDot.Dot.Position.Y * _ratio * _height;
                    distanceBetweenCenter = (float)Math.Sqrt(Math.Pow(xdiffer, 2) + Math.Pow(ydiffer, 2));
                    if (distanceBetweenCenter < (overlapDot.Raduis + criticalDot.Raduis) / 2)
                    {
                        overlapDot.Raduis = Math.Max(1, Convert.ToInt32((Convert.ToDouble(overlapDot.Raduis) / (overlapDot.Raduis + criticalDot.Raduis)) * distanceBetweenCenter / 1.1)); // 1.1 for some distance between 2 dots.
                        criticalDot.Raduis = Math.Max(1, Convert.ToInt32((Convert.ToDouble(criticalDot.Raduis) / (overlapDot.Raduis + criticalDot.Raduis)) * distanceBetweenCenter / 1.1));
                    }
                }
                _aggregatedDots.Add(overlapDot);
            }
        }

        #endregion

        public void DrawDots(int width, int height, Graphics graph, float ratio)
        {
            ApplyAggregationAlgorithm(width, height, ratio);
            for (int i = 0; i < _aggregatedDots.Count; i++)
            {
                _aggregatedDots[i].DrawDot(width, height, graph, ratio);
            }
        }

        #region measurement

        // Measure the accuracy of the aggregation algorithm.
        public string MeasureAggregationAccuracy()
        {
            double aggregatedwest = 0;
            double aggregatedeast = 0;
            double aggregatedsouth = 0;
            double aggregatednorth = 0;
            double aggregatednoneu = 0;

            double westpercentage = 0;
            double eastpercentage = 0;
            double southpercentage = 0;
            double northpercentage = 0;
            double noneupercentage = 0;

            double originalwest = 0;
            double originaleast = 0;
            double originalsouth = 0;
            double originalnorth = 0;
            double originalnoneu = 0;

            foreach (AggregatedDot dot in _aggregatedDots)
            {
                switch (dot.Dot.Region)
                {
                    case Region.West:
                        aggregatedwest++;
                        break;
                    case Region.East:
                        aggregatedeast++;
                        break;
                    case Region.South:
                        aggregatedsouth++;
                        break;
                    case Region.North:
                        aggregatednorth++;
                        break;
                    case Region.NonEU:
                        aggregatednoneu++;
                        break;
                    default:
                        break;
                }
            }

            foreach (Dot dot in _dots)
            {
                switch (dot.Region)
                {
                    case Region.West:
                        originalwest++;
                        break;
                    case Region.East:
                        originaleast++;
                        break;
                    case Region.South:
                        originalsouth++;
                        break;
                    case Region.North:
                        originalnorth++;
                        break;
                    case Region.NonEU:
                        originalnoneu++;
                        break;
                    default:
                        break;
                }
            }

            aggregatedwest *= NumberOfDotsPerGroup;
            aggregatedeast *= NumberOfDotsPerGroup;
            aggregatedsouth *= NumberOfDotsPerGroup;
            aggregatednorth *= NumberOfDotsPerGroup;
            aggregatednoneu *= NumberOfDotsPerGroup;

            westpercentage = Math.Round(aggregatedwest / originalwest, 2);
            eastpercentage = Math.Round(aggregatedeast / originaleast, 2);
            southpercentage = Math.Round(aggregatedsouth / originalsouth, 2);
            northpercentage = Math.Round(aggregatednorth / originalnorth, 2);
            noneupercentage = Math.Round(aggregatednoneu / originalnoneu, 2);

            string info = "";
            info = "West EU: " + aggregatedwest + " / " + originalwest + " => " + westpercentage + "\r\n" +
                 "East EU: " + aggregatedeast + " / " + originaleast + " => " + eastpercentage + "\r\n" +
                  "South EU: " + aggregatedsouth + " / " + originalsouth + " => " + southpercentage + "\r\n" +
                   "North EU: " + aggregatednorth + " / " + originalnorth + " => " + northpercentage + "\r\n" +
                    "Non EU: " + aggregatednoneu + " / " + originalnoneu + " => " + noneupercentage + "\r\n" +
                    "Total:" + _aggregatedDots.Count * ((NumberOfDotsPerGroup+ NumberOfDotsPerGroup*Setting.MinimumPercentageToShow)/2) + " / " + _dots.Count + " => " + Math.Round(Convert.ToDouble(_aggregatedDots.Count) * ((NumberOfDotsPerGroup + NumberOfDotsPerGroup * Setting.MinimumPercentageToShow) / 2) / _dots.Count, 2);

            info += "\r\n" + MeasureLocationAccuracy();
            info += "\r\n" + MeasureRadiusAccuracy();
            return info;
        }

        public string MeasureLocationAccuracy()
        {
            double sum = 0;
            _groupEuclideans.ForEach(x => sum += x);
            var result = sum / _groupEuclideans.Count;
            return "Average euclidean distance: " + result.ToString();
        }

        public string MeasureRadiusAccuracy()
        {
            double sumBefore = 0, sumAfter = 0;
            foreach(var d in _beforeOverlapFix)
            {
                sumBefore += d.Raduis;
            }
            foreach (var d in _aggregatedDots)
            {
                sumAfter += d.Raduis;
            }
            return "Radius ratio: "+ sumAfter + " / " + sumBefore + " => " + (sumAfter / sumBefore).ToString();
        }

        #endregion

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------

        // This is not used in this program, we simply take the middle point's location as the location of the aggregation dot.
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

        // Another solution for DetermineMainGroup (taking 2 groups), but it is not used in this program.
        // It is not applied becuase it may cause a lot of overlap, and solving the overlap is expensive.
        // Compared with the benefits it brings (less loss of information), we decided to abandon this solution.  
        #region another solution
        private List<AggregatedDot> DetermineMainGroupSolution2(List<Dot> dots)
        {
            List<AggregatedDot> result = new List<AggregatedDot>();
            List<Dot> westDots = new List<Dot>();
            List<Dot> northDots = new List<Dot>();
            List<Dot> southDots = new List<Dot>();
            List<Dot> eastDots = new List<Dot>();
            List<Dot> nonEuDots = new List<Dot>();

            Region mainGroupRegion = Region.Error;
            Region secondaryGroupRegion = Region.Error;
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

            var maxNumber = numbers.Max();
            List<Dot> mainGroupDots = new List<Dot>();
            if (maxNumber == west) { mainGroupRegion = Region.West; mainGroupDots = westDots; }
            else if (maxNumber == east) { mainGroupRegion = Region.East; mainGroupDots = eastDots; }
            else if (maxNumber == north) { mainGroupRegion = Region.North; mainGroupDots = northDots; }
            else if (maxNumber == south) { mainGroupRegion = Region.South; mainGroupDots = southDots; }
            else if (maxNumber == noneu) { mainGroupRegion = Region.NonEU; mainGroupDots = nonEuDots; }

            numbers.Remove(maxNumber);
            maxNumber = numbers.Max();
            List<Dot> secondaryGroupDots = new List<Dot>();
            if (maxNumber == west) { secondaryGroupRegion = Region.West; secondaryGroupDots = westDots; }
            else if (maxNumber == east) { secondaryGroupRegion = Region.East; secondaryGroupDots = eastDots; }
            else if (maxNumber == north) { secondaryGroupRegion = Region.North; secondaryGroupDots = northDots; }
            else if (maxNumber == south) { secondaryGroupRegion = Region.South; secondaryGroupDots = southDots; }
            else if (maxNumber == noneu) { secondaryGroupRegion = Region.NonEU; secondaryGroupDots = nonEuDots; }

            Circle circle = MinimumCoverCircle.GetMinimumCoverCircle(dots);
            result.Add(new AggregatedDot(new Dot(mainGroupRegion, new PointF((float)circle.c.x, (float)circle.c.y)), Math.Max((int)(circle.r * _ratio * Math.Min(_width, _height)), Setting.MinimumAggregationDotRadius)));
            circle = MinimumCoverCircle.GetMinimumCoverCircle(dots);
            result.Add(new AggregatedDot(new Dot(secondaryGroupRegion, new PointF((float)circle.c.x, (float)circle.c.y)), Math.Max((int)(circle.r * _ratio * Math.Min(_width, _height)), Setting.MinimumAggregationDotRadius)));

            return result;
        }
        #endregion
    }
}
