using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vault2git
{
    struct Label
    {
        public DateTime date;
        public string actionString;

        public string Tag
        {
            get
            {
                if(actionString.StartsWith("Labeled Build "))
                {
                    return "v" + actionString.Substring(14);
                } else
                {
                    return actionString;
                }
            }
        }
    }
}
