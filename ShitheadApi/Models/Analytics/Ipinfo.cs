using System;

namespace ShitheadApi.Models.Analytics
{
    public class Ipinfo
    {

        public Guid Id { get; set; }

        public Guid DeviceId { get; set; }

        public Device Device { get; set; }

        public DateTime Date { get; set; }

        #region Ipinfo

        public string Ip { get; set; }

        public string Hostname { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string Loc { get; set; }

        public string Org { get; set; }

        public string Postal { get; set; }

        public string Timezone { get; set; }

        #endregion
    }
}
