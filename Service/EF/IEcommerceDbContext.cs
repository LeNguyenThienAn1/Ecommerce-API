using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF
{
    public interface IEcommerceDbContext : IDisposable
    {
        DbSet<ProductEntity> Products { get; set; }
        DatabaseFacade Database {  get; set; }
        ChangeTracker ChangeTracker { get; set; } 
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
