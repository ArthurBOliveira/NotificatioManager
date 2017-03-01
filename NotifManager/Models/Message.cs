using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotifManager.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public string RestKey { get; set; }
        public Guid AppId { get; set; }

        [Required(ErrorMessage = "Título Obrigatório")]
        [Display(Name = "Título")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Conteúdo Obrigatório")]
        [Display(Name = "Conteúdo")]
        public string Content { get; set; }

        [Display(Name = "Sub Título")]
        public string SubTitle { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "Link para Direcionamento")]
        public string Url { get; set; }
        public string Log { get; set; }
    }
}