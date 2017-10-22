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

        public Color Color
        {
            get
            {
                return ColorUtil.GetRainbowColor((int)Region);
                //if (Region == Region.West) return ColorUtil.GetRainbowColor((int)Region.West);
                //else if (Region == Region.South) return ColorUtil.GetRainbowColor((int)Region.South);
                //else if (Region == Region.North) return ColorUtil.GetRainbowColor((int)Region.North);
                //else if (Region == Region.East) return ColorUtil.GetRainbowColor((int)Region.East);
                //else if (Region == Region.NonEU) return ColorUtil.GetRainbowColor((int)Region.NonEU);
                //else return Color.White;
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
            if (ratio < 1)
                point = new PointF(Position.X * width * ratio + (Math.Abs(1 - ratio)) * width / 2, Position.Y * height * ratio + (Math.Abs(1 - ratio)) * height / 2);
            else
                point = new PointF(Position.X * width * ratio, Position.Y * height * ratio);
            graph.FillEllipse(new SolidBrush(Color), point.X, point.Y, raduis, raduis);
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
