using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Call_to_Kursovaya
{
    public class Resizing
    {
        public int x, y, width, height;
        public float fontsize;
        public static List<Resizing> elements = new List<Resizing>();
        public static int start_y;
        private Resizing(int x, int y, int width, int height, float fontsize)  
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.fontsize = fontsize;

            elements.Add(this);
        }

        public Resizing (List<Label> labels, List<Button> buttons)
        {
            foreach (Label i in labels)
                new Resizing(i.Location.X, i.Location.Y, i.Width, i.Height, i.Font.Size);
            foreach (Button i in buttons)
                new Resizing(i.Location.X, i.Location.Y, i.Width, i.Height, i.Font.Size);
        }


        public static Point Resize(double resizeX, int startY, int i)
        {
            return new Point(Form1.Round(elements[i].x, resizeX), startY);
        }
        public static Font Resize(Font font, double resizeX, double resizeY, int i)
        {
            return new Font(font.FontFamily, (float)(elements[i].fontsize * ((float)resizeX + (float)resizeY) / (elements[i].width / (float)elements[i].height)));
        }
        public static Size Resize(double resizeX, double resizeY, int i)
        {
            return new Size(Form1.Round(elements[i].width, resizeX), Form1.Round(elements[i].height, resizeY));
        }


    }
}
