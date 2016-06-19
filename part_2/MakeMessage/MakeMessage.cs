/////////////////////////////////////////////////////////////////////////
// MessageMaker.cs - Construct ICommService Messages                   //
// ver 1.0                                                             //
// Application: DCSE681-SMA, Project#4                                 //
// Language:    C#, ver 6.0, Visual Studio 2015                        //
// Platform:    Mac Book Pro, Core-i5, Windows 10                      //
// Source :     Jim Fawcett, CST 4-187, Syracuse University            //
// Author:      Saurabh Patel, MSCE Grad Student                       //
//              Syracuse University                                    //
//              (315) 751-3911, skpatel@syr.edu                        //
/////////////////////////////////////////////////////////////////////////
/*
 * Purpose:
 *----------
 * This is a placeholder for application specific message construction
 * In this package, we are constructing message template from xml input file.
 *
 * Additions to C# Console Wizard generated code:
 */
/*
* Maintenance:
* ------------
* Required Files: ICommService and Utilities
*
* Build Process:  devenv Project4.sln /Rebuild debug
*                 Run from Developer Command Prompt
*                 To find: search for developer
/*
* Maintenance History:
* --------------------
* ver 1.1 : 22 Nov 2015
* - first release
* ver 1.0 : 29 Oct 2015
* - first release
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Project4Starter
{
    public class MessageTemplate
    {
        public string messageType { get; set; }
        public int messageSize { get; set; }
        public string content { get; set; }

        /// <summary>
        /// Get message template list from input xml file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<MessageTemplate> GetMessageList(String fileName)
        {
            List<MessageTemplate> listOfMsgs = new List<MessageTemplate>();
            try
            {
                XDocument xdoc = new XDocument();
                xdoc = XDocument.Load(fileName);

                var elem = from c in xdoc.Descendants("queries") select c;

                foreach (var item in elem.Elements())
                {
                    MessageTemplate msg = new MessageTemplate();
                    msg.messageType = item.Attribute("type").Value;
                    foreach (var childs in item.Elements())
                    {
                        if (childs.Name.LocalName.Equals("size"))
                            msg.messageSize = Convert.ToInt32(childs.Value);
                        if (childs.Name.LocalName.Equals("messageContent"))
                            msg.content = childs.ToString();
                    }
                    listOfMsgs.Add(msg);
                }
            }
            catch (Exception e)
            {
                Console.Write("GetMessageList Exception : " + e.StackTrace);
            }
            return listOfMsgs;
        }
    }

  public class MessageMaker
  {
    public static int msgCount { get; set; } = 0;
    public Message makeMessage(string fromUrl, string toUrl)
    {
      Message msg = new Message();
      msg.fromUrl = fromUrl;
      msg.toUrl = toUrl;
      msg.content = String.Format("\n  message #{0}", ++msgCount);
      return msg;
    }
#if (TEST_MESSAGEMAKER)
    static void Main(string[] args)
    {
      MessageMaker mm = new MessageMaker();
      Message msg = mm.makeMessage("fromFoo", "toBar");
      Utilities.showMessage(msg);
      Console.Write("\n\n");
    }
#endif
  }
}
