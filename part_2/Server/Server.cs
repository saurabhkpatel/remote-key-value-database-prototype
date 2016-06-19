//////////////////////////////////////////////////////////////////////////
// Server.cs - CommService server sends and receives messages          //
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
 * Added new code where server is receving messages from all clients.
 * This class is responsible to make communication between reader/writer/WCF client and server
 * We are measuring performance also in this package. Overall throughput of server operations and client round trip time.
 *
 * Note:
 * - This server now receives and then sends back received messages.
 */
/*
 * Plans:
 * - Add message decoding and NoSqlDb calls in performanceServiceAction.
 * - Provide requirements testing in requirementsServiceAction, perhaps
 *   used in a console client application separate from Performance 
 *   Testing GUI.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBElement.cs,DBEngine.cs, DbExtensions.cs,
                   Display.cs, PersistEngine.cs, QueryProcessEngine.cs
                    ICommService, Sender, Receiver, Utilities,
                   UtilitiesExtensions.cs, TestTimer
 *
 * Build Process:  devenv Project4.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *  
 * Maintenance History:
 * --------------------
 * ver 2.4 : 22 Nov 2015
 * Edited according to Project#4 requirements
 * ver 2.3 : 29 Oct 2015
 * - added handling of special messages: 
 *   "connection start message", "done", "closeServer"
 * ver 2.2 : 25 Oct 2015
 * - minor changes to display
 * ver 2.1 : 24 Oct 2015
 * - added Sender so Server can echo back messages it receives
 * - added verbose mode to support debugging and learning
 * - to see more detail about what is going on in Sender and Receiver
 *   set Utilities.verbose = true
 * ver 2.0 : 20 Oct 2015
 * - Defined Receiver and used that to replace almost all of the
 *   original Server's functionality.
 * ver 1.0 : 18 Oct 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project4Starter
{
    using System.Xml.Linq;
    using Util = Utilities;
    class Server
    {
        DBEngine<string, DBElement<string, List<string>>> db = new DBEngine<string, DBElement<string, List<string>>>();
        List<int> writerClientTime = new List<int>();
        List<int> readerClientTime = new List<int>();
        static ulong numMessages = 0;
        static ulong avgTime = 0;
        static ulong totalTime = 0;
        string address { get; set; } = "localhost";
        string port { get; set; } = "8080";


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

        //----< identify operation type when xml message received from client >-----
        // Based on operation type take action and process message.
        public static string identifyOperation(XDocument xdoc)
        {
            var elem = xdoc.Root;
            return elem.Attribute("commandType").Value;
        }

        //----< get key value type from xml message. >-----
        public static int getKeyValueType(XDocument xdoc)
        {

            var elem = from c in xdoc.Descendants("message") select c;
            foreach (var item in elem.Elements())
            {
                //Console.WriteLine(item.Name);
                if (item.Name == "keyType")
                {
                    if (item.Value.Equals("string"))
                    {
                        return 1;
                    }
                    if (item.Value.Equals("Int"))
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }

        //----< get key from xml message. >-----
        public string getKey(XDocument xdoc)
        {
            string key = "";
            var elem = from c in xdoc.Descendants("message") select c;
            foreach (var item in elem.Elements())
            {
                if (item.Name == "key")
                    key = item.Value.ToString();
            }
            return key;
        }

        //----< create DB element for add record operation. >-----
        public static string createAddDBElement(XDocument xdoc, out DBElement<string, List<string>> element)
        {
            element = new DBElement<string, List<string>>();
            string key = "";
            var elem = from c in xdoc.Descendants("message") select c;
            foreach (var item in elem.Elements())
            {
                if (item.Name == "key")
                    key = item.Value.ToString();
                else if (item.Name == "value")
                {
                    foreach (var valueItem in item.Elements())
                    {
                        if (valueItem.Name == "name")
                            element.name = valueItem.Value;
                        else if (valueItem.Name == "desc")
                            element.descr = valueItem.Value;
                        else if (valueItem.Name == "time")
                            element.timeStamp = Convert.ToDateTime(valueItem.Value);
                        else if (valueItem.Name == "payload")
                        {
                            element.payload = new List<string>();
                            foreach (var payloadItem in valueItem.Elements())
                            {
                                if (payloadItem.Name == "item")
                                    element.payload.Add(payloadItem.Value);
                            }
                        }
                        else if (valueItem.Name == "children")
                        {
                            element.children = new List<string>();
                            foreach (var childs in valueItem.Elements())
                            {
                                if (childs.Name == "item")
                                    element.children.Add(childs.Value);
                            }
                        }
                    }
                }
            }
            return key;
        }

        //----< process delete message request >-----
        public string processDeleteMessage(XDocument xdoc)
        {
            string content = "";
            int type = getKeyValueType(xdoc);
            if (type == 1)
            {
                string keyToDeleted = getKey(xdoc);
                if (db.delete(keyToDeleted))
                {
                    content = keyToDeleted + " record is deleted.";
                }
                else
                {
                    content = keyToDeleted + " record is  not deleted.";
                }
            }
            return content;
        }
        //----< process add message request >-----
        public string processAddMessage(XDocument xdoc)
        {
            string content = "";
            int type = getKeyValueType(xdoc);
            if (type == 1)
            {
                DBElement<string, List<string>> element = new DBElement<string, List<string>>();
                string key = createAddDBElement(xdoc, out element);
                if (db.insert(key, element))
                {
                    content = key + " record is inserted Successfully.";
                }
                else
                {
                    content = key + " record is not inserted.";
                }
            }
            return content;
        }
        //----< process edit message request >-----
        public string processEditMessage(XDocument xdoc)
        {
            string content = "";
            IEnumerable<string> keys = db.Keys();
            List<string> keyList = keys.ToList();

            string keyToEdit = getKey(xdoc);

            if (!keyList.Contains(keyToEdit))
            {
                Console.WriteLine("Key {0} is not present in the DB", keyToEdit);
            }
            else
            {
                int type = getKeyValueType(xdoc);
                if (type == 1)
                {
                    DBElement<string, List<string>> element = new DBElement<string, List<string>>();
                    string key = createAddDBElement(xdoc, out element);
                    if (db.edit(key, element))
                    {
                        content = key + " record is edited Successfully.";
                    }
                    else
                    {
                        content = key + " record is not edited.";
                    }
                }
            }
            return content;
        }
        //----< get End date from xml message >-----
        private DateTime getEndDate(XDocument xdoc)
        {
            DateTime toDate = new DateTime();
            var elem = from c in xdoc.Descendants("message") select c;
            foreach (var item in elem.Elements())
            {
                if (item.Name == "endTime")
                    toDate = Convert.ToDateTime(item.Value);
            }
            return toDate;
        }
        //----< get Start date from xml message >-----
        private DateTime getStartDate(XDocument xdoc)
        {
            DateTime fromDate = new DateTime();
            var elem = from c in xdoc.Descendants("message") select c;
            foreach (var item in elem.Elements())
            {
                if (item.Name == "startTime")
                    fromDate = Convert.ToDateTime(item.Value);
            }
            return fromDate;
        }
        //----< process query 5 message request. >-----
        public string processQuery5Message(XDocument xdoc)
        {
            string content = "";
            DateTime startDate = getStartDate(xdoc);
            DateTime endData = getEndDate(xdoc);
            int type = getKeyValueType(xdoc);
            if (type == 1)
            {
                List<string> results;
                QueryProcessEngine<string, List<string>> queryEngine = new QueryProcessEngine<string, List<string>>(db);
                queryEngine.processTimeIntervalQuery(queryEngine.defineTimeStampQuery(startDate, endData), out results);
                if (results.Count() > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string str in results)
                    {
                        sb.Append(str);
                        sb.Append("\n");
                        content = "\n  Result of Query5 : " + "Keys within given time range are :\n" + sb;
                    }
                }
                else
                {
                    content = "\n  Result of Query5 : " + "Keys are not found in given time range are :\n";
                }
            }
            return content;
        }

        //----< get search keywords from query message request. >-----
        public string getSeachKeyWords(XDocument xdoc)
        {
            string pattern = "";
            var elem = from c in xdoc.Descendants("message") select c;
            foreach (var item in elem.Elements())
            {
                if (item.Name == "searchParaMeter")
                    pattern = item.Value.ToString();
            }
            return pattern;
        }

        //----< process query4 message and parse xdocument and take action.>-----
        public string processQuery4Message(XDocument xdoc)
        {
            string content = "";
            string keywords = getSeachKeyWords(xdoc);
            int type = getKeyValueType(xdoc);
            if (type == 1)
            {
                List<string> results;
                QueryProcessEngine<string, List<string>> queryEngine = new QueryProcessEngine<string, List<string>>(db);
                queryEngine.processPatternMatchInMetaDataQuery(queryEngine.defineQueryValuePatternSearch(keywords), out results);
                if (results.Count() > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string str in results)
                    {
                        sb.Append(str);
                        sb.Append("\n");
                        content = "\n  Result of Query4 : " + "Keys with pattern like " + keywords + " in the metadata are :\n" + sb;
                    }
                }
                else
                {
                    content = "\n  Keys with date keywords like  " + keywords + " in the metadata not found";
                }
            }
            return content;
        }

        //----< getpattern form xdocument.>-----
        public string getPattern(XDocument xdoc)
        {
            string pattern = "";
            var elem = from c in xdoc.Descendants("message") select c;
            foreach (var item in elem.Elements())
            {
                if (item.Name == "pattern")
                    pattern = item.Value.ToString();
            }
            return pattern;
        }

        //----< process query3 message and parse xdocument and take action.>-----
        public string processQuery3Message(XDocument xdoc)
        {
            string content = "";
            string pattern = getPattern(xdoc);
            int type = getKeyValueType(xdoc);
            if (type == 1)
            {
                QueryProcessEngine<string, List<string>> queryEngine = new QueryProcessEngine<string, List<string>>(db);
                List<string> results;
                if (queryEngine.processPatternMatchInKeysQuery(queryEngine.defineQueryKeyPatternSearch(pattern), out results))
                {
                    if (results.Count() > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (string str in results)
                        {
                            sb.Append(str);
                            sb.Append("\n");
                        }
                        content = "\n  Result of Query3 : " + "Keys with pattern like " + pattern + " are :\n" + sb;
                    }
                    else
                    {
                        content = "\n  Keys with pattern like  " + pattern + " not found";
                    }
                }
                else
                {
                    content = "\n  Keys with pattern like  " + pattern + " not found";
                }
            }
            return content;
        }

        //----< process query2 message and parse xdocument and take action.>-----
        public string processQuery2Message(XDocument xdoc)
        {
            string content = "";
            IEnumerable<string> keys = db.Keys();
            List<string> keyList = keys.ToList();
            string keyToSearch = keyList.First();

            if (!keyList.Contains(keyToSearch))
            {
                Console.WriteLine("Key {0} is not present in the DB", keyToSearch);
                content = "Key " + keyToSearch + " is not present in the DB";
            }
            else
            {
                int type = getKeyValueType(xdoc);
                if (type == 1)
                {
                    List<string> childrens;
                    QueryProcessEngine<string, List<string>> queryEngine = new QueryProcessEngine<string, List<string>>(db);
                    if (queryEngine.processChildrenQuery(keyToSearch, out childrens))
                    {
                        if (childrens.Count() > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (string str in childrens)
                            {
                                sb.Append(str);
                                sb.Append("\n");
                            }
                            content = "\n  Result of Query : " + "Children of " + keyToSearch + " is :\n" + sb;
                        }
                        else
                        {
                            content = "\n  Children of " + keyToSearch + " not found";
                        }
                    }
                    else
                    {
                        content = "\n  Children of " + keyToSearch + " not found";
                    }
                }
            }
            return content;
        }

        //----< process query1 message and parse xdocument and take action.>-----
        public string processQuery1Message(XDocument xdoc)
        {
            string content = "";
            IEnumerable<string> keys = db.Keys();
            List<string> keyList = keys.ToList();
            //string keyToSearch = getKey(xdoc);
            string keyToSearch = keyList.First();
            if (!keyList.Contains(keyToSearch))
            {
                content = "  Key " + keyToSearch + " is not present in the DB";
            }
            else
            {
                int type = getKeyValueType(xdoc);
                if (type == 1)
                {
                    DBElement<string, List<string>> element;
                    QueryProcessEngine<string, List<string>> queryEngine = new QueryProcessEngine<string, List<string>>(db);
                    queryEngine.processValueQuery(keyToSearch, out element);
                    if (element != null)
                    {
                        content = "\n  Result of Query1 : " + "Value of " + keyToSearch + " is :\n" + element.showMetaData();
                    }
                }
                else
                {
                    content = "  Key " + keyToSearch + "'s value not found from database.";
                }
            }
            return content;
        }

        //----< persist data to file when request came from client.>-----
        private void persistDatatToFile()
        {
            PersistData<string, List<string>> persistEngine = new PersistData<string, List<string>>();
            persistEngine.restoreDataToXML("PersistData.xml", db);
        }

        //----< preload data in server.>-----
        private void preLoadData()
        {
            int count = 20;
            string child1 = "preload item1";
            string child2 = "preload item2";
            string child3 = "preload item3";
            for (int key = 11; key < count; key++)
            {
                DBElement<string, List<string>> elem = new DBElement<string, List<string>>();
                createDataElement(elem, new List<string> { child1 + key, child2 + key, child3 + key }, new List<string> { "SMA" + 1, "681" + 1 }, "name" + key, "description" + 1);
                db.insert(key.ToString(), elem);
            }
        }

        //----< create data element in preload data >-----
        private void createDataElement(DBElement<string, List<string>> elem, List<string> childrens, List<string> payload, string name = "unnamed preload data", string descr = "no description")
        {
            elem.name = name + "preload";
            elem.descr = descr;
            elem.timeStamp = new DateTime(2014, 6, 15, 0, 0, 0);
            elem.children = childrens;
            elem.payload = payload;
        }

        //----< get average time >-----
        private ulong getAvgTime(ulong time, ulong commands)
        {
            if (commands != 0)
                return time / commands;
            return 0;
        }

        //----< create performance message for WPF >-----
        private string createPerformanceMessageForWPF(XDocument xdoc, string throughput)
        {
            XElement root = xdoc.Root;
            XElement throughputTimeNode = new XElement("throughputTime", throughput);
            root.Add(throughputTimeNode);
            return xdoc.ToString();
        }
        //----< read latency time. >-----
        public string readLatencyTime(XDocument xdoc)
        {
            string time = "";
            var elem = from c in xdoc.Descendants("message") select c;
            foreach (var item in elem.Elements())
            {
                if (item.Name == "time")
                    time = item.Value.ToString();
            }
            return time;
        }

        public string processDataRestoreMessageRequest()
        {
            string content = "";
            try
            {
                PersistData<string, List<string>> persistEngine = new PersistData<string, List<string>>();
                DBEngine<string, DBElement<string, List<string>>> restoreData = new DBEngine<string, DBElement<string, List<string>>>();
                persistEngine.restoreData(restoreData, "PersistData.xml");

                if (restoreData.Keys().Count() > 0)
                {
                    content = "\nDatabase restored successfully from PersistData.xml";
                }
                else
                {
                    content = "\nDatabase restore failed";
                }
            }
            catch (Exception e)
            {
                Console.Write(" Exception : " + e.StackTrace);
                content = "\nDatabase restore failed";
            }
            return content;
        }

        //----< Main method. >-----
        static void Main(string[] args)
        {
            Util.verbose = false;
            Util.verboseApp = true;
            Server srvr = new Server();
            srvr.ProcessCommandLine(args);
            srvr.preLoadData();

            Console.Title = "Server";
            Console.Write(String.Format("\n  Starting CommService server listening on port {0}", srvr.port));
            Console.Write("\n ====================================================\n");

            Sender sndr = new Sender(Util.makeUrl(srvr.address, srvr.port));
            //Sender sndr = new Sender();
            Receiver rcvr = new Receiver(srvr.port, srvr.address);

            // - serviceAction defines what the server does with received messages
            // - This serviceAction just announces incoming messages and echos them
            //   back to the sender.  
            // - Note that demonstrates sender routing works if you run more than
            //   one client.

            Action serviceAction = () =>
            {
                Message msg = null;
                HiResTimer hrt = new HiResTimer();
                while (true)
                {
                    msg = rcvr.getMessage();   // note use of non-service method to deQ messages
                    if (Util.verbose)
                    {
                        Console.Write("\n  Received message:");
                        Console.Write("\n  sender is {0}", msg.fromUrl);
                        Console.Write("\n  content is {0}\n", msg.content);
                    }
                    if (msg.content == "connection start message")
                    {
                        continue; // don't send back start message
                    }
                    if (msg.content == "done")
                    {
                        continue;
                    }
                    if (msg.content == "closeServer")
                    {
                        Console.Write("received closeServer");
                        break;
                    }
                    try
                    {
                        XDocument xdoc = XDocument.Parse(msg.content);

                        string operationCalled = identifyOperation(xdoc);
                        // swap urls for outgoing message
                        if (operationCalled == "add")
                        {
                            if (Util.verboseApp)
                                Console.WriteLine("add Operation Called");
                            hrt.Start();
                            msg.content = srvr.processAddMessage(xdoc);
                            hrt.Stop();
                            Server.numMessages++;
                            Server.totalTime += hrt.ElapsedMicroseconds;
                            Server.avgTime = srvr.getAvgTime(Server.totalTime, Server.numMessages);
                            Util.swapUrls(ref msg);
                            sndr.sendMessage(msg);
                        }
                        else if (operationCalled == "delete")
                        {
                            if (Util.verboseApp)
                                Console.WriteLine("delete Operation Called");
                            hrt.Start();
                            msg.content = srvr.processDeleteMessage(xdoc);
                            hrt.Stop();
                            Server.numMessages++;
                            Server.totalTime += hrt.ElapsedMicroseconds;
                            Server.avgTime = srvr.getAvgTime(Server.totalTime, Server.numMessages);
                            Util.swapUrls(ref msg);
                            sndr.sendMessage(msg);
                        }

                        else if (operationCalled == "edit")
                        {
                            if (Util.verboseApp)
                                Console.WriteLine("Edit Operation Called");
                            hrt.Start();
                            msg.content = srvr.processEditMessage(xdoc);
                            hrt.Stop();
                            Server.numMessages++;
                            Server.totalTime += hrt.ElapsedMicroseconds;
                            Server.avgTime = srvr.getAvgTime(Server.totalTime, Server.numMessages);
                            Util.swapUrls(ref msg);
                            sndr.sendMessage(msg);
                        }

                        else if (operationCalled == "query1")
                        {
                            if (Util.verboseApp)
                                Console.WriteLine("query1 Operation Called");
                            hrt.Start();
                            msg.content = srvr.processQuery1Message(xdoc);
                            hrt.Stop();
                            Server.numMessages++;
                            Server.totalTime += hrt.ElapsedMicroseconds;
                            Server.avgTime = srvr.getAvgTime(Server.totalTime, Server.numMessages);
                            Util.swapUrls(ref msg);
                            sndr.sendMessage(msg);
                        }
                        else if (operationCalled == "query2")
                        {
                            if (Util.verboseApp)
                                Console.WriteLine("query2 Operation Called");
                            hrt.Start();
                            msg.content = srvr.processQuery2Message(xdoc);
                            hrt.Stop();
                            Server.numMessages++;
                            Server.totalTime += hrt.ElapsedMicroseconds;
                            Server.avgTime = srvr.getAvgTime(Server.totalTime, Server.numMessages);
                            Util.swapUrls(ref msg);
                            sndr.sendMessage(msg);
                        }
                        else if (operationCalled == "query3")
                        {
                            if (Util.verboseApp)
                                Console.WriteLine("query3 Operation Called");
                            hrt.Start();
                            msg.content = srvr.processQuery3Message(xdoc);
                            hrt.Stop();
                            Server.numMessages++;
                            Server.totalTime += hrt.ElapsedMicroseconds;
                            Server.avgTime = srvr.getAvgTime(Server.totalTime, Server.numMessages);
                            Util.swapUrls(ref msg);
                            sndr.sendMessage(msg);
                        }
                        else if (operationCalled == "query4")
                        {
                            if (Util.verboseApp)
                                Console.WriteLine("query4 Operation Called");
                            hrt.Start();
                            msg.content = srvr.processQuery4Message(xdoc);
                            hrt.Stop();
                            Server.numMessages++;
                            Server.totalTime += hrt.ElapsedMicroseconds;
                            Server.avgTime = srvr.getAvgTime(Server.totalTime, Server.numMessages);
                            Util.swapUrls(ref msg);
                            sndr.sendMessage(msg);
                        }
                        else if (operationCalled == "query5")
                        {
                            if (Util.verboseApp)
                                Console.WriteLine("query5 Operation Called");
                            hrt.Start();
                            msg.content = srvr.processQuery5Message(xdoc);
                            hrt.Stop();
                            Server.numMessages++;
                            Server.totalTime += hrt.ElapsedMicroseconds;
                            Server.avgTime = srvr.getAvgTime(Server.totalTime, Server.numMessages);
                            Util.swapUrls(ref msg);
                            sndr.sendMessage(msg);
                        }
                        else if (operationCalled == "persist")
                        {
                            if (Util.verboseApp)
                                Console.WriteLine("persist Operation Called");
                            hrt.Start();
                            srvr.persistDatatToFile();
                            hrt.Stop();
                            Server.numMessages++;
                            Server.totalTime += hrt.ElapsedMicroseconds;
                            Server.avgTime = srvr.getAvgTime(Server.totalTime, Server.numMessages);
                            Util.swapUrls(ref msg);
                            msg.content = "Persist operation performed.";
                            sndr.sendMessage(msg);
                        }
                        else if (operationCalled == "restore")
                        {
                            if (Util.verboseApp)
                                Console.WriteLine("Restore Operation Called");
                            hrt.Start();
                            msg.content = srvr.processDataRestoreMessageRequest();
                            hrt.Stop();
                            Server.numMessages++;
                            Server.totalTime += hrt.ElapsedMicroseconds;
                            Server.avgTime = srvr.getAvgTime(Server.totalTime, Server.numMessages);
                            Util.swapUrls(ref msg);
                            sndr.sendMessage(msg);
                            continue;
                        }
                        else if (operationCalled == "performance")
                        {
                            if (Util.verboseApp)
                                Console.WriteLine("performance Operation Called");
                            Message msgToWpf1 = new Message();
                            msgToWpf1.fromUrl = Util.makeUrl(srvr.address, srvr.port);
                            msgToWpf1.toUrl = "http://localhost:8089/CommService";
                            msgToWpf1.content = srvr.createPerformanceMessageForWPF(xdoc, avgTime.ToString());
                            sndr.sendMessage(msgToWpf1);
                            continue;
                        }
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Catch " + e.StackTrace + "\n");
                    }


#if (TEST_WPFCLIENT)
                    /////////////////////////////////////////////////
                    // The statements below support testing the
                    // WpfClient as it receives a stream of messages
                    // - for each message received the Server
                    //   sends back 1000 messages
                    //
                    int count = 0;
                    for (int i = 0; i < 2; ++i)
                    {
                        Message testMsg = new Message();
                        testMsg.toUrl = msg.toUrl;
                        testMsg.fromUrl = msg.fromUrl;
                        testMsg.content = String.Format("test message #{0}", ++count);
                        Console.Write("\n  sending testMsg: {0}", testMsg.content);
                        sndr.sendMessage(testMsg);
                    }
#else
                    /////////////////////////////////////////////////
                    // Use the statement below for normal operation
                    // sndr.sendMessage(msg);
#endif
                }
            };

            if (rcvr.StartService())
            {
                rcvr.doService(serviceAction); // This serviceAction is asynchronous,
            }                                // so the call doesn't block.
            Util.waitForUser();
        }

    }
}
