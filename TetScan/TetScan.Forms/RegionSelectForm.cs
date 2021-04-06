using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TetScan.TetScan.Forms
{
    public partial class RegionSelectForm : Form
    {
        // Mouse Position vars
        Point initPos;
        Point pos;

        // Drawing state
        bool isDrawing;

        // Graphics
        int penWidth = 4;
        Color colour = Color.LimeGreen;
        int gridW;
        int gridH;

        // Captured Screen Copy
        Bitmap capturedScreen;
        Color bgSample_1, bgSample_2;

        public RegionSelectForm(int _gridW, int _gridH, Image background)
        {
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.White;
            Opacity = 1;
            Cursor = Cursors.Cross;
            DoubleBuffered = true;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            KeyDown += OnKeyDown;
            Paint += OnPaint;
            gridW = _gridW;
            gridH = _gridH;
            capturedScreen = (Bitmap)background.Clone();

            // Dim the capture
            Image backgroundCopy = (Image)background.Clone();
            Rectangle r = new Rectangle(0, 0, backgroundCopy.Width, backgroundCopy.Height);
            int alpha = 50;
            using (Graphics g = Graphics.FromImage(backgroundCopy))
            {
                using (Brush cloud_brush = new SolidBrush(Color.FromArgb(alpha, Color.Black)))
                {
                    g.FillRectangle(cloud_brush, r);
                }
            }

            BackgroundImage = backgroundCopy;
        }

        public Color[] getBackgroundColor()
        {
            return new Color[] { bgSample_1, bgSample_2 };
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Escape to cancel
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            pos = initPos = e.Location;
            isDrawing = true;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            pos = e.Location;
            if (isDrawing) Invalidate();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        public Rectangle getCaptureArea()
        {
            return new Rectangle(
                Math.Min(initPos.X, pos.X),
                Math.Min(initPos.Y, pos.Y),
                Math.Abs(initPos.X - pos.X),
                Math.Abs(initPos.Y - pos.Y));
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(colour, penWidth);
            if (isDrawing)
            {
                Rectangle rect = getCaptureArea();
                e.Graphics.DrawRectangle(pen, rect);

                int distW = rect.Width / gridW;
                int distH = rect.Height / gridH;

                bgSample_1 = new Color();
                bgSample_2 = new Color();

                if (distW > 0 && distH > 0)
                {
                    for (int h = distH / 2, row = 0; h < rect.Height; h += distH, row++) 
                    {
                        for (int w = distW / 2, col = 0; w < rect.Width; w += distW, col++)
                        {
                            // Background Color capture
                            if(row == 0)
                            {
                                // Top left corner
                                if(col == 0)
                                {
                                    bgSample_1 = capturedScreen.GetPixel(rect.X + w, rect.Y + h);
                                }
                                // Top right corner
                                if(col == gridW - 1)
                                {
                                    bgSample_2 = capturedScreen.GetPixel(rect.X + w, rect.Y + h);
                                }
                            }

                            e.Graphics.DrawEllipse(pen, new Rectangle(rect.X + w, rect.Y + h, 3, 3));
                        }
                    }
                }
            }
        }
    }
}
