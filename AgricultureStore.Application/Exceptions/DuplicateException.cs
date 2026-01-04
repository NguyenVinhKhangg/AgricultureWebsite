using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgricultureStore.Application.Exceptions
{
    public class DuplicateException : Exception
    {
         public DuplicateException(string message) : base(message) {}
    }
}