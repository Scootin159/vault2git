using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace vault2git
{
    class Authors
    {
        static Dictionary<string, string> authors = new Dictionary<string, string>();

        public static void ParseAuthorsFile()
        {
            if (File.Exists(Program.options.GitAuthorsPath))
            {
                Log.Debug(string.Format("Parsing authors file {0}", Program.options.GitAuthorsPath));

                XmlDocument xml = new XmlDocument();
                xml.Load(Program.options.GitAuthorsPath);

                foreach (XmlElement element in xml.GetElementsByTagName("author"))
                {
                    string vaultname = element.Attributes["vaultname"].Value;
                    string gitname = element.Attributes["name"].Value
                        + " <"
                        + element.Attributes["email"].Value
                        + ">";

                    Log.Debug(string.Format("Author: {0} = {1}", vaultname, gitname));
                    authors.Add(vaultname.ToLower(), gitname);
                }

                Log.Info(string.Format("{0} authors parsed", authors.Count));
            }
            else
            {
                Log.Debug(string.Format("Authors file {0} not found", Program.options.GitAuthorsPath));
            }
        }

        public static string GetGitAuthor(string vault_user)
        {
            vault_user = vault_user.ToLower();
            return authors.ContainsKey(vault_user) ? authors[vault_user] : null;
        }
    }
}
