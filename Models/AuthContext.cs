using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crud11API.Models
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<PaymentDetailContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
    }
}
