using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotifManager.Models
{
    public class MessageReply
    {
        public Guid Id { get; set; }
        public int Sucessfuls { get; set; }
        public int Faileds { get; set; }
        public int Converteds { get; set; }
        public DateTime DateSented { get; set; }
        public string Log { get; set; }
    }
}