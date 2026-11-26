namespace ControlTickets.Core.Entities;

public class Ticket
{
    public int Id { get; set; }
    public string CodigoTicket { get; set; } = "SIN-TICKET";
    public DateTime FechaApertura { get; set; } = DateTime.Now;
    public DateTime? FechaCierre { get; set; }

    public string Solicitante { get; set; } = string.Empty;
    public string Asunto { get; set; } = string.Empty;
    public EstatusTicket Estatus { get; set; } = EstatusTicket.Abierto;

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public int PrioridadId { get; set; }
    public Prioridad Prioridad { get; set; } = null!;

    public int CanalOrigenId { get; set; }
    public CanalOrigen CanalOrigen { get; set; } = null!;

    // Propiedades calculadas en tiempo real
    public string MesReporte => FechaApertura.ToString("yyyy-MM");

    public double TiempoResolucionDias =>
        Math.Round(((FechaCierre ?? DateTime.Now) - FechaApertura).TotalDays, 2);

    public bool CumpleSla =>
        Prioridad != null && TiempoResolucionDias <= Prioridad.SlaObjetivoDias;
}