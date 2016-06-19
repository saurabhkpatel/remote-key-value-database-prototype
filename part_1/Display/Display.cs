﻿///////////////////////////////////////////////////////////////
// Display.cs - define methods to simplify display actions   //
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
 * This package implements formatting functions and tests them
 * along with DBExtensions.
 *
 * Note: This package defines some formatting functions but
 *       doesn't use them yet.  It simply tests use of
 *       DBExtensions.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: Display.cs, DBEngine.cs, DBElement.cs, 
 *                 DBExtensions.cs, UtilityExtensions.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.3 : 07 Oct 15
 *  Added some methods which supports functionality where we can show <int ListOfStrings>
 * type database and also shows dependency xml database.
 *   
 * ver 1.2 : 24 Sep 15
 * - minor tweeks to extension methods to use names from
 *   DBExtensions
 * ver 1.1 : 15 Sep 15
 * - fixed a couple of minor bugs and added more comments
 * ver 1.0 : 13 Sep 15
 * - first release
 *
 * Note:
 * -----
 * It's purpose is to compress
 * displays for TestExec.cs
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Project2Starter
{
    /////////////////////////////////////////////////////////////
    // ElementFormatter class
    // - I'm experimenting with this class.
    // - The intent is to compress the db displays so
    //   entire element is displayed on one line.
    // - It's not working correctly yet.
    // 
    public class ElementFormatter
    {
        //----< adds 2 blank spaces to the beginning of lines >----

        public static string makeMargin(string src)
        {
            StringBuilder temp = new StringBuilder("  ");
            foreach (char ch in src)
            {
                if (ch != '\n')
                    temp.Append(ch);
                else
                {
                    temp.Append(ch).Append("  ");
                }
            }
            return temp.ToString();
        }
        //----< replaces newline with ", " >-----------------------

        public static string makeLinear(string src)
        {
            StringBuilder temp = new StringBuilder();
            foreach (char ch in src)
            {
                if (ch != '\n')
                    temp.Append(ch);
                else
                    temp.Append(", ");
            }
            return temp.ToString();
        }

        //----< use this method to format db elements >------------
        public static string formatElement(string src, bool showMargin = true)
        {
            if (showMargin)
                return makeMargin(src.ToString());
            return makeLinear(src.ToString());
        }
        //----< use this method to display db elements >-----------
        public static void showElement(string src)
        {
            Write("\n{0}", makeMargin(src.ToString()));
        }
    }
    /*
     *  The purpose of these extension methods is to avoid using long generic type names,
     *  e.g., use:
     *    showEnumerableElement();
     *  instead of:
     *    showEnumerable<string, DBElement<string, List<string>>, List<string>, string>();
     *  and use:
     *    showEnumerableDB()
     *  instead of:
     *    showEnumerable<string, DBElement<string, List<string>>, List<string>, string>();
     *
     *  If you need to use a database with other types, just create extension methods for
     *  that as I've done in DBExtensions and here.
     */
    public static class DisplayExtensions
    {
        /// <summary>
        /// show db engine element in fomratted way.
        /// </summary>
        /// <param name="element"></param>
        public static void showElement(this DBElement<int, string> element)
        {
            Console.Write(element.showElement<int, string>());
        }
        /// <summary>
        /// show element which has key as integer and element as ListOfString.
        /// </summary>
        /// <param name="element"></param>
        public static void showElement(this DBElement<int, ListOfStrings> element)
        {
            Console.Write(element.showElement<int, ListOfStrings>());
        }

        /// <summary>
        /// show enumerable element.
        /// </summary>
        /// <param name="enumElement"></param>
        public static void showEnumerableElement(this DBElement<string, List<string>> enumElement)
        {
            Console.Write(enumElement.showElement<string, List<string>, string>());
        }

        /// <summary>
        /// show whole db engine database.
        /// </summary>
        /// <param name="db"></param>
        public static void showDB(this DBEngine<int, DBElement<int, string>> db)
        {
            db.show<int, DBElement<int, string>, string>();
        }

        //// show dependency xml which stores in db.
        public static void showDependencyDB(this DBEngine<string, DBElement<string, ListOfStrings>> db)
        {
            db.show<string, DBElement<string, ListOfStrings>, ListOfStrings>();
        }

        // show DB which has key as integer and value as ListOfStrings.
        public static void showDB(this DBEngine<int, DBElement<int, ListOfStrings>> db)
        {
            db.show<int, DBElement<int, ListOfStrings>, ListOfStrings>();
        }

        // show enumerable database.
        public static void showEnumerableDB(this DBEngine<string, DBElement<string, List<string>>> db)
        {
            db.show<string, DBElement<string, List<string>>, List<string>, string>();
        }
    }

#if (TEST_DISPLAY)

    class TestDisplay
    {
        static bool verbose = false;

        static void Main(string[] args)
        {
            "Testing DBEngine Package".title('='); ;
            WriteLine();

            "Test db of scalar elements".title();
            WriteLine();

            DBElement<int, string> elem1 = new DBElement<int, string>();
            elem1.payload = "a payload";

            DBElement<int, string> elem2 = new DBElement<int, string>("Darth Vader", "Evil Overlord");
            elem2.payload = "The Empire strikes back!";

            var elem3 = new DBElement<int, string>("Luke Skywalker", "Young HotShot");
            elem3.payload = "X-Wing fighter in swamp - Oh oh!";

            if (verbose)
            {
                Write("\n --- Test DBElement<int,string> ---");
                WriteLine();
                elem1.showElement();
                WriteLine();
                elem2.showElement();
                WriteLine();
                elem3.showElement();
                WriteLine();

                /* ElementFormatter is not ready for prime time yet */
                //Write(ElementFormatter.formatElement(elem1.showElement<int, string>(), false));
            }

            Write("\n --- Test DBEngine<int,DBElement<int,string>> ---");
            WriteLine();

            int key = 0;
            Func<int> keyGen = () => { ++key; return key; };

            DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
            bool p1 = db.insert(keyGen(), elem1);
            bool p2 = db.insert(keyGen(), elem2);
            bool p3 = db.insert(keyGen(), elem3);
            if (p1 && p2 && p3)
                Write("\n  all inserts succeeded");
            else
                Write("\n  at least one insert failed");
            db.showDB();
            WriteLine();

            "Test db of enumerable elements".title();
            WriteLine();

            DBElement<string, List<string>> newelem1 = new DBElement<string, List<string>>();
            newelem1.name = "newelem1";
            newelem1.descr = "test new type";
            newelem1.payload = new List<string> { "one", "two", "three" };

            DBElement<string, List<string>> newerelem1 = new DBElement<string, List<string>>();
            newerelem1.name = "newerelem1";
            newerelem1.descr = "better formatting";
            newerelem1.payload = new List<string> { "alpha", "beta", "gamma" };
            newerelem1.payload.Add("delta");
            newerelem1.payload.Add("epsilon");

            DBElement<string, List<string>> newerelem2 = new DBElement<string, List<string>>();
            newerelem2.name = "newerelem2";
            newerelem2.descr = "better formatting";
            newerelem2.children.AddRange(new List<string> { "first", "second" });
            newerelem2.payload = new List<string> { "a", "b", "c" };
            newerelem2.payload.Add("d");
            newerelem2.payload.Add("e");

            if (verbose)
            {
                Write("\n --- Test DBElement<string,List<string>> ---");
                WriteLine();
                newelem1.showEnumerableElement();
                WriteLine();
                newerelem1.showEnumerableElement();
                WriteLine();
                newerelem2.showEnumerableElement();
                WriteLine();
            }

            Write("\n --- Test DBEngine<string,DBElement<string,List<string>>> ---");

            int seed = 0;
            string skey = seed.ToString();
            Func<string> skeyGen = () =>
            {
                ++seed;
                skey = "string" + seed.ToString();
                skey = skey.GetHashCode().ToString();
                return skey;
            };

            DBEngine<string, DBElement<string, List<string>>> newdb =
              new DBEngine<string, DBElement<string, List<string>>>();
            newdb.insert(skeyGen(), newelem1);
            newdb.insert(skeyGen(), newerelem1);
            newdb.insert(skeyGen(), newerelem2);
            newdb.showEnumerableDB();
            Write("\n\n");
        }
    }
#endif
}

