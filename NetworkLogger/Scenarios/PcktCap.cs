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

namespace StandardNetCapB.Scenarios
{
    public partial class PcktCap : Form
    {
        private string fileNameEtl = "";
        public PcktCap()
        {
            InitializeComponent();

            btn_CpLogsPath.Enabled = false;
            packetCaptureStart();
            btn_finish.Text = "Stop";
        }
        Stopwatch watch = new Stopwatch();


        private void btn_finish_Click(object sender, EventArgs e)
        {
            if (btn_finish.Text == "Stop")
            {
                packetCaptureStop();
                btn_finish.Text = "Exit";
            }
            else
            {
                //little pop up message closing the app and exiting the program
                string message = "See you soon";
                string title = "Closing";
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                System.Windows.Forms.Application.Exit();
            }

        }




        //Start Packet capture
        private void packetCaptureStart()
        {

            fillDetailsStart();
            fileNameEtl = ("CircularPckt" + DateTime.Now.ToString("MMddyyyyTHHmmssff"));

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.StandardInput.WriteLine("");
            //cmd.StandardInput.WriteLine(@"pktmon start  --capture -f "+fileNameEtl+".etl -s 1024");
            cmd.StandardInput.WriteLine("netsh trace start capture=yes tracefile=./Output/" + fileNameEtl + ".etl maxsize=1024 filemode=circular overwrite=yes report=no ");

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
                MessageBox.Show(outputCMD + Environment.NewLine + Environment.NewLine + "----------------------------------" + Environment.NewLine + Environment.NewLine + " >>>>Please open the .exe file as administrator", "Permissions error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }

        }
        //Stop Packet capture
        private void packetCaptureStop()
        {
            btn_finish.Enabled = false;
            lbl_flag.Text = "Generating Files";
            lbl_flag.ForeColor = Color.IndianRed;
            Process cmd2 = new Process();
            cmd2.StartInfo.FileName = "cmd.exe";
            cmd2.StartInfo.RedirectStandardInput = true;
            cmd2.StartInfo.RedirectStandardOutput = true;
            cmd2.StartInfo.CreateNoWindow = true;
            cmd2.StartInfo.UseShellExecute = false;
            cmd2.Start();

            //cmd2.StandardInput.WriteLine("pktmon stop");
            cmd2.StandardInput.WriteLine("netsh trace stop");
            cmd2.StandardInput.Flush();
            cmd2.StandardInput.Close();
            cmd2.WaitForExit();
            MessageBox.Show(cmd2.StandardOutput.ReadToEnd());
            convertEtl2Png(fileNameEtl);
            FillDetailsStop();
            btn_finish.Enabled = true;
        }

        private void convertEtl2Png(string file)
        {
            Process cmd2 = new Process();
            cmd2.StartInfo.FileName = "cmd.exe";
            cmd2.StartInfo.RedirectStandardInput = true;
            cmd2.StartInfo.RedirectStandardOutput = true;
            cmd2.StartInfo.CreateNoWindow = true;
            cmd2.StartInfo.UseShellExecute = false;
            cmd2.Start();


            cmd2.StandardInput.WriteLine(@"etl2pcapng.exe ./Output/" + file + ".etl " + " ./Output/" + file + ".pcapng");
            cmd2.StandardInput.Flush();
            cmd2.StandardInput.Close();
            cmd2.WaitForExit();
            MessageBox.Show(cmd2.StandardOutput.ReadToEnd());
        }
        private void fillDetailsStart()
        {

            lbl_PacketCaptureType.Text = "Circular";
            lbl_flag.Text = "Running";
            lbl_flag.ForeColor = Color.Green;
            lbl_flag.Font = new Font(Font.FontFamily, 10, FontStyle.Bold);
            lbl_StartPcktTime.Text = DateTime.Now.ToString();
        }
        private void FillDetailsStop()
        {

            lbl_StopPcktTime.Text = DateTime.Now.ToString();

            lbl_flag.ForeColor = Color.Black;
            lbl_FilesPcktGenerated.Text = "Completed";
            lbl_FilesPcktGenerated.ForeColor = Color.Green;
            lbl_flag.Font = new Font(Font.FontFamily, 11, FontStyle.Regular);
            lbl_flag.Text = "Stopped";
            btn_CpLogsPath.Enabled = true;
            txt_PathLogs.Text = System.AppDomain.CurrentDomain.BaseDirectory.ToString()+ @"Output\";

        }

        private void btn_CpLogsPath_Click(object sender, EventArgs e)
        {
            //get path for files
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = txt_PathLogs.Text,
                UseShellExecute = true,
                Verb = "open"
            });
        }
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

    }
}
