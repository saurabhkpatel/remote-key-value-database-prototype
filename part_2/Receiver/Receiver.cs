﻿///////////////////////////////////////////////////////////////////////////////
// Receiver.cs - CommService Receiver listens for messages              //
// Ver 2.2                                                             //
// Application: CSE681 - Software Modeling and Analysis, Project #4    //
// Language:    C#, ver 6.0, Visual Studio 2015                        //
// Platform:    Mac Book Pro, Core-i5, Windows 10                      //
// Source :     Jim Fawcett, CST 4-187, Syracuse University            //
// Author:      Saurabh Patel, MSCE Grad Student                       //
//              Syracuse University                                    //
//              (315) 751-3911, skpatel@syr.edu                        //
////////////////////////////////////////////////////////////////////////
/*
 * Receiver:
 * - listens for incoming connection requests
 * - provides sendMessage() for Senders to post messages
 * - provides serviceAction to determine what happens to received messages
 */
/*
 * Additions to C# Console Wizard generated code:
 * - Added reference to System.ServiceModel
 * - Added using System.ServiceModel
 * - Added using System.ServiceModel.Description
 * - Added reference to ICommService
 * - Added reference to CommService
 */
/*
 * Maintenance:
 * ------------
 * Required Files: ICommService, Commservice,Utilities
 *
 * Build Process:  devenv Project4.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 2.2 : 22 Nov 2015
 * - Edited according to Project#4 requirements
 * ver 2.1 : 29 Oct 2015
 * - added comment prologue to shutDown()
 * ver 2.0 : 24 Oct 2015
 * - Provided mechanism to define new Receiver msg processing.
 *   See methods defaultServiceAction, serverProcessMessage, and doService.
 * - added verbose mode to support debugging and learning
 * - to see more detail about what is going on in Sender and Receiver
 *   set Utilities.verbose = true
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;

namespace Project4Starter
{
    using Util = Utilities;

    ///////////////////////////////////////////////////////////////////////
    // Receiver class
    // - provides a method, Message getMessage(), to retrieve messages
    //   sent to its instances
    // - has public string properties for listener address and port
    //
    public class Receiver
    {
        /*
         * Address can be in any of these forms:
         * - localhost
         * - ipaddress, e.g., 192.168.1.100
         * - machine name, e.g., Godzilla
         * But sender must match format used by server.
         */
        int count = 0;
        bool lastMessageFlag = false;
        int messageSize;

        public string address { get; set; }
        /*
         * Port should be greater than 1024.
         * If you choose a port already in use the listener will throw an exception.
         */
        public string port { get; set; }

        CommService svc = null;
        ServiceHost host = null;

        //----< constructor sets listening endpoint >------------------------

        public Receiver(string Port = "8080", string Address = "localhost")
        {
            address = Address;
            port = Port;
        }

        //----< creates listener but does not start it >---------------------

        public ServiceHost CreateListener()
        {
            string url = "http://" + this.address + ":" + this.port + "/CommService";
            BasicHttpBinding binding = new BasicHttpBinding();
            Uri address = new Uri(url);
            Type service = typeof(CommService);
            ServiceHost host = new ServiceHost(service, address);
            host.AddServiceEndpoint(typeof(ICommService), binding, address);
            return host;
        }
        //----< Create CommService and listener and start it >---------------

        public bool StartService()
        {
            if (Util.verbose)
                Console.Write("\n  Receiver starting service");
            try
            {
                host = CreateListener();
                host.Open();
                svc = new CommService();
            }
            catch (Exception ex)
            {
                Console.Write("\n\n --- creation of Receiver listener failed ---\n");
                Console.Write("\n    {0}", ex.Message);
                Console.Write("\n    exiting\n\n");
                return false;
            }
            return true;
        }
        public int getTotalMessageSize()
        {
            return messageSize+1;
        }

        public void setTotalMessageSize(int messageSize)
        {
            //Console.WriteLine("set message " + messageSize);
            this.messageSize = messageSize;
        }

        public Boolean getLastFlag()
        {
            return lastMessageFlag;
        }
        //----< serviceAction defines what happens to received messages >----
        /*
         * - Default service action is to display each received message.
         * - serverProcessMessage(msg) does nothing, but can be overridden
         *   to provide additional server processing.
         */
        public Action defaultServiceAction()
        {
            Action serviceAction = () =>
            {
                if (Util.verboseApp)
                    Console.Write("\n  starting Receiver.defaultServiceAction");
                Message msg = null;
                while (true)
                {
                    msg = getMessage();   // note use of non-service method to deQ messages
                    if (Util.verboseApp)
                    {
                        Console.Write("\n  Received message:");
                        Console.Write("\n  sender is {0}", msg.fromUrl);
                        Console.Write("\n  content is {0}\n", msg.content);
                    }
                    serverProcessMessage(msg);
                    if (msg.content == "closeReceiver")
                        break;
                    count++;
                    //Console.WriteLine("**" + count + " :: "+getTotalMessageSize());
                    if (count == getTotalMessageSize())
                    {
                        lastMessageFlag = true;
                    }
                }
            };
            return serviceAction;
        }
        //----< one way to define Receiver functionality >-------------------
        /*
         * - This function not  used in these demos.
         * - Instead, Receiver functionality is defined by a serviceAction.
         * - Look at Server main for an example.
         */
        public virtual void serverProcessMessage(Message msg)
        {
            // To define work for the server you can derive from Receiver,
            // overriding this method to use database or whatever you need
            // to do.  Then you use the derived Receiver in your server.
            //
            // Alternately you can simply define a serviceAction that does
            // what you need and use that as an argument to method 
            // doservice(Action serviceAction), below.
            //
            // For either of these approaches you should write a set of functions
            // that define processing you need and call from your serviceAction
            // or overridden serverProcessMessage.
        }
        //----< run the service action >-------------------------------------
        /*
         * - Provides a mechanism for applications to define service operations.
         *   Look at test stub for an example of how to define the serviceAction.
         * - Runs asynchronously
         */
        public void doService(Action serviceAction)
        {
            ThreadStart ts = () =>
            {
                if (Util.verbose)
                    Console.Write("\n  doService thread started");
                serviceAction.Invoke();  // usually has while loop that runs until closed
            };
            Thread t = new Thread(ts);
            t.IsBackground = true;
            t.Start();
        }
        //----< runs defaultServiceAction >----------------------------------

        public void doService()
        {
            doService(defaultServiceAction());
        }
        //----< application hosting Receiver calls this method >-------------

        public Message getMessage()
        {
            if (Util.verbose)
                Console.Write("\n  calling CommService.getMessage()");
            Message msg = svc.getMessage();
            if (Util.verbose)
                Console.Write("\n  returned from CommService.getMessage()");
            return msg;
        }
        //----< send closeReceiver message to local Receiver >---------------

        public void shutDown()
        {
            Console.Write("\n  local receiver shutting down");
            Message msg = new Message();
            msg.content = "closeReceiver";
            msg.toUrl = Util.makeUrl(address, port);
            msg.fromUrl = msg.toUrl;
            Util.showMessage(msg);
            svc.sendMessage(msg);
            host.Close();
        }
        //----< quick way to grab ports and addresses from commandline >-----

        public void ProcessCommandLine(string[] args)
        {
            if (args.Length > 0)
            {
                port = args[0];
            }
            if (args.Length > 1)
            {
                address = args[1];
            }
        }
        //----< Test Stub >--------------------------------------------------

#if (TEST_RECEIVER)

        static void Main(string[] args)
        {
            Util.verbose = true;

            Console.Title = "CommService Receiver";
            Console.Write("\n  Starting CommService Receiver");
            Console.Write("\n ===============================\n");

            Receiver rcvr = new Receiver();
            rcvr.ProcessCommandLine(args);

            Console.Write("\n  Receiver url = {0}\n", Util.makeUrl(rcvr.address, rcvr.port));

            // serviceAction defines what the server does with received messages

            if (rcvr.StartService())
            {
                //rcvr.doService();
                rcvr.doService(rcvr.defaultServiceAction());  // equivalent to rcvr.doService()
            }
            Console.Write("\n  press any key to exit: ");
            Console.ReadKey();
            Console.Write("\n\n");
        }
#endif
    }
}