using System;
using System.Drawing;

namespace GA_Group7_DotMap
{
    /// <summary>
    /// One dot means one person on the map.
    /// </summary>
    public class Dot
    {
        private Graphics _graph = null;

        public Region Region { get; private set; }
        public PointF Position { get; private set; }

        // each color represnts one region.
        public Color Color
        {
            get
            {
                return ColorUtil.GetRainbowColor((int)Region);
            }
        }

        public Dot(Region region, PointF position)
        {
            Region = region;
            Position = position;
        }

        public void DrawDot(int width, int height, float raduis, Graphics graph, float ratio)
        {
            _graph = graph;
            PointF point;
            // if ratio is less than 1, then we keep the map in the middle.
            if (ratio < 1)
                point = new PointF(Position.X * width * ratio + (Math.Abs(1 - ratio)) * width / 2, Position.Y * height * ratio + (Math.Abs(1 - ratio)) * height / 2);
            else
                point = new PointF(Position.X * width * ratio, Position.Y * height * ratio);
            graph.FillEllipse(new SolidBrush(Color), point.X, point.Y, raduis, raduis);
        }
    }


    public class AggregatedDot
    {
        public Dot Dot { get; private set; }
        public int Raduis { get; private set; }

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

    /// <summary>
    /// We assume we have 5 different type of people.
    /// West EU, South EU, North EU, East EU and Non EU.
    /// Each group is represented by a specific color.
    /// </summary>
    public enum Region
    {
        West = 1,
        South = 2,
        North = 3,
        East = 4,
        NonEU = 5,
        Error = 6
    }
}
