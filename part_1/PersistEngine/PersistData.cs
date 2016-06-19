///////////////////////////////////////////////////////////////
// PersistData.cs - define functions which supports to persist//
//                  and restore data from xml file.          //
// Ver 1.2                                                   //
// Application: Demonstration for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Dell Inspiron 14z, Core-i5, Windows 10       //
// Author:      Saurabh Patel, MSCE Grad Student             //
//              Syracuse University                          //
//              (315) 751-3911, skpatel@syr.edu              //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package uses LINQ to xml to create , read and query xml file.
 * This package provides functions to load xml file, write database in xml file to persist it.
 * Using restore method we can restore data from xml file too.
 * 
 * 
 * Public Interface:
 * ==============================
 * Load xml file, before any operations on xml file.
    public XDocument loadXmlFile(String inputFile)
 * Load xml file, which filename gives as input.
 * We also need to define keytype and dataype , so it will load as per that.
    public void loadXmlFile(String inputFile, String keyType, String dataType)
 * Add record in xml file. needs to provide input as key and value.
    public bool addRecord(Key key, DBElement<Key, Value> data)
* check key present or not in xml file.
    private bool keyPresent(Key key)
* Persist data which resides in db engine that would be save in xml as per scheduler configuation.
    public void persistDatatoXML(ref DBEngine<Key, DBElement<Key, Value>> dbEngine)
* Retrive data from xml file. augmented in DBEngine.
    public void retrieveDataFromDependencyXML(DBEngine<string, DBElement<string, ListOfStrings>> dbEngine, String inputFile)
* Retrive data from xml file. augmented in DBEngine.
    public void retrieveDataFromXML(DBEngine<int, DBElement<int, ListOfStrings>> dbEngine, String inputFile)

 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs, PayloadWrapper.cs
 *                 UtilityExtensions.cs, Display.cs DBExtensions.cs only if you enable the test stub
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

using Project2Starter;
using System;
using System.Linq;
using static System.Console;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;

namespace Project2Starter
{

    public class PersistData<Key, Value>
    {
        XDocument document = new XDocument();
        String fileName = "";

        
        // Load xml file, before any operations on xml file.
        public XDocument loadXmlFile(String inputFile)
        {
            fileName = @".\" + inputFile;
            //We know it exists so we can load it
            document = XDocument.Load(fileName);
            return document;
        }

        
        //Load xml file, which filename gives as input.
         //We also need to define keytype and dataype , so it will load as per that.
        public void loadXmlFile(String inputFile, String keyType, String dataType)
        {
            fileName = @".\" + inputFile;
            if (!File.Exists(inputFile))
            {
                document.Declaration = new XDeclaration("1.0", "utf-8", "yes");
                XElement root = new XElement("noSqlDb");
                document.Add(root);

            }
            else         //We know it exists so we can load it
                document = XDocument.Load(inputFile);
            addKeyPayLoadType(keyType, dataType);
            document.Save(inputFile);
        }

        /*
        * Add key and payload type in xml file.
        */
        private void addKeyPayLoadType(String keyType, String dataType)
        {
            if (document != null)
            {
                XElement root = document.Root;
                if (root == null)
                {

                    root = new XElement("noSqlDb");
                    document.Add(root);
                }
                XElement keyElement = new XElement("keyType", keyType);
                XElement dataElement = new XElement("payloadType", dataType);
                if (!(document.Element("noSqlDb")
                   .Elements("keyType").Any() && document.Element("noSqlDb").Elements("payloadType").Any()))
                {
                    root.Add(keyElement);
                    root.Add(dataElement);
                }
            }
        }

        /*
        * Add record in xml file. 
        * needs to provide input as key and value.
        */
        public bool addRecord(Key key, DBElement<Key, Value> data)
        {
            if (!keyPresent(key))
            {
                addRecordInXml(key, data);
                return true;
            }
            else
                return false;
        }

