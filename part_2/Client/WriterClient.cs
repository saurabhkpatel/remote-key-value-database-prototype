//////////////////////////////////////////////////////////////////////////
// Writer.cs - CommService writer client sends and receives messages    //
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
 * - Added reference to ICommService, Sender, Receiver, Utilities. TestTimer and MakeMessage

 * Build Process:  devenv Project4.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
                        
 *
 * Note:
 * - This writer client will send all write related operation requests to remote database.
 * - in this incantation the client has Sender and now has Receiver to
 *   retrieve Server echo-back messages.
 * - If you provide command line arguments they should be ordered as:
 *   remotePort, remoteAddress, localPort, localAddress
 */
/*
 * Maintenance History:
 * --------------------
 * ver 2.4 : 22 Nov 2015
 * Edited according to Project#4 requirements
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
        List<MessageTemplate> listOfMsgs;
        List<string> listOfKeys = new List<string>();
        string localUrl { get; set; } = "http://localhost:8081/CommService";
        string remoteUrl { get; set; } = "http://localhost:8080/CommService";
        Random random = new Random();
        HiResTimer timer = new HiResTimer();
        int totalCount = 0;

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

                if (messageTemplate.messageType.Equals("add"))
                {
                    msgdetails = createAddMessage();
                }
                else if (messageTemplate.messageType.Equals("delete"))
                {
                    msgdetails = createDeleteMessage();
                }
                else if (messageTemplate.messageType.Equals("edit"))
                {
                    msgdetails = createEditMessage();
                }
                else if (messageTemplate.messageType.Equals("persist"))
                {
                    msgdetails = createPersistMessage();
                }
                else if (messageTemplate.messageType.Equals("restore"))
                {
                    msgdetails = createRestoreDataRequestMessage();
                }

                Message msg = new Message();
                msg.fromUrl = localUrl;
                msg.toUrl = remoteUrl;
                msg.content = msgdetails.ToString();
                Thread.Sleep(10);
                if (!sndr.sendMessage(msg))
                    return;
            }
        }

        //----< create message for restore data>--------
        private string createRestoreDataRequestMessage()
        {
            XElement messageNode = new XElement("message");
            XAttribute att = new XAttribute("commandType", "restore");
            messageNode.Add(att);
            return messageNode.ToString();
        }

        //----< create message for persist data>--------
        private string createPersistMessage()
        {
            XElement messageNode = new XElement("message");
            XAttribute att = new XAttribute("commandType", "persist");
            messageNode.Add(att);
            return messageNode.ToString();
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
            XElement type = new XElement("clientType", "Writer");
            messageNode.Add(timeNode);
            messageNode.Add(fromNode);
            messageNode.Add(numberOfMessages);
            messageNode.Add(type);
            return messageNode.ToString();
        }
        
        //----< create edit message in xml format >--------
        private string createEditMessage()
        {

            XElement messageNode = new XElement("message");
            XAttribute att = new XAttribute("commandType", "edit");
            messageNode.Add(att);
            string rndKey = listOfKeys[random.Next(listOfKeys.Count)];
            //Console.WriteLine("Edit key : " + rndKey);
            XElement keyTypenode = new XElement("keyType", "string");
            XElement valueTypeNode = new XElement("valueType", "ListOfString");
            XElement keynode = new XElement("key", rndKey.ToString());
            XElement valueNode = new XElement("value");
            XElement name = new XElement("name", "Updated-name : " + rndKey.ToString());
            XElement desc = new XElement("desc", "Updated-desc : " + rndKey.ToString());
            XElement timestamp = new XElement("time", DateTime.Now);
            XElement payload = new XElement("payload");
            int rndPayloadSize = random.Next(2, 10);
            for (int j = 0; j < rndPayloadSize; j++)
            {
                XElement item = new XElement("item", "Updated-Payload" + j + " of " + rndKey);
                payload.Add(item);
            }
            XElement children = new XElement("children");
            int childrenSize = random.Next(3, 5);
            for (int j = 1; j < childrenSize; j++)
            {
                XElement item = new XElement("item", "updated-child" + j.ToString());
                children.Add(item);
            }
            valueNode.Add(name);
            valueNode.Add(desc);
            valueNode.Add(timestamp);
            valueNode.Add(payload);
            valueNode.Add(children);
            messageNode.Add(keyTypenode);
            messageNode.Add(valueTypeNode);
            messageNode.Add(keynode);
            messageNode.Add(valueNode);
            return messageNode.ToString();
        }

        //----< create delete message in xml format >--------
        private string createDeleteMessage()
        {
            XElement messageNode = new XElement("message");
            XAttribute att = new XAttribute("commandType", "delete");
            messageNode.Add(att);

            XElement keyTypenode = new XElement("keyType", "string");
            XElement valueTypeNode = new XElement("valueType", "ListOfString");
            string rndKey = listOfKeys[random.Next(listOfKeys.Count)];
            //Console.WriteLine("delete key : " + rndKey);
            XElement key = new XElement("key", rndKey.ToString());
            messageNode.Add(keyTypenode);
            messageNode.Add(valueTypeNode);
            messageNode.Add(key);
            return messageNode.ToString();
        }

        //----< create add message in xml format >--------
        private string createAddMessage()
        {
            XElement messageNode = new XElement("message");
            XAttribute att = new XAttribute("commandType", "add");
            messageNode.Add(att);

            int rndKey = random.Next(1, 10000);
            XElement keyTypenode = new XElement("keyType", "string");
            XElement valueTypeNode = new XElement("valueType", "ListOfString");

            XElement keynode = new XElement("key", rndKey.ToString());
            listOfKeys.Add(rndKey.ToString());
            XElement valueNode = new XElement("value");

            XElement name = new XElement("name", "name : " + rndKey.ToString());
            XElement desc = new XElement("desc", "desc : " + rndKey.ToString());
            XElement timestamp = new XElement("time", DateTime.Now);
            XElement payload = new XElement("payload");
            int rndPayloadSize = random.Next(2, 10);
            for (int j = 0; j < rndPayloadSize; j++)
            {
                XElement item = new XElement("item", "item" + j + " of " + rndKey);
                payload.Add(item);
            }

            XElement children = new XElement("children");
            int childrenSize = random.Next(3, 5);
            for (int j = 1; j < childrenSize; j++)
            {
                XElement item = new XElement("item", j.ToString());
                children.Add(item);
            }
            valueNode.Add(name);
            valueNode.Add(desc);
            valueNode.Add(timestamp);
            valueNode.Add(payload);
            valueNode.Add(children);

            messageNode.Add(keyTypenode);
            messageNode.Add(valueTypeNode);
            messageNode.Add(keynode);
            messageNode.Add(valueNode);

            return messageNode.ToString();
        }


        //----< retrieve urls from the CommandLine if there are any >--------
        public void processCommandLine(string[] args)
        {
            if (args.Length == 0)
                return;
            localUrl = Util.processCommandLineForLocal(args, localUrl);
            remoteUrl = Util.processCommandLineForRemote(args, remoteUrl);
            Util.verboseApp = Util.processCommandLineForLogging(args);
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
            Console.Write("\n  Starting Writer client");
            Console.Write("\n =============================\n");
            Console.Title = "Writer";
            Client clnt = new Client();
            try
            {
                clnt.processCommandLine(args);
                string localPort = Util.urlPort(clnt.localUrl);
                string localAddr = Util.urlAddress(clnt.localUrl);
                Receiver rcvr = new Receiver(localPort, localAddr);
                Sender sndr = new Sender(clnt.localUrl);  // Sender needs localUrl for start message

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
                clnt.readMessageTemplates("XMLWriter.xml");
                if (clnt.listOfMsgs.Count() > 0)
                {
                    rcvr.setTotalMessageSize(clnt.getTotalNumberOfMessages(clnt.listOfMsgs));
                    clnt.processMessages(clnt.listOfMsgs, sndr);
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
            catch (Exception e)
            {
                Console.Write("Invalid XMl file or some exception occured.");
                Console.Write("Exception : "+e.StackTrace);
                throw;
            }
        }
    }
}
