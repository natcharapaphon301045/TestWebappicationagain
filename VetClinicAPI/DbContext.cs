// Models/SampleDBContext.cs

using Microsoft.EntityFrameworkCore;

namespace DotnetWebApiWithEFCodeFirst.Models
{
     public partial class SampleDBContext : DbContext
     {
       public SampleDBContext(DbContextOptions
       <SampleDBContext> options)
           : base(options)
       {
       }
       public virtual DbSet<Customer> Customer { get; set; }
       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           modelBuilder.Entity<Customer>(entity => {
               entity.HasKey(p => p.Customer_Id);
           });
           OnModelCreatingPartial(modelBuilder);
       }
       partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
     }
}