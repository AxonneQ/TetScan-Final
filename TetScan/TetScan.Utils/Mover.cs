using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TetScan.TetScan.Utils
{
    [Flags]
    public enum KeyEventF
    {
        KeyDown = 0x0000,
        ExtendedKey = 0x0001,
        KeyUp = 0x0002,
        Unicode = 0x0004,
        Scancode = 0x0008
    }

    class Mover
    {
        Mover() { }

        public enum TetrisKeys:int
        {
            Left, Right, Rotate, Drop
        }

        public static (ushort scancode, bool extended)[] TetrisControls = {
            (0x4b, true),   // Left Arrow
            (0x4d, true),   // Right Arrow
            (0x2c, false),  // Z
            (0x39, false)   // Space
        };

        public static void setKey(TetrisKeys tetrisKey, KeyEventArgs keyEvent )
        {
            ushort scancode = (ushort) MapVirtualKey((uint)keyEvent.KeyValue, 0);

            if( keyEvent.KeyCode == Keys.Up ||
                keyEvent.KeyCode == Keys.Down ||
                keyEvent.KeyCode == Keys.Left ||
                keyEvent.KeyCode == Keys.Right )
            {
                TetrisControls[(int)tetrisKey] = (scancode, true);
            } else
            {
                TetrisControls[(int)tetrisKey] = (scancode, false);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardInput
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseInput
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HardwareInput
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)] public MouseInput mi;
            [FieldOffset(0)] public KeyboardInput ki;
            [FieldOffset(0)] public HardwareInput hi;
        }

        public struct Input
        {
            public int type;
            public InputUnion u;
        }

        [Flags]
        public enum KeyEventF
        {
            KeyDown = 0x0000,
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            Unicode = 0x0004,
            Scancode = 0x0008
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        public static void SendKeyboardInput(KeyboardInput[] kbInputs)
        {
            Input[] inputs = new Input[kbInputs.Length];

            for (int i = 0; i < kbInputs.Length; i++)
            {
                inputs[i] = new Input
                {
                    type = 1,
                    u = new InputUnion
                    {
                        ki = kbInputs[i]
                    }
                };
            }

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        public static void PressKey((ushort scancode, bool extended) key)
        {
            if(key.extended == true)
            {
                SendKeyboardInput(new KeyboardInput[]
                {
                    new KeyboardInput
                    {
                        wScan = 0xe0,
                        dwFlags = (uint)(KeyEventF.ExtendedKey | KeyEventF.Scancode),
                    },
                    new KeyboardInput
                    {
                        wScan = key.scancode,
                        dwFlags = (uint)(KeyEventF.ExtendedKey | KeyEventF.Scancode | KeyEventF.KeyDown)
                    },
                    new KeyboardInput
                    {
                        wScan = key.scancode,
                        dwFlags = (uint)(KeyEventF.ExtendedKey | KeyEventF.Scancode | KeyEventF.KeyUp),
                    }
                });
            } else {
                SendKeyboardInput(new KeyboardInput[]
                {
                    new KeyboardInput
                    {
                        wScan = key.scancode,
                        dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
                    },
                    new KeyboardInput
                    {
                        wScan = key.scancode,
                        dwFlags = (uint)(KeyEventF.KeyUp | KeyEventF.Scancode),
                    }
                });
            }
        }

        public static void Left()
        {
            PressKey(TetrisControls[(int)TetrisKeys.Left]);
        }

        public static void Right()
        {
            PressKey(TetrisControls[(int)TetrisKeys.Right]);
        }

        public static void Rotate()
        {
            PressKey(TetrisControls[(int)TetrisKeys.Rotate]);
        }

        public static void Drop()
        {
            PressKey(TetrisControls[(int)TetrisKeys.Drop]);
        }
    }
}
