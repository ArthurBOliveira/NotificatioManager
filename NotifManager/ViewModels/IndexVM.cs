using NotifManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotifManager.ViewModels
{
    public class IndexVM
    {
        public List<App> Apps { get; set; }
        public List<MessageVM> Messages { get; set; }
        public bool isPremium { get; set; }
    }
}