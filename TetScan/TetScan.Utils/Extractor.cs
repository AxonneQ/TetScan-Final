using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using TetScan.TetScan.Forms;
using System.IO;
using System.Windows.Forms;

namespace TetScan.TetScan.Utils
{ 
    class Extractor {
        private static Extractor instance;
        
        int capX;
        int capY;
        int capW;
        int capH;

        int gridW;
        int gridH;

        Color bgColor1;
        Color bgColor2;

        bool isCapturing = false;
        bool isGhostEnabled = false;
        AI ai;

        private Extractor() {
            ai = AI.getInstance();
  
            //ai.startConnection();

            string pathToModel = Path.Combine(Directory.GetCurrentDirectory(), "python\\tetris_training\\trained_models\\bestAI").Replace("\\bin\\Debug", "");

            //ai.loadModel(pathToModel);
        }

        public Mat loadImage(string path)
        {
            return CvInvoke.Imread(path);
        }

        public void setCaptureArea(int x, int y, int w, int h)
        {
            capX = x;
            capY = y;
            capW = w;
            capH = h;
        }

        public void setBackgroundColor(Color[] color_samples)
        {
            if (color_samples[0] == color_samples[1])
            {
                bgColor1 = color_samples[0];
                bgColor2 = new Color();
            } else
            {
                bgColor1 = color_samples[0];
                bgColor2 = color_samples[1];
            }
        }

        public void setGhostPiece(bool isGhostEnabled)
        {
            this.isGhostEnabled = isGhostEnabled;
        }

        public void setCaptureGrid(int _gridW, int _gridH)
        {
            gridW = _gridW;
            gridH = _gridH;
        }

        public async void start()
        {
            Console.WriteLine("Starting Extractor");
            ai.startConnection();
            isCapturing = true;

            await Task.Run(() => captureScreen(2));
        }

        public void stop()
        {
            ai.stopConnection();
            isCapturing = false;
            Console.WriteLine("Stopping Extractor");
        }

        public void captureScreen(int ups) {
            /*float interval = (1.0f / ups) * 1000;*/
            

            while (isCapturing)
            {
                Bitmap bitmap = new Bitmap(capW, capH);
                Graphics graphics = Graphics.FromImage(bitmap as Image);
                graphics.CopyFromScreen(capX, capY, 0, 0, bitmap.Size);

                Image<Bgr, Byte> img = bitmap.ToImage<Bgr, Byte>();
                Mat image = img.Mat;
                short[,] state;
                (state) = getState(image);
                // printState(state);
                ai.getMove(state, isGhostEnabled);
            }       
        }

        public short[,] getState(Mat image)
        {
            Mat mask = new Mat();

            ScalarArray bgColor1_lower = new ScalarArray(new MCvScalar(bgColor1.B - 1, bgColor1.G - 1, bgColor1.R - 1));
            ScalarArray bgColor1_upper = new ScalarArray(new MCvScalar(bgColor1.B, bgColor1.G, bgColor1.R));

            CvInvoke.InRange(image, bgColor1_lower, bgColor1_upper, mask);

            if (bgColor2.IsEmpty)
            {
                Mat mask2 = new Mat();

                ScalarArray bgColor2_lower = new ScalarArray(new MCvScalar(bgColor2.B - 1, bgColor2.G - 1, bgColor2.R - 1));
                ScalarArray bgColor2_upper = new ScalarArray(new MCvScalar(bgColor2.B, bgColor2.G, bgColor2.R));

                CvInvoke.InRange(image, bgColor2_lower, bgColor2_upper, mask2);

                CvInvoke.BitwiseAnd(mask, mask2, mask);
            }

            // Invert
            CvInvoke.BitwiseNot(mask, mask);

            Image<Gray, Single> outputImg = mask.ToImage<Gray, Single>();

            short[,] state = new short[gridH, gridW];

            int distW = image.Width / gridW;
            int distH = image.Height / gridH;

            for (int h = distH / 2, y = 0; y < gridH; h += distH, y++)
            {
                for (int w = distW / 2, x = 0; x < gridW; w += distW, x++)
                {
                    if (outputImg.Data[h, w, 0] > 0)
                    {
                        state[y, x] = 1;
                    }
                    else
                    {
                        state[y, x] = 0;
                    }
                }
            }

            return state;
        }

        public void printState(short[,] state)
        {
            Console.WriteLine("Current State:");
            for (int x = 0; x < state.GetLength(0); x += 1)
            {
                for (int y = 0; y < state.GetLength(1); y += 1)
                {
                    Console.Write(state[x, y] + " ");
                }
                Console.WriteLine();
            }
        }

        public static Extractor getInstance()
        {
            if (instance == null )
            {
                instance = new Extractor();
            }
            return instance;
        }
    }
}
