using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA_Group7_DotMap
{
    public class Metrics
    {
        public static double CalculateLocation(List<Dot> dots, AggregatedDot center)
        {
            var centerX = center.Dot.Position.X;
            var centerY = center.Dot.Position.Y;
            double sumEucledeans = 0;
            foreach (Dot d in dots)
            {
                var euclidean = Math.Sqrt(Math.Pow(centerX - d.Position.X,2) + Math.Pow(centerY - d.Position.Y, 2));
                sumEucledeans += euclidean;
            }
            var average = sumEucledeans / dots.Count;
            return average;
        }
    }
}
