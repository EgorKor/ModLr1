using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLR1
{
    public class SyntaxValidationException : Exception
    {
        public SyntaxValidationException(string message) : base(message) { 
        }

        public SyntaxValidationException() { }

    }
}
