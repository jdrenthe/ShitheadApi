using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShitheadApi.Models.Analytics
{
    public class Device
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Model { get; set; }

        public string Brand { get; set; }

        public string ApplicationName { get; set; }

        public List<OS> OS { get; set; }

        public List<Browser> Browsers { get; set; }

        public List<Ipinfo> Ipinfos { get; set; }

        public List<Visit> Visits { get; set; }

        #region NotMapped

        [NotMapped]
        public string UserAgent { get; set; }

        [NotMapped]
        public string IPAddress { get; set; }

        #endregion
    }
}
