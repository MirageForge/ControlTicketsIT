using ControlTickets.Core.Entities;
using ControlTickets.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;

namespace ControlTickets.Infrastructure.Services;

public class CsvImportService
{
    private readonly TicketsDbContext _context;

    public CsvImportService(TicketsDbContext context)
    {
        _context = context;
    }

    public async Task<int> ImportarDesdeStreamAsync(Stream csvStream)
    {
        using var parser = new TextFieldParser(csvStream)
        {
            TextFieldType = FieldType.Delimited,
            HasFieldsEnclosedInQuotes = true
        };
        parser.SetDelimiters(",");

        if (parser.EndOfData) return 0;

        // Leer encabezados y mapear posiciones dinámicamente
        var headers = parser.ReadFields() ?? Array.Empty<string>();
        int idxFechaApertura = Array.FindIndex(headers, h => h.Trim().Equals("Fecha_Apertura", StringComparison.OrdinalIgnoreCase));
        int idxIdTicket = Array.FindIndex(headers, h => h.Trim().Equals("ID_Ticket", StringComparison.OrdinalIgnoreCase));
        int idxCanal = Array.FindIndex(headers, h => h.Trim().Equals("Canal_Origen", StringComparison.OrdinalIgnoreCase));
        int idxSolicitante = Array.FindIndex(headers, h => h.Trim().Equals("Solicitante", StringComparison.OrdinalIgnoreCase));
        int idxCategoria = Array.FindIndex(headers, h => h.Trim().Equals("Categoria", StringComparison.OrdinalIgnoreCase));
        int idxAsunto = Array.FindIndex(headers, h => h.Trim().Equals("Asunto", StringComparison.OrdinalIgnoreCase));
        int idxPrioridad = Array.FindIndex(headers, h => h.Trim().Equals("Prioridad", StringComparison.OrdinalIgnoreCase));
        int idxEstatus = Array.FindIndex(headers, h => h.Trim().Equals("Estatus", StringComparison.OrdinalIgnoreCase));
        int idxFechaCierre = Array.FindIndex(headers, h => h.Trim().Equals("Fecha_Cierre", StringComparison.OrdinalIgnoreCase));

        var categorias = await _context.Categorias.ToListAsync();
        var prioridades = await _context.Prioridades.ToListAsync();
        var canales = await _context.CanalesOrigen.ToListAsync();

        int importados = 0;

        while (!parser.EndOfData)
        {
            var cols = parser.ReadFields();
            if (cols == null || cols.Length == 0) continue;

            string GetCol(int idx) => idx >= 0 && idx < cols.Length ? cols[idx].Trim() : string.Empty;

            var solicitante = GetCol(idxSolicitante);
            if (string.IsNullOrWhiteSpace(solicitante)) continue;

            var catNombre = GetCol(idxCategoria);
            var prioNombre = GetCol(idxPrioridad);
            var canalNombre = GetCol(idxCanal);

            var cat = categorias.FirstOrDefault(c => c.Nombre.Equals(catNombre, StringComparison.OrdinalIgnoreCase))
                      ?? categorias.First(c => c.Nombre == "Soporte General");

            var prio = prioridades.FirstOrDefault(p => p.Nombre.Equals(prioNombre, StringComparison.OrdinalIgnoreCase))
                       ?? prioridades.First(p => p.Nombre == "Media");

            var canal = canales.FirstOrDefault(c => c.Nombre.Equals(canalNombre, StringComparison.OrdinalIgnoreCase))
                        ?? canales.First(c => c.Nombre == "Portal de Tickets");

            var estatus = GetCol(idxEstatus).ToLower() switch
            {
                "cerrado" => EstatusTicket.Cerrado,
                "en proceso" => EstatusTicket.EnProceso,
                _ => EstatusTicket.Abierto
            };

            DateTime.TryParse(GetCol(idxFechaApertura), out var fechaApertura);
            DateTime? fechaCierre = DateTime.TryParse(GetCol(idxFechaCierre), out var fc) ? fc : null;
            var idTicket = GetCol(idxIdTicket);

            _context.Tickets.Add(new Ticket
            {
                FechaApertura = fechaApertura == default ? DateTime.Now : fechaApertura,
                CodigoTicket = string.IsNullOrWhiteSpace(idTicket) ? "SIN-TICKET" : idTicket,
                CanalOrigenId = canal.Id,
                Solicitante = solicitante,
                CategoriaId = cat.Id,
                Asunto = GetCol(idxAsunto),
                PrioridadId = prio.Id,
                Estatus = estatus,
                FechaCierre = fechaCierre
            });

            importados++;
        }

        await _context.SaveChangesAsync();
        return importados;
    }
}