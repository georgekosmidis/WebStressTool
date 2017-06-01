using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using System.Diagnostics;
using WebStressTest.Helpers;
using System.Reflection;
using WebStressTest.Attributes;

namespace WebStressTest
{
    public partial class Form1 : Form
    {

        private List<Thread> _threads;

        public Form1()
        {
            InitializeComponent();
            Logging.TextBoxStatus = this.txtLog;

            _threads = new List<Thread>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            absProperties.SelectedObject = new Models.AbsArguments();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Uri uriResult;
            //if (!Uri.TryCreate(txtURL.Text, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            //    MessageBox.Show("URL is not valid!");

            Process p = new Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.FileName = x64x86.IsWow64() ? "x64" + Path.DirectorySeparatorChar + "abs.exe" : "x86" + Path.DirectorySeparatorChar + "abs.exe";
            p.StartInfo.Arguments = GetArguments(absProperties.SelectedObject as Models.AbsArguments);
            p.Start();
            txtLog.Text = p.StandardOutput.ReadToEnd().Replace("\n", Environment.NewLine);
            p.WaitForExit();

        }

        private string GetArguments(Models.AbsArguments obj)
        {
            var sb = new StringBuilder();
            var url = "";
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                var argument = "";

                var val = property.GetValue(obj, null);
                if (val == null)
                    continue;
                foreach (object attr in property.GetCustomAttributes(true))
                    if (attr as ArgumentAttribute != null)
                        argument = (attr as ArgumentAttribute).Argument;
                if (argument == "")
                    continue;
                if (argument == "-B")
                {
                    url = val.ToString();
                    continue;
                }
                if (property.PropertyType == typeof(bool))
                {
                    if ((bool)val)
                        sb.Append(argument + " ");
                }
                else if (property.PropertyType == typeof(int))
                {
                    if ((int)val > 0)
                        sb.Append(argument + " " + val.ToString() + " ");
                }
                else if (property.PropertyType == typeof(Dictionary<string, string>))
                    sb.Append("");
                else if (property.PropertyType == typeof(List<string>))
                    sb.Append("");
                else
                    sb.Append(argument + " " + val.ToString() + " ");


            }
            sb.Append(url);
            return sb.ToString();
        }

    }
}
