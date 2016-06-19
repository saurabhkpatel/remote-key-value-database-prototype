//////////////////////////////////////////////////////////////////////////
// ReaderClient.cs - CommService client sends and receives messages    //
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
 * Additions to C# Console Wizard generated code:
 * - Added using System.Threading
 * - Added reference to ICommService, Sender, Receiver, Utilities,
                        MakeMessage, TestTimer
 * - Added all types of read query realted messages which are going to execute by any client on remote server.
 * Note:
 * - in this incantation the client has Sender and now has Receiver to
 *   retrieve Server echo-back messages.
 * - If you provide command line arguments they should be ordered as:
 *   remotePort, remoteAddress, localPort, localAddress
 */
/*
 * Maintenance:
 * ------------
 * Required Files: ICommService, Sender, Receiver, Utilities,
                   MakeMessage, TestTimer
 *
 * Build Process:  devenv Project4.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *  
 * Maintenance History:
 * --------------------
 * ver 2.2 : 22 Nov 2015
 * - added new methods to perform reader client realted actions.
 * - Edited according to Project#4 requirements
 * ver 2.1 : 29 Oct 2015
 * - fixed bug in processCommandLine(...)
 * - added rcvr.shutdown() and sndr.shutDown() 
 * ver 2.0 : 20 Oct 2015
 * - replaced almost all functionality with a Sender instance
 * - added Receiver to retrieve Server echo messages.
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
using System.Threading;

namespace Project4Starter
{
    using System.Xml.Linq;
    using Util = Utilities;

    ///////////////////////////////////////////////////////////////////////
    // Client class sends and receives messages in this version
    // - commandline format: /L http://localhost:8085/CommService 
    //                       /R http://localhost:8080/CommService
    //   Either one or both may be ommitted

    class Client
    {
        string localUrl { get; set; } = "http://localhost:8082/CommService";
        string remoteUrl { get; set; } = "http://localhost:8080/CommService";
        List<MessageTemplate> listOfMsgs;
        Random random = new Random();
        int totalCount = 0;
        HiResTimer timer = new HiResTimer();

        //----< retrieve urls from the CommandLine if there are any >--------
        public void processCommandLine(string[] args)
        {
            if (args.Length == 0)
                return;
            localUrl = Util.processCommandLineForLocal(args, localUrl);
            remoteUrl = Util.processCommandLineForRemote(args, remoteUrl);
            Util.verboseApp = Util.processCommandLineForLogging(args);
            
        }

        //----< read Message templates from xml file >--------
        public List<MessageTemplate> readMessageTemplates(String fileName)
        {
            MessageTemplate msgTemplate = new MessageTemplate();
            listOfMsgs = msgTemplate.GetMessageList(fileName);
            return listOfMsgs;
        }

        //----< process messages and send messages as per message template to server >--------
        private void processMessages(List<MessageTemplate> listOfMsgs, Sender sndr)
        {
            if (listOfMsgs.Count > 0)
            {
                for (int i = 0; i < listOfMsgs.Count; i++)
                {
                    processMessageTemplate(listOfMsgs[i], sndr);
                }
            }
            else
            {
                Console.WriteLine("message template list is empty");
            }
        }

        //----< get total Number of messages which are going to sent by this client. >--------
        private int getTotalNumberOfMessages(List<MessageTemplate> list)
        {
            int messageSize = 0;
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    messageSize = messageSize + list[i].messageSize;
                }
            }
            totalCount = messageSize;
            return messageSize;
        }

        //----< process messages templages and create random messages and send them to server.>--------
        private void processMessageTemplate(MessageTemplate messageTemplate, Sender sndr)
        {
            Console.WriteLine("  Operation Type : " + messageTemplate.messageType + " Size : " + messageTemplate.messageSize);
            for (int i = 0; i < messageTemplate.messageSize; i++)
            {
                string msgdetails = "";

                if (messageTemplate.messageType.Equals("Query1"))
                {
                    msgdetails = createQuery1TypeMessage();
                }
                if (messageTemplate.messageType.Equals("Query2"))
                {
                    msgdetails = createQuery2TypeMessage();
                }
                if (messageTemplate.messageType.Equals("Query3"))
                {
                    msgdetails = createQuery3TypeMessage();
                }
                if (messageTemplate.messageType.Equals("Query4"))
                {
                    msgdetails = createQuery4TypeMessage();
                }
                if (messageTemplate.messageType.Equals("Query5"))
                {
                    msgdetails = createQuery5TypeMessage();
                }

                Message msg = new Message();
                msg.fromUrl = localUrl;
                msg.toUrl = remoteUrl;
                msg.content = msgdetails.ToString();
                //Console.WriteLine("msg : " + msg.content);
                Thread.Sleep(50);
                if (!sndr.sendMessage(msg))
                    return;
            }
        }

        //----< create message for performance >--------
        private string createPerformanceMessage(string time)
        {
            XElement messageNode = new XElement("message");
            XAttribute att = new XAttribute("commandType", "performance");
            messageNode.Add(att);
            XElement timeNode = new XElement("time", time);
            XElement fromNode = new XElement("fromUrl", localUrl);
            XElement numberOfMessages = new XElement("numberMessages", totalCount);
            XElement type = new XElement("clientType", "Reader");
            messageNode.Add(timeNode);
            messageNode.Add(fromNode);
            messageNode.Add(numberOfMessages);
            messageNode.Add(type);
            return messageNode.ToString();
        }
        //----< create xml message for query 5>--------
        // will get keys of data with in start and end date.
        private string createQuery5TypeMessage()
        {
            DateTime toDate = new DateTime();
            DateTime fromDate = new DateTime(2013, 6, 15, 0, 0, 0);
            XElement messageNode = new XElement("message");
            XAttribute att = new XAttribute("commandType", "query5");
            messageNode.Add(att);
            XElement keyTypenode = new XElement("keyType", "string");
            XElement valueTypeNode = new XElement("valueType", "ListOfString");
            XElement startTime = new XElement("startTime", fromDate);
            XElement endTime = new XElement("endTime", toDate);
            messageNode.Add(keyTypenode);
            messageNode.Add(valueTypeNode);
            messageNode.Add(startTime);
            messageNode.Add(endTime);
            return messageNode.ToString();
        }

        //----< create xml message for query 4>--------
        // will get results of keys which meta data contains particular pattern
        private string createQuery4TypeMessage()
        {
            XElement messageNode = new XElement("message");
            XAttribute att = new XAttribute("commandType", "query4");
            messageNode.Add(att);
            XElement keyTypenode = new XElement("keyType", "string");
            XElement valueTypeNode = new XElement("valueType", "ListOfString");
            XElement searchParaMeter = new XElement("searchParaMeter", "preload");
            messageNode.Add(keyTypenode);
            messageNode.Add(valueTypeNode);
            messageNode.Add(searchParaMeter);
            return messageNode.ToString();
        }

        //----< create xml message for query 3>--------
        // will get results of keys which contains keys of input pattern
        private string createQuery3TypeMessage()
        {
            XElement messageNode = new XElement("message");
            XAttribute att = new XAttribute("commandType", "query3");
            messageNode.Add(att);
            XElement keyTypenode = new XElement("keyType", "string");
            XElement valueTypeNode = new XElement("valueType", "ListOfString");
            XElement patternNode = new XElement("pattern", "1");
            messageNode.Add(keyTypenode);
            messageNode.Add(valueTypeNode);
            messageNode.Add(patternNode);
            return messageNode.ToString();
        }

        //----< create xml message for query 2>--------
        // will get children of input key
        private string createQuery2TypeMessage()
        {
            XElement messageNode = new XElement("message");
            XAttribute att = new XAttribute("commandType", "query2");
            messageNode.Add(att);
            XElement keyTypenode = new XElement("keyType", "string");
            XElement valueTypeNode = new XElement("valueType", "ListOfString");
            int rndPayloadSize = random.Next(11, 20);
            XElement keynode = new XElement("key", rndPayloadSize.ToString());
            messageNode.Add(keyTypenode);
            messageNode.Add(valueTypeNode);
            messageNode.Add(keynode);
            return messageNode.ToString();
        }

        //----< create xml message for query 2>--------
        // will get value of input key
        private string createQuery1TypeMessage()
        {
            XElement messageNode = new XElement("message");
            XAttribute att = new XAttribute("commandType", "query1");
            messageNode.Add(att);
            XElement keyTypenode = new XElement("keyType", "string");
            XElement valueTypeNode = new XElement("valueType", "ListOfString");
            int rndPayloadSize = random.Next(11, 20);
            XElement keynode = new XElement("key", rndPayloadSize.ToString());
            messageNode.Add(keyTypenode);
            messageNode.Add(valueTypeNode);
            messageNode.Add(keynode);
            return messageNode.ToString();
        }

        //----< send message for performance to server >--------
        public void sendPerformanceMessage(ulong latency, Sender sndr)
        {
            Message msg = new Message();
            msg.fromUrl = localUrl;
            msg.toUrl = remoteUrl;
            msg.content = createPerformanceMessage(latency.ToString());
            if (!sndr.sendMessage(msg))
                return;
        }
        static void Main(string[] args)
        {
            Console.Write("\n  starting CommService Reader Client");
            Console.Write("\n =============================\n");
            Console.Title = "Reader Client";
            Client clnt = new Client();
            clnt.processCommandLine(args);

            string localPort = Util.urlPort(clnt.localUrl);
            string localAddr = Util.urlAddress(clnt.localUrl);
            Receiver rcvr = new Receiver(localPort, localAddr);
            // Sender needs localUrl for start message
            Sender sndr = new Sender(clnt.localUrl);

            if (rcvr.StartService())
            {
                rcvr.doService(rcvr.defaultServiceAction());
            }

            Message msg = new Message();
            msg.fromUrl = clnt.localUrl;
            msg.toUrl = clnt.remoteUrl;

            Console.Write("\n  sender's url is {0}", msg.fromUrl);
            Console.Write("\n  attempting to connect to {0}\n", msg.toUrl);

            if (!sndr.Connect(msg.toUrl))
            {
                Console.Write("\n  could not connect in {0} attempts", sndr.MaxConnectAttempts);
                sndr.shutdown();
                rcvr.shutDown();
                return;
            }
            clnt.timer.Start();
            clnt.readMessageTemplates("XMLReader.xml");
            if(clnt.listOfMsgs.Count() > 0)
            {
                rcvr.setTotalMessageSize(clnt.getTotalNumberOfMessages(clnt.listOfMsgs));
                clnt.processMessages(clnt.listOfMsgs,sndr);
                while (true)
                {
                    if (rcvr.getLastFlag())
                    {
                        clnt.timer.Stop();
                        break;
                    }
                }
            }
            ulong latency = clnt.timer.ElapsedMicroseconds;
            clnt.sendPerformanceMessage(latency, sndr);
            msg.content = "done";
            sndr.sendMessage(msg);
            // Wait for user to press a key to quit.
            // Ensures that client has gotten all server replies.
            Util.waitForUser();
            // shut down this client's Receiver and Sender by sending close messages
            rcvr.shutDown();
            sndr.shutdown();
            Console.Write("\n\n");
        }
    }
}

