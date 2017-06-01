using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStressTest.Attributes
{
    public class ArgumentAttribute : Attribute
    {
        public string Argument { get; private set; }

        public ArgumentAttribute(string argument)
        {
            Argument = argument;
        }
    }
}
