using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Client.Exceptions
{
    public class RequestFailedException : Exception
    {
        public RequestFailedException()
            : base()
        {
        }

        public RequestFailedException(string message)
            : base(message)
        {
        }

        public RequestFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
