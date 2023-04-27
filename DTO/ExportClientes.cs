using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GEPV.Domain.DTO
{
    [Keyless]
    public class ExportClientes
    {
        public string Linha { get; set; } = "Linha vazia\n";
    

    }
}
