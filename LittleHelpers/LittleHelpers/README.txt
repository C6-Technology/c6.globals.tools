Release Notes:

Features in V1

The importer handles the following CSV specific 'issues'
1)	__,__
2)	"__","__"				
3)	"__",__
4)	__,"__"
5)	"_""_""_"				"" escaped within a "__"
6)	""")					"" escaped with left or right side
6.1) "("")					(left side "" escaped)
6.2) ("")"					(right side "" escaped)
7)	"_"",""_"				escaped "" with comma included
7.1) "__""__,__""__"
8) Multiline records including columns spread over multiple lines

Note(s): 
- _ ~ Space or Text

Known bug(s):
- Under certain circumstances, some rows are not correctly imported - usually this happens if the headline doesn't exist. Also within such event, it may be that the class completely stops (see Roadmap: Add correct errors and error handling)!
- Error messages are not always very helpful
- the headline is not delivered if the import happens and NO table is created.

Planned Roadmap / Next Features:
- Add the ability to define columns
	- This should include spanning of columns
	- Implementation will allow the definition similiar to HTML: <td name="ID"><td name="Currency" colspan="2">
		- If a file, with the same name like the CSV but extension: .CSVHeader exists, the importer should take it automatically. If the file exists and the definition has been 
		  handover to importer, both have to be identically or an error should be raised.

- Add correct errors and error handling

- Remove all hacks

- Evaluate and result depending implement StringBuilder instead of current string handling.

- Throw row error and go on with importing, export the 'defect' row to a new file, called CSVName + _ImportErrors.csv and altenatively, deliver the import error via Event.
	- consequentely, allow to avoid error throwing in favour to event throwing.

- Create a string array or events with a given string template, e.g. a SQL String like INSERT INTO table (column1, column2) VALUES(%column1%, %column2%); were %column1% and %column2%
  are replaced by the importer.

- Implement V1 as Excel 2011 add-in
	- later on, make the Excel 2011 add-in a stand alone project
	- implement complete error handling for the add-in

Current version:
STATUS: in between

	a) ExcelTools development went in the wrong direction. This is stored as "archive" and another way of implementing the needed functionality starts. - rework in progress

	b) Refactored the whole development and redid LittleHelpers class in terms of consequently implementing everything, especially the CSVReader as sub class. Also renamed the method names in CSVReader to make their functionality clearer. - done

	c) Excel AddIn: Designed. Functionality is developed, that's why ExcelTools comes in place and are currently under development.

	d) Excel AddIn: The AddIn does work stable but error handling is not yet implemented completely.

	e) changed Headline handling to OrderedDictionary to ensure the correct sequence of headlines.

	ALL OVER STATUS. The CSVReader itself would work with known bugs, the rest is in between!