using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vault2git
{
    class Program
    {
        public static Options options = new Options();

        static void Main(string[] args)
        {
            if (!CommandLine.Parser.Default.ParseArguments(args, options)) { return; }

            using (Log log = new Log())
            {
                Log.Info(string.Format("Starting processing of {0}", options.VaultRepoPath));

                Authors.ParseAuthorsFile();
                Git.InitWorkingDirectory();
                Vault.SetWorkingDirectory();

                Version[] versions = Vault.GetVersionHistory();
                Label[] labels = Vault.GetLabelHistory();

                int last_label_written = -1;
                for(int i = 0; i < versions.Length; i++)
                {
                    Version version = versions[i];

                    #region Create any labels dated before this version

                    while ((last_label_written + 1) < labels.Length)
                    {
                        Label label = labels[last_label_written + 1];
                        if (label.date < version.date)
                        {
                            Git.CreateTag(label);
                            last_label_written++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    #endregion

                    Log.Info(string.Format("Processing version {0} of {1}", i, versions.Length));
                    CleanWorkingDirectory();
                    Vault.GetVersion(version);
                    Git.CommitVersion(version);
                }

                #region Create any labels dated after the last version

                while ((last_label_written + 1) < labels.Length)
                {
                    Label label = labels[last_label_written + 1];
                    Git.CreateTag(label);
                    last_label_written++;
                }

                #endregion

                Log.Info("Processing complete");
            }
        }

        static void CleanWorkingDirectory()
        {
            foreach (string directory in Directory.GetDirectories(options.GitWorkingPath))
            {
                if (directory.Equals(Path.Combine(options.GitWorkingPath, ".git"))) { continue; }
                Log.Debug("Deleting directory " + directory);
                Directory.Delete(directory, true);
            }

            foreach (string file in Directory.GetFiles(options.GitWorkingPath))
            {
                if (file.Equals(Path.Combine(options.GitWorkingPath, ".gitignore"))) { continue; }
                Log.Debug("Deleting file " + file);
                File.Delete(file);
            }
        }
    }
}
