///////////////////////////////////////////////////////////////
// TestExec.cs - Test Requirements for Project #2            //
// Ver 1.2                                                   //
// Application: Demonstration for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Dell Inspiron 14z, Core-i5, Windows 10       //
// Source :     Jim Fawcett, CST 4-187, Syracuse University  //
// Author:      Saurabh Patel, MSCE Grad Student             //
//              Syracuse University                          //
//              (315) 751-3911, skpatel@syr.edu              //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package does the demonstration of meeting requirements.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   TestExec.cs,  DBElement.cs, DBEngine, Display, 
 *   DBExtensions.cs, UtilityExtensions.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.2 : 07 Oct 15
 * Final Release.
 * ver 1.1 : 24 Sep 15
 * ver 1.0 : 18 Sep 15
 * - first release
 *
 */

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using static System.Console;

namespace Project2Starter
{
    class TestExec
    {
        DBEngine<int, DBElement<int, ListOfStrings>> db1 = new DBEngine<int, DBElement<int, ListOfStrings>>();
        DBEngine<int, DBElement<int, string>> db2 = new DBEngine<int, DBElement<int, string>>();

        void TestR2()
        {
            "Demonstrating Requirement #2".title('='); // First type of database.
            "  First Type of database.".title('-');
            DBElement<int, string> elem = new DBElement<int, string>();
            elem.name = "1st Key/Value = Int/String";
            elem.descr = "This is the first element which is int/string key value pair.";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 10, 11, 12 });
            elem.payload = "Payload : Key/Value = Int/String";
            db2.insert(1, elem);

            DBElement<int, string> elem21 = new DBElement<int, string>();
            elem21.name = "2nd Key/Value = Int/String";
            elem21.descr = "This is the second element which is int/string key value pair.";
            elem21.timeStamp = DateTime.Now;
            elem21.children.AddRange(new List<int> { 20, 21, 22 });
            elem21.payload = "Payload 2 : Key/Value = Int/String";
            db2.insert(1, elem21);

            WriteLine("  This shows Int/String key/value paris data in-memory database.");
            db2.showDB();
            WriteLine();

            // second type of database.
            WriteLine();
            "  Second Type of database.".title('-');
            WriteLine();
            DBElement<int, ListOfStrings> elem1 = new DBElement<int, ListOfStrings>();
            elem1.name = "1st Key/Value = Int/ListOfStrings";
            elem1.descr = "This is first element of Int/ListOfStrings key value pair.";
            elem1.timeStamp = DateTime.Now;
            elem1.children.AddRange(new List<int> { 100, 101, 102, 103, 104, 105 });
            elem1.payload = new ListOfStrings();
            elem1.payload.theWrappedData = new List<string> { "CSE681", "SMA", "C#.net", "AI" };
            db1.insert(1, elem1);

            DBElement<int, ListOfStrings> elem2 = new DBElement<int, ListOfStrings>();
            elem2.name = "element int-ListOfString 2";
            elem2.descr = "test element int-ListOfString 2";
            elem2.timeStamp = DateTime.Now;
            elem2.children.AddRange(new List<int> { 10, 22, 23, 24, 25, 26 });
            elem2.payload = new ListOfStrings();
            elem2.payload.theWrappedData = new List<string> { "CSE6812", "SMA2", "C#.net2", "AI2" };
            db1.insert(2, elem2);

