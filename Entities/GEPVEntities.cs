namespace GEPV.Domain.Entities
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Pomelo.EntityFrameworkCore;
    using System.Linq;
    using GEPV.Domain.DTO;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Configuration;
    using System.Data.Common;

    public class GEPVEntities : DbContext
    {

        static IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);

        public static IConfigurationRoot configuration = builder.Build();

        public string connectionString = configuration.GetConnectionString("DefaultConnection");

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                // connect to mysql with connection string from app settings
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }
        }

        public virtual DbSet<Cliente> Cliente { get; set; }

        public virtual DbSet<ExportClientes> ExportClientes { get; set; }
        public virtual DbSet<Vendedor> Comprador { get; set; }
        public virtual DbSet<Vendedor> Vendedor { get; set; }
        public virtual DbSet<Mensagem> Mensagem { get; set; }
        public virtual DbSet<Feriado> Feriado { get; set; }
        public virtual DbSet<Contatos> Contatos { get; set; }
        public virtual DbSet<Fornecedor> Fornecedor { get; set; }
        public virtual DbSet<Estado> Estado { get; set; }
        public virtual DbSet<Regiao> Regiao { get; set; }
        public virtual DbSet<FornecedorPorCliente> FornecedorPorCliente  { get; set; }
        public virtual DbSet<RegiaoMapa> RegiaoMapa { get; internal set; }
        public virtual DbSet<LatiLongCliente> LatiLongCliente { get; internal set; }        
        public virtual DbSet<FeriadoCliente> FeriadoCliente { get; internal set; }
        public virtual DbSet<TarefasVendedores> TarefasVendedores { get; internal set; }
        public virtual DbSet<TarefasClientes> TarefasClientes { get; internal set; }
        public virtual DbSet<HistoricoDTO> HistoricoDTO { get; internal set; }
        public virtual DbSet<TarefasFornecedores> TarefasFornecedores { get; internal set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>()
            .HasOne(x => x.Vendedor)
            .WithMany(x => x.Clientes)
            .HasForeignKey(x => x.IdVendedor)
            .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Mensagem>();
            
            modelBuilder.Entity<Vendedor>();

            modelBuilder.Entity<Contatos>();            
            modelBuilder.Entity<Fornecedor>();
            modelBuilder.Entity<Estado>();
            modelBuilder.Entity<Regiao>();
            modelBuilder.Entity<FornecedorPorCliente>();

            base.OnModelCreating(modelBuilder);
        }

    }
        
}