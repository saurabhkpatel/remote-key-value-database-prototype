///////////////////////////////////////////////////////////////
// QueryProcessEngine.cs - define functions which supports to persist//
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
* public constructor.
        public QueryProcessEngine(DBEngine<Key, DBElement<Key, Value>> dbEngine)
* delegate for time stamp query.
        public Func<Key, bool> defineTimeStampQuery(DateTime startTime, DateTime? endTime = null)
* delegate for key pattern search.
        public Func<Key, bool> defineQueryKeyPatternSearch(string queryTerm)
* deletgate for string search in metadata of any key.
        public Func<Key, bool> defineQueryValuePatternSearch(string search)
* process get value for input key query.
        public void processValueQuery(Key key, out DBElement<Key, Value> value)
* process get children query for input key.
        public bool processChildrenQuery(Key key, out List<Key> childrens)
* query for pattern match in keys
        public bool processPatternMatchInKeysQuery(Func<Key, bool> queryPredicate, out Dictionary<Key, DBElement<Key, Value>> results)
* query for pattern match in metadata.
        public bool processPatternMatchInMetaDataQuery(Func<Key, bool> queryPredicate, out Dictionary<Key, DBElement<Key, Value>> results)
* query for time interval of database.
        public bool processTimeIntervalQuery(Func<Key, bool> queryPredicate, out Dictionary<Key, DBElement<Key, Value>> results)

 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs, PayloadWrapper.cs
 *                 UtilityExtensions.cs, Display.cs PersistData.cs
                   DBExtensions.cs only if you enable the test stub
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
using System.Linq;
using System.Text.RegularExpressions;
using static System.Console;

namespace Project4Starter
{
    public class QueryProcessEngine<Key, Value>
    {
        private DBEngine<Key, DBElement<Key, Value>> db = null;

        // public constructor.
        public QueryProcessEngine(DBEngine<Key, DBElement<Key, Value>> dbEngine)
        {
            db = dbEngine;
        }

        // delegate for keypattern query.
        public Func<Key, bool> defineQueryKeyPatternSearch(string str = ".*")
        {
            Func<Key, bool> queryPredicate = (Key key) =>
            {
                if (!db.Keys().Contains(key))
                    return false;
                else
                {
                    try
                    {
                        if (Regex.IsMatch(key.ToString(), str))
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLine("\n  Invalid Regular Expression. Error Message : {0}\n", ex.Message);
                        return false;
                    }
                }
                return false;
            };
            return queryPredicate;
        }

        // delegate for time stamp query.
        public Func<Key, bool> defineTimeStampQuery(DateTime startTime, DateTime? endTime = null)
        {
            Func<Key, bool> queryPredicate = (Key key) =>
            {
                if (!db.Keys().Contains(key))
                    return false;
                else
                {
                    if (endTime == null)
                        endTime = DateTime.Now;
                    DBElement<Key, Value> value;
                    db.getValue(key, out value);
                    DBElement<Key, Value> elem = value as DBElement<Key, Value>;
                    int cond1 = DateTime.Compare(elem.timeStamp, startTime);
                    int cond2 = DateTime.Compare(elem.timeStamp, (DateTime)endTime);
                    if (cond1 >= 0 && cond2 <= 0)
                    {
                        return true;
                    }
                }
                return false;
            };
            return queryPredicate;
        }


        // deletgate for string search in metadata of any key.
        public Func<Key, bool> defineQueryValuePatternSearch(string search)
        {
            Func<Key, bool> queryPredicate = null;
            queryPredicate = (Key key) =>
            {
                if (db != null)
                {
                    if (db.Dictionary[key].name.Contains(search))
                        return true;
                    else if (db.Dictionary[key].descr.Contains(search))
                        return true;
                    else if (db.Dictionary[key].timeStamp.ToString().Contains(search))
                        return true;
                    else if (db.Dictionary[key].payload != null)
                    {
                        List<string> payload = db.Dictionary[key].payload as List<string>;
                        foreach (var item in payload)
                        {
                            if (item.ToString().Contains(search))
                                return true;
                        }
                    }
                    else if (db.Dictionary[key].children != null)
                    {
                        List<Key> childrens = db.Dictionary[key].children;
                        foreach (var item in childrens)
                        {
                            if (item.ToString().Contains(search))
                                return true;
                        }
                    }

                }
                return false;
            };
            return queryPredicate;

        }

