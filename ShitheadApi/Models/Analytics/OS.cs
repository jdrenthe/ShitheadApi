using System;

namespace ShitheadApi.Models.Analytics
{
    public class OS
    {
        public Guid Id { get; set; }

        public Guid DeviceId { get; set; }

        public Device Device { get; set; }

        public string Name { get; set; }

        public string Major { get; set; }

        public string Minor { get; set; }

        public DateTime Date { get; set; }
    }
}
