using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotifManager.Models
{
    public class App
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }

        [Required(ErrorMessage = "Nome Obrigatório")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Url do Site")]
        [Required(ErrorMessage = "Url Obrigatório")]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        [Display(Name = "Url do Ícone")]
        [Required(ErrorMessage = "Ícone Obrigatório")]
        [DataType(DataType.Url)]
        public string Icon { get; set; }

        public string SubDomain { get; set; }
        public string RestKey { get; set; }
        public string Log { get; set; }
        public Guid SafariId { get; set; }
    }
}