using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeEngine.Exceptions
{
    [Serializable]
    public class ConnectorNotPresentException : Exception
    {
        public ConnectorNotPresentException()
        {

        }

        public ConnectorNotPresentException(string name)
            : base("Connector is not configured yet. Name : " + name)
        {

        }

        public ConnectorNotPresentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConnectorNotPresentException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }

}
