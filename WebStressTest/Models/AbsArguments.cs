using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStressTest.Attributes;
using WebStressTest.Converters;
using WebStressTest.Editors;

namespace WebStressTest.Models
{
    public class AbsArguments
    {
        //http://httpd.apache.org/docs/current/programs/ab.html
        //

        [Category("Authentication")]
        [DisplayName("Auth User/Pass")]
        [Description("Supply BASIC Authentication credentials to the server. The username and password are separated by a single : and sent on the wire base64 encoded. The string is sent regardless of whether the server needs it (i.e., has sent an 401 authentication needed).")]
        [Argument("-A")]
        public string AuthUsernamePassword { get; set; }

        [Category("Authentication")]
        [DisplayName("Proxy Auth User/Pass")]
        [Description("SSupply BASIC Authentication credentials to a proxy en-route. The username and password are separated by a single : and sent on the wire base64 encoded. The string is sent regardless of whether the proxy needs it (i.e., has sent an 407 proxy authentication needed).")]
        [Argument("-p")]
        public string ProxyAuthenticationNeeded { get; set; }

        [Category("Benchmarking")]
        [DisplayName("Concurrency")]
        [Description("Number of multiple requests to perform at a time. Default is one request at a time.")]
        [Argument("-c")]
        [DefaultValue(10)]
        //[TypeConverter(typeof(NumericUpDownTypeConverter))]
        //[Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMaxAttribute(0, 32)]
        public int Concurrency { get; set; } = 10;

        [Category("Benchmarking")]
        [DisplayName("Requests")]
        [Description("Number of requests to perform for the benchmarking session. The default is to just perform a single request which usually leads to non-representative benchmarking results.")]
        [Argument("-n")]
        [DefaultValue(10)]
        public int Requests { get; set; } = 10;

        [Category("Request")]
        [DisplayName("POST File")]
        [Description("File containing data to POST. Remember to also set Http Method and Content Type.")]
        [Argument("-p")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string PostFile { get; set; }

        [Category("Request")]
        [DisplayName("PUT File")]
        [Description("File containing data to PUT. Remember to also set Http Method and Content Type.")]
        [Argument("-u")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string PutFile { get; set; }

        [Category("Benchmarking")]
        [DisplayName("Timeout")]
        [Description("Maximum number of seconds to wait before the socket times out.")]
        [Argument("-s")]
        [DefaultValue(30)]
        public int TimeOut { get; set; } = 30;

        [Category("Benchmarking")]
        [DisplayName("Time Limit")]
        [Description("Maximum number of seconds to spend for benchmarking. This implies a -n 50000 internally. Use this to benchmark the server within a fixed total amount of time. Per default there is no timelimit.")]
        [Argument("-t")]
        [DefaultValue(0)]
        public int TimeLimit { get; set; } = 0;

        [Category("Reporting")]
        [DisplayName("Verbosity")]
        [Description("Set verbosity level - 4 and above prints information on headers, 3 and above prints response codes (404, 200, etc.), 2 and above prints warnings and info.")]
        [Argument("-v")]
        [DefaultValue(Verbosity.Default)]
        public Verbosity Verbosity { get; set; }

        [Category("Reporting")]
        [DisplayName("Save a CSV File")]
        [Description("Write a Comma separated value (CSV) file which contains for each percentage (from 1% to 100%) the time (in milliseconds) it took to serve that percentage of the requests. This is usually more useful than the 'gnuplot' file; as the results are already 'binned'")]
        [Argument("-e")]
        [DefaultValue(false)]
        public bool CsvFile { get; set; }

        [Category("Reporting")]
        [DisplayName("Save a GnuPlot File")]
        [Description("Write all measured values out as a 'gnuplot' or TSV (Tab separate values) file. This file can easily be imported into packages like Gnuplot, IDL, Mathematica, Igor or even Excel. The labels are on the first line of the file.")]
        [Argument("-g")]
        [DefaultValue(false)]
        public bool GnuplotFile { get; set; }

        [Category("Request")]
        [DisplayName("Window Size")]
        [Description("Size of TCP send/receive buffer, in bytes.")]
        [Argument("-b")]
        public string WindowSize { get; set; }

        [Category("Request")]
        [DisplayName("Address")]
        [Description("Address to bind to when making outgoing connections.")]
        [Argument("-B")]
        [DefaultValue("https://www.google.com/")]
        public string LocalAddress { get; set; } = "https://www.google.com/";

        [Category("Request")]
        [DisplayName("Cookies")]
        [Description("Add a Cookie: line to the request. The argument is typically in the form of a name=value pair.")]
        [Argument("-C")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(CsvConverter))]
        public List<string> Cookies { get; set; } = new List<String>();

        [Category("Request")]
        [DisplayName("Protocol")]
        [Description("Specify SSL/TLS protocol ")]
        [Argument("-f")]
        [DefaultValue(Protocol.ALL)]
        public Protocol Protocol { get; set; }

        [Category("Request")]
        [DisplayName("Custom Header")]
        [Description("Append extra headers to the request. The argument is typically in the form of a valid header line, containing a colon-separated field-value pair (i.e., \"Accept-Encoding: zip/zop;8bit\").")]
        [Argument("-H")]
        public string CustomHeader { get; set; }

        [Category("Request")]
        [DisplayName("Keep Alive")]
        [Description("Enable the HTTP KeepAlive feature, i.e., perform multiple requests within one HTTP session.")]
        [Argument("-k")]
        [DefaultValue(false)]
        public bool KeepAlive { get; set; }

        [Category("Request")]
        [DisplayName("Http Method")]
        [Description("Custom HTTP method for the requests. ")]
        [Argument("-m")]
        [DefaultValue(RequestMethod.GET)]
        public RequestMethod HttpMethod { get; set; } = RequestMethod.GET;

        [Category("Request")]
        [DisplayName("No Exit On Socket Errors")]
        [Description("Don't exit on socket receive errors.")]
        [Argument("-r")]
        [DefaultValue(false)]
        public bool NoExitOnSocketErrors { get; set; } = false;

        [Category("Request")]
        [DisplayName("Content Type")]
        [Description("Content-type header to use for POST/PUT data, eg. application/x-www-form-urlencoded.")]
        [Argument("-T")]
        [DefaultValue("text/plain")]
        public string ContentType { get; set; } = "text/plain";

        [Category("Request")]
        [DisplayName("Proxy Port")]
        [Description("Use a proxy server for the requests.")]
        [Argument("-X")]
        public string ProxyPort { get; set; }




        //public bool ShowPercentageServed { get; set; }
        //public bool ExitOnSocketReceiveErrors { get; set; }

        //public string GnuplotFile { get; set; }
        //public string PostFile { get; set; }
        //public string CsvFile { get; set; }
        //public string PutFile { get; set; }
    }
}
