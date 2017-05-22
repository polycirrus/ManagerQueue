using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Client.Exceptions
{
    public class ServiceCommunicationException : Exception
    {
        public ServiceCommunicationException()
            : base()
        {
        }

        public ServiceCommunicationException(string message)
            : base(message)
        {
        }

        public ServiceCommunicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
