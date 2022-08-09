using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net;
using StandardNetCapB.Scenarios;

namespace StandardNetCapB
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
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
        private void btn_next_Click(object sender, EventArgs e)
        {   
            if(txt_String.Text == "")
            {
                MessageBox.Show("Please insert a testing string", "", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {
                if (txt_String.Text == "**")
                {
                    this.Hide();
                    var form2 = new PcktCap();
                    form2.Closed += (s, args) => this.Close();
                    form2.Show();
                }
                else
                {
                    try
                    {
                        string[] array = txt_String.Text.Split(',');
                        if (array.Length == 3)
                        {
                            try
                            {
                                var ip = IPAddress.Parse(array[0]);
                                var port = Convert.ToInt32(array[1]);
                                if (array[2].ToLower() == "circular" || array[2].ToLower() == "normal")
                                {
                                    this.Hide();
                                    var form2 = new SourceTraffic(txt_String.Text);
                                    form2.Closed += (s, args) => this.Close();
                                    form2.Show();
                                }
                                else
                                {
                                    MessageBox.Show("Please insert on the string either \"circular\" or \"normal\" according to the packet capture you need", "", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                }
                            }
                            catch (Exception excpe)
                            {
                                MessageBox.Show(excpe.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);


                            }

                        }

                    }
                    catch (Exception excp)
                    {
                        MessageBox.Show(excp.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Do you wish to leave?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (confirmResult == DialogResult.Yes)
            {
                
                System.Windows.Forms.Application.Exit();

            }
           
            
           
        }

        private void btn_Info_Click(object sender, EventArgs e)
        {
            string message = "Please insert the following string accordingly:" + Environment.NewLine+ Environment.NewLine + "Generate TCP traffic and Packet Capture: <DestinationIP>,<TCPport>,<Normal/Circular>" + Environment.NewLine + "Packet Capture: **";
            string title = "Closing";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

    

        private void txt_String_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_next_Click(sender, e);
            }
        }
    }
}
