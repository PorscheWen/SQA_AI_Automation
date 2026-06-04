using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Demo2Desktop.UI
{
    public static class DefectChartRenderer
    {
        const int MarginLeft = 88;
        const int MarginRight = 16;
        const int MarginTop = 36;
        const int MarginBottom = 48;

        public static void PaintChart(Graphics g, Rectangle bounds, IList<string> testTypes, IList<int> defectValues)
        {
            if (testTypes == null || defectValues == null || testTypes.Count == 0)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Color.White);

            Rectangle plot = new Rectangle(
                bounds.Left + MarginLeft,
                bounds.Top + MarginTop,
                Math.Max(10, bounds.Width - MarginLeft - MarginRight),
                Math.Max(10, bounds.Height - MarginTop - MarginBottom));

            using (Font titleFont = new Font("Microsoft JhengHei UI", 10f, FontStyle.Bold))
            using (Font axisFont = new Font("Microsoft JhengHei UI", 8f))
            using (Font labelFont = new Font("Microsoft JhengHei UI", 7f))
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(40, 40, 40)))
            using (Pen axisPen = new Pen(Color.Gray, 1f))
            using (Pen gridPen = new Pen(Color.FromArgb(220, 220, 220), 1f))
            using (Pen linePen = new Pen(Color.FromArgb(0, 102, 204), 2f))
            using (SolidBrush pointBrush = new SolidBrush(Color.FromArgb(220, 50, 50)))
            {
                // 對調：X = Defect Number, Y = Test Type
                g.DrawString("X: Defect Number  |  Y: Test Type", titleFont, textBrush,
                    bounds.Left + MarginLeft, bounds.Top + 8);

                int count = Math.Min(testTypes.Count, defectValues.Count);
                int minDefect = int.MaxValue;
                int maxDefect = int.MinValue;
                for (int i = 0; i < count; i++)
                {
                    if (defectValues[i] < minDefect) minDefect = defectValues[i];
                    if (defectValues[i] > maxDefect) maxDefect = defectValues[i];
                }
                if (minDefect == maxDefect)
                {
                    minDefect = Math.Max(0, minDefect - 10);
                    maxDefect = maxDefect + 10;
                }
                int defectPad = Math.Max(1, (maxDefect - minDefect) / 10);
                minDefect = Math.Max(0, minDefect - defectPad);
                maxDefect = maxDefect + defectPad;

                g.DrawLine(axisPen, plot.Left, plot.Top, plot.Left, plot.Bottom);
                g.DrawLine(axisPen, plot.Left, plot.Bottom, plot.Right, plot.Bottom);

                int gridLinesX = 5;
                for (int i = 0; i <= gridLinesX; i++)
                {
                    int x = plot.Left + (plot.Width * i / gridLinesX);
                    g.DrawLine(gridPen, x, plot.Top, x, plot.Bottom);
                    int tick = minDefect + ((maxDefect - minDefect) * i / gridLinesX);
                    string tickText = tick.ToString();
                    SizeF tickSize = g.MeasureString(tickText, axisFont);
                    g.DrawString(tickText, axisFont, textBrush, x - (tickSize.Width / 2f), plot.Bottom + 4f);
                }

                PointF[] points = new PointF[count];
                float stepY = count > 1 ? (float)plot.Height / (count - 1) : 0f;
                for (int i = 0; i < count; i++)
                {
                    float y = plot.Bottom - (stepY * i);
                    float ratio = (float)(defectValues[i] - minDefect) / (maxDefect - minDefect);
                    float x = plot.Left + (ratio * plot.Width);
                    points[i] = new PointF(x, y);

                    string typeLabel = Abbreviate(testTypes[i], 10);
                    SizeF typeSize = g.MeasureString(typeLabel, labelFont);
                    g.DrawString(typeLabel, labelFont, textBrush, bounds.Left + 4, y - (typeSize.Height / 2f));

                    if (count > 1)
                    {
                        int gridY = plot.Top + (plot.Height * i / (count - 1));
                        g.DrawLine(gridPen, plot.Left, gridY, plot.Right, gridY);
                    }
                }

                if (count > 1)
                    g.DrawCurve(linePen, points, 0.35f);
                else
                    g.DrawLine(linePen, plot.Left, points[0].Y, plot.Right, points[0].Y);

                float pointRadius = 4f;
                for (int i = 0; i < count; i++)
                {
                    g.FillEllipse(pointBrush, points[i].X - pointRadius, points[i].Y - pointRadius,
                        pointRadius * 2, pointRadius * 2);
                }

                SizeF xLabelSize = g.MeasureString("Defect Number (X)", axisFont);
                g.DrawString("Defect Number (X)", axisFont, textBrush,
                    plot.Left + (plot.Width - xLabelSize.Width) / 2f, plot.Bottom + 18f);

                g.DrawString("Test Type (Y)", axisFont, textBrush, bounds.Left + 4, plot.Top - 2);
            }
        }

        static string Abbreviate(string text, int maxLen)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            if (text.Length <= maxLen)
                return text;
            return text.Substring(0, maxLen - 1) + "…";
        }
    }
}
