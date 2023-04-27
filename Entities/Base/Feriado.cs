using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPV.Domain.Entities
{
    [Table("FERIADO")]
    public class Feriado
    {
        public Feriado()
        {
            
        }
        [Column("ID")]
        [Key]
        public int Id { get; set; }
        [Column("DATA")]
        public string Data { get; set; }

        [Column("TIPO")]
        public int Tipo { get; set; }

        [Column("NOME")]
        public string Nome { get; set; }

        [Column("DESCRICAO")]
        public string Descricao { get; set; }

        [Column("UF")]
        public string Uf { get; set; }

        [Column("MUNICIPIO")]
        [DisplayName("Cidade")]
        public string Municipio { get; set; }

    }
}
