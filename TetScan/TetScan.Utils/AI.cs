using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Windows.Forms;
using TetScan.TetScan.Utils;

namespace TetScan.TetScan.Utils
{
    class Response
    {
        public short[,] state;
        public int x;
        public int rot;
    }
    class AI
    {
        private delegate void setTextSafe(string text);
        private static TextBox tb;
        private static AI instance;
        TcpClient client;
        NetworkStream ns;
        ProcessMonitor pm;
        
        public AI()
        {
            client = new TcpClient();
            pm = new ProcessMonitor();
        }

        public static void setTextBoxRef(TextBox _tb)
        {
            tb = _tb;
        }

        private void setText(string text)
        {
            tb.Text = text;
        }

        public void loadModel(string path)
        {
            ActionRequest ar = new ActionRequest();
            ar.action = "load";
            ar.payload = path;

            string ar_string = JsonConvert.SerializeObject(ar);

            try
            {
                byte[] sendBuff = new byte[2048];
                byte[] recvBuff = new byte[2048];
                string recvMessage = "";

                sendBuff = Encoding.ASCII.GetBytes(ar_string);

                ns.Write(sendBuff, 0, sendBuff.Length);

                int bytesRead = ns.Read(recvBuff, 0, recvBuff.Length);
                Console.WriteLine(bytesRead);
                recvMessage += Encoding.ASCII.GetString(recvBuff, 0, bytesRead);
                Console.WriteLine(recvMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void getMove(short[,] state, bool isGhostEnabled)
        {
            ActionRequest ar = new ActionRequest();
            ar.action = "predict";
            ar.payload = state;
            ar.options = new Dictionary<string, string>
            {
                { "ghost", isGhostEnabled.ToString() }
            };
  
            string ar_string = JsonConvert.SerializeObject(ar);

            try
            {
                byte[] sendBuff = new byte[2048];
                byte[] recvBuff = new byte[2048];
                string recvMessage = "";

                sendBuff = Encoding.ASCII.GetBytes(ar_string);

                ns.Write(sendBuff, 0, sendBuff.Length);
                int bytesRead = ns.Read(recvBuff, 0, recvBuff.Length);
                recvMessage += Encoding.ASCII.GetString(recvBuff, 0, bytesRead);

                if(recvMessage != "none")
                {
                    Response ret = JsonConvert.DeserializeObject<Response>(recvMessage);
                    Console.WriteLine($"{ret.x}, {ret.rot}");
                    if(tb.InvokeRequired)
                    {
                        var d = new setTextSafe(setText);
                        string direction = "";
                        if(ret.x > 0)
                        {
                            direction = $"move right {Math.Abs(ret.x)} times.";
                        } else if(ret.x < 0)
                        {
                            direction = $"move left {Math.Abs(ret.x)} times.";
                        } else if(ret.x == 0)
                        {
                            direction = "don't move.";
                        }
                        tb.Invoke(d, new object[] { $"Last move: rotate {ret.rot} times, {direction}" });
                    }
                    performMoves(ret.x, ret.rot);
                } else
                {
                    // If no action was returned, wait 0.5 second and try again.
                    Console.WriteLine("Waiting for piece identification...");
                    System.Threading.Thread.Sleep(200);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void performMoves(int x, int rotation)
        {
            int moves = Math.Abs(x);

            // Rotation
            for (int rot = 0; rot < rotation; rot++)
            {
                // Perform rotation
                Mover.Rotate();
                System.Threading.Thread.Sleep(40);
            }

            for (int i = 0; i < moves; i++)
            {
                // X movement
                if (x > 0)
                {
                    Mover.Right();
                }
                else if (x < 0)
                {
                    Mover.Left();
                }

                System.Threading.Thread.Sleep(40);
            }

            Mover.Drop();
            System.Threading.Thread.Sleep(100);
        }

        public bool ping()
        {
            ActionRequest ar = new ActionRequest();
            ar.action = "ping";
            ar.payload = "";

            string ar_string = JsonConvert.SerializeObject(ar);

            try
            {
                byte[] sendBuff = new byte[2048];
                byte[] recvBuff = new byte[2048];
                string recvMessage = "";

                sendBuff = Encoding.ASCII.GetBytes(ar_string);

                ns.Write(sendBuff, 0, sendBuff.Length);
                int bytesRead = ns.Read(recvBuff, 0, recvBuff.Length);
                recvMessage += Encoding.ASCII.GetString(recvBuff, 0, bytesRead);

                if(recvMessage == "ok")
                {
                    return true;
                } else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public void startConnection()
        {
            if(!client.Connected)
            {
                client = new TcpClient("127.0.0.1", 49995);
                ns = client.GetStream();
            }
        }

        public void stopConnection()
        {
            client.Close();
        }

        public void startAIServer(TextBox tb, string pathToModel)
        {
            try
            {
                // Kill previous server intance
                string id_str = FileIO.read("last_instance");
                if (!string.IsNullOrEmpty(id_str))
                {
                    tb.AppendText("Terminating any previous server instances..." + Environment.NewLine);
                    Console.WriteLine($"String = {id_str}");
                    int id = int.Parse(id_str);
                    Console.WriteLine($"{id}");
                    ProcessMonitor.KillProcessAndChildren(id);
                }

                string pythonPath = "";
                List<string> pathEnv = Environment.GetEnvironmentVariable("PATH").Split(';').ToList();
                foreach (string p in pathEnv)
                {
                    if (p.Contains("Python3"))
                    {
                        pythonPath = p + "python.exe";
                    }
                }

                string AIRootDir = Path.Combine(Directory.GetCurrentDirectory(), "python\\tetris_training\\").Replace("\\bin\\Debug", "");
                string AIexe = AIRootDir + "AI.py";

                Process process = pm.Start(new ProcessStartInfo(pythonPath, $"{AIexe} --path {pathToModel}")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = true
                });

                FileIO.write(process.Id.ToString(), "last_instance");

                tb.AppendText("Starting AI Server..." + Environment.NewLine);

                ((Action)(async () =>
                {
                    int tries = 3;
                    bool responded = false;

                    while (tries > 0) {
                        await Task.Run(() => {
                            System.Threading.Thread.Sleep(4000);
                            startConnection();

                            if (ping())
                            {
                                responded = true;
                            }
                        });

                        if(responded)
                        {
                            break;
                        }

                        tries--;
                    }

                    if (responded)
                    {
                        tb.AppendText("AI server running..");
                    } else
                    {
                        tb.AppendText("ERROR: Server Timeout..");
                    }
                }))();

                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public static AI getInstance()
        {
            if (instance == null)
            {
                instance = new AI();
            }
            return instance;
        }
    }
}
