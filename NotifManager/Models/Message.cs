using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotifManager.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public string RestKey { get; set; }
        public Guid AppId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string SubTitle { get; set; }
        public string Url { get; set; }
        public string Log { get; set; }
    }
}