using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace vault2git
{
    class Vault
    {
        public static void SetWorkingDirectory()
        {
            Log.Info("Set Vault working folder");
            RunCommand("setworkingfolder",
                "\"" + Program.options.VaultRepoPath + "\"",
                "\"" + Program.options.GitWorkingPath + "\"");
        }

        public static Version[] GetVersionHistory()
        {
            Log.Info("Getting Vault version history");

            List<string> args = new List<string>();
            if(!string.IsNullOrWhiteSpace(Program.options.StartDate))
            {
                args.Add("-begindate");
                args.Add("\"" + Program.options.StartDate + "\"");
            }
            if (!string.IsNullOrWhiteSpace(Program.options.EndDate))
            {
                args.Add("-enddate");
                args.Add("\"" + Program.options.EndDate + "\"");
            }
            args.Add("-rowlimit"); args.Add("0");
            args.Add("\"" + Program.options.VaultRepoPath + "\"");
            string output = RunCommand("versionhistory", args.ToArray());

            List<Version> versions = new List<Version>();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(output);

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("//vault/history/item"))
            {
                if(node.NodeType != XmlNodeType.Element) { continue; }
                XmlElement element = node as XmlElement;
                if(element == null) { continue; }

                Version version = new Version();
                version.txid = int.Parse(element.Attributes["txid"].Value);
                version.version = int.Parse(element.Attributes["version"].Value);
                version.date = DateTime.Parse(element.Attributes["date"].Value);
                version.user = element.Attributes["user"].Value;
                version.comment = element.HasAttribute("comment") ? element.Attributes["comment"].Value : null;
                versions.Add(version);
            }

            // sort the versions numerically
            versions.Sort(new Comparison<Version>(delegate (Version a, Version b)
            {
                return a.version.CompareTo(b.version);
            }));

            Log.Info(string.Format("{0} versions loaded", versions.Count));

            return versions.ToArray();
        }

        public static int[] GetComplexChangeTxids()
        {
            Log.Info("Getting Vault complex changes history");

            List<string> args = new List<string>();
            if (!string.IsNullOrWhiteSpace(Program.options.StartDate))
            {
                args.Add("-begindate");
                args.Add("\"" + Program.options.StartDate + "\"");
            }
            if (!string.IsNullOrWhiteSpace(Program.options.EndDate))
            {
                args.Add("-enddate");
                args.Add("\"" + Program.options.EndDate + "\"");
            }
            args.Add(" -rowlimit"); args.Add("0");
            args.Add(" -datesort"); args.Add("asc");
            args.Add(" -includeactions"); args.Add("delete,move,rename,rollback,share");
            args.Add("\"" + Program.options.VaultRepoPath + "\"");
            string output = RunCommand("history", args.ToArray());

            List<int> txids = new List<int>();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(output);

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("//vault/history/item"))
            {
                if (node.NodeType != XmlNodeType.Element) { continue; }
                XmlElement element = node as XmlElement;
                if (element == null) { continue; }

                int txid = int.Parse(element.Attributes["txid"].Value);
                if (!txids.Contains(txid))
                {
                    txids.Add(txid);
                }
            }

            Log.Info(string.Format("{0} complex change txids loaded", txids.Count));

            return txids.ToArray();
        }

        public static Label[] GetLabelHistory()
        {
            Log.Info("Getting Vault label history");

            List<string> args = new List<string>();
            if (!string.IsNullOrWhiteSpace(Program.options.StartDate))
            {
                args.Add("-begindate");
                args.Add("\"" + Program.options.StartDate + "\"");
            }
            if (!string.IsNullOrWhiteSpace(Program.options.EndDate))
            {
                args.Add("-enddate");
                args.Add("\"" + Program.options.EndDate + "\"");
            }
            args.Add(" -rowlimit"); args.Add("0");
            args.Add(" -datesort"); args.Add("asc");
            args.Add(" -includeactions"); args.Add("label");
            args.Add("\"" + Program.options.VaultRepoPath + "\"");
            string output = RunCommand("history", args.ToArray());

            List<Label> labels = new List<Label>();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(output);

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("//vault/history/item"))
            {
                if (node.NodeType != XmlNodeType.Element) { continue; }
                XmlElement element = node as XmlElement;
                if (element == null) { continue; }

                Label label = new Label();
                label.date = DateTime.Parse(element.Attributes["date"].Value);
                label.actionString = element.Attributes["actionString"].Value;
                labels.Add(label);
            }

            Log.Info(string.Format("{0} labels loaded", labels.Count));

            return labels.ToArray();
        }

        public static void GetVersion(Version version)
        {
            Log.Info(string.Format("Getting version #{0}", version.version));
            RunCommand("getversion",
                "-backup", "no",
                "-merge", "overwrite",
                "-setfiletime", "checkin",
                "-makewritable",
                "-useworkingfolder",
                version.version.ToString(),
                "\"" + Program.options.VaultRepoPath + "\"",
                "\"" + Program.options.GitWorkingPath + "\"");
        }

        private static string RunCommand(string command, params string[] args)
        {
            StringBuilder arguments = new StringBuilder();
            arguments.Append(command + " ");

            arguments.Append("-host " + Program.options.VaultHostname + " ");
            if (Program.options.VaultUseSSL) { arguments.Append("-ssl "); }
            arguments.Append("-username " + Program.options.VaultUsername + " ");
            arguments.Append("-password " + Program.options.VaultPassword + " ");
            arguments.Append("-repository \"" + Program.options.VaultRepoName + "\" ");

            foreach (string arg in args)
            {
                arguments.Append(arg + " ");
            }

            Log.Debug(string.Format("Running Vault: {0}", arguments.ToString()));

            Process process = new Process();
            process.StartInfo.FileName = Program.options.VaultEXEPath;
            process.StartInfo.Arguments = arguments.ToString();
            process.StartInfo.WorkingDirectory = Program.options.GitWorkingPath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
            process.Close();

            Log.Debug(string.Format("Vault output: \r\n{0}", output));

            return output;
        }
    }
}
