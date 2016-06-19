///////////////////////////////////////////////////////////////////////
// DBFactory.cs - Stores result which computed by query engine     //
//                We can use this results for compound query.      //
// Ver 1.2                                                         //
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
 * This package DBFactory<Key, Value>s to wrap dictionary. It contains dictionary which has
 * expossed in to db factory constructor.
 * 
 * Public Interface:
 * ==============================
 * public DBFactory(Dictionary<Key, Value> db)
    constructor of DBFactory which takes input of dictionary.
 * public bool getValue(Key key, out Value val)
    get value for input key, which provided as a parameter.
    out val : value parameter where value will be fill up for key.
 * public IEnumerable<Key> Keys() 
     return keys list of dictionary
 * public void showDBFactory()
   show DB factory data.
 *
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs, and
 *                 Payloadwrapper.cs
 *                 UtilityExtensions.cs , Display and DBExtensions.cs only if you enable the test stub
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

using static System.Console;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Project2Starter
{
    public class DBFactory<Key, Value>
    {
        // variable which store in memory immutable database.
        private Dictionary<Key, Value> dbStore;
        // constructor of DBFactory which takes input of dictionary.
        public DBFactory(Dictionary<Key, Value> db)
        {
            dbStore = db;
        }
        /*
         * get value for input key, which provided as a parameter.
         * out val : value parameter where value will be fill up for key.
         */
        public bool getValue(Key key, out Value val)
        {
            if (dbStore.Keys.Contains(key))
            {
                val = dbStore[key];
                return true;
            }
            val = default(Value);
            return false;
        }
        /*
         * return keys list of dictionary
         */
        public IEnumerable<Key> Keys()
        {
            return dbStore.Keys;
        }
        /*
         * show DB factory data.
         */
        public void showDBFactory()
        {
            foreach (Key key in dbStore.Keys)
            {
                Value value = dbStore[key];
                DBElement<int, ListOfStrings> elem = value as DBElement<int, ListOfStrings>;
                Write("\n\n  -- key = {0} --", key);
                elem.showElement();
            }
        }
    }


#if (TEST_DBFACTORY)
    class TestDBFactory
    {
        static void Main(string[] args)
        {
            "Testing DBFactory Package".title('=');
            WriteLine();
            "Testing func immutable keys from key collection".title();
            Dictionary<int, DBElement<int, ListOfStrings>> db = new Dictionary<int, DBElement<int, ListOfStrings>>();
            DBElement<int, ListOfStrings> elem2 = new DBElement<int, ListOfStrings>();
            elem2.name = "element int-ListOfString 2";
            elem2.descr = "test element int-ListOfString 2";
            elem2.timeStamp = DateTime.Now;
            elem2.children.AddRange(new List<int> { 10, 22, 23, 24, 25, 26 });
            elem2.payload = new ListOfStrings();
            elem2.payload.theWrappedData = new List<string> { "CSE6812", "SMA2", "C#.net2", "AI2" };
            db.Add(1,elem2);
            DBFactory<int, DBElement<int, ListOfStrings>> dbFac = new DBFactory<int, DBElement<int, ListOfStrings>>(db);
            dbFac.showDBFactory();
            Write("\n\n");
        }
    }
#endif
}