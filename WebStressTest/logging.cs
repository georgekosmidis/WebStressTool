using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStressTest {
    public static class Logging {

        public static string sHoldStatus;
        public static System.Windows.Forms.TextBox TextBoxStatus;
        private static DateTime dtStart = DateTime.Now;

        public static void WriteStatus( string sLabel ) {
            dtStart = DateTime.Now;//Reset Datetime
            sHoldStatus += (DateTime.Now.ToString( "G" ) + " " + sLabel).Trim() + System.Environment.NewLine;

            TextBoxStatus.Text = sHoldStatus;

            TextBoxStatus.SelectionStart = TextBoxStatus.Text.Length;
            TextBoxStatus.ScrollToCaret();
            TextBoxStatus.Refresh();
            //Application.DoEvents();

            //writeFile( System.AppDomain.CurrentDomain.FriendlyName + ".status", sHoldStatus, false );

        }
    }
}
