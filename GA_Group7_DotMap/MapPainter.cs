using System.Collections.Generic;
using System.Drawing;

namespace GA_Group7_DotMap
{
    public class MapPainter
    {
        private List<CityBlock> _blocks = new List<CityBlock>();
        private List<List<PointF>> _blockCoordinates = new List<List<PointF>>();
        
        public List<CityBlock> Blocks { get { return _blocks; } }

        public MapPainter()
        {
            InitializeBorderCoordinates();
            InitializeInnerBlocksCoordinates();
            _blockCoordinates.ForEach(x => _blocks.Add(new CityBlock(x)));
        }

        public void DrawMap(int width, int height, Graphics graph, float ratio)
        {
            for (int i = 0; i < _blocks.Count; i++)
            {
                if (i == 0) _blocks[i].DrawBlock(width, height, Setting.BorderWidth, graph, ratio);
                else _blocks[i].DrawBlock(width, height, Setting.CityBlockWidth, graph, ratio);
            }
        }

        private void InitializeBorderCoordinates()
        {
            // border information of the map, predefined.
            var borderCoordinates = new List<PointF>();
            borderCoordinates.Add(new PointF(13f / 24, 0.5f / 18));
            borderCoordinates.Add(new PointF(11f / 24, 0.5f / 18));
            borderCoordinates.Add(new PointF(8.5f / 24, 1.5f / 18));
            borderCoordinates.Add(new PointF(7f / 24, 2.5f / 18));
            borderCoordinates.Add(new PointF(5f / 24, 3.5f / 18));
            borderCoordinates.Add(new PointF(4.5f / 24, 4.5f / 18));
            borderCoordinates.Add(new PointF(3.5f / 24, 5.5f / 18));
            borderCoordinates.Add(new PointF(0f / 24, 8.2f / 18));
            borderCoordinates.Add(new PointF(0.3f / 24, 9f / 18));
            borderCoordinates.Add(new PointF(0.5f / 24, 11.7f / 18));
            borderCoordinates.Add(new PointF(2.7f / 24, 11.7f / 18));
            borderCoordinates.Add(new PointF(3.6f / 24, 11.5f / 18));
            borderCoordinates.Add(new PointF(5.2f / 24, 12.5f / 18));
            borderCoordinates.Add(new PointF(6.4f / 24, 12.1f / 18));
            borderCoordinates.Add(new PointF(6.8f / 24, 13.1f / 18));
            borderCoordinates.Add(new PointF(7.5f / 24, 12.5f / 18));
            borderCoordinates.Add(new PointF(8.3f / 24, 13f / 18));
            borderCoordinates.Add(new PointF(8.5f / 24, 15.3f / 18));
            borderCoordinates.Add(new PointF(8.3f / 24, 17.8f / 18));
            borderCoordinates.Add(new PointF(14.7f / 24, 17.5f / 18));
            borderCoordinates.Add(new PointF(20.7f / 24, 17.8f / 18));
            borderCoordinates.Add(new PointF(20.2f / 24, 13.5f / 18));
            borderCoordinates.Add(new PointF(20.8f / 24, 12.6f / 18));
            borderCoordinates.Add(new PointF(22.5f / 24, 12.5f / 18));
            borderCoordinates.Add(new PointF(21f / 24, 10.5f / 18));
            borderCoordinates.Add(new PointF(20.7f / 24, 9.5f / 18));
            borderCoordinates.Add(new PointF(21f / 24, 7.6f / 18));
            borderCoordinates.Add(new PointF(18.7f / 24, 6f / 18));
            borderCoordinates.Add(new PointF(18.2f / 24, 5f / 18));
            borderCoordinates.Add(new PointF(17.8f / 24, 3f / 18));
            borderCoordinates.Add(new PointF(18f / 24, 2f / 18));
            borderCoordinates.Add(new PointF(19f / 24, 0.5f / 18));
            borderCoordinates.Add(new PointF(13f / 24, 0.5f / 18));
            _blockCoordinates.Add(borderCoordinates);
        }

