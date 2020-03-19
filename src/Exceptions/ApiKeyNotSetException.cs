using System;
using System.Collections.Generic;
using System.Text;

namespace PlacePopularity.Exceptions
{
    public class ApiKeyNotSetException : Exception
    {
        #region constructor
        public ApiKeyNotSetException() { }

        public ApiKeyNotSetException(string message) : base(message) { }

        public ApiKeyNotSetException(string message, Exception inner) : base(message, inner) { }
        #endregion
    }
}
