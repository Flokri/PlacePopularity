using System;
using System.Collections.Generic;
using System.Text;

namespace PlacePopularity.Exceptions
{
    public class MultipleRequestInHour : Exception
    {
        #region constructor
        public MultipleRequestInHour() { }
        public MultipleRequestInHour(string message) : base(message) { }
        public MultipleRequestInHour(string message, Exception inner) : base(message, inner) { }
        #endregion 
    }
}
