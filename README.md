RDLCReportRunner
================

A simple RDLC report runner library. A handy little tool to own if you ever want to create pdf files on the fly on your application.
Unless you want to hand roll your pdf object using some of the open source or paid for 3rd party libraries, the easiest way to create
a pdf document is to use and RDLC report.

I know, I know, RDLC sucks, it is old and ugly. But it is free and easy to use once you get the hang of it. The designer is bad (kinda
like an Access Form) and there are not many options, but it can get the job done.

So how do you use the RDLCReportRunner class?

###Instantiate the RDLC Report Runner###

```
var rdlcReportRunner = new RdlcReportRunner
   {
       OutputPath = @"C:\Temp", // the output directory
       OutputFileName = "TestReportOutput", // the output file name - no extension is required
       ReportDataFile = null, // an xml file with the report datasources
       RdlcReportName = @"TestRdlc.rdlc",  // the actual RDLC report to render with the data
       ReportParameters = paramDictionary // key value pairs with parameter name and parameter value
};
```
The comments next to each parameter are self explanatory. To populate the data in the report you can provide a ReportDataFile, ReportParameters or both. Of course, you will need to define the appropriate datasource and parameters in the rdlc file itself, but as long as the xml data file and the parameter names match, you are good to go

###Run the report###

```
var reportData = rdlcReportRunner.GenerateReport();
```
The `GenerateReport()` is where the magic happens. The method parses the data and popuplates the report. Expect an exception if there is an error. RDLC reports are very temperamental and will throw errors for everything. There is no margin for error, so make sure everything is in order. Hopefully, the error message you help you out in case of an error, just remember you've been warned. 

This methods returns a byte[]. This is handy if you want to stream the report back to the user from a browswer etc. 

###Export the report to the file system###
Finally, if you want to get your hands on the precious .pdf file, you can run the `WriteReportToFile()` method as per the example below:

```
rdlcReportRunner.WriteReportToFile(reportData).Wait();
```
You need to pass the byte[] that we got earlier and the full report path will be: **OutputPath + OutputFileName.pdf**. If you have been paying attention, this is an async call as we interact with the file system and the write operation is async as well. It's down to you if you want to call this synchronously or asynchronously.

###Sample Code###
You can download and run the project. The solution contains a small console app that executes the code I've shown you above and generate a simple RDLC report with a "Hello world" message.

Feel free to email or raise any issues if you come across a problem
