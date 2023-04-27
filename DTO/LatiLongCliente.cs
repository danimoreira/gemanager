using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GEPV.Domain.DTO
{

    [Keyless]
    public class LatiLongCliente
    {
        public int Id { get; set; }
        public string Latilong { get; set; }
    }
}