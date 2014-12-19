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
            if ( txtURL.Text.Substring( 0, "http://".Length ) != "http://" ) {
                MessageBox.Show( "URL must start with http://", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            txtLog.Text = "";
            StopThreads = false;
            btnStop.Text = "Stop";
            _threads.Clear();
            GI = 0;
            CI = 0;
            OnWorkComplete_i = 0;
            Logging.WriteStatus( "Initializing Threads" );

            for ( int i = 0; i < numConThreads.Value; i++ ) {
                var thread = new Thread(
                        () => {
                            if ( StopThreads ) {
                                i = (int)numConThreads.Value;
                                return;
                            }
                            var t =  Thread.CurrentThread.Name;
                            for ( int j=0; j < numTries.Value; j++ ) {
                                double dTM = 0;
                                try {
                                    dTM = CallResource();
                                }
                                catch ( Exception ex ) {
                                    Invoke( (MethodInvoker)delegate {
                                        Logging.WriteStatus( "*********Call " + (j + 1) + " from thread " + t + " said: " + ex.Message );
                                    } );
                                    continue;
                                }
                                GI += dTM;
                                CI++;

                                Invoke( (MethodInvoker)delegate {
                                    Logging.WriteStatus( "Call " + j + " from thread " + t + " duration " + dTM );
                                    lblStatus.Text = "Call " + CI + " from " + (numConThreads.Value * numTries.Value);
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
                thread.Name = string.Format( "{0}", (i + 1) );

                _threads.Add( thread );
            }
            Logging.WriteStatus( "Executing Threads" );
            foreach ( var t in _threads )
                t.Start();

        }

        int OnWorkComplete_i = 0;
        void OnWorkComplete() {

            OnWorkComplete_i++;
            if ( numConThreads.Value <= OnWorkComplete_i ) {
                Invoke( (MethodInvoker)delegate {
                    Logging.WriteStatus( "Avg Time: " + (GI / CI) );
                    btnStop.Text = "Stop";
                } );
            }
            //StopThreads = false;

        }

        private double CallResource() {
            var dt = DateTime.Now;

            var request = (HttpWebRequest)WebRequest.Create( txtURL.Text );
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "text/html";
            request.Timeout = 1000 * 60 * 60;

            using ( var response = (HttpWebResponse)request.GetResponse() ) {
                using ( var dataStream = response.GetResponseStream() ) {
                    using ( StreamReader reader = new StreamReader( dataStream ) ) {
                        var s = reader.ReadToEnd().ToString();
                        return (DateTime.Now - dt).TotalMilliseconds;
                    }
                }
            }
        }

        private void btnStop_Click( object sender, EventArgs e ) {
            StopThreads = true;
            btnStop.Text = "Stoping...";
            foreach ( var t in _threads ) {
                t.Abort();
            }
            //Invoke( (MethodInvoker)delegate {
            //    Logging.WriteStatus( "Avg Time: " + (GI / (double)(numConThreads.Value + numConThreads.Value)) );
            //} );
        }
    }
}
