using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vault2git
{
    struct Label
    {
        public int version;
        public DateTime date;
        public string actionString;

        public string Tag
        {
            get
            {
                return "v" + actionString.Replace("Labeled ", string.Empty).Replace("Build ", string.Empty);
            }
        }
    }
}
