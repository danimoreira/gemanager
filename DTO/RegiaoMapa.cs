using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GEPV.Domain.DTO
{

    [Keyless]
    public class RegiaoMapa
    {
        public string Descricao { get; set; }
        public string Features { get; set; }        
    }
}