using System;
using GEPV.Domain.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.Types;

namespace GEPV.Domain.Entities
{
    [Table("MENSAGEM")]
    public class Mensagem
    {
        public Mensagem()
        {
            
        }
        [Column("ID")]
        [Key]
        public int Id { get; set; }
        [Column("TEXTO")]
        public string Texto { get; set; }

        [Column("DATA_VALIDADE")]
        [Display(Name = "Data de Validade")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataValidade { get; set; }

        [Column("TIPO")]
        public string Tipo { get; set; }

        [NotMapped]
        public string DataValidadeFormatadaYMD
        {
            get
            {
                return DataValidade.HasValue ? DataValidade.Value.ToString("yyyy-MM-dd") : "";
            }
        }

    }
}
