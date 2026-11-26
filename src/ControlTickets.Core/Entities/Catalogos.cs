
namespace ControlTickets.Core.Entities;

public enum EstatusTicket
{
    Abierto,
    EnProceso,
    Cerrado
}

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string DescripcionOperativa { get; set; } = string.Empty;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

public class Prioridad
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty; // Crítica, Alta, Media, Baja
    public double SlaObjetivoDias { get; set; }        // Ej: 0.5, 1.0, 2.0, 5.0
    public string NivelAtencion { get; set; } = string.Empty;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

public class CanalOrigen
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty; // Portal de Tickets, Correo Directo, etc.
    public bool EsFormal { get; set; }                 // true = Con Ticket, false = Ad-hoc
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}