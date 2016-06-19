///////////////////////////////////////////////////////////////
// DBElement.cs - Define element for noSQL database          //
// Ver 1.2                                                   //
// Application: Demonstration for CSE681-SMA, Project#2      //
// Language:    C#, ver 6.0, Visual Studio 2015              //
// Platform:    Dell XPS2700, Core-i7, Windows 10            //
// Author:      Jim Fawcett, CST 4-187, Syracuse University  //
//              (315) 443-3948, jfawcett@twcny.rr.com        //
///////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements the DBElement<Key, Data> type, used by 
 * DBEngine<key, Value> where Value is DBElement<Key, Data>.
 *
 * The DBElement<Key, Data> state consists of metadata and an
 * instance of the Data type.
 *
 * I intend this DBElement type to be used by both:
 *
 *   ItemFactory - used to ensure that all db elements have the
 *                 same structure even if built by different
 *                 software parts.
 *   ItemEditor  - used to ensure that db elements are edited
 *                 correctly and maintain the intended structure.
 *
 *   The Factory and Edit classes can be quite simple, I think,
 *   if you use the DBElement class.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBElement.cs, UtilityExtensions.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.1 : 24 Sep 15
 * - removed extension methods, removed tests from test stub
 * - Testing now  uses DBEngineTest.cs
 * ver 1.0 : 13 Sep 15
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
  /////////////////////////////////////////////////////////////////////
  // DBElement<Key, Data> class
  // - Instances of this class are the "values" in our key/value 
  //   noSQL database.
  // - Key and Data are unspecified classes, to be supplied by the
  //   application that uses the noSQL database.
  //   See the teststub below for examples of use.

  public class DBElement<Key, Data>
  {
    public string name { get; set; }          // metadata    |
    public string descr { get; set; }         // metadata    |
    public DateTime timeStamp { get; set; }   // metadata   value
    public List<Key> children { get; set; }   // metadata    |
    public Data payload { get; set; }         // data        |

    public DBElement(string Name = "unnamed", string Descr = "undescribed")
    {
      name = Name;
      descr = Descr;
      timeStamp = DateTime.Now;
      children = new List<Key>();
    }
  }


    public class RepoDBElement<Key, Data>
    {
        public string revisionNumber { get; set; }     // metadata    |
        public string filePath { get; set; }         // metadata    |
        public string fileOwnerName { get; set; }   // metadata    |
        public long fileSize { get; set; }         // metadata    |
        public bool isLocked { get; set; }         // metadata    |
        public DateTime timeStamp { get; set; }   // metadata   value
        public List<Key> dependentFiles { get; set; }   // metadata    |
        public string moduleName { get; set; }   // metadata    |
    }

    public class BuildServerDBElement<Key, Data>
    {
        public string buildImagPath { get; set; }     // build image path where build image generated and stored on file server|
        public string buildImagecheckSum { get; set; }  // build image checksum |
        public int buildImageFileSize { get; set; }   // build image file size |
        public string buildReportFilePath { get; set; } // filepath where build report stores |
        public string buildLoggerFile { get; set; } // filepath where logger file resides |
        public string moduleName { get; set; }   // module Name
        public string moduleVersionNumber { get; set; }   // module Version Number    |
        public DateTime timestamp { get; set; }   // timestamp    |
    }

    public class TestHarnessServerDBElement<Key, Data>
    {
        public string buildImagPath { get; set; }     // build image path where build image generated and stored on file server|
        public string testSuiteFile { get; set; }  // test suite file |
        public int buildImageFileSize { get; set; }   // build image file size |
        public string testReportFilePath { get; set; } // filepath of test summary report |
        public string testSummary { get; set; } // High level test summary |
        public string loggerFile { get; set; }   // test process logs
        public DateTime timestamp { get; set; }   // timestamp    |
    }

    public class CollServerDBElement<Key, Data>
    {
        public string documentName { get; set; }         // document name
        public string workDescription { get; set; }         // brief description of document
        public DateTime timeCreated { get; set; } // time at which document is created originally
        public long numberOfHours { get; set; }       // size of document in bytes
        public string workPackageAssignedTo { get; set; }  // work package assign information
        public string moduleName { get; set; }      //module Name
    }

    public class TestSuiteDBElement<Key, Data>
    {
        public string testSuiteFile { get; set; }  // test suite file |
    }





#if (TEST_DBELEMENT)


    class TestDBElement
  {
    static void Main(string[] args)
    {
      "Testing DBElement Package".title('=');
      WriteLine();

      Write("\n  All testing of DBElement class moved to DBElementTest package.");
      Write("\n  This allow use of DBExtensions package without circular dependencies.");

      Write("\n\n");
    }
  }
#endif
}
