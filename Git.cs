using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vault2git
{
    class Git
    {
        public static void InitWorkingDirectory()
        {
            Log.Info("Initializing working directory");

            Directory.CreateDirectory(Program.options.GitWorkingPath);

            RunCommand("init", null, Program.options.GitWorkingPath);

            string gitignorefile = Path.Combine(Program.options.GitWorkingPath, ".gitignore");
            if (!File.Exists(gitignorefile))
            {
                Log.Info("Writing a default .gitignore file");
                using (StreamWriter sw = new StreamWriter(gitignorefile))
                {
                    sw.WriteLine("*.vspscc");
                    sw.WriteLine("*.vssscc");
                }
            }
        }

        public static void CommitVersion(Version version)
        {
            Log.Info(string.Format("Committing version #{0}", version.version));

            RunCommand("add", version.date, "--all", ".");

            List<string> commit_args = new List<string>();
            if (!string.IsNullOrWhiteSpace(version.comment))
            {
                commit_args.Add("-m");
                commit_args.Add("\"" + version.comment.Replace("\\", "\\\\") + "\"");
            }
            commit_args.Add("-m");
            commit_args.Add(string.Format(
                "\"Original Vault Commit: version {0} on {1:yyyy-MM-dd HH:mm:ss} by {2}\"",
                version.version,
                version.date,
                version.user));
            commit_args.Add(string.Format("--date=\"{0:yyyy-MM-ddTHH:mm:ss}\"", version.date));
            if (version.GitAuthor != null)
            {
                commit_args.Add(string.Format("--author=\"{0}\"", version.GitAuthor));
            }
            commit_args.Add("-a");
            RunCommand("commit", version.date, commit_args.ToArray());
        }

        public static void CreateTag(Label label)
        {
            Log.Info(string.Format("Creating Tag for label {0}", label.actionString));

            RunCommand("tag", label.date,
                "-a", label.Tag,
                "-m", "\"" + label.actionString.Replace("\\", "\\\\") + "\"");
        }

        private static void RunCommand(string command, DateTime? date, params string[] args)
        {
            StringBuilder arguments = new StringBuilder();
            arguments.Append(command + " ");

            foreach (string arg in args)
            {
                arguments.Append(arg + " ");
            }

            Log.Debug(string.Format("Running Git: {0}", arguments.ToString()));

            Process process = new Process();
            process.StartInfo.FileName = Program.options.GitEXEPath;
            process.StartInfo.Arguments = arguments.ToString();
            process.StartInfo.WorkingDirectory = Program.options.GitWorkingPath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            if (date.HasValue)
            {
                process.StartInfo.EnvironmentVariables.Add("GIT_COMMITTER_DATE", date.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
            }
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();
            process.Close();

            if (!string.IsNullOrWhiteSpace(output))
            {
                Log.Debug(string.Format("Git output: \r\n{0}", output));
            }
            if (!string.IsNullOrWhiteSpace(error))
            {
                Log.Debug(string.Format("Git error: \r\n{0}", error));
            }
        }
    }
}
