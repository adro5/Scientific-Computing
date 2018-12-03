using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISC_Project
{
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException()
        {

        }
        public InvalidPasswordException(string cReason) : base(cReason)
        {
            // Initializer list sets the super class Exception's message
        }
    }
}
