/////////////////////////////////////////////////////////////////////
// Scheduler.cs - define methods to start scheduler to persist data.//
// Ver 1.0                                                         //
// Application: Demonstration for CSE681-SMA, Project#2            //
// Language:    C#, ver 6.0, Visual Studio 2015                    //
// Platform:    Dell Inspiron 14z, Core-5, Windows 10              //
// Author:      Saurabh Patel, MSCE Graduate Student,              //
//              Syracuse University                                //
//              (315) 751-3911, skpatel@syr.edu                    //
/////////////////////////////////////////////////////////////////////

/*
 * Package Operations:
 * -------------------
 * This package starts schedular for specific time interval, it will store data from 
 * from in-memory database to persist xml file.
 * 
 * 
 * Public Interface:
 * ==============================
 * Schedular constructor which schedule timer based on input interval timings.
        public Scheduler(double interval, DBEngine<int, DBElement<int, ListOfStrings>> dbEngine)
 * 
 */
/*
 * Maintenance:
 * ------------
 * Required Files: Scheduler.cs DBEngine.cs
 *                 DBElement.cs, PayloadWrapper.cs
 *                 PersistEngine.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 07 Oct 15
 * - first release
 *
 */

using System;
using System.Collections.Generic;
using System.Timers;

namespace Project2Starter
{
    public class Scheduler
    {
        public Timer schedular { get; set; } = new Timer();

        /////////////////////////////////////////////////////////////////
        // Schedular constructor which schedule timer based on input interval timings.
        public Scheduler(double interval, DBEngine<int, DBElement<int, ListOfStrings>> dbEngine)
        {
      
            schedular.Interval = interval;
            schedular.AutoReset = true;

            PersistData<int, ListOfStrings> persistEngine = new PersistData<int, ListOfStrings>();
            persistEngine.loadXmlFile("Data1.xml");
            schedular.Elapsed += (object source, ElapsedEventArgs e) =>
            {
                persistEngine.persistDatatoXML(ref dbEngine);
                Console.Write("  Persist Data to XML called by Scheduler.\n");
            };
        }

        static void Main(string[] args)
        {
            "Testing Scheduler Package".title('=');
            Console.WriteLine();

            DBEngine<int, DBElement<int, ListOfStrings>> dbEngineType1 = new DBEngine<int, DBElement<int, ListOfStrings>>();
            DBElement<int, ListOfStrings> elem = new DBElement<int, ListOfStrings>();
            "Testing function makeDataForDB() by creating metadata for Type1 = <string,ListOfStrings> and Type2 = <int,string>".title('-');
            elem.name = "X";
            elem.descr = "description";
            elem.timeStamp = DateTime.Now;
            elem.payload = new ListOfStrings();
            elem.payload.theWrappedData = new List<string> { "payload1", "payload2" };
            dbEngineType1.insert(1, elem);
           
            "Scheduler is persisting data every one second. Press any key to exit".title('-');
            Scheduler schd = new Scheduler(1000, dbEngineType1);
            schd.schedular.Enabled = true;
            Console.ReadKey();
            Console.Write("\n\n");
        }
    }
}
