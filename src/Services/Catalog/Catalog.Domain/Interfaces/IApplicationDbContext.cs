using System.Collections.Generic;
using Catalog.Domain.Entities;
using Catalog.Domain.Enum;
using Catalog.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Domain.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Plate> Plates { get; }
    }
}
