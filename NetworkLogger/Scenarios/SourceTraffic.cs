using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Xml;
using System.Threading;
using System.IO;

namespace StandardNetCapB.Scenarios
{
    public partial class SourceTraffic : Form
    {
        //Var
        private DataParameter _inputparameter;
        public SourceTraffic(string TestParams)
        {
            //parse testing string
            string[] arrayTestingString = TestParams.Split(',');
            //Initialize Form
            InitializeComponent();
            //Fill initial details
            fillFirstLoadingDetails(arrayTestingString);
            //Run background worker
            runBackgroundTask();
        }

        #region Functions
        //initial gui input
        private void fillFirstLoadingDetails(string[] arrayTestingString)
        {
            //fill Connecting to and type of packet capture
            lbl_ConnectingTo.Text = (arrayTestingString[0] + ":" + arrayTestingString[1]);
            lbl_PacketCaptureType.Text = arrayTestingString[2];
            //get ipv4 of the machine
            getIpv4();
        }
        //get IPv4
        private void getIpv4()
        {
            int i = 0;
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork && !ip.Address.ToString().Contains("127.0.0"))
                        {
                            lbl_ConnectingFrom.Text = ip.Address.ToString();
                            i++;
                        }
                    }
                }
            }
            if (i == 0)
            {
                lbl_ConnectingFrom.Text = "NoIPv4Found";
            }

        }
        //Start Packet capture
        private string packetCaptureStart(string packetCaptureType)
        {
            string fileNameEtl = "";

            if (packetCaptureType == "Circular")
            {
                 fileNameEtl = ("CircularPacketCapture_" + DateTime.Now.ToString("MMddyyyyTHHmmssff"));

                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.StandardInput.WriteLine(@"netsh trace start capture=yes tracefile="+ fileNameEtl + ".etl maxsize=1024 filemode=circular overwrite=yes report=no");
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
                string outputCMD = cmd.StandardOutput.ReadToEnd();
                if (outputCMD.ToLower().Contains("running"))
                {
                    MessageBox.Show(cmd.StandardOutput.ReadToEnd(), "Running Packet Capture", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (outputCMD.ToLower().Contains("not found"))
                {
                    MessageBox.Show(outputCMD + Environment.NewLine + Environment.NewLine + "----------------------------------" + Environment.NewLine + Environment.NewLine + " >>>>Command not found, please reach out to the josesilva@microsoft.com", "Permissions error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Windows.Forms.Application.Exit();
                }
                else
                {
                    MessageBox.Show(outputCMD + Environment.NewLine + Environment.NewLine + "----------------------------------" + Environment.NewLine + Environment.NewLine + " >>>>Please open the .exe file as administrator", "Permissions error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Windows.Forms.Application.Exit();
                }
            }
            else
            {
                 fileNameEtl = ("NormalPcktCapture_" + DateTime.Now.ToString("MMddyyyyTHHmmssff"));

                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                cmd.StandardInput.WriteLine(@"netsh trace start capture=yes tracefile=./Output/" + fileNameEtl + ".etl overwrite=yes maxsize=1024");
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
                string outputCMD = cmd.StandardOutput.ReadToEnd();
                if (outputCMD.ToLower().Contains("running"))
                {
                    MessageBox.Show(outputCMD, "Running Packet Capture", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (outputCMD.ToLower().Contains("not found"))
                {
                    MessageBox.Show(outputCMD + Environment.NewLine + Environment.NewLine + "----------------------------------" + Environment.NewLine + Environment.NewLine + " >>>>Command not found, please reach out to the josesilva@microsoft.com", "Permissions error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Windows.Forms.Application.Exit();
                }
                else
                {
                    MessageBox.Show(outputCMD + Environment.NewLine + Environment.NewLine+"----------------------------------"  +Environment.NewLine + Environment.NewLine + " >>>>Please open the .exe file as administrator", "Permissions error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Windows.Forms.Application.Exit();
                }
                invUpdateLSB(outputCMD);
            }
            return fileNameEtl;

        }
        //Stop Packet capture
        private void packetCaptureStop(string fileName,string flag = "Completed")
        {
            if (lbl_FilesPcktGenerated.InvokeRequired)
            {
                Action safeWrite = delegate
                {
                    btn_cancel.Enabled = false;
                    lbl_FilesPcktGenerated.Text = "Generating Files";
                    lbl_FilesPcktGenerated.ForeColor = Color.IndianRed;
                };
                lbl_FilesPcktGenerated.Invoke(safeWrite);
            }
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(@"netsh trace stop");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            string cmdOutput = cmd.StandardOutput.ReadToEnd();
            MessageBox.Show(cmdOutput);
            invUpdateLSB(cmdOutput);

            //convertFile
            convertEtl2Png(fileName);

            if (lbl_FilesPcktGenerated.InvokeRequired)
            {
                Action safeWrite = delegate
                {
                    btn_cancel.Enabled = true;
                    lbl_FilesPcktGenerated.Text = "Completed";
                    lbl_FilesPcktGenerated.ForeColor = Color.Green;
                };
                lbl_FilesPcktGenerated.Invoke(safeWrite);
            }
            //Fill Form details
            invPacketCaptureStop(flag);
        }
        //------------------------------Implement etl2png
        private void convertEtl2Png(string file)
        {
            Process cmd2 = new Process();
            cmd2.StartInfo.FileName = "cmd.exe";
            cmd2.StartInfo.RedirectStandardInput = true;
            cmd2.StartInfo.RedirectStandardOutput = true;
            cmd2.StartInfo.CreateNoWindow = true;
            cmd2.StartInfo.UseShellExecute = false;
            cmd2.Start();

            cmd2.StandardInput.WriteLine(@"etl2pcapng.exe ./Output/" + file +".etl"+ " ./Output/" + file+".pcapng");
            cmd2.StandardInput.Flush();
            cmd2.StandardInput.Close();
            cmd2.WaitForExit();
            string cmdOutput = cmd2.StandardOutput.ReadToEnd();
            MessageBox.Show(cmdOutput);
            invUpdateLSB(cmdOutput);
        }
        private void doTCPpingNormal(DoWorkEventArgs e, int iterations, int o, int x, int y, string filename)
        {
            //Instance Variable
            string sourceIP = "";
            //Destination
            var ip = IPAddress.Parse(lbl_ConnectingTo.Text.Split(':')[0]);
            int port = Convert.ToInt32(lbl_ConnectingTo.Text.Split(':')[1]);
            var ipEnd = new IPEndPoint(ip, port);
            //Time (ms)
            var times = new List<double>();
            //Iterations
            //   Ok -> o
            //   Nok -> x
            //   Total -> y

            for (int i = 0; i < iterations; i++)
            {
                //Check for cancel action
                if (backgroundWorker.CancellationPending == true)
                {
                    e.Cancel = true;
                    packetCaptureStop(filename);
                    invLogsOutputTcp();
                    //check if we had any successful communication
                    //updating statiscs with and without commmunications made
                    if (times.Count == 0)
                    {
                        invUpdateStatisticsLSB(ipEnd.Address.ToString(), ipEnd.Port.ToString(), sourceIP, y.ToString(), x.ToString(), o.ToString(), "", "", "");
                    }
                    else
                    {
                        invUpdateStatisticsLSB(ipEnd.Address.ToString(), ipEnd.Port.ToString(), sourceIP, y.ToString(), x.ToString(), o.ToString(), times.Min().ToString(), times.Average().ToString(), times.Max().ToString());
                    }
                    return;
                }
                //Create sock
                var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Blocking = true;
                //Instance StopWatch
                var stopwatch = new Stopwatch();
                // Measure the Connect call only
                stopwatch.Start();
                //Trying connection
                try
                {
                    sock.Connect(ipEnd);
                    stopwatch.Stop();
                    double t = stopwatch.Elapsed.TotalMilliseconds;
                    invUpdateOkLSB(o.ToString(), y.ToString(), ipEnd.Address.ToString(), ipEnd.Port.ToString(), sock.LocalEndPoint.ToString(), t.ToString());
                    times.Add(t);
                    o++;
                    y++;

                }
                catch (SocketException exc)
                {
                    invUpdateNokLSB(x.ToString(), y.ToString(), exc.Message.ToString(), exc.ErrorCode.ToString(), exc.SocketErrorCode.ToString(), lbl_ConnectingFrom.Text);
                    x++;
                    y++;
                }
                //check we're abble to get source+port, else get ip from network Interface
                if (sock.LocalEndPoint != null)
                {
                    sourceIP = sock.LocalEndPoint.ToString();
                }
                else
                {
                    sourceIP = lbl_ConnectingFrom.Text;

                }
                //check if we had any successful communication
                if (times.Count == 0)
                {
                    invUpdateStatistics(o, x, y, 0, 0, 0);
                }
                else
                {
                    invUpdateStatistics(o, x, y, times.Min(), times.Average(), times.Max());
                }
                //close connection
                sock.Close();
                //take a nap for 2 seconds
                System.Threading.Thread.Sleep(2000);
            }
            //updating statiscs with and without commmunications made
            if (times.Count == 0)
            {
                invUpdateStatisticsLSB(ipEnd.Address.ToString(), ipEnd.Port.ToString(), sourceIP, y.ToString(), x.ToString(), o.ToString(), "", "", "");
            }
            else
            {
                invUpdateStatisticsLSB(ipEnd.Address.ToString(), ipEnd.Port.ToString(), sourceIP, y.ToString(), x.ToString(), o.ToString(), times.Min().ToString(), times.Average().ToString(), times.Max().ToString());
            }

            //Stop packet capture and update GUI
            packetCaptureStop(filename);
            //Create txt and update gui
            invLogsOutputTcp();
        }
        private void doTCPpingCircular(DoWorkEventArgs e, string fileName)
        {
            int flag = 0;
            //Instance Variable
            string sourceIP = "";
            //Destination
            var ip = IPAddress.Parse(lbl_ConnectingTo.Text.Split(':')[0]);
            int port = Convert.ToInt32(lbl_ConnectingTo.Text.Split(':')[1]);
            var ipEnd = new IPEndPoint(ip, port);
            //Time (ms)
            var times = new List<double>();
            //Iterations
            //   Ok
            int o = 0;
            //   Nok
            int x = 0;

            //   Total
            int y = 0;
            for (; ; )
            {
                //Check for cancel action
                if (backgroundWorker.CancellationPending == true)
                {
                    e.Cancel = true;
                    //updating statiscs with and without commmunications made
                    if (times.Count == 0)
                    {
                        invUpdateStatisticsLSB(ipEnd.Address.ToString(), ipEnd.Port.ToString(), sourceIP, y.ToString(), x.ToString(), o.ToString(), "", "", "", "Stopped");
                    }
                    else
                    {
                        invUpdateStatisticsLSB(ipEnd.Address.ToString(), ipEnd.Port.ToString(), sourceIP, y.ToString(), x.ToString(), o.ToString(), times.Min().ToString(), times.Average().ToString(), times.Max().ToString(), "Stopped");
                    }


                    //Stop packet capture and update GUI
                    packetCaptureStop(fileName);
                    //Create txt and update gui
                    invLogsOutputTcp();
                    return;
                }
                //Create sock
                var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Blocking = true;
                //Instance StopWatch
                var stopwatch = new Stopwatch();
                // Measure the Connect call only
                stopwatch.Start();
                //Trying connection
                try
                {
                    sock.Connect(ipEnd);
                    stopwatch.Stop();
                    double t = stopwatch.Elapsed.TotalMilliseconds;
                    invUpdateOkLSB(o.ToString(), y.ToString(), ipEnd.Address.ToString(), ipEnd.Port.ToString(), sock.LocalEndPoint.ToString(), t.ToString());
                    times.Add(t);
                    o++;
                    y++;

                }
                catch (SocketException exc)
                {

                    invUpdateNokLSB(x.ToString(), y.ToString(), exc.Message.ToString(), exc.ErrorCode.ToString(), exc.SocketErrorCode.ToString(), lbl_ConnectingFrom.Text);
                    x++;
                    y++;
                    doTCPpingNormal(e, 15, o, x, y, fileName);
                    flag = 1;
                    break;
                }
                //check we're abble to get source+port, else get ip from network Interface
                if (sock.LocalEndPoint != null)
                {
                    sourceIP = sock.LocalEndPoint.ToString();
                }
                else
                {
                    sourceIP = lbl_ConnectingFrom.Text;

                }
                //check if we had any successful communication
                if (times.Count == 0)
                {
                    invUpdateStatistics(o, x, y, 0, 0, 0);
                }
                else
                {
                    invUpdateStatistics(o, x, y, times.Min(), times.Average(), times.Max());
                }
                //close connection
                sock.Close();
                //take a nap for 2 seconds
                System.Threading.Thread.Sleep(2000);
            }
            if (flag != 1)
            {
                //updating statiscs with and without commmunications made
                if (times.Count == 0)
                {
                    invUpdateStatisticsLSB(ipEnd.Address.ToString(), ipEnd.Port.ToString(), sourceIP, o.ToString(), x.ToString(), y.ToString(), "", "", "");
                }
                else
                {
                    invUpdateStatisticsLSB(ipEnd.Address.ToString(), ipEnd.Port.ToString(), sourceIP, o.ToString(), x.ToString(), y.ToString(), times.Min().ToString(), times.Average().ToString(), times.Max().ToString());
                }


                //Stop packet capture and update GUI
                packetCaptureStop(fileName);
                //Create txt and update gui
                invLogsOutputTcp();
            }

        }
        #endregion

        #region gui and stuff
        //disable exit button
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        //Timer ticking
        private void timerCurrent_Tick(object sender, EventArgs e)
        {
            lbl_CurrentTime.Text = DateTime.Now.ToLongTimeString();
        }
        //drag window
        /*protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
                m.Result = (IntPtr)(HT_CAPTION);
        }
        //drag window
        private const int WM_NCHITTEST = 0x84;
        private const int HT_CAPTION = 0x2;*/
        #endregion

        #region buttons
        private void btn_redo_Click(object sender, EventArgs e)
        {
            runBackgroundTask();
            btn_cancel.Text = "Stop";
            lbl_FilesPcktGenerated.ForeColor = Color.Black;
        }
        private void btn_cancel_Click(object sender, EventArgs e)
        {//logic for back/cancel bttn        
            if (btn_cancel.Text == "Back")
            {
                //move form to homeWinfow if text is back
                this.Hide();
                var form2 = new Home();
                form2.Closed += (s, args) => this.Close();
                form2.Show();
            }
            else
            {
                //cancel operation logic + change gui labels values

                var confirmResult = MessageBox.Show("Do you wish to stop the operation?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                btn_cancel.Enabled = false;
                if (confirmResult == DialogResult.Yes)
                {
                    if (backgroundWorker.IsBusy)
                    {
                        backgroundWorker.CancelAsync();
                        btn_finish.Enabled = true;
                        btn_redo.Enabled = true;
                        btn_cancel.Text = "Back";
                        lbl_StopPcktTime.Text = DateTime.Now.ToString();
                    }

                }
                else
                {
                    btn_cancel.Enabled = true;
                }

            }
        }
        private void btn_finish_Click(object sender, EventArgs e)
        {
            //little pop up message closing the app and exiting the program
            string message = "See you soon";
            string title = "Closing";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            System.Windows.Forms.Application.Exit();
        }
        private void btn_CpLogsPath_Click(object sender, EventArgs e)
        {
            //get path for files
            if(txt_PathLogs.Text != "")
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = txt_PathLogs.Text + @"\Output\",
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
           
        }
        #endregion

        #region invoke update main thread

        private void invUpdateLSB(string output)
        {
            if (lbl_StopPcktTime.InvokeRequired)
            {
                Action safeWrite = delegate
                {
                    lsb_OutputTCP.Items.Add(output);
                    lsb_OutputTCP.SelectedIndex = lsb_OutputTCP.Items.Count - 1;
                };
                lsb_OutputTCP.Invoke(safeWrite);

            }
            else
            {
                lsb_OutputTCP.Items.Add(output);
                lsb_OutputTCP.SelectedIndex = lsb_OutputTCP.Items.Count - 1;
            }

        }
        //Updating gui information after packet capture closure
        private void invPacketCaptureStop( string flag = "completed")
        {

            if (lbl_StopPcktTime.InvokeRequired)
            {
                Action safeWrite = delegate
                {
                    lbl_StopPcktTime.Text = DateTime.Now.ToString();
                    lbl_FilesPcktGenerated.Text = flag;
                    btn_finish.Enabled = true;
                    btn_redo.Enabled = true;
                    btn_cancel.Text = "Back";
                };
                lbl_StopPcktTime.Invoke(safeWrite);
            }
            else
            {
                lbl_StopPcktTime.Text = DateTime.Now.ToString();
                lbl_FilesPcktGenerated.Text = flag;
                btn_finish.Enabled = true;
                btn_redo.Enabled = true;
                btn_cancel.Text = "Back";
            }
        }
        //Updating LSB with OK TCP output
        private void invUpdateOkLSB(string a, string b, string ip, string port, string localCon, string ms)
        {
            if (lsb_OutputTCP.InvokeRequired)
            {
                Action safeWrite = delegate
                {
                    lsb_OutputTCP.Items.Add(("[" + a + "/" + b + "] - " + "[" + DateTime.Now.ToString() + "]  ++Connecting to " + ip + ":" + port + "  ++ From " + localCon + "  ++" + ms + "ms"));
                    lsb_OutputTCP.SelectedIndex = lsb_OutputTCP.Items.Count - 1;

                };
                lsb_OutputTCP.Invoke(safeWrite);
            }
            else
            {
                lsb_OutputTCP.Items.Add(("[" + a + "/" + b + "] - " + "[" + DateTime.Now.ToString() + "]  ++Connecting to " + ip + ":" + port + "  ++ From " + localCon + "  ++" + ms + "ms"));
                lsb_OutputTCP.SelectedIndex = lsb_OutputTCP.Items.Count - 1;
            }
        }
        //Updating LSB with NOK TCP output
        private void invUpdateNokLSB(string a, string b, string msg, string errorc, string sckterror, string localCon)
        {
            if (lsb_OutputTCP.InvokeRequired)
            {
                Action safeWrite = delegate
                {
                    lsb_OutputTCP.Items.Add("[" + a + "/" + b + "] - " + "[" + DateTime.Now.ToString() + "] " + msg + " | " + " Error Code: " + errorc + " - " + sckterror);
                    lsb_OutputTCP.SelectedIndex = lsb_OutputTCP.Items.Count - 1;
                };
                lsb_OutputTCP.Invoke(safeWrite);
            }
            else
            {
                lsb_OutputTCP.Items.Add("[" + a + "/" + b + "] - " + "[" + DateTime.Now.ToString() + "] " + msg + " | " + " Error Code: " + errorc + " - " + sckterror);
                lsb_OutputTCP.SelectedIndex = lsb_OutputTCP.Items.Count - 1;
            }
        }
        //Updating LSB statistics after TCP output end
        private void invUpdateStatisticsLSB(string ip, string port, string localCon, string a, string b, string c, string minMs, string msAvg, string maxMs, string flag = "completed")
        {
            if (lsb_OutputTCP.InvokeRequired)
            {
                Action safeWrite = delegate
                {
                    lsb_OutputTCP.Items.Add("");
                    lsb_OutputTCP.Items.Add("TCP connect ++To " + ip + ":" + port + "  ++From " + localCon.Split(':')[0]);
                    lsb_OutputTCP.Items.Add("Sent = " + a + ", Received = " + c + ", Lost = " + b);
                    lsb_OutputTCP.Items.Add("Minimum = " + minMs + "ms, Maxmimum = " + maxMs + "ms, Average = " + msAvg + "ms");
                    lsb_OutputTCP.Items.Add("");
                    lsb_OutputTCP.SelectedIndex = lsb_OutputTCP.Items.Count - 1;
                };
                btn_redo.Invoke(safeWrite);
            }
            else
            {
                lsb_OutputTCP.Items.Add("");
                lsb_OutputTCP.Items.Add("TCP connect ++To " + ip + ":" + port + "  ++From " + localCon);
                lsb_OutputTCP.Items.Add("Sent = " + a + ", Received = " + c + ", Lost = " + b);
                lsb_OutputTCP.Items.Add("Minimum = " + minMs + "ms, Maxmimum = " + maxMs + "ms, Average = " + msAvg + "ms");
                lsb_OutputTCP.Items.Add("");
                lsb_OutputTCP.SelectedIndex = lsb_OutputTCP.Items.Count - 1;
            }
        }
        //Creating txt file and update GUI
        private void invLogsOutputTcp()
        {
            using (StreamWriter writetext = new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory.ToString() + @"\Output\write_" + DateTime.Now.ToString("MMddyyyyTHHmmssff") +".txt"))
            {
                foreach (var item in lsb_OutputTCP.Items)
                {
                    writetext.WriteLine(item.ToString());
                }
            }

            if (lbl_FilesLogsGenerated.InvokeRequired)
            {
                Action safeWrite = delegate
                {
                    lbl_FilesLogsGenerated.Text = "Completed";
                    txt_PathLogs.Text = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
                    lbl_FilesPcktGenerated.ForeColor = Color.Green;
                };
                lbl_FilesLogsGenerated.Invoke(safeWrite);
            }
            else
            {
                lbl_FilesLogsGenerated.Text = "Completed";
                txt_PathLogs.Text = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
                lbl_FilesPcktGenerated.ForeColor = Color.Green;
            }
        }
        //Updating Details after New start
        private void invFillDetailsStart()
        {
            if (lbl_StartPcktTime.InvokeRequired)
            {
                // Call this same method but append THREAD2 to the text
                Action safeWrite = delegate
                {
                    btn_redo.Enabled = false; btn_finish.Enabled = false;
                    lbl_StartPcktTime.Text = DateTime.Now.ToString();
                    lbl_StopPcktTime.Text = "-----";
                    lbl_FilesLogsGenerated.Text = "-----";
                    lbl_FilesPcktGenerated.Text = "-----";

                };
                lbl_StartPcktTime.Invoke(safeWrite);
            }
            else
            {
                btn_finish.Enabled = false;
                btn_redo.Enabled = false;
                lbl_StartPcktTime.Text = DateTime.Now.ToString();
            }
        }
        //Updating Statistics
        private void invUpdateStatistics(int o, int x, int y, double tmin, double tavg, double tmax)
        {
            if (btn_finish.InvokeRequired)
            {
                Action safeWrite = delegate
                {
                    lbl_SuccessCount.Text = o.ToString();
                    lbl_FailedCount.Text = x.ToString();
                    lbl_TotalCount.Text = y.ToString();

                    lbl_MinMs.Text = tmin.ToString();
                    lbl_AvgMs.Text = tavg.ToString();
                    lbl_MaxMs.Text = tmax.ToString();
                };
                btn_redo.Invoke(safeWrite);
            }
            else
            {
                lbl_SuccessCount.Text = o.ToString();
                lbl_FailedCount.Text = x.ToString();
                lbl_TotalCount.Text = y.ToString();

                lbl_MinMs.Text = tmin.ToString();
                lbl_AvgMs.Text = tavg.ToString();
                lbl_MaxMs.Text = tmax.ToString();
            }

        }
        #endregion

        #region BackgroundWorker
        //Starting Background worker
        private void runBackgroundTask()
        {
            string fileName = "";
            if (!backgroundWorker.IsBusy)
            {
                _inputparameter.Delay = 100;
                _inputparameter.Proccess = 1200;
                backgroundWorker.RunWorkerAsync(_inputparameter);
            }
        }
        //DataParameter
        struct DataParameter
        {
            public int Proccess;
            public int Delay;
        }
        //Doing work
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Refresh Gui details
            invFillDetailsStart();
            //Start packet capture
            string fileName = packetCaptureStart(lbl_PacketCaptureType.Text);
            //logic normal or circular
            if (lbl_PacketCaptureType.Text == "normal")
            {

                doTCPpingNormal(e, 10, 0, 0, 0, fileName);
            }
            else
            {
                doTCPpingCircular(e, fileName);
            }





        }
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //no progress bar yet
        }
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //check if operation was canceled
            if (e.Cancelled)
            {
                //update gui with canceled status
                Action safeWrite = delegate
                {

                    btn_cancel.Enabled = true;

                };
                btn_cancel.Invoke(safeWrite);
                //pop short messagebox
                MessageBox.Show("Operation Stopped", "---", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Thread.Sleep(1500);
            }
            //check for errors
            else if (e.Error != null)
            {
                MessageBox.Show(e.ToString());
            }
            //else all went well
            else
            {
                MessageBox.Show("Operation has been completed.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }

        #endregion
    }





}
