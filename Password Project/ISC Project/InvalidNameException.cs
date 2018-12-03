using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISC_Project
{
    public class InvalidNameException : Exception
    {
        public InvalidNameException()
        {

        }
        public InvalidNameException(string cReason) : base(cReason)
        {
            // Initializer list sets the super class Exception's message
        }
    }
}
