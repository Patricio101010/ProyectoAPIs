using System;

namespace WebApiRepoteria.AL.Entidades
{
    /// <summary>
    /// Representa la solicitud para obtener cuentas por pagar para la exportación a Excel.
    /// </summary>
    /// .

    public class ParametrosEntradaRequest
    {
        public int registro { get; set; }
        public int CantidadRegistro { get; set; }
        public int OrderColumna { get; set; }
        public string DireccionOrder { get; set; }
    }
    public class CuentasPorPagarRequest
    {
        public long RutCliente { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int IdMoneda { get; set; }
        public int IdEstado { get; set; }
        public long NumeroCuenta { get; set; }
    }

    public class CuentasPorCobrarRequest
    {
        public long RutCliente { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int IdMoneda { get; set; }
        public int IdEstado { get; set; }
        public long NumeroCuenta { get; set; }
    }

    public class DocumentoCompradoRequest
    {
        public long RutCliente { get; set; }
        public long RutDeudor { get; set; }
        public long TipoDocto { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int NroOperacion { get; set; }
        public DateTime FechaCurseDesde { get; set; }
        public DateTime FechaCurseHasta { get; set; }
        public string NroDocto { get; set; }
        public int IdEstado { get; set; }
    }

    public class DocumentoDevengadoRequest
    {
        public int anno { get; set; }
        public int mes { get; set; }

    }


    public class OperacionCursadasRequest
    {
        public long RutCliente { get; set; }
        public int IdSucursal { get; set; }
        public int IdEjecutivo { get; set; }
        public int TipoDocto { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

    }

    public class ExcedentesRequest
    {
        public long RutCliente { get; set; }
        public bool Vigente { get; set; }

    }
}
