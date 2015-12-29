using UDHMHelperTool.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace UDHMHelperTool.BusinessLogic
{
    internal static class ZipFileExtractor
    {
        /// <summary>
        /// Extracts the list of files given in csv from zip files to a destination folder
        /// </summary>
        /// <param name="cmdLineParams"></param>
        public static bool ExtractFilesFromZipFiles(CmdLineParams cmdLineParams)
        {
            string fileContent = string.Empty;

            string destinationFilePath = cmdLineParams.DestinationPath;
            if (string.IsNullOrEmpty(destinationFilePath))
            {
                Console.WriteLine("Destination Path not passed.Getting from the config file");
                destinationFilePath = ConfigurationController.Default.DestinationPath;
                if (!Directory.Exists(destinationFilePath))
                    Directory.CreateDirectory(destinationFilePath);
                Console.WriteLine("New Directory {0} Created", destinationFilePath);
            }
            if (string.IsNullOrEmpty(cmdLineParams.ListOfFiles))
                fileContent = File.ReadAllText(ConfigurationController.Default.FileListPath);
            else
                fileContent = File.ReadAllText(cmdLineParams.ListOfFiles);
            if (string.IsNullOrEmpty(fileContent))
            {
                Console.WriteLine("File Content is empty...Press Any Key to Continue");
                Console.ReadKey();
                return false;
            }
            string[] listOfFiles = fileContent.Split(',');
            List<string> filesToBeExtracted = new List<string>();
            filesToBeExtracted.AddRange(listOfFiles);

            string ZipFilePath = string.Empty;
            if (string.IsNullOrEmpty(cmdLineParams.ZipFilePath))
                ZipFilePath = ConfigurationController.Default.ZipPath;
            else
                ZipFilePath = cmdLineParams.ZipFilePath;
            string[] zipFiles = Directory.GetFiles(cmdLineParams.ZipFilePath, "*.zip", SearchOption.AllDirectories);
            var stopWatch = Stopwatch.StartNew();
            if (zipFiles != null)
            {
                foreach (string zipFile in zipFiles)
                {
                    using (var archive = ZipFile.OpenRead(zipFile))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            if (!string.IsNullOrEmpty(entry.Name))
                            {
                                if (IsFilePresent(filesToBeExtracted, entry.Name))
                                {
                                    if (!Directory.Exists(destinationFilePath))
                                    {
                                        Console.WriteLine("{0} does not exists... Creating {0}", destinationFilePath);
                                        Directory.CreateDirectory(destinationFilePath);
                                    }
                                    Console.Write("Extracting file {0} to {1}", entry.Name, destinationFilePath);
                                    stopWatch.Restart();
                                    entry.ExtractToFile(Path.Combine(destinationFilePath, entry.Name), true);
                                    stopWatch.Stop();
                                    Console.WriteLine("...Done in :{0}", stopWatch.Elapsed);
                                    filesToBeExtracted.Remove(entry.Name);
                                }
                            }


                        }
                    }
                }
            }
            else
            {
                Console.Error.Write("Not Zip Files found to be extracted");
                return false;
            }
            Console.WriteLine("Extraction Successful\nPress Any key to exit");
            Console.ReadKey();
            return true;
        }

        /// <summary>
        /// Checks whether a filename is present in the list of filename
        /// </summary>
        /// <param name="listOfFiles"> List of filenames</param>
        /// <param name="filename">Filename</param>
        /// <returns></returns>
        private static bool IsFilePresent(List<string> listOfFiles, string filename)
        {
            try
            {
                foreach (string file in listOfFiles)
                {
                    if (file.Contains(filename) || filename.Contains(file))
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error Occurred file Checking file name {0}", ex.Message);
                return false;
            }
        }
    }
}
