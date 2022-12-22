using System;
using Fiddler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiddlerCore_tuto_1
{
    public partial class Form1 : Form
    {
        private string separator = "----------------------------------------";
        public Form1()
        {
            InitializeComponent();
        }
        private void startfiddler()
        {
            if (!FiddlerApplication.IsStarted())
            {
                FiddlerApplication.AfterSessionComplete += FiddlerApplication_AfterSessionComplete;
                FiddlerApplication.Startup(8888, true, true, true);
            }
            else
            {
                appentext("fiddler is already running");
            }
        }

        private void FiddlerApplication_AfterSessionComplete(Session oSession)
        {
            if(oSession.RequestMethod == "CONNECT")
            {
                return;
            }
            if(oSession ==null || oSession.oRequest==null || oSession.oRequest.headers == null)
            {
                return;
            }
            string headers = oSession.oRequest.headers.ToString();
            string body = Encoding.UTF8.GetString(oSession.RequestBody);
            string firstline = oSession.RequestMethod + " " + oSession.fullUrl + " " + oSession.oRequest.headers.HTTPVersion;
            int at =  headers.IndexOf("\r\n");
            if (at < 0)
                return;
            string output = firstline+ Environment.NewLine + headers.Substring(at+1);
            if (body != null)
            {
                output += body + Environment.NewLine;
            }
            output += separator + Environment.NewLine;
            appentext(output);
        }

        private void stopfiddler()
        {
            if (!FiddlerApplication.IsStarted())
            {
            }
            else
            {
                FiddlerApplication.Shutdown();
            }
        }
        private void installcert()
        {
            if (!CertMaker.rootCertExists())
            {
                CertMaker.createRootCert();
                CertMaker.trustRootCert();
            }
        }
        private void removecert()
        {
            if (CertMaker.rootCertExists())
            {
                CertMaker.removeFiddlerGeneratedCerts();
            }
        }
        private void appentext(string value)
        {
            if (InvokeRequired)
            {
                richTextBox1.Invoke(new Action<string>(appentext), new object[] { value });
                return;
            }
            richTextBox1.AppendText(value);
            richTextBox1.ScrollToCaret();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopfiddler();
            if (FiddlerApplication.oProxy != null)
            {
                if (FiddlerApplication.oProxy.IsAttached)
                {
                    FiddlerApplication.oProxy.Detach();
                }
            }
        }

        private void start_Click(object sender, EventArgs e)
        {
            startfiddler();
        }

        private void stop_Click(object sender, EventArgs e)
        {
            stopfiddler();
        }

        private void install_Click(object sender, EventArgs e)
        {
            installcert();
        }

        private void remove_Click(object sender, EventArgs e)
        {
            removecert();
        }
    }
}