        /*
         * check key present or not in xml file.
         */
        private bool keyPresent(Key key)
        {
            bool elementPresent = document.Element("noSqlDb").Elements("elements").Any();
            if (elementPresent)
            {
                bool keyPresent = document.Descendants().Any(e => e.Name == "key" && e.Attributes().Any(a => a.Name == "id" && a.Value == key.ToString()));
                if (!keyPresent)
                    return false;
            }
            else
                return false;
            return true;
        }

        /*
         * Add record in xml file.
         * Input will be key and value pair.
         */
        private void addRecordInXml(Key key, DBElement<Key, Value> record)
        {
            bool present = document.Descendants("elements").Any();
            if (!present)
            {
                XElement element = new XElement("elements");
                document.Root.Add(element);
            }
            // Add key-value pair under elements TAG.
            XElement keyElement = new XElement("key", new XAttribute("id", key.ToString()));
            XElement valueElement = new XElement("value");
            XElement nameElement = new XElement("name", record.name);
            XElement timeElement = new XElement("time", record.timeStamp.ToString());
            XElement descrElement = new XElement("desc", record.descr);
            XElement payLoadElement = new XElement("payload");
            String payloadType = record.payload.GetType().ToString();
            ListOfStrings payload = record.payload as ListOfStrings;
            if (payload != null)
            {
                foreach (var item in payload.theWrappedData)
                {
                    XElement payLoadChildElement = new XElement("item", item.ToString());
                    payLoadElement.Add(payLoadChildElement);
                }
            }
            valueElement.Add(nameElement);
            valueElement.Add(timeElement);
            valueElement.Add(descrElement);
            valueElement.Add(payLoadElement);


            // add children in xml file.
            List<Key> listChildren = record.children;
            if (listChildren.Count > 0)
            {
                XElement children = new XElement("children");
                foreach (var item in listChildren)
                {
                    XElement xChildren = new XElement("item", item);
                    children.Add(xChildren);
                }
                valueElement.Add(children);
            }

            keyElement.Add(valueElement);

            document.Element("noSqlDb").Element("elements").Add(keyElement);
            document.Save(fileName);
        }

