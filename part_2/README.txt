ORDER OF EXECUTION
==================

BUILDING/GENERATING
-------------------------------------------------------------------------------------------
STEP1 - compile.bat 	// used to compile
STEP2 - run.bat	// will run the executive.(demonstrates the requirements)

-----------------------------------------------EOF-----------------------------------------



Notes: 

1) Persist xml file will be store in root folder.
2) I have used <string, List<string> to demonstrate the requirements.
3) You can add number of reader and number of writer clients from command line arguments of test executive.
	For Example : 3 3 Y
	
	Here, first argument is Number of reader clients
		  second argument is Number of writer clients
		  third argument is Yes or No to enable logs for clients and server.

4) There are two xml files for reader and writer clients, these clients will read 
   the content of several different messages from an XML file, read at client startup. 
   
   Reader Client : "XMLReader.xml"
   Writer Client : "XMLWriter.xml"
   
   You can change number of messages, as per given message size clients will send messages to server.

5) Please check performance tab in GUI for performance details.
 
Saurabh Patel
skpatel@syr.edu