        private void InitializeInnerBlocksCoordinates()
        {
            var blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(6.5f / 24, 9f / 18));
            blockCoordinates.Add(new PointF(5f / 24, 10.3f / 18));
            blockCoordinates.Add(new PointF(7.5f / 24, 10.2f / 18));
            blockCoordinates.Add(new PointF(6.5f / 24, 9f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(6.2f / 24, 10.7f / 18));
            blockCoordinates.Add(new PointF(7.7f / 24, 10.5f / 18));
            blockCoordinates.Add(new PointF(7.5f / 24, 12f / 18));
            blockCoordinates.Add(new PointF(6.4f / 24, 12.1f / 18));
            blockCoordinates.Add(new PointF(6.2f / 24, 10.7f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(8.2f / 24, 10.6f / 18));
            blockCoordinates.Add(new PointF(9.5f / 24, 12f / 18));
            blockCoordinates.Add(new PointF(10.7f / 24, 12f / 18));
            blockCoordinates.Add(new PointF(11.7f / 24, 9.8f / 18));
            blockCoordinates.Add(new PointF(10.7f / 24, 9.2f / 18));
            blockCoordinates.Add(new PointF(8.2f / 24, 10.6f / 18));
            _blockCoordinates.Add(blockCoordinates);
            
            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(11.7f / 24, 9.8f / 18));
            blockCoordinates.Add(new PointF(10.7f / 24, 12f / 18));
            blockCoordinates.Add(new PointF(14.4f / 24, 12.2f / 18));
            blockCoordinates.Add(new PointF(14f / 24, 10.5f / 18));
            blockCoordinates.Add(new PointF(11.7f / 24, 9.8f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(9f / 24, 14.4f / 18));
            blockCoordinates.Add(new PointF(9f / 24, 17.5f / 18));
            blockCoordinates.Add(new PointF(10.5f / 24, 16.5f / 18));
            blockCoordinates.Add(new PointF(10.7f / 24, 13.5f / 18));
            blockCoordinates.Add(new PointF(9f / 24, 14.4f / 18));
            _blockCoordinates.Add(blockCoordinates);
            
            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(10.7f / 24, 13.5f / 18));
            blockCoordinates.Add(new PointF(13f / 24, 15.8f / 18));
            blockCoordinates.Add(new PointF(14.4f / 24, 12.2f / 18));
            blockCoordinates.Add(new PointF(10.7f / 24, 13.5f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(14f / 24, 10.5f / 18));
            blockCoordinates.Add(new PointF(14.8f / 24, 14f / 18));
            blockCoordinates.Add(new PointF(17f / 24, 12.2f / 18));
            blockCoordinates.Add(new PointF(14f / 24, 10.5f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(14.8f / 24, 14f / 18));
            blockCoordinates.Add(new PointF(14.7f / 24, 16.7f / 18));
            blockCoordinates.Add(new PointF(18.2f / 24, 16.2f / 18));
            blockCoordinates.Add(new PointF(15.7f / 24, 13.5f / 18));
            blockCoordinates.Add(new PointF(14.8f / 24, 14f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(15.7f / 24, 13.5f / 18));
            blockCoordinates.Add(new PointF(18.2f / 24, 16.2f / 18));
            blockCoordinates.Add(new PointF(19.5f / 24, 13.5f / 18));
            blockCoordinates.Add(new PointF(17f / 24, 12.2f / 18));
            blockCoordinates.Add(new PointF(15.7f / 24, 13.5f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(14f / 24, 10.5f / 18));
            blockCoordinates.Add(new PointF(17.5f / 24, 12f / 18));
            blockCoordinates.Add(new PointF(17.4f / 24, 10.5f / 18));
            blockCoordinates.Add(new PointF(14f / 24, 10.5f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(17.4f / 24, 10.5f / 18));
            blockCoordinates.Add(new PointF(17.5f / 24, 12f / 18));
            blockCoordinates.Add(new PointF(19.5f / 24, 12.1f / 18));
            blockCoordinates.Add(new PointF(20.7f / 24, 9.5f / 18));
            blockCoordinates.Add(new PointF(17.4f / 24, 10.5f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(17.5f / 24, 8f / 18));
            blockCoordinates.Add(new PointF(17.3f / 24, 10f / 18));
            blockCoordinates.Add(new PointF(20f / 24, 9.6f / 18));
            blockCoordinates.Add(new PointF(20f / 24, 8.9f / 18));
            blockCoordinates.Add(new PointF(17.5f / 24, 8f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(15.6f / 24, 5.7f / 18));
            blockCoordinates.Add(new PointF(18.7f / 24, 6f / 18));
            blockCoordinates.Add(new PointF(15f / 24, 10f / 18));
            blockCoordinates.Add(new PointF(15.6f / 24, 5.7f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(15f / 24, 10f / 18));
            blockCoordinates.Add(new PointF(14f / 24, 10.5f / 18));
            blockCoordinates.Add(new PointF(14.5f / 24, 7f / 18));
            blockCoordinates.Add(new PointF(15.6f / 24, 5.7f / 18));
            blockCoordinates.Add(new PointF(15f / 24, 10f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(14f / 24, 10.5f / 18));
            blockCoordinates.Add(new PointF(11.5f / 24, 7f / 18));
            blockCoordinates.Add(new PointF(14.5f / 24, 7f / 18));
            blockCoordinates.Add(new PointF(14f / 24, 10.5f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(14.5f / 24, 7f / 18));
            blockCoordinates.Add(new PointF(15.6f / 24, 5.7f / 18));
            blockCoordinates.Add(new PointF(15.6f / 24, 4f / 18));
            blockCoordinates.Add(new PointF(13.2f / 24, 4f / 18));
            blockCoordinates.Add(new PointF(10.7f / 24, 5.2f / 18));
            blockCoordinates.Add(new PointF(11.5f / 24, 7f / 18));
            blockCoordinates.Add(new PointF(14.5f / 24, 7f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(7.5f / 24, 3.5f / 18));
            blockCoordinates.Add(new PointF(8.7f / 24, 5.4f / 18));
            blockCoordinates.Add(new PointF(10.5f / 24, 5f / 18));
            blockCoordinates.Add(new PointF(8.7f / 24, 2.5f / 18));
            blockCoordinates.Add(new PointF(7.5f / 24, 3.5f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(11.5f / 24, 9f / 18));
            blockCoordinates.Add(new PointF(12f / 24, 8f / 18));
            blockCoordinates.Add(new PointF(14f / 24, 10.5f / 18));
            blockCoordinates.Add(new PointF(11.5f / 24, 9f / 18));
            _blockCoordinates.Add(blockCoordinates);
            
            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(15.6f / 24, 4f / 18));
            blockCoordinates.Add(new PointF(18.7f / 24, 6f / 18));
            blockCoordinates.Add(new PointF(18.2f / 24, 5f / 18));
            blockCoordinates.Add(new PointF(15.6f / 24, 4f / 18));
            blockCoordinates.Add(new PointF(15.6f / 24, 4f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(18.2f / 24, 5f / 18));
            blockCoordinates.Add(new PointF(15.6f / 24, 4f / 18));
            blockCoordinates.Add(new PointF(15.8f / 24, 2.6f / 18));
            blockCoordinates.Add(new PointF(16.5f / 24, 2.1f / 18));
            blockCoordinates.Add(new PointF(17.8f / 24, 3f / 18));
            blockCoordinates.Add(new PointF(18.2f / 24, 5f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(15.6f / 24, 4f / 18));
            blockCoordinates.Add(new PointF(13.2f / 24, 4f / 18));
            blockCoordinates.Add(new PointF(13f / 24, 1.5f / 18));
            blockCoordinates.Add(new PointF(13.2f / 24, 0.5f / 18));
            blockCoordinates.Add(new PointF(15f / 24, 1.5f / 18));
            blockCoordinates.Add(new PointF(16.5f / 24, 2.1f / 18));
            blockCoordinates.Add(new PointF(15.8f / 24, 2.6f / 18));
            blockCoordinates.Add(new PointF(15.6f / 24, 4f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(13.2f / 24, 4f / 18));
            blockCoordinates.Add(new PointF(12.5f / 24, 4.2f / 18));
            blockCoordinates.Add(new PointF(11.5f / 24, 2f / 18));
            blockCoordinates.Add(new PointF(13f / 24, 1.5f / 18));
            blockCoordinates.Add(new PointF(13.2f / 24, 4f / 18));
            _blockCoordinates.Add(blockCoordinates);

            blockCoordinates = new List<PointF>();
            blockCoordinates.Add(new PointF(8.7f / 24, 2.5f / 18));
            blockCoordinates.Add(new PointF(11f / 24, 0.5f / 18));
            blockCoordinates.Add(new PointF(11.5f / 24, 3.5f / 18));
            blockCoordinates.Add(new PointF(10.4f / 24, 4.3f / 18));
            blockCoordinates.Add(new PointF(8.7f / 24, 2.5f / 18));
            _blockCoordinates.Add(blockCoordinates);
        }
    }
}
