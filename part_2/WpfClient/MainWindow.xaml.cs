//////////////////////////////////////////////////////////////////////////
// MainWindows.cs - CommService GUI Client                              //
// Ver 2.2                                                             //
// Application: CSE681 - Software Modeling and Analysis, Project #4    //
// Language:    C#, ver 6.0, Visual Studio 2015                        //
// Platform:    Mac Book Pro, Core-i5, Windows 10                      //
// Source :     Jim Fawcett, CST 4-187, Syracuse University            //
// Author:      Saurabh Patel, MSCE Grad Student                       //
//              Syracuse University                                    //
//              (315) 751-3911, skpatel@syr.edu                        //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# WPF Wizard generated code:
 * - Added reference to ICommService, Sender, Receiver, MakeMessage, Utilities
 * - Added using Project4Starter
 *
 *
 * Build Process:  devenv Project4.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
                        
 *
 * Note:
 * - This client receives and sends messages.
 */
/*
 * - Added some methods which parse performance messages from server and shows performance on performance tab.
 * - Add message decoding and NoSqlDb calls in performanceServiceAction.
 * - Provide requirements testing in requirementsServiceAction, perhaps
 *   used in a console client application separate from Performance 
 *   Testing GUI.
 */
/*
 * Maintenance History:
 * --------------------
 * ver 2.0 : 29 Oct 2015
 * - changed Xaml to achieve more fluid design
 *   by embedding controls in grid columns as well as rows
 * - added derived sender, overridding notification methods
 *   to put notifications in status textbox
 * - added use of MessageMaker in send_click
 * ver 1.0 : 25 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Project4Starter;
using System.Xml.Linq;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        static bool firstConnect = true;
        static Receiver rcvr = null;
        static wpfSender sndr = null;
        string localAddress = "localhost";
        string localPort = "8089";
        string remoteAddress = "localhost";
        string remotePort = "8080";

        /////////////////////////////////////////////////////////////////////
        // nested class wpfSender used to override Sender message handling
        // - routes messages to status textbox
        public class wpfSender : Sender
        {
            TextBox lStat_ = null;  // reference to UIs local status textbox
            System.Windows.Threading.Dispatcher dispatcher_ = null;

            public wpfSender(TextBox lStat, System.Windows.Threading.Dispatcher dispatcher)
            {
                dispatcher_ = dispatcher;  // use to send results action to main UI thread
                lStat_ = lStat;
            }
            public override void sendMsgNotify(string msg)
            {
                Action act = () => { lStat_.Text = msg; };
                dispatcher_.Invoke(act);

            }
            public override void sendExceptionNotify(Exception ex, string msg = "")
            {
                Action act = () => { lStat_.Text = ex.Message; };
                dispatcher_.Invoke(act);
            }
            public override void sendAttemptNotify(int attemptNumber)
            {
                Action act = null;
                act = () => { lStat_.Text = String.Format("attempt to send #{0}", attemptNumber); };
                dispatcher_.Invoke(act);
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            lAddr.Text = localAddress;
            lPort.Text = localPort;
            rAddr.Text = remoteAddress;
            rPort.Text = remotePort;
            Title = "Prototype WPF Client";
            send.IsEnabled = false;
            Loaded += wpfWindow_Loaded;

        }

        private void wpfWindow_Loaded(object sender, RoutedEventArgs e)
        {
            localPort = lPort.Text;
            localAddress = lAddr.Text;
            remoteAddress = rAddr.Text;
            remotePort = rPort.Text;

            if (firstConnect)
            {
                firstConnect = false;
                if (rcvr != null)
                    rcvr.shutDown();
                setupChannel();
            }
            rStat.Text = "connect setup";
            send.IsEnabled = true;
            connect.IsEnabled = false;
            lPort.IsEnabled = false;
            lAddr.IsEnabled = false;
        }
        //----< trim off leading and trailing white space >------------------

        string trim(string msg)
        {
            StringBuilder sb = new StringBuilder(msg);
            for (int i = 0; i < sb.Length; ++i)
                if (sb[i] == '\n')
                    sb.Remove(i, 1);
            return sb.ToString().Trim();
        }

        //----< get throughput from string message.>------------------
        private string getThroughput(string content)
        {
            XDocument xdoc = XDocument.Parse(content);
            string time = "";
            var elem = from c in xdoc.Descendants("message") select c;
            foreach (var item in elem.Elements())
            {
                if (item.Name == "throughputTime")
                    time = item.Value.ToString();
            }

            return "Server operations average throughPut time = " + time.ToString() + " microseconds";
        }

        //----< get message parameters and build performance message>------------------
        private string getMessageParameters(string content)
        {
            XDocument xdoc = XDocument.Parse(content);
            StringBuilder sb = new StringBuilder();
            string time = "";
            string number = "";
            string fromUrl = "";
            string type = "";

            var elem = from c in xdoc.Descendants("message") select c;
            foreach (var item in elem.Elements())
            {
                if (item.Name == "time")
                    time = item.Value.ToString();
                if (item.Name == "fromUrl")
                    fromUrl = item.Value.ToString();
                if (item.Name == "numberMessages")
                    number = item.Value.ToString();
                if (item.Name == "clientType")
                    type = item.Value.ToString();
            }
            ulong timeint = Convert.ToUInt64(time);
            ulong numberInt = Convert.ToUInt64(number);
            ulong avg = 0;
            if (numberInt > 0)
                avg = timeint / numberInt;

            //string temp = "   " + fromUrl + "                              |    " + type + "     |     " + number + "     |     " + time + "     |    " + avg + "    ";

            sb.Append(String.Format("{0,-20} || ", fromUrl));
            sb.Append(String.Format("{0,-30} || ", type));
            sb.Append(String.Format("{0,-30} ||", number));
            sb.Append(String.Format("  {0,-40} ||", time));
            sb.Append(String.Format("  {0,-40} ", avg));

            return sb.ToString();
        }

        //----< indirectly used by child receive thread to post results >----
        public void postRcvMsg(string content)
        {
            TextBlock i = new TextBlock();
            i.Text = trim(content);
            rcvmsgs.Items.Insert(0, i);
            if (content.Contains("message commandType"))
            {
                TextBlock item = new TextBlock();
                string result = getMessageParameters(content);
                item.Text = trim(result);
                item.FontSize = 12;
                lst_perfromance.Items.Insert(0, item);
                
                TextBlock item1 = new TextBlock();
                string result1 = getThroughput(content);
                item1.Text = trim(result1);
                item1.FontSize = 12;
                lst_throughput.Items.Insert(0, item1);
            }
        }
        //----< used by main thread >----------------------------------------


        public void postSndMsg(string content)
        {
            TextBlock item = new TextBlock();
            item.Text = trim(content);
            item.FontSize = 16;
            sndmsgs.Items.Insert(0, item);
        }
        //----< get Receiver and Sender running >----------------------------

        void setupChannel()
        {
            rcvr = new Receiver(localPort, localAddress);
            Action serviceAction = () =>
            {
                try
                {
                    Message rmsg = null;
                    while (true)
                    {
                        rmsg = rcvr.getMessage();
                        Action act = () => { postRcvMsg(rmsg.content); };
                        Dispatcher.Invoke(act, System.Windows.Threading.DispatcherPriority.Background);
                    }
                }
                catch (Exception ex)
                {
                    Action act = () => { lStat.Text = ex.Message; };
                    Dispatcher.Invoke(act);
                }
            };
            if (rcvr.StartService())
            {
                rcvr.doService(serviceAction);
            }

            sndr = new wpfSender(lStat, this.Dispatcher);
        }
        //----< set up channel after entering ports and addresses >----------

        private void start_Click(object sender, RoutedEventArgs e)
        {
            localPort = lPort.Text;
            localAddress = lAddr.Text;
            remoteAddress = rAddr.Text;
            remotePort = rPort.Text;

            if (firstConnect)
            {
                firstConnect = false;
                if (rcvr != null)
                    rcvr.shutDown();
                setupChannel();
            }
            rStat.Text = "connect setup";
            send.IsEnabled = true;
            connect.IsEnabled = false;
            lPort.IsEnabled = false;
            lAddr.IsEnabled = false;
        }
        //----< send a demonstraton message >--------------------------------

        private void send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region
                /////////////////////////////////////////////////////
                // This commented code was put here to allow
                // user to change local port and address after
                // the channel was started.  
                //
                // It does what is intended, but would throw 
                // if the new port is assigned a slot that
                // is in use or has been used since the
                // TCP tables were last updated.
                //
                // if (!localPort.Equals(lPort.Text))
                // {
                //   localAddress = rcvr.address = lAddr.Text;
                //   localPort = rcvr.port = lPort.Text;
                //   rcvr.shutDown();
                //   setupChannel();
                // }
                #endregion
                if (!remoteAddress.Equals(rAddr.Text) || !remotePort.Equals(rPort.Text))
                {
                    remoteAddress = rAddr.Text;
                    remotePort = rPort.Text;
                }
                // - Make a demo message to send
                // - You will need to change MessageMaker.makeMessage
                //   to make messages appropriate for your application design
                // - You might include a message maker tab on the UI
                //   to do this.

                MessageMaker maker = new MessageMaker();
                Message msg = maker.makeMessage(
                  Utilities.makeUrl(lAddr.Text, lPort.Text),
                  Utilities.makeUrl(rAddr.Text, rPort.Text)
                );
                lStat.Text = "sending to" + msg.toUrl;
                sndr.localUrl = msg.fromUrl;
                sndr.remoteUrl = msg.toUrl;
                lStat.Text = "attempting to connect";
                if (sndr.sendMessage(msg))
                    lStat.Text = "connected";
                else
                    lStat.Text = "connect failed";
                postSndMsg(msg.content);
            }
            catch (Exception ex)
            {
                lStat.Text = ex.Message;
            }
        }
    }
}
