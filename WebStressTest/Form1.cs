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
using WebStressTest.Models;

namespace WebStressTest
{
    public partial class Form1 : Form
    {

        private List<Thread> _threads;
        Process process = new Process();

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

        private async void button1_Click(object sender, EventArgs e)
        {
            if (!Validations(absProperties.SelectedObject as Models.AbsArguments))
                return;

            txtLog.Clear();

            await Task.Factory.StartNew(() =>
             {
                 ProcessStartInfo processToRunInfo = new ProcessStartInfo();
                 processToRunInfo.UseShellExecute = false;
                 processToRunInfo.CreateNoWindow = true;
                 processToRunInfo.RedirectStandardOutput = true;
                 processToRunInfo.RedirectStandardError = true;
                 processToRunInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                 processToRunInfo.FileName = x64x86.IsWow64() ? "x64" + Path.DirectorySeparatorChar + "abs.exe" : "x86" + Path.DirectorySeparatorChar + "abs.exe";
                 processToRunInfo.Arguments = GetArguments(absProperties.SelectedObject as Models.AbsArguments);
                 //processToRunInfo.WindowStyle = ProcessWindowStyle.Normal;
                 process = new Process();
                 process.StartInfo = processToRunInfo;
                 process.Start();
                 process.BeginOutputReadLine();
                 process.BeginErrorReadLine();
                 process.OutputDataReceived += delegate (object processSender, DataReceivedEventArgs processArgs)
                 {
                     txtLog.Invoke((MethodInvoker)delegate
                     {
                         if (processArgs.Data != null)
                             txtLog.AppendText(processArgs.Data.Replace("\n", Environment.NewLine) + Environment.NewLine);
                     });
                 };
                 process.ErrorDataReceived += delegate (object processSender, DataReceivedEventArgs processArgs)
                 {
                     txtLog.Invoke((MethodInvoker)delegate
                     {
                         if (processArgs.Data != null)
                             txtLog.AppendText(processArgs.Data.Replace("\n", Environment.NewLine) + Environment.NewLine);
                     });
                 };

                 process.WaitForExit();

             });
        }

        private bool Validations(Models.AbsArguments obj)
        {
            Uri uriResult;
            if (Uri.TryCreate(obj.LocalAddress, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                obj.LocalAddress = uriResult.ToString();
            else
            {
                txtLog.Clear();
                txtLog.Text = "Address is not valid!";
                return false;
            }

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
                    {
                        argument = (attr as ArgumentAttribute).Argument;
                        break;
                    }
                if (argument == "")
                    continue;

                if (property.PropertyType == typeof(int))
                    if ((int)val < 0)
                    {
                        txtLog.Clear();
                        txtLog.Text = property.Name +" must be greater than 0";
                        return false;
                    }
               
            }

            return true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (!process.HasExited)
                    process.Kill();
            }
            catch { }
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
                    {
                        argument = (attr as ArgumentAttribute).Argument;
                        break;
                    }
                if (argument == "")
                    continue;
                if (property.Name == "GnuplotFile")
                {
                    if ((bool)val)
                        sb.Append(argument + " " + GetFilePath("tsv") + " ");
                    continue;
                }
                if (property.Name == "CsvFile")
                {
                    if ((bool)val)
                        sb.Append(argument + " " + GetFilePath("csv") + " ");
                    continue;
                }

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
                else if (property.PropertyType == typeof(List<string>))
                    foreach (var l in val as List<string>)
                        sb.Append(argument + " " + l + " ");
                else if (property.PropertyType == typeof(Verbosity))
                    sb.Append(argument + " " + (int)val + " ");
                else if (property.PropertyType == typeof(Protocol))
                    sb.Append(argument + " " + val.ToString().Replace("_", ".") + " ");
                else
                    sb.Append(argument + " " + val.ToString() + " ");
            }

            sb.Append(url);

            return sb.ToString();
        }

        private string GetFilePath(string extension)
        {
            var filename = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "_" + (absProperties.SelectedObject as Models.AbsArguments).LocalAddress;
            foreach (var c in Path.GetInvalidFileNameChars())
                filename = filename.Replace(c, '_');
            filename = filename.Replace('.', '_').Replace(' ', '_').Replace('-', '_');

            return AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + filename + "." + extension;
        }


    }
}
