using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

// Project includes
using TetScan.TetScan.Forms;
using TetScan.TetScan.Utils;

namespace TetScan
{
    public partial class MainForm : Form
    {
        private KeyInputForm keyInputForm;
        private bool isCapturing = false;
        private bool isCaptureAreaSet = false;
        private Bitmap capturedScreen = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainApp_Load(object sender, EventArgs e)
        {
            Extractor.getInstance();
            AI.getInstance();
        }

        private void MainApp_Shown(Object sender, EventArgs e)
        {
            Console.WriteLine(Application.StartupPath);
            Console.WriteLine(Directory.GetCurrentDirectory());
            string pathToModel = $"{Application.StartupPath}\\trained_models\\training1\\model.hdf";
            AI ai = AI.getInstance();
            AI.setTextBoxRef(logger);
            Console.WriteLine(pathToModel);
            ai.startAIServer(logger, pathToModel);
            currentState.Text = "Select game region...";
        }

        private void regionSelectButton_MouseEnter(object sender, EventArgs e)
        {
            if(capturedScreen != null)
            {
                Console.WriteLine("Disposing cs");
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            
            // Take screenshot
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

            Graphics g;
            capturedScreen = new Bitmap((int)screenWidth, (int)screenHeight);
            g = Graphics.FromImage(capturedScreen);
            g.CopyFromScreen(Point.Empty, Point.Empty, Screen.PrimaryScreen.Bounds.Size);

        }

        private void regionSelectButton_Click(object sender, EventArgs e)
        {
            using (RegionSelectForm regSelect = new RegionSelectForm(10, 20, capturedScreen))
            {
                if (regSelect.ShowDialog() == DialogResult.OK)
                {
                    Rectangle capArea = regSelect.getCaptureArea();
                    Extractor extractor = Extractor.getInstance();
                    extractor.setCaptureArea(capArea.X, capArea.Y, capArea.Width, capArea.Height);
                    extractor.setBackgroundColor(regSelect.getBackgroundColor());
                    isCaptureAreaSet = true;
                    startButton.Enabled = true;
                    currentState.Text = $"Game region: {capArea.Width}x{capArea.Height} at ({capArea.X}, {capArea.Y})";
                }
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            Extractor extractor = Extractor.getInstance();

            if (isCaptureAreaSet)
            {
                // Setup
                if (!isCapturing)
                {
                    extractor.setCaptureGrid(10, 20);
                    extractor.setGhostPiece(ghostPieceCheck.Checked);
                    extractor.start();
                    isCapturing = true;
                    startButton.Text = "Stop";
                } else // Running
                {
                    extractor.stop();
                    isCapturing = false;
                    startButton.Text = "Start";
                }
            }
        }

        private void inputConfigButton_Click(object sender, EventArgs e)
        {
            if (keyInputForm == null)
            {
                keyInputForm = new KeyInputForm();
            }
            keyInputForm.Show();
        }

        private void ghostPieceCheck_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

    public static class GraphicsExtensions
    {
        public static void DrawRectangle(this Graphics g, Pen pen, RectangleF rect) =>
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
    }
}
