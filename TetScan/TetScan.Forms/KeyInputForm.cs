using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TetScan.TetScan.Utils;

namespace TetScan.TetScan.Forms
{
    public partial class KeyInputForm : Form
    {
        private bool isBinding;

        private Button pressedButton;
        private string pressedButtonText;

        public KeyInputForm()
        {
            InitializeComponent();
            isBinding = false;

            UpdateKeyBindings();

            foreach (Control control in Controls)
            {
                control.PreviewKeyDown += new PreviewKeyDownEventHandler(KeyInputForm_PreviewKeyDown);
            }
        }

        private void UpdateKeyBindings()
        {
            KeysConverter kc = new KeysConverter();
            List<string> keys = new List<string>();

            foreach((ushort s, bool e) move in Mover.TetrisControls)
            {
                uint keyCode = Mover.MapVirtualKey(move.s, 1);
                keys.Add(kc.ConvertToString((Keys)keyCode));
            }

            Console.WriteLine(keys);

            LeftText.Text = keys[0];
            RightText.Text = keys[1];
            RotateText.Text = keys[2];
            DropText.Text = keys[3];
        }

        private void KeyInputForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (isBinding)
            {
                switch (pressedButtonText)
                {
                    case "Left":
                        Mover.setKey(Mover.TetrisKeys.Left, e);
                        break;

                    case "Right":
                        Mover.setKey(Mover.TetrisKeys.Right, e);
                        break;

                    case "Rotate":
                        Mover.setKey(Mover.TetrisKeys.Rotate, e);
                        break;

                    case "Drop":
                        Mover.setKey(Mover.TetrisKeys.Drop, e);
                        break;
                }
                UpdateKeyBindings();
                pressedButton.Text = pressedButtonText;

                isBinding = false;
            } 
            e.Handled = true;
        }


        private void Button_Click(object sender, EventArgs e)
        {
            if(pressedButton != null)
            {
                pressedButton.Text = pressedButtonText;
            }

            Button btn = sender as Button;
            pressedButton = btn;
            pressedButtonText = btn.Text;
            
            isBinding = true;

            btn.Text = "Press Key";
        }

        private void KeyInputForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void KeyInputForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }
    }
}