        // process get value for input key query.
        public void processValueQuery(Key key, out DBElement<Key, Value> value)
        {
            value = default(DBElement<Key, Value>);
            if (db != null && db.Dictionary.Keys.Contains(key))
            {
                db.getValue(key, out value);
            }
        }

        // process get children query for input key.
        public bool processChildrenQuery(Key key, out List<Key> childrens)
        {
            childrens = new List<Key>();
            if (db != null && db.Dictionary.Keys.Contains(key))
            {
                childrens = db.Dictionary[key].children;
                if (childrens.Count() != 0)
                    return true;
            }
            return false;
        }

        // query for pattern match in keys
        public bool processPatternMatchInKeysQuery(Func<Key, bool> queryPredicate, out List<Key> childrens)
        {
            childrens = new List<Key>();
            IEnumerable<Key> keysCollection = db.Keys();
            foreach (var item in keysCollection)
            {
                if (queryPredicate(item))
                {
                    childrens.Add(item);
                }
            }
            if (childrens.Count() == 0)
                return false;
            else
                return true;
        }

        // query for pattern match in metadata.
        public bool processPatternMatchInMetaDataQuery(Func<Key, bool> queryPredicate, out List<Key> values)
        {
            values = new List<Key>();
            IEnumerable<Key> keysCollection = db.Keys();
            foreach (var item in keysCollection)
            {
                if (queryPredicate(item))
                {
                    values.Add(item);
                }
            }
            if (values.Count() == 0)
                return false;
            else
                return true;
        }

        // query for time interval of database.
        public bool processTimeIntervalQuery(Func<Key, bool> queryPredicate, out List<Key> values)
        {
            values = new List<Key>();
            IEnumerable<Key> keysCollection = db.Keys();
            foreach (var item in keysCollection)
            {
                if (queryPredicate(item))
                {
                    values.Add(item);
                }
            }
            if (values.Count() == 0)
                return false;
            else
                return true;
        }

        // compound query.
        public bool processCompoundQuery(Func<Key, bool> queryPredicate, out Dictionary<Key, DBElement<Key, Value>> results)
        {
            results = new Dictionary<Key, DBElement<Key, Value>>();
            IEnumerable<Key> keysCollection = db.Keys();
            foreach (var item in keysCollection)
            {
                if (queryPredicate(item))
                {
                    results.Add(item, db.Dictionary[item]);
                }
            }
            if (results.Count() == 0)
                return false;
            else
                return true;
        }
    }


#if (TEST_QUERY)
    
    class TestQuery
    {
        static void Main(string[] args)
        {
            "Testing QueryProcessing Package".title('=');
            WriteLine();

            DBEngine<int, DBElement<int, ListOfStrings>> db1 = new DBEngine<int, DBElement<int, ListOfStrings>>();
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

            WriteLine("  Query : value of key = 2");
            QueryProcessEngine<int, ListOfStrings> queryEngine = new QueryProcessEngine<int, ListOfStrings>(db1);
            DBElement<int, ListOfStrings> result;
            queryEngine.processValueQuery(14, out result);
            result.showElement();

            WriteLine("  Query : Children of key = 1");
            List<int> childrens;
            queryEngine.processChildrenQuery(1, out childrens);
            foreach (var item in childrens)
            {
                WriteLine(item);
            }
            WriteLine();

            "The set of all keys matching a specified pattern which defaults to all keys.".title('-');
            Dictionary<int, DBElement<int, ListOfStrings>> results;
            //queryEngine.processPatternMatchInKeysQuery(queryEngine.defineQueryKeyPatternSearch("1"), out results);
            //DBFactory<int, DBElement<int, ListOfStrings>> dbFactory = new DBFactory<int, DBElement<int, ListOfStrings>>(results);
            //dbFactory.showDBFactory();

            WriteLine();
            "All keys that contain a specified string in their metadata section.".title('-');
            //results.Clear();
            //queryEngine.processPatternMatchInMetaDataQuery(queryEngine.defineQueryValuePatternSearch("Scheduler"), out results);
            //dbFactory = new DBFactory<int, DBElement<int, ListOfStrings>>(results);
            //dbFactory.showDBFactory();

            WriteLine();
            "All keys that contain values written within a specified time-date interval".title('-');
            DateTime dt1 = new DateTime(1990, 6, 14, 0, 0, 0);
            DateTime dt2 = new DateTime(1990, 6, 17, 0, 0, 0);
            //results.Clear();
            //queryEngine.processTimeIntervalQuery(queryEngine.defineTimeStampQuery(dt1, dt2), out results);
            //dbFactory.showDBFactory();
        }
    }
#endif

}
