using NotifManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotifManager.ViewModels
{
    public class MessageVM
    {
        public Guid Id { get; set; }
        public Guid AppId { get; set; }
        public string AppName { get; set; }
        public string AppIcon { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string SubTitle { get; set; }
        public string Url { get; set; }

        public MessageVM(Guid id, Guid appId, string appName, string appIcon, string title, string content, string subTitle, string url)
        {
            Id = id;
            AppId = appId;
            AppName = appName;
            AppIcon = appIcon;
            Title = title;
            Content = content;
            SubTitle = subTitle;
            Url = url;
        }
    }
}