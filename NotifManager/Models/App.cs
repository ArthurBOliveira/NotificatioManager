using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotifManager.Models
{
    public class App
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string SubDomain { get; set; }
        public string RestKey { get; set; }
        public string Log { get; set; }
        public Guid SafariId { get; set; }
    }
}