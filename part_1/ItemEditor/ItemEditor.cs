///////////////////////////////////////////////////////////////////////
// ItemEditor.cs - Edit item of dictionary in terms of name,        //
//                 description, time and other metadata items.      //
// Ver 1.2                                                          //
// Application: Demonstration for CSE681-SMA, Project#2             //
// Language:    C#, ver 6.0, Visual Studio 2015                     //
// Platform:    Dell Inspiron 14z, Core-5, Windows 10               // 
// Author:      Saurabh Patel, MSCE Graduate Student,              //
//              Syracuse University                                //
//              (315) 751-3911, skpatel@syr.edu                    //
/////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package provides functions to edit any key,value element pair from in-memory database.
 * 
 * Public Interface:
 * ==============================
 * clone function will create clone of element before do any changes.
        public void clone()
 * This function will edit name in item's metadata. New or update name will be provide in function argument.
        public bool editByName(ref DBEngine<Key, DBElement<Key, ListOfStrings>> dbEngine, Key key, String name)
 * This function will edit time in item's metadata. New time will be provide in function argument.
        public bool editByTime(ref DBEngine<Key, DBElement<Key, ListOfStrings>> dbEngine, Key key, DateTime newTime)
 * This function will edit description in item's metadata. New description will be provide in function argument.
        public bool editByDescr(ref DBEngine<Key, DBElement<Key, ListOfStrings>> dbEngine, Key key, String descr)
 * This function will edit children details in item's metadata. New description will be provide in function argument.
        public bool editByChild(ref DBEngine<Key, DBElement<Key, ListOfStrings>> dbEngine, Key key, List<Key> children)
 * This function will edit children index  details in item's metadata.
        public bool editByChildIndex(ref DBEngine<Key, DBElement<Key, ListOfStrings>> dbEngine, Key key, int index, Key childKey)
 * This function will edit payload details in metadata of item.
        public bool editPayloadByListOfString(ref DBEngine<Key, DBElement<Key, ListOfStrings>> dbEngine, Key key, List<string> payload)
 *
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs, and
 *                 Payloadwrapper.cs
 *                 UtilityExtensions.cs , Display only if you enable the test stub
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

namespace Project2Starter
{
    public class ItemEditor<Key, Value>
    {
        // db element which is going to change.
        private DBElement<Key, ListOfStrings> dbElement;
        // cloned element of input element.
        private DBElement<Key, ListOfStrings> dbElementCloned;

        // clone function will create clone of element before do any changes.
        public void clone()
        {
            dbElementCloned = new DBElement<Key, ListOfStrings>();
            dbElementCloned.name = String.Copy(dbElement.name);
            dbElementCloned.descr = String.Copy(dbElement.descr);
            dbElementCloned.timeStamp = DateTime.Now;
            dbElementCloned.children = new List<Key>();
            dbElementCloned.children.AddRange(dbElement.children);
            dbElementCloned.payload = dbElement.payload.clone() as ListOfStrings;

        }

        // This function will edit name in item's metadata. New or update name will be provide in function argument.
        public bool editByName(ref DBEngine<Key, DBElement<Key, ListOfStrings>> dbEngine, Key key, String name)
        {

            if (dbEngine.Dictionary.Keys.Contains(key))
            {
                dbElement = dbEngine.Dictionary[key];
                clone();
                dbElementCloned.name = name;
                editDictionary(ref dbEngine, key);
                return true;
            }
            else return false;
        }

        // This function will edit time in item's metadata. New time will be provide in function argument.
        public bool editByTime(ref DBEngine<Key, DBElement<Key, ListOfStrings>> dbEngine, Key key, DateTime newTime)
        {

            if (dbEngine.Dictionary.Keys.Contains(key))
            {
                dbElement = dbEngine.Dictionary[key];
                clone();
                dbElementCloned.timeStamp = newTime;
                editDictionary(ref dbEngine, key);
                return true;
            }
            else return false;
        }

        // This function will edit description in item's metadata. New description will be provide in function argument.
        public bool editByDescr(ref DBEngine<Key, DBElement<Key, ListOfStrings>> dbEngine, Key key, String descr)
        {

            if (dbEngine.Dictionary.Keys.Contains(key))
            {
                dbElement = dbEngine.Dictionary[key];
                clone();
                dbElementCloned.descr = descr;
                editDictionary(ref dbEngine, key);
                return true;
            }
            else return false;
        }

        // This function will edit children details in item's metadata. New description will be provide in function argument.
        public bool editByChild(ref DBEngine<Key, DBElement<Key, ListOfStrings>> dbEngine, Key key, List<Key> children)
        {

            if (dbEngine.Dictionary.Keys.Contains(key))
            {
                dbElement = dbEngine.Dictionary[key];
                clone();
                dbElementCloned.children = children;
                editDictionary(ref dbEngine, key);
                return true;
            }
            else return false;
        }

        // This function will edit children index  details in item's metadata.
        public bool editByChildIndex(ref DBEngine<Key, DBElement<Key, ListOfStrings>> dbEngine, Key key, int index, Key childKey)
        {

            if (dbEngine.Dictionary.Keys.Contains(key))
            {
                dbElement = dbEngine.Dictionary[key];
                clone();
                List<Key> childrens = dbElementCloned.children;
                if (index < childrens.Count)
                {
                    childrens[index - 1] = childKey;
                    dbElement.children = childrens;
                    editDictionary(ref dbEngine, key);
                    return true;
                }
                else return false;
            }
            else return false;
        }


        // This function will edit payload details in metadata of item.
        public bool editPayloadByListOfString(ref DBEngine<Key, DBElement<Key, ListOfStrings>> dbEngine, Key key, List<string> payload)
        {
            if (dbEngine.Dictionary.Keys.Contains(key))
            {
                dbElement = dbEngine.Dictionary[key];
                clone();
                dbElementCloned.payload.theWrappedData = payload;
                editDictionary(ref dbEngine, key);
                return true;
            }
            else return false;
        }

        // This function will updated new updated things in main dictionary.
        private void editDictionary(ref DBEngine<Key, DBElement<Key, ListOfStrings>> dbEngine, Key key)
        {
            dbEngine.Dictionary[key] = dbElementCloned;
        }

    }

#if (TEST_ITEMEDITOR)
    class TestItemEditor
    {
        static void Main(string[] args)
        {
            "Testing ItemEditor Package".title('=');
            Console.WriteLine();
            DBEngine<int, DBElement<int, ListOfStrings>> db1 = new DBEngine<int, DBElement<int, ListOfStrings>>();
            ItemEditor<int, string> itemEditor = new ItemEditor<int, string>();
            "For key = 1, metadata before editing is: ".title('-');
            DBElement<int, ListOfStrings> elem1 = new DBElement<int, ListOfStrings>();
            elem1.name = "1st Key/Value = Int/ListOfStrings";
            elem1.descr = "This is first element of Int/ListOfStrings key value pair.";
            elem1.timeStamp = DateTime.Now;
            elem1.children.AddRange(new List<int> { 100, 101, 102, 103, 104, 105 });
            elem1.payload = new ListOfStrings();
            elem1.payload.theWrappedData = new List<string> { "CSE681", "SMA", "C#.net", "AI" };
            db1.insert(1, elem1);
            db1.showDB();

            Console.WriteLine("\n\n");
            "For key = 1, metadata after editing is: ".title('-');
            itemEditor.editByName(ref db1, 1, "NewX");
            itemEditor.editByDescr(ref db1, 1, "NewDescription");
            db1.showDB();
            Console.WriteLine("\n\n");
        }
    }
#endif
}
