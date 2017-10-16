using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sgq
{
    public class Field
    {
        public bool key { get; set; }
        public string type { get; set; }
        public string target { get; set; }
        public string source { get; set; }

        public Field()
        {
            this.key = false;
            this.type = "A";
        }
    }
}
