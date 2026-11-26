using ControlTickets.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlTickets.Infrastructure.Data;

public class TicketsDbContext : DbContext
{
    public TicketsDbContext(DbContextOptions<TicketsDbContext> options) : base(options) { }

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Prioridad> Prioridades => Set<Prioridad>();
    public DbSet<CanalOrigen> CanalesOrigen => Set<CanalOrigen>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ignorar propiedades calculadas en base de datos
        modelBuilder.Entity<Ticket>().Ignore(t => t.MesReporte);
        modelBuilder.Entity<Ticket>().Ignore(t => t.TiempoResolucionDias);
        modelBuilder.Entity<Ticket>().Ignore(t => t.CumpleSla);

        // Datos Semilla (Catálogos maestros)
        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Accesos y Permisos", DescripcionOperativa = "Gestión de carpetas compartidas, permisos de red, VPN y credenciales" },
            new Categoria { Id = 2, Nombre = "Hardware y Equipos", DescripcionOperativa = "Entrega, asignación, mantenimiento y periféricos" },
            new Categoria { Id = 3, Nombre = "Software y Redes", DescripcionOperativa = "Instalación de aplicaciones, conectividad Wi-Fi y herramientas" },
            new Categoria { Id = 4, Nombre = "Virtualización / Infraestructura", DescripcionOperativa = "Máquinas virtuales, servidores y entornos cloud" },
            new Categoria { Id = 5, Nombre = "Soporte General", DescripcionOperativa = "Escaneo, impresión, correo y asistencia operativa diaria" }
        );

        modelBuilder.Entity<Prioridad>().HasData(
            new Prioridad { Id = 1, Nombre = "Crítica", SlaObjetivoDias = 0.5, NivelAtencion = "Interrupción total / Bloqueo crítico" },
            new Prioridad { Id = 2, Nombre = "Alta", SlaObjetivoDias = 1.0, NivelAtencion = "Impacto alto en operación" },
            new Prioridad { Id = 3, Nombre = "Media", SlaObjetivoDias = 2.0, NivelAtencion = "Solicitud estándar con alternativa temporal" },
            new Prioridad { Id = 4, Nombre = "Baja", SlaObjetivoDias = 5.0, NivelAtencion = "Requerimiento menor o programable" }
        );

        modelBuilder.Entity<CanalOrigen>().HasData(
            new CanalOrigen { Id = 1, Nombre = "Portal de Tickets", EsFormal = true },
            new CanalOrigen { Id = 2, Nombre = "Correo Directo", EsFormal = false },
            new CanalOrigen { Id = 3, Nombre = "Chat / Teams", EsFormal = false },
            new CanalOrigen { Id = 4, Nombre = "Presencial / Pasillo", EsFormal = false },
            new CanalOrigen { Id = 5, Nombre = "Llamada Telefónica", EsFormal = false }
        );
    }
}