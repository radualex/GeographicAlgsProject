using System;
using System.Collections.Generic;
using System.Drawing;

namespace GA_Group7_DotMap
{
    /// <summary>
    /// Each cityblock, which is represented as a polygon, contians some positions, which contributes to the corners of the polygon.
    /// </summary>
    public class CityBlock
    {
        private Graphics _graph = null;
        // the coordinates (position) of its corners.
        private List<PointF> _coordinates = null;
        public List<PointF> Coordinates { get { return _coordinates; } }
        
        public CityBlock(List<PointF> coordinates)
        {
            _coordinates = coordinates;
        }

        public void DrawBlock(int width, int height, float borderWidth, Graphics graph, float ratio)
        {
            _graph = graph;
            var arr = InitializeCoordinates(width, height, ratio).ToArray();
            graph.DrawLines(new Pen(Color.Black, borderWidth), arr);
        }

        private List<PointF> InitializeCoordinates(int width, int height, float ratio)
        {
            var arr = new List<PointF>();
            _coordinates.ForEach(point =>
            {
                if (ratio < 1)
                    // if ratio is less than 1, then we keep the map in the middle.
                    arr.Add(new PointF(point.X * width * ratio + (Math.Abs(1 - ratio)) * width / 2, point.Y * height * ratio + (Math.Abs(1 - ratio)) * height / 2));
                else
                    arr.Add(new PointF(point.X * width * ratio, point.Y * height * ratio));
            });
            return arr;
        }
    }
}
