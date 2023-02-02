using System;

namespace ShitheadApi.Models.Analytics
{
    public class Visit
    {
        public Guid Id { get; set; }

        public Guid DeviceId { get; set; }

        public Device Device { get; set; }

        public string PageName { get; set; }

        public string ReferrerUrl { get; set; }

        public double LoadTime { get; set; }

        public DateTime Date { get; set; }
    }
}
