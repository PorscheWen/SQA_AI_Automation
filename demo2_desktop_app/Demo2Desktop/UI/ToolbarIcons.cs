using System.Drawing;
using System.Drawing.Drawing2D;

namespace Demo2Desktop.UI
{
    public static class ToolbarIcons
    {
        public static Image CreateExcelIcon()
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (SolidBrush green = new SolidBrush(Color.FromArgb(33, 115, 70)))
                    g.FillRectangle(green, 1, 1, 14, 14);

                using (Pen gridPen = new Pen(Color.White, 1f))
                {
                    g.DrawLine(gridPen, 5, 1, 5, 14);
                    g.DrawLine(gridPen, 9, 1, 9, 14);
                    g.DrawLine(gridPen, 1, 5, 14, 5);
                    g.DrawLine(gridPen, 1, 9, 14, 9);
                }

                using (Font font = new Font("Arial", 7f, FontStyle.Bold))
                using (SolidBrush white = new SolidBrush(Color.White))
                    g.DrawString("X", font, white, 2f, 2f);
            }
            return bmp;
        }

        public static Image CreateAboutIcon()
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen ring = new Pen(Color.FromArgb(0, 102, 204), 2f))
                    g.DrawEllipse(ring, 2, 2, 12, 12);
                using (Font font = new Font("Arial", 9f, FontStyle.Bold))
                using (SolidBrush blue = new SolidBrush(Color.FromArgb(0, 102, 204)))
                    g.DrawString("i", font, blue, 5f, 1f);
            }
            return bmp;
        }

        public static Image CreateImportJsonIcon()
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (SolidBrush orange = new SolidBrush(Color.FromArgb(230, 126, 34)))
                    g.FillRectangle(orange, 1, 1, 10, 10);

                using (Font font = new Font("Arial", 6f, FontStyle.Bold))
                using (SolidBrush white = new SolidBrush(Color.White))
                    g.DrawString("J", font, white, 3f, 2f);

                using (Pen arrow = new Pen(Color.FromArgb(200, 80, 0), 2f))
                {
                    g.DrawLine(arrow, 12, 3, 12, 10);
                    g.DrawLine(arrow, 10, 8, 12, 10);
                    g.DrawLine(arrow, 14, 8, 12, 10);
                }

                using (SolidBrush tray = new SolidBrush(Color.FromArgb(120, 120, 120)))
                    g.FillRectangle(tray, 8, 12, 7, 3);
            }
            return bmp;
        }

        public static Image CreateDataTableIcon()
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush header = new SolidBrush(Color.FromArgb(68, 114, 196)))
                    g.FillRectangle(header, 1, 1, 14, 4);
                using (Pen grid = new Pen(Color.FromArgb(68, 114, 196), 1f))
                {
                    for (int y = 6; y <= 14; y += 3)
                        g.DrawLine(grid, 1, y, 14, y);
                    g.DrawLine(grid, 6, 5, 6, 14);
                    g.DrawLine(grid, 11, 5, 11, 14);
                }
            }
            return bmp;
        }

        public static Image CreateChartIcon()
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen blue = new Pen(Color.FromArgb(0, 102, 204), 2f))
                {
                    g.DrawLine(blue, 1, 13, 5, 9);
                    g.DrawLine(blue, 5, 9, 9, 11);
                    g.DrawLine(blue, 9, 11, 15, 2);
                }
                using (SolidBrush dot = new SolidBrush(Color.FromArgb(220, 50, 50)))
                {
                    g.FillEllipse(dot, 4, 8, 3, 3);
                    g.FillEllipse(dot, 8, 10, 3, 3);
                    g.FillEllipse(dot, 14, 1, 3, 3);
                }
            }
            return bmp;
        }
    }
}
