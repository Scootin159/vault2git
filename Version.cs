using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vault2git
{
    struct Version
    {
        public int txid;
        public int version;
        public DateTime date;
        public string user;
        public string comment;

        public string GitAuthor { get { return Authors.GetGitAuthor(user); } }
    }
}
