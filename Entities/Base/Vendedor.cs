using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPV.Domain.Entities
{
    [Table("VENDEDOR")]
    public class Vendedor
    {
        public Vendedor()
        {
            
        }
        [Column("ID")]
        [Key]
        public int Id { get; set; }

        [Column("NOME")]
        [Required(ErrorMessage = "É obrigatório preencher o Nome!")]
        public string Nome { get; set; }

        [Column("DATA_NASCIMENTO")]
        [Display(Name = "Data de Nascimento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataNascimento { get; set; }

        [Column("EMAIL")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "É obrigatório preencher o Email!")]
        public string Email { get; set; }

        [Column("USUARIO")]
        [Display(Name = "Usuário")]
        [Required(ErrorMessage = "É obrigatório preencher o Usuário!")]
        public string Usuario { get; set; }

        [Column("SENHA")]
        [Display(Name = "Senha")]
        [Required(ErrorMessage = "É obrigatório preencher a Senha!")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [NotMapped]
        [Display(Name = "Confirmar Senha")]
        [Required(ErrorMessage = "É obrigatório preencher a Confirmação da Senha!")]
        [DataType(DataType.Password)]
        [Compare(nameof(Senha), ErrorMessage = "As senhas precisam ser iguais!")]
        public string ConfirmarSenha { get; set; }

        [Column("ADMIN")]
        [Display(Name = "Tipo de Usuário")]
        [Required(ErrorMessage = "É obrigatório preencher o Tipo de Usuário!")]
        public int? Admin { get; set; }

        public virtual IEnumerable<Cliente> Clientes { get; set; }

        [NotMapped]
        public string DataNascimentoFormatadaYMD
        {
            get
            {
                return DataNascimento.HasValue ? DataNascimento.Value.ToString("yyyy-MM-dd") : "";
            }
        }

    }
}
