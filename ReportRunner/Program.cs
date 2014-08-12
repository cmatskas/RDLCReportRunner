namespace ReportRunner
{
    using System;
    using System.Collections.Generic;
    using RDLCReportRunner;

    public class Program
    {
        public static void Main(string[] args)
        {
            var paramDictionary = new Dictionary<string, string> {{"param1", "Hello"}, {"param2", "back!"}};

            var rdlcReportRunner = new RdlcReportRunner
            {
                OutputPath = @"C:\Temp", // the output directory
                OutputFileName = "TestReportOutput", // the output file name - no extension is required
                ReportDataFile = null, // an xml file with the report datasources
                RdlcReportName = @"TestRdlc.rdlc",  // the actual RDLC report to render with the data
                ReportParameters = paramDictionary // key value pairs with parameter name and parameter value
            };

            var reportData = rdlcReportRunner.GenerateReport(); // run the report and return the bytes[] - can be used on a website to return a stream to the user
            rdlcReportRunner.WriteReportToFile(reportData).Wait(); // write the report to an actual file on the local directory

            Console.WriteLine("Report run completed.");
            Console.ReadKey();
        }
    }
}
