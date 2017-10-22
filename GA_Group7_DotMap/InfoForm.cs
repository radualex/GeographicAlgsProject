using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static GA_Group7_DotMap.ColorUtil;

namespace GA_Group7_DotMap
{
    public partial class InfoForm : Form
    {
        List<ColorRGB> listOfColors = new List<ColorRGB>();
        List<Rectangle> listOfDots = new List<Rectangle>();
        Label lastLabel;
        public InfoForm(int nrOfRegions)
        {
            InitializeComponent();
            MaximumSize = Size;
            MinimumSize = Size;
            ControlBox = false;
            CreateColors(nrOfRegions);
        }

        private void CreateColors(int nrOfRegions)
        {
            for (var i = 1; i <= nrOfRegions; i++)
            {
                listOfColors.Add(ColorUtil.GetRainbowColor(i));
            }
        }

        public void UpdateMessage(string message)
        {
            listOfDots.Clear();
            var splitted = message.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 0; i < splitted.Length; i++)
            {
                if (i < splitted.Length - 1)
                {
                    Rectangle rectangle = new Rectangle(20, 20 * (i + 1), 10, 10);
                    listOfDots.Add(rectangle);
                    var label = new Label
                    {
                        Location = new Point(rectangle.X + 20, rectangle.Y),
                        Text = splitted[i],
                        AutoSize = true
                    };

                    if (i == splitted.Length - 2)
                    {
                        lastLabel = label;
                    }                    
                    Controls.Add(label);
                }
                else
                {
                    var label = new Label
                    {
                        Location = new Point(lastLabel.Location.X, lastLabel.Location.Y + 20),
                        Text = splitted[i],
                        AutoSize = true
                    };

                    Controls.Add(label);
                }
            }
            Refresh();
        }

        private void InfoForm_Paint(object sender, PaintEventArgs e)
        {
            if (listOfDots == null || listOfDots.Count == 0) { return; }
            for (int i = 0; i < listOfDots.Count; i++)
            {
                e.Graphics.FillEllipse(new SolidBrush(listOfColors[i]), listOfDots[i]);
            }
        }
    }
}
