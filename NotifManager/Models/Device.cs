using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotifManager.Models
{
    public class Device
    {
        public Guid Id { get; set; }
        public int SessionCount { get; set; }
        public string DeviceType { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceOS { get; set; }
        public DateTime LastActive { get; set; }
    }
}