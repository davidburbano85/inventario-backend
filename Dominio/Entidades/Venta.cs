// Ubicación: /src/Dominio/Entidades/Venta.cs

using System.Security.Cryptography.X509Certificates;

namespace inventarioWebAI.Dominio.Entidades;

public class Venta
{
    public Guid Id { get; set; }

    public Guid EmpresaId { get; set; }

    public Guid? ClienteId { get; set; }
    public string Factura { get; set; } = string.Empty;

    public decimal Total { get; set; }


    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Activo { get; set; } 
    public List<VentaDetalle> Detalles { get; set; } = new();
}