///////////////////////////////////////////////////////////////
// TestExecutive.cs - Test Requirements for Project #4       //
// Ver 1.0                                                   //
// Application: Demonstration for CSE681-SMA, Project#4      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Mac Book Pro, Core-i5, Windows 10            //
// Author:      Saurabh Patel, MSCE Grad Student             //
//              Syracuse University                          //
//              (315) 751-3911, skpatel@syr.edu              //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package begins the demonstration of meeting requirements.
 * All other packages are called from here.
 *
 * Required Files:
 * Utilities.cs 
 *
 * Build Process:  devenv Project4.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Activities
 * -------------------
 * On starting the Test Executive, one instance of each wpf client and 
 * server will start.
 * Server will be using the port 8080, All writerClient are using ports which rae starting from 9080
 * and all reader clients are using ports which are starting from 9090
 * 
 * Command Line Arguments for Test Executives
 * -------------------------------------------
 * eg 3 3 Y :- "No. of Read Clients" "No. of write Clients" "Message Logging on the Console"
 * 
 * Maintenance History:
 * --------------------
 * ver 1.0 : 22 Nov 15
 */

using System;

namespace Project4Starter
{
    using System.Diagnostics;
    using Util = Utilities;
    class TestExec
    {
        int numR { get; set; } = 1;
        int numW { get; set; } = 1;
        string log { get; set; } = "N";

        static int serverPort = 8080;
        static int writerClientPort = 9080;
        static int readerClientPort = 9090;
        public void processCommandLine(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("  Invalid command line arguments.");
            }
            else if (args.Length == 3)
            {
                try
                {
                    numR = Int32.Parse(args[0]);
                    numW = Int32.Parse(args[1]);
                    log = (args[2] == "Y") ? "Y" : "N";
                }
                catch (Exception e)
                {
                    Console.WriteLine("processCommandLine Exception :" + e.StackTrace);
                }
            }
            else
            {
                Console.WriteLine("  Invalid command line arguments.");
            }

        }

        public string getProcessPath(string packagename)
        {
            string temp = AppDomain.CurrentDomain.BaseDirectory;
            int j = 0;
            while (j != 4)
            {
                int i = temp.LastIndexOf("\\");
                temp = temp.Substring(0, i);
                j++;
            }
            packagename = temp + "\\" + packagename + "\\bin\\debug\\" + packagename + ".exe";
            return packagename;
        }

        void requirementR2()
        {
            "  Demonstrating Requirement 2".title();
            Console.WriteLine();
            Console.WriteLine("  Shall use the noSQL database you implemented in Project #2");
            Console.WriteLine("  Here, Reader and Writer clients are using NoSQL database which is implemented on remote using WCF.");
            Console.WriteLine("  Reader clients are doing all 5 types of queries which are implemented in project 2 and \n  Writer Clients for Insert/Update/Delete/Edit/Persist DB");
            Console.WriteLine("\n");
        }
        void requirementR3()
        {
            "Demonstrating Requirement 3".title();
            Console.WriteLine();
            Console.WriteLine("  Used WCF to communicate between clients and a server that exposes the noSQL database \n  through messages that are sent by clients and enqueued by the server.");
            Console.WriteLine("  Each message is processed by the server to interact with the database and results are sent,\n  as messages, back to queues provided by each client requestor.");
            Console.WriteLine("\n");
        }

        void requirementR4()
        {
            "Demonstrating Requirement 4".title();
            Console.WriteLine();
            Console.WriteLine("  Add, delete, and edit key/value pairs is demonstrated in Writer Client");
            Console.WriteLine("  Persist and Restore the database from an XML file is demonstrated in Writer Client");
            Console.WriteLine("  All types of queries are demonstrated in Reader Clients");
            Console.WriteLine("  Support the same queries as required in Project , \n   All of the above operation requests shall be sent to the remote database in the form of messages\n  described by a WCF Data Contract2. Replies are returned to the requestor in the form of WCF messages\n  using a suitable Data Contract");
        }

        void requirementR5()
        {
            "Demonstrating Requirement 5".title();
            Console.WriteLine();
            Console.WriteLine("  Writer Client: is a console Client that send data to the remote database");
            Console.WriteLine("  The content of several different messages is defined in an XML file --XMLWriter.xml--");
            Console.WriteLine("  In this file, User can also define number of messages which are going to sent to server.");
            Console.WriteLine("  Based on these number of message size, client will send messages to server and measure performance.");
            Console.WriteLine("\n");
        }
        void requirementR6()
        {
            "Demonstrating Requirement 6".title();
            Console.WriteLine();
            Console.WriteLine("  Pass Command Line Argument{3} as Y in test executives arguments to enable logging and N to disable");
            Console.WriteLine("\n");
        }
        void requirementR7()
        {
            "Demonstrating Requirement 7".title();
            Console.WriteLine();
            Console.WriteLine("  Read Client: is a CONSOLE Client that send different types of queries request to the remote database");
            Console.WriteLine("  The content of several different messages is defined in an XML file --XMLReader.xml--");
            Console.WriteLine("  Reader client contains all types of query operations which we implemented in project-2");
            Console.WriteLine("      Query-1 : The value of a specified key.");
            Console.WriteLine("      Query-2 : The children of a specified key.");
            Console.WriteLine("      Query-3 : The set of all keys matching a specified pattern which defaults to all keys.");
            Console.WriteLine("      Query-4 : All keys that contain a specified string in their metadata section.");
            Console.WriteLine("      Query-4 : All keys that contain values written within a specified time-date interval.");
            Console.WriteLine("\n");
        }

        // 
        void requirementR8()
        {
            "Demonstrating Requirement 8".title();
            Console.WriteLine();
            Console.WriteLine("  Read Client: will display the result of the queries if the logging in ON");
            Console.WriteLine("  The content of several different messages is defined in an XML file --XMLReader.xml--");
            Console.WriteLine("  In this file, User can also define number of messages which are going to sent to server.");
            Console.WriteLine("  Based on these number of message size, client will send messages to server and measure performance.");
            Console.WriteLine("\n");
        }

        static void Main(string[] args)
        {
            Console.Write("\n     Starting Test-Executive    ");
            Console.Write("\n =============================\n");
            Console.Title = "Test Executive";
            TestExec testExec = new TestExec();

            testExec.requirementR2();
            testExec.requirementR3();
            testExec.requirementR4();
            testExec.requirementR5();
            testExec.requirementR6();
            testExec.requirementR7();
            testExec.requirementR8();

            // Test if input arguments were supplied:
            Process.Start(testExec.getProcessPath("Server"));
            Process.Start(testExec.getProcessPath("WpfClient"));
            if (args.Length == 3)
            {
                string arg = "";
                testExec.processCommandLine(args);
                int i = 0;
                while (i < testExec.numR)
                {
                    arg = "/R http://localhost:" + TestExec.serverPort + "/CommService /L http://localhost:" + (TestExec.readerClientPort + i) + "/CommService";
                    if (testExec.log == "Y")
                        arg = arg + " /O";
                    Process.Start(testExec.getProcessPath("Client2"), arg);
                    i++;
                }
                i = 0;
                while (i < testExec.numW)
                {
                    arg = "/R http://localhost:" + TestExec.serverPort + "/CommService /L http://localhost:" + (TestExec.writerClientPort + i) + "/CommService";
                    if (testExec.log == "Y")
                        arg = arg + " /O";
                    Process.Start(testExec.getProcessPath("Client"), arg);
                    i++;
                }
                return;
            }
            else
            {
                Console.WriteLine("  Invalid command line arguments.");
                return;
            }
        }
    }
}