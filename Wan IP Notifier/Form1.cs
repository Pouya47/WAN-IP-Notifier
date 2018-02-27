using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using xClient.Core.Helper;
using System.Threading;
using System.Net;

namespace wan_IP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// my previous wan ip.
        /// </summary>
        string prev_ip = "";

        private void Form1_Load(object sender, EventArgs e)
        {
            loading f = new loading();
            f.Show();
            f.Refresh();
            getWanInfo("");
            f.Close();
            maskedTextBox1.ValidatingType = typeof(System.Net.IPAddress);
            maskedTextBox1.TypeValidationCompleted += new TypeValidationEventHandler(IPAdress_TypeValidationCompleted);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPAddress ip;
            if (IPAddress.TryParse(textBox1.Text, out ip))
            {
                getWanInfo(ip.ToString());
                errorProvider1.Clear();
                
            }
            else
            {
                errorProvider1.SetError(textBox1, "IP Address is incorrect");
                return;
            }

            //If you want use masked textbox uncomment it.
            /* 
            if (errorProvider1.GetError(maskedTextBox1) == "" && maskedTextBox1.Text != "   .   .   .")
            {
                getWanInfo(maskedTextBox1.Text);
            }
            else
                errorProvider1.SetError(maskedTextBox1, "Enter Ip Address");
                */
        }

        private void buttonmyWanIP_Click(object sender, EventArgs e)
        {
            getWanInfo("");
        }

        /// <summary>
        /// Sraw wan info.
        /// </summary>
        /// <param name="ip">if is null return current ip info.</param>
        void getWanInfo(string ip)
        {
            toolStripStatusLabelWoring.Text = "Working";
            
            this.Invoke((MethodInvoker)delegate
            {
                Refresh();
            });

            GeoLocationHelper geoip = new GeoLocationHelper();
            geoip.Initialize(ip);

            string country = geoip.GeoInfo.CountryCode;
            if (country == null)//No country found
                country = "xy";
            else
                country = country.ToLower();

            string[] internal_names = new string[] { "as", "do", "in", "is" };
            if (internal_names.Contains(country))
                country = "_" + country;
            pictureBox1.Image = (Image)WAN_IP_Notifier.Properties.Resources.ResourceManager.GetObject(country);

            
            if (geoip.GeoInfo.Status != "success")
            {
                //labelStatus.Text = "Error";
                labelStatus.Invoke((MethodInvoker)delegate
                {
                    labelStatus.Text = "Error";
                    labelStatus.ForeColor = Color.Red;
                });

                labelAS.Invoke((MethodInvoker)delegate { labelAS.Text = ""; });
                labelCity.Invoke((MethodInvoker)delegate { labelCity.Text = ""; });
                labelCountry.Invoke((MethodInvoker)delegate { labelCountry.Text = ""; });
                labelIP.Invoke((MethodInvoker)delegate { labelIP.Text = ""; });
                labelIsp.Invoke((MethodInvoker)delegate { labelIsp.Text = ""; });
                labelOrganization.Invoke((MethodInvoker)delegate { labelOrganization.Text = ""; });
                labelRegion.Invoke((MethodInvoker)delegate { labelRegion.Text = ""; });
                labelRegionName.Invoke((MethodInvoker)delegate { labelRegionName.Text = ""; });
                labelTimeZone.Invoke((MethodInvoker)delegate { labelTimeZone.Text = ""; });
                labelZip.Invoke((MethodInvoker)delegate { labelZip.Text = ""; });

                notifyIcon1.ShowBalloonTip(10, "Error", "Error in get wan IP",ToolTipIcon.Error);
            }

            else
            {

                labelStatus.Invoke((MethodInvoker)delegate{
                    labelStatus.Text = geoip.GeoInfo.Status;
                    labelStatus.ForeColor = Color.Green;
                });

                labelAS.Invoke((MethodInvoker)delegate { labelAS.Text = geoip.GeoInfo.As; });
                labelCity.Invoke((MethodInvoker)delegate { labelCity.Text = geoip.GeoInfo.City; });
                labelCountry.Invoke((MethodInvoker)delegate { labelCountry.Text = geoip.GeoInfo.Country + " (" + geoip.GeoInfo.CountryCode + ")"; });
                labelIP.Invoke((MethodInvoker)delegate { labelIP.Text = geoip.GeoInfo.Ip; });
                labelIsp.Invoke((MethodInvoker)delegate { labelIsp.Text = geoip.GeoInfo.Isp; });
                labelOrganization.Invoke((MethodInvoker)delegate { labelOrganization.Text = geoip.GeoInfo.Org; });
                labelRegion.Invoke((MethodInvoker)delegate { labelRegion.Text = geoip.GeoInfo.Region; });
                labelRegionName.Invoke((MethodInvoker)delegate { labelRegionName.Text = geoip.GeoInfo.RegionName; });
                labelTimeZone.Invoke((MethodInvoker)delegate { labelTimeZone.Text = geoip.GeoInfo.Timezone; });
                labelZip.Invoke((MethodInvoker)delegate { labelZip.Text = geoip.GeoInfo.Zip; });

                toolStripStatusLabel1.Text = "Last update: " + DateTime.Now.ToString();

                if (ip == "")//if is local ip
                    if (prev_ip != geoip.GeoInfo.Ip)
                    {
                        notifyIcon1.ShowBalloonTip(20, "WAN ip changed", String.Format("New Wan IP is: {0} ({1})",
                                                                        geoip.GeoInfo.Ip, geoip.GeoInfo.Country),
                                                                ToolTipIcon.Info);
                        notifyIcon1.Text=prev_ip = geoip.GeoInfo.Ip;

                    }
            }
            toolStripStatusLabelWoring.Text = "Idle";
            // Refresh();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new AboutBox1().ShowDialog();
        }

       
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        Thread thread;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (thread == null || !thread.IsAlive)
                {
                    thread = new Thread(() => getWanInfo(""));
                    thread.IsBackground = true;
                    thread.Start();
                }
                // else
                // MessageBox.Show("sorry is running");
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
           // timer1.Interval = (int)numericUpDown1.Value * 1000;
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            timer1.Interval = (int)numericUpDown1.Value * 1000;
            if (checkBox1.Checked)
                numericUpDown1.Enabled = false;
            else
                numericUpDown1.Enabled = true;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                //notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                //notifyIcon1.Visible = false;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripMenuItemShow_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
       
       
        void IPAdress_TypeValidationCompleted(object sender, TypeValidationEventArgs e)
        {
            if (!e.IsValidInput)
            {
                errorProvider1.SetError(this.maskedTextBox1, "INVALID IP!");
            }
            else
            {
                errorProvider1.SetError(this.maskedTextBox1, String.Empty);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow 3 . in text
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.Split('.').Length>3))
            {
                e.Handled = true;
            }
        }
    }
}
