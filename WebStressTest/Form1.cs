using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Deamons.UI;
using System.Threading;
using System.Net;
using System.IO;

namespace WebStressTest {
    public partial class Form1 : Form {

        private List<Thread> _threads;
        double GI = 0;
        double CI = 0;
        bool StopThreads = false;

        public Form1() {
            InitializeComponent();
            Logging.TextBoxStatus = this.txtLog;

            _threads = new List<Thread>();
        }

        private void button1_Click( object sender, EventArgs e ) {
            txtLog.Text = "";
            StopThreads = false;
            btnStop.Text = "Stop";
            Logging.WriteStatus( "Starting Threads" );

            for ( int i = 0; i < numConThreads.Value; i++ ) {
                var thread = new Thread(
                        () => {
                            if ( StopThreads ) {
                                i = (int)numConThreads.Value;
                                return;
                            }
                            var t =  Thread.CurrentThread.Name;
                            for ( int j=0; j < numTries.Value; j++ ) {
                                var dtStart = DateTime.Now;
                                try {
                                    CallResource();
                                }
                                catch {
                                    Invoke( (MethodInvoker)delegate {
                                        Logging.WriteStatus( "*********Call " + j + " from thread " + t + " threw an exception!!!!!!!!!!" );
                                    } );
                                    continue;
                                }
                                GI += (DateTime.Now - dtStart).TotalMilliseconds;
                                CI++;

                                Invoke( (MethodInvoker)delegate {
                                    Logging.WriteStatus( "Call " + j + " from thread " + t + " duration " + (DateTime.Now - dtStart).TotalMilliseconds );
                                    lblStatus.Text = "Κλήση " + CI + " από " + (numConThreads.Value * numTries.Value);
                                } );

                                if ( StopThreads ) {
                                    Invoke( (MethodInvoker)delegate {
                                        Logging.WriteStatus( "Thread " + t + " stopped " );
                                    } );
                                    return;
                                }
                            }
                            OnWorkComplete();
                        }
                );
                thread.IsBackground = true;
                thread.Name = string.Format( "{0}", i );


                _threads.Add( thread );
                thread.Start();
            }

        }
        int OnWorkComplete_i = 0;
        void OnWorkComplete() {

            OnWorkComplete_i++;
            if ( numConThreads.Value <= OnWorkComplete_i ) {
                Invoke( (MethodInvoker)delegate {
                    Logging.WriteStatus( "Avg Time: " + (GI / (double)(numConThreads.Value * numTries.Value)) );
                    btnStop.Text = "Stop";
                } );
            }
            //StopThreads = false;

        }

        void CallResource() {
            var request = (HttpWebRequest)WebRequest.Create( txtURL.Text );
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "text/html";
            request.Timeout = 1000 * 60 * 60;

            using ( var response = (HttpWebResponse)request.GetResponse() ) {
                using ( var dataStream = response.GetResponseStream() ) {
                    using ( StreamReader reader = new StreamReader( dataStream ) ) {
                        var s = reader.ReadToEnd().ToString();
                    }
                }
            }
        }

        private void btnStop_Click( object sender, EventArgs e ) {
            StopThreads = true;
            btnStop.Text = "Stoping...";
            //Invoke( (MethodInvoker)delegate {
            //    Logging.WriteStatus( "Avg Time: " + (GI / (double)(numConThreads.Value + numConThreads.Value)) );
            //} );
        }
    }
}
