namespace NameDayClient
{
    using System;
    using System.Collections.Generic;

    class ServiceResult
    {
        public DateTime Date
        {
            get; set;
        }

        public List<ResultName> Names
        {
            get; set;
        }

        public List<string> Contacts
        {
            get; set;
        }
    }
}
