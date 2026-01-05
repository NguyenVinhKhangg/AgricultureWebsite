using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgricultureStore.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message){}

        public NotFoundException(string name, object key) 
            : base ($"{name} with identifier {key} was not found."){}
    }
}