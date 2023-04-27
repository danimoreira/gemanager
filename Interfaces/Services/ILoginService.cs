﻿using GEPV.Domain.DTO;
using GEPV.Domain.Entities;
using GEPV.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEPV.Domain.Interfaces.Services
{
    public interface ILoginService : IServiceBase<Vendedor>
    {
        Vendedor Logar(LoginDto dados, VendedorService vendedorService);
    }
}
