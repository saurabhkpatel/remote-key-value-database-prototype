///////////////////////////////////////////////////////////////
// DBEngine.cs - define noSQL database and in-memory database//
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
 * This package implements DBEngine<Key, Value> where Value
 * is the DBElement<key, Data> type.
 *
 * This class is a starter for the DBEngine package you need to create.
 * It doesn't implement many of the requirements for the db, e.g.,
 * It doesn't remove elements, doesn't persist to XML, doesn't retrieve
 * elements from an XML file, and it doesn't provide hook methods
 * for scheduled persistance.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs, and
 *                 UtilityExtensions.cs only if you enable the test stub
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.2 : 24 Sep 15
 * - removed extensions methods and tests in test stub
 * - testing is now done in DBEngineTest.cs to avoid circular references
 * ver 1.1 : 15 Sep 15
 * - fixed a casting bug in one of the extension methods
 * ver 1.0 : 08 Sep 15
 * - first release
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Project4Starter
{
    public class DBEngine<Key, Value>
    {
        // variable which store in memory database.
        private Dictionary<Key, Value> dbStore;

         /*
        * Property to get dictionary.
        */
        public Dictionary<Key, Value> Dictionary
        {
            get { return dbStore; }
            set { dbStore = value; }
        }

        /*
         * DBEngine : Constructor.
         */
        public DBEngine()
        {
            dbStore = new Dictionary<Key, Value>();
        }


        /*
         * Insert key and value in database.
         * key : key of DB element.
         * val : value of DB element.
         */
        public bool insert(Key key, Value val)
        {
            if (dbStore.Keys.Contains(key))
                return false;
            dbStore[key] = val;
            return true;
        }

        public bool edit(Key key, Value val)
        {
            dbStore[key] = val;
            return true;
        }

        /*
         * delete key/value pair from dictionary.
         * key : key which we want to delete.
         */
        public bool delete(Key key)
        {
            return dbStore.Remove(key);
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
         * Clear dictionary.
         */
        public void Clear()
        {
            dbStore.Clear();
        }
    }

#if (TEST_DBENGINE)

    class TestDBEngine
    {
        static void Main(string[] args)
        {
            "Testing DBEngine Package".title('=');
            WriteLine();

            Write("\n  All testing of DBEngine class moved to DBEngineTest package.");
            Write("\n  This allow use of DBExtensions package without circular dependencies.");

            Write("\n\n");
        }
    }
#endif
}
