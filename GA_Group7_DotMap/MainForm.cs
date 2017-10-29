using System;
using System.Drawing;
using System.Windows.Forms;

namespace GA_Group7_DotMap
{
    public partial class Main_Form : Form
    {
        private MapPainter mapPainter;
        private DotDistrubutor dotDistributor;
        private InfoForm infoForm;

        private Graphics graph;
        private float ratio = 1;

        private ContextMenu contextMenu = null;

        public Main_Form()
        {
            InitializeComponent();

            MinimumSize = Size;
            panel_main.Width = Width;
            panel_main.Height = Height;
            KeyPress += Main_Form_KeyPress;
            PreviewKeyDown += Main_Form_PreviewKeyDown;

            AutoScroll = true;
            VerticalScroll.Enabled = true;
            HorizontalScroll.Enabled = true;
            HorizontalScroll.Visible = true;
            VerticalScroll.Visible = true;

            graph = panel_main.CreateGraphics();
            mapPainter = new MapPainter();
            dotDistributor = new DotDistrubutor(mapPainter);

            //Declare the menu items and the shortcut menu.
            MenuItem[] menuItems = new MenuItem[3];
            menuItems[0] = new MenuItem("Zoom In");
            menuItems[1] = new MenuItem("Zoom Out");
            menuItems[2] = new MenuItem("Restore to default");
            contextMenu = new ContextMenu(menuItems);
            menuItems[0].Click += new EventHandler((obj, evargs) => ZoomIn());
            menuItems[1].Click += new EventHandler((obj, evargs) => ZoomOut());
            menuItems[2].Click += new EventHandler((obj, evargs) => RestoreToDefault());

            infoForm = new InfoForm(5);
            infoForm.Visible = true;
            infoForm.TopMost = true;
        }

        private void Main_Form_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    ZoomOut();
                    break;
                case Keys.Right:
                    ZoomIn();
                    break;
                case Keys.Up:
                    ZoomIn();
                    break;
                case Keys.Left:
                    ZoomOut();
                    break;
            }
        }

        private void Main_Form_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '+') ZoomIn();
            if (e.KeyChar == '-') ZoomOut();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            RestoreToDefault();
        }

        private void panel_main_Paint(object sender, PaintEventArgs e)
        {
            // give some distance to the form border.
            var width = Width - 60;
            var height = Height - 40;
            mapPainter.DrawMap(width, height, e.Graphics, ratio);
            dotDistributor.DrawDots(width, height, e.Graphics, ratio);
            infoForm.UpdateMessage(GetMessage(dotDistributor.NumberOfDotsPerGroup), dotDistributor.MeasureAggregationAccuracy());
        }

        private void ZoomIn()
        {
            if (ratio >= 1)
            {
                int newWidth = Convert.ToInt32(panel_main.Width * 1.1);
                int newHeight = Convert.ToInt32(panel_main.Height * 1.1);
                panel_main.Width = newWidth;
                panel_main.Height = newHeight;
            }
            else
            {
                panel_main.Width = Width;
                panel_main.Height = Height;
            }

            if (ratio < 100)
            {
                // zoom in 10%.
                ratio *= 1.1f;
                panel_main.Invalidate();
            }
        }

        private void ZoomOut()
        {
            if (panel_main.Width > Width && panel_main.Height > Height)
            {
                int newWidth = Convert.ToInt32(panel_main.Width / 1.1);
                int newHeight = Convert.ToInt32(panel_main.Height / 1.1);
                panel_main.Width = newWidth;
                panel_main.Height = newHeight;
            }

            if (ratio > 0.2)
            {
                // zoom ou 10%.
                ratio /= 1.1f;
                panel_main.Invalidate();
            }
        }

        private void RestoreToDefault()
        {
            ratio = 1;
            panel_main.Width = Width;
            panel_main.Height = Height;
            panel_main.Invalidate();
            Setting.BaseNumberOfGroupsPerLine = Width / 70;
            ZoomIn(); ZoomOut();
        }

        private void panel_main_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenu.Show(panel_main, new System.Drawing.Point(e.X, e.Y));
        }

        private string GetMessage(int number)
        {
            return $"West EU \r\nEast EU\r\nSouth EU\r\nNorth EU \r\nNon-EU \r\nPropotion: 1 dot = {number} Persons.";
        }

        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            infoForm.Visible = false;
            infoForm.Close();
        }
    }
}