        /*
         * Persist data which resides in db engine that would be save in xml as per scheduler configuation.
         */
        public void persistDatatoXML(ref DBEngine<Key, DBElement<Key, Value>> dbEngine)
        {
            IEnumerable<Key> keysCollection = dbEngine.Keys().ToList();
            foreach (Key item in keysCollection)
            {
                addRecord(item, dbEngine.Dictionary[item]);
            }
        }
        /*
        * Retrive data from  dependency xml file. augmented in DBEngine.
        */
        public void retrieveDataFromDependencyXML(DBEngine<string, DBElement<string, ListOfStrings>> dbEngine, String inputFile)
        {
            fileName = inputFile;
            var elem = from c in document.Descendants("elements") select c;

            for (int i = 0; i < elem.Elements().Count(); i++)
            {
                DBElement<string, ListOfStrings> dbElement = new DBElement<string, ListOfStrings>();
                string key = elem.Elements().Attributes().ElementAt(i).Value;

                for (int count = 0; count < elem.Elements().Attributes().ElementAt(i).Parent.Descendants().Count(); count++)
                {
                    XElement elementRecord = elem.Elements().Attributes().ElementAt(i).Parent.Descendants().ElementAt(count);
                    if (elementRecord.Name.ToString().Equals("name"))
                    {
                        dbElement.name = elementRecord.Value;
                    }
                    else if (elementRecord.Name.ToString().Equals("desc"))
                    {
                        dbElement.descr = elementRecord.Value;
                    }
                    else if (elementRecord.Name.ToString().Equals("time"))
                    {
                        dbElement.timeStamp = DateTime.Parse(elementRecord.Value);
                    }
                    else if (elementRecord.Name.ToString().Equals("children"))
                    {
                        List<string> children = new List<string>();
                        for (int j = 0; j < elementRecord.Descendants().Count(); j++)
                        {
                            children.Add(elementRecord.Descendants().ElementAt(j).Value);
                        }
                        dbElement.children = children;
                    }
                    else if (elementRecord.Name.ToString().Equals("payload"))
                    {
                        ListOfStrings payload = new ListOfStrings();
                        for (int j = 0; j < elementRecord.Descendants().Count(); j++)
                        {
                            payload.theWrappedData.Add(elementRecord.Descendants().ElementAt(j).Value);
                        }
                        dbElement.payload = payload;
                    }
                }
                dbEngine.Dictionary.Add(key, dbElement);
            }
        }
        /*
         * Retrive data from xml file. augmented in DBEngine.
         */
        public void retrieveDataFromXML(DBEngine<int, DBElement<int, ListOfStrings>> dbEngine, String inputFile)
        {
            fileName = inputFile;
            var elem = from c in document.Descendants("elements") select c;

            for (int i = 0; i < elem.Elements().Count(); i++)
            {
                DBElement<int, ListOfStrings> dbElement = new DBElement<int, ListOfStrings>();
                int key = Int32.Parse(elem.Elements().Attributes().ElementAt(i).Value);

                for (int count = 0; count < elem.Elements().Attributes().ElementAt(i).Parent.Descendants().Count(); count++)
                {
                    XElement elementRecord = elem.Elements().Attributes().ElementAt(i).Parent.Descendants().ElementAt(count);
                    if (elementRecord.Name.ToString().Equals("name"))
                    {
                        dbElement.name = elementRecord.Value;
                    }
                    else if (elementRecord.Name.ToString().Equals("desc"))
                    {
                        dbElement.descr = elementRecord.Value;
                    }
                    else if (elementRecord.Name.ToString().Equals("time"))
                    {
                        dbElement.timeStamp = DateTime.Parse(elementRecord.Value);
                    }
                    else if (elementRecord.Name.ToString().Equals("children"))
                    {
                        List<int> children = new List<int>();
                        for (int j = 0; j < elementRecord.Descendants().Count(); j++)
                        {
                            children.Add(Int32.Parse(elementRecord.Descendants().ElementAt(j).Value));
                        }
                        dbElement.children = children;
                    }
                    else if (elementRecord.Name.ToString().Equals("payload"))
                    {
                        ListOfStrings payload = new ListOfStrings();
                        for (int j = 0; j < elementRecord.Descendants().Count(); j++)
                        {
                            payload.theWrappedData.Add(elementRecord.Descendants().ElementAt(j).Value);
                        }
                        dbElement.payload = payload;
                    }
                }
                dbEngine.Dictionary.Add(key, dbElement);
            }
        }
    }

#if (TEST_PERSISTENGINE)
    class TestPersistEngine
    {
        static void Main(string[] args)
        {
            "Testing Persist Engine Package".title('=');
            DBEngine<int, DBElement<int, ListOfStrings>> db1 = new DBEngine<int, DBElement<int, ListOfStrings>>();
            PersistData<int, ListOfStrings> persistEngine1 = new PersistData<int, ListOfStrings>();
            "Write in-memory database in XML file, please check DataPersistTest.xml file.".title('-');
            persistEngine1.loadXmlFile("DataPersistTest.xml", typeof(int).Name, typeof(ListOfStrings).Name);
            if (!persistEngine1.addRecord(1, db1.Dictionary[1]))
                WriteLine("Key 1 is already in xml file.");
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

            "Database restored or augmented from an existing XML (Data1.xml) file".title('-');

            DBEngine<int, DBElement<int, ListOfStrings>> augmentData = new DBEngine<int, DBElement<int, ListOfStrings>>();
            persistEngine1.retrieveDataFromXML(augmentData, "DataPersistTest.xml");
            augmentData.showDB();

            WriteLine();
        }
    }
#endif
}
