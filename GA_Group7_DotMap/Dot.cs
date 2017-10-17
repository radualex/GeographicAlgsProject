using System;
using System.Drawing;

namespace GA_Group7_DotMap
{
    public class Dot
    {
        private Graphics _graph = null;

        public Region Region { get; private set; }
        public PointF Position { get; private set; }

        public Color Color
        {
            get
            {
                if (Region == Region.West) return Color.Green;
                else if (Region == Region.South) return Color.Blue;
                else if (Region == Region.North) return Color.Yellow;
                else if (Region == Region.East) return Color.Red;
                else return Color.Black;
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

    public enum Region
    {
        West = 0,
        South = 1,
        North = 2,
        East = 3,
        NonEU = 4,
        Error = 5
    }
}
