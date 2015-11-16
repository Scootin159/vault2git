using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;

namespace vault2git
{
    class Options
    {
        #region Vault Settings

        [Option("vault_path",
            DefaultValue = @"C:\Program Files (x86)\SourceGear\Vault Client\vault.exe",
            HelpText = "The path to the SourceGear Vault command line utility (vault.exe)")]
        public string VaultEXEPath { get; set; }

        [Option('s', "host", Required = true,
            HelpText = "Vault server hostname")]
        public string VaultHostname { get; set; }
        [Option("ssl", DefaultValue =false,
            HelpText ="Use SSL for the vault connection")]
        public bool VaultUseSSL { get; set; }
        [Option('u', "username", Required = true,
            HelpText = "Vault username")]
        public string VaultUsername { get; set; }
        [Option('p', "password", Required = true,
            HelpText = "Vault Password")]
        public string VaultPassword { get; set; }
        [Option('r', "repo", Required = true,
            HelpText = "The Vault repository name")]
        public string VaultRepoName { get; set; }
        [Option('i', "input", 
            DefaultValue = "$",
            HelpText = "The Vault respository path ($/source/path)")]
        public string VaultRepoPath { get; set; }

        [Option("start",
            DefaultValue = null,
            HelpText = "The Vault date to start converting at")]
        public string StartDate { get; set; }
        [Option("end",
            DefaultValue = null,
            HelpText = "The Vault date to end converting at")]
        public string EndDate { get; set; }

        #endregion

        #region Git Options

        [Option("git_path",
            DefaultValue ="git.exe",
            HelpText = "The path to the Git command-line tool")]
        public string GitEXEPath { get; set; }

        [Option("authors",
            DefaultValue = "authors.xml",
            HelpText = "The path to an XML mapping of Vault users to Git users")]
        public string GitAuthorsPath { get; set; }

        [Option('o', "output", Required = true,
            HelpText = "The Git output working directory")]
        public string GitWorkingPath { get; set; }

        #endregion

        #region Global Options

        [Option("logfile",
            DefaultValue ="log.txt",
            HelpText = "The log file path")]
        public string LogFilePath { get; set; }

        #endregion

        #region Public Helpers

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        #endregion
    }
}
