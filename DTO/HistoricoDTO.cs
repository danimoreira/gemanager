﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPV.Domain.DTO
{
    [Keyless]
    public class HistoricoDTO
    {
        public int IdHistorico { get; set; }
        public int IdCliente { get; set; }
        public string CnpjCliente { get; set; }
        public string NomeCliente { get; set; }
        public int IdFornecedor { get; set; }
        public string NomeFornecedor { get; set; }
        public int IdVendedor { get; set; }
        public string NomeVendedor { get; set; }
        public DateTime? DataContato { get; set; }
        public DateTime? DataCompra { get; set; }
        public DateTime? DataAgenda { get; set; }
        public string Negociacao { get; set; }        
    }
}
