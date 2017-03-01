using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotifManager.Models
{
    public class Client
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Nome Obrigatório")]
        [Display(Name = "Nome Completo")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email Obrigatório")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Email Inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha Obrigatória")]
        [StringLength(100, ErrorMessage = "Senha precisa ter pelo menos 6 caracteres"), MinLength(6, ErrorMessage = "Senha precisa ter pelo menos 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmação Obrigatória")]
        [StringLength(100, ErrorMessage = "Senha precisa ter pelo menos 6 caracteres"), MinLength(6, ErrorMessage = "Senha precisa ter pelo menos 6 caracteres")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirmar Senha")]
        public string ConfirmPassword { get; set; }


    }
}