            WriteLine("  This shows Int/ListOfString key/value paris data in-memory database.");
            db1.showDB();
            WriteLine();
        }
        void TestR3()
        {
            "Demonstrating Requirement #3".title('=');
            WriteLine();
            WriteLine("");
            "Before Addition database looks like : ".title('-');
            db1.showDB();

            DBElement<int, ListOfStrings> elem3 = new DBElement<int, ListOfStrings>();
            elem3.name = "element int-ListOfString 3";
            elem3.descr = "test element int-ListOfString 3";
            elem3.timeStamp = DateTime.Now;
            elem3.children.AddRange(new List<int> { 30, 32, 33, 34, 35, 36 });
            elem3.payload = new ListOfStrings();
            elem3.payload.theWrappedData = new List<string> { "CSE6813", "SMA3", "C#.net3", "A3I" };
            db1.insert(3, elem3);

            DBElement<int, ListOfStrings> elem4 = new DBElement<int, ListOfStrings>();
            elem4.name = "element int-ListOfString  4";
            elem4.descr = "test element int-ListOfString 4";
            elem4.timeStamp = DateTime.Now;
            elem4.children.AddRange(new List<int> { 40, 42, 43, 44, 45, 46 });
            elem4.payload = new ListOfStrings();
            elem4.payload.theWrappedData = new List<string> { "CSE6814", "SMA4", "C#.net4", "AI4" };
            db1.insert(4, elem4);
            "After Addition of 2 elements database looks like : ".title('-');
            db1.showDB();
            WriteLine();
            db1.delete(3);
            "After Deletion of 3rd key and its value from data : ".title('-');
            db1.showDB();
        }
        void TestR4()
        {
            " Demonstrating Requirement #4 ".title('=');
            WriteLine();
            "Before edit of Key=2 and Key=4 values.".title('-');
            DBElement<int, ListOfStrings> oldElement = db1.Dictionary[2];
            oldElement.showElement();
            ItemEditor<int, ListOfStrings> itemEditor = new ItemEditor<int, ListOfStrings>();
            // Creates a DateTime for the local time.
            itemEditor.editByName(ref db1,2,"edited name for key=2");
            itemEditor.editByChild(ref db1, 2, new List<int> { 205, 206, 207, 208, 209, 210 });
            itemEditor.editByDescr(ref db1, 2, "edited descr for key=2");
            DBElement<int, ListOfStrings> newElement123d = db1.Dictionary[2];
            newElement123d.showElement();
            itemEditor.editPayloadByListOfString(ref db1, 2, new List<string> { "CSE681_2_New", "SMA_2_New", "C#.net_2_New", "AI_2_New" });
            DateTime newTime = new DateTime(1990, 6, 15, 0, 0, 0);
            itemEditor.editByTime(ref db1, 4, newTime);
            DateTime newTime2 = new DateTime(1990, 6, 16, 0, 0, 0);
            itemEditor.editByTime(ref db1, 2, newTime2);
            "After edit of Key = 2 and Key = 4 values.".title('-');
            WriteLine();
            WriteLine("You can see in updated element, Name,description,children,time and payload attributes are changed.");
            DBElement<int, ListOfStrings> newElement1 = db1.Dictionary[2];
            DBElement<int, ListOfStrings> newElement2 = db1.Dictionary[4];
            newElement1.showElement();
            newElement2.showElement();
            WriteLine();
        }
        void TestR5()
        {
            PersistData<int, ListOfStrings> persistEngine1 = new PersistData<int, ListOfStrings>();
            "Demonstrating Requirement #5".title();
            "Write in-memory database in XML file to persist data, please check Data1.xml file.".title('-');
            persistEngine1.loadXmlFile("Data1.xml", typeof(int).Name, typeof(ListOfStrings).Name);

            if (!persistEngine1.addRecord(1, db1.Dictionary[1]))
                WriteLine("\nKey 1 is already in xml file.");
            else
                WriteLine("Key 1 is inserted in xml file.");
            if (!persistEngine1.addRecord(2, db1.Dictionary[2]))
                WriteLine("Key 2 is already in xml file.");
            else
                WriteLine("Key 2 is inserted in xml file.");
            if (!persistEngine1.addRecord(4, db1.Dictionary[4]))
                WriteLine("Key 4 is already in xml file.");
            else
                WriteLine("Key 4 is inserted in xml file.");
            if (!persistEngine1.addRecord(2, db1.Dictionary[4]))
                WriteLine("Key 4 is already in xml file.");
            else
                WriteLine("Key 4 is inserted in xml file.");

            "Persist XML file looks like this ".title('-');
            XDocument document = persistEngine1.loadXmlFile("Data1.xml");
            WriteLine(document.ToString());
            "Database restored or augmented from an existing XML (Data1.xml) file".title('-');


            DBEngine<int, DBElement<int, ListOfStrings>> augmentData = new DBEngine<int, DBElement<int, ListOfStrings>>();
            persistEngine1.retrieveDataFromXML(augmentData, "Data1.xml");
            augmentData.showDB();

            WriteLine();
        }
        void TestR6()
        {
            int interval = 100;
            "Demonstrating Requirement #6".title();
            WriteLine("\n  Enabled Scheduler for {0} miliseconds", interval);
            Scheduler schedular = new Scheduler(interval, db1);
            schedular.schedular.Enabled = true;
            WriteLine();
            for (int i = 10; i < 20; i++)
            {
                DBElement<int, ListOfStrings> newelement = new DBElement<int, ListOfStrings>();
                newelement.name = "newelement int-ListOfString : " + i.ToString();
                newelement.descr = "Scheduler requirement demonstration : " + i.ToString();
                newelement.timeStamp = DateTime.Now;
                newelement.children.AddRange(new List<int> { 1 + i, 2 + i, 3 + i, 4 + i, 5 + i });
                newelement.payload = new ListOfStrings();
                newelement.payload.theWrappedData = new List<string> { "CSE6814" + i.ToString(), "SMA4" + i.ToString(), "C#.net4" + i.ToString(), "AI4" + i.ToString() };
                if (db1.insert(i, newelement))
                    WriteLine("  Added in in-memory database.");
                else
                    WriteLine("  Insert failed into in-memory database.");
            }
            WriteLine();
            WriteLine("  Adding new elements in in-memory database and it will be persist to xml file automatically.");

        }
        void TestR7()
        {
            "Demonstrating Requirement #7".title();
            "The Value of a specified key".title('-');
            WriteLine();
            WriteLine("  Query : value of key = 14");
            QueryProcessEngine<int, ListOfStrings> queryEngine = new QueryProcessEngine<int, ListOfStrings>(db1);
            DBElement<int, ListOfStrings> elem1;
            queryEngine.processValueQuery(14, out elem1);
            elem1.showElement();
            "The children of a specified key".title('-');
            WriteLine("  Query : children of key = 1");
            List<int> childrens;
            queryEngine.processChildrenQuery(1,out childrens);
            WriteLine("  Children of key = 1 ");
            foreach (var item in childrens)
            {
                WriteLine(" --> "+item);
            }
            WriteLine();
            "The set of all keys matching a specified pattern which defaults to all keys.".title('-');
            WriteLine("  Query : Search keys which contains \"1\" in their keys value.");
            Dictionary<int, DBElement<int, ListOfStrings>> results;
            queryEngine.processPatternMatchInKeysQuery(queryEngine.defineQueryKeyPatternSearch("1"), out results);
            DBFactory<int, DBElement<int,ListOfStrings>> dbFactory = new DBFactory<int, DBElement<int, ListOfStrings>>(results);
            dbFactory.showDBFactory();

            WriteLine();
            "All keys that contain a specified string in their metadata section.".title('-');
            WriteLine("  Query : Search keys/values which contains \"Scheduler\" text in their metadata value.");
            results.Clear();
            queryEngine.processPatternMatchInMetaDataQuery(queryEngine.defineQueryValuePatternSearch("Scheduler") ,out results);
            dbFactory = new DBFactory<int, DBElement<int, ListOfStrings>>(results);
            dbFactory.showDBFactory();
            
            WriteLine();
            "All keys that contain values written within a specified time-date interval".title('-');
            DateTime dt1 = new DateTime(1990,6, 14, 0, 0, 0);
            DateTime dt2 = new DateTime(1990, 6, 17, 0, 0, 0);
            results.Clear();
            queryEngine.processTimeIntervalQuery(queryEngine.defineTimeStampQuery(dt1,dt2),out results);
            dbFactory = new DBFactory<int, DBElement<int, ListOfStrings>>(results);
            dbFactory.showDBFactory();

        }
        void TestR8()
        {
            "Demonstrating Requirement #8".title();
            WriteLine();
            WriteLine("In requirement-7 you can check requirement number-8, Where I used DBFactory object to store query processing results.");
            WriteLine("which supports the creation of a new immutable database constructed from the result of any query");
            WriteLine("For more details, you can checkout DBFactory.cs file, where I have not put any method which can change dictionary of DBFactory.s");
            WriteLine();
        }

        void TestR9()
        {
            "Demonstrating Requirement #9".title();
            WriteLine();
            PersistData<int, ListOfStrings> persistEngine = new PersistData<int, ListOfStrings>();
            DBEngine<string, DBElement<string, ListOfStrings>> augmentData = new DBEngine<string, DBElement<string, ListOfStrings>>();
            persistEngine.loadXmlFile("Dependency.xml");
            persistEngine.retrieveDataFromDependencyXML(augmentData, "Dependency.xml");
            augmentData.showDependencyDB();
            WriteLine();
        }

        static void Main(string[] args)
        {
            TestExec exec = new TestExec();
            "Demonstrating Project#2 Requirements".title('=');
            WriteLine();
            exec.TestR2();
            exec.TestR3();
            exec.TestR4();
            exec.TestR5();
            exec.TestR6();
            exec.TestR7();
            exec.TestR8();
            exec.TestR9();
            WriteLine("\nPlease Press any key to stop Scheduler.");
            Console.ReadKey();
            Write("\n\n");
        }
    }
}
