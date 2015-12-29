using Plossum.CommandLine;
using System;
using UDHMHelperTool.BusinessLogic;
using UDHMHelperTool.Entities;
namespace UDHMHelperTool
{
    class Program
    {
        private static int Main(string[] args)
        {
            var cmdLineParams = new CmdLineParams();

            try
            {
                using (var parser = new CommandLineParser(cmdLineParams))
                {
                    parser.Parse();
                    if (parser.HasErrors)
                    {
                        Console.WriteLine(parser.UsageInfo.GetErrorsAsString(78));
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return -1;
            }
            try
            {

                if (cmdLineParams.ExtractFile)
                {
                    bool result = ZipFileExtractor.ExtractFilesFromZipFiles(cmdLineParams);
                    if (result)
                        return 0;
                    else
                        return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Occured {0}", ex.Message);
            }
            return 0;
        }
    }
}
