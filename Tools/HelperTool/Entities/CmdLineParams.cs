using Plossum.CommandLine;

namespace UDHMHelperTool.Entities
{
    [CommandLineManager(ApplicationName = "HelperTool")]
    [CommandLineOptionGroup("commands", Name = "Commands", Require = OptionGroupRequirement.ExactlyOne)]
    [CommandLineOptionGroup("createOptions", Name = "CreateOptions", Require = OptionGroupRequirement.All)]
    [CommandLineOptionGroup("createSubOptions", Name = "CreateSubOptions", Require = OptionGroupRequirement.AtMostOne)]
    [CommandLineOptionGroup("optionalparams", Name = "OptionalParams", Require = OptionGroupRequirement.None)]
    public class CmdLineParams
    {
        [CommandLineOption(Name = "e", Aliases = "extract", GroupId = "commands", Description = "Extract file command")]
        public bool ExtractFile { get; set; }

        [CommandLineOption(Name = "zp", Aliases = "zippath", GroupId = "optionalparams", Description = "Directory of file")]
        public string ZipFilePath { get; set; }

        [CommandLineOption(Name = "des", Aliases = "destination", GroupId = "optionalparams", Description = "Destination of folder to be extracted")]
        public string DestinationPath { get; set; }

        [CommandLineOption(Name = "fl", Aliases = "filelist", GroupId = "createSubOptions", Description = "List of files(path of text or csv file)")]
        public string ListOfFiles { get; set; }
    }
}
