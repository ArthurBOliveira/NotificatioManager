using NotifManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotifManager.ViewModels
{
    public class IndexVM
    {
        public List<Client> Clients { get; set; }
        public List<App> Apps { get; set; }
        public List<Message> Messages { get; set; }
    }
}