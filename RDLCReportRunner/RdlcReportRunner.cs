namespace RDLCReportRunner
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Reporting.WebForms;

    public class RdlcReportRunner
    {
        private const string FileExtension = "pdf";
        private string rdlcReportName;

        private IEnumerable<ReportDataSource> reportData;

        private IEnumerable<ReportParameter> reportParams; 

        public Dictionary<string, string> ReportParameters { get; set; }

        public string OutputPath { get; set; }

        public string ReportDataFile { get; set; }

        public string RdlcReportName { get; set; }

        public string OutputFileName
        {
            get { return rdlcReportName + "." + FileExtension; }
            set { rdlcReportName = value; }
        }

        public byte[] GenerateReport()
        {
            GetReportDataFromXml();
            GetReportParametersFromDictionary();

            return RunLocalReport();
        }

        private void GetReportParametersFromDictionary()
        {
            reportParams = ReportParameters.Select(parameter => new ReportParameter(parameter.Key, parameter.Value));
        }

        public async Task WriteReportToFile(byte[] renderedReportBytes)
        {
            var fullFilePath = Path.Combine(OutputPath, OutputFileName);
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }

            if (renderedReportBytes != null)
            {
                using (var fs = new FileStream(fullFilePath, FileMode.Create))
                {
                    await fs.WriteAsync(renderedReportBytes, 0, renderedReportBytes.Length);
                }
            }
        }

        private void GetReportDataFromXml()
        {
            var ds = new DataSet();
            
            if (!string.IsNullOrEmpty(ReportDataFile))
            {
                ds.ReadXml(ReportDataFile);
            }

            if (ds.Tables.Count == 0)
            {
                return;
            }

            reportData = (from DataTable dt in ds.Tables select new ReportDataSource(dt.TableName, dt)).ToList();
        }

       
        private static string GetReportDeviceInfo()
        {
            //// The DeviceInfo settings should be changed based on the reportType
            //// http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            return "<DeviceInfo><OutputFormat>PDF</OutputFormat></DeviceInfo>";
        }

        private byte[] RunLocalReport()
        {
            using (var localReport = new LocalReport { ReportPath = RdlcReportName })
            {
                byte[] renderedBytes;
                try
                {
                    using (var textReader = new StreamReader(RdlcReportName))
                    {
                        localReport.LoadReportDefinition(textReader);
                    }

                    if (reportData != null)
                    {
                        foreach (var ds in reportData)
                        {
                            localReport.DataSources.Add(ds);
                        }
                    }

                    if (reportParams != null && reportParams.Any())
                    {
                        localReport.SetParameters(reportParams);
                    }

                    var deviceInfo = GetReportDeviceInfo();

                    localReport.Refresh();

                    string mimeType;
                    string encoding;
                    string fileNameExtension;
                    Warning[] warnings;
                    string[] streams;
                    renderedBytes = localReport.Render(
                        "PDF",
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} and {1}", e.Message, e.InnerException.InnerException.Message);
                    return null;
                }

                return renderedBytes;
            }
        }
    }
}