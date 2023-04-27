using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GEPV.Domain.DTO
{
    [Keyless]
    public class FeriadoCliente
    {
        public int Tipo { get; set; }
        public string Uf { get; set; }
        public string Municipio { get; set; }
        public string Nome { get; set; }
    }
}