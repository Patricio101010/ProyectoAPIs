using System;
using System.Collections.Generic;

namespace WebEntidades.EN.Entidades
{
    public class LineaFinanciamientoEN
    {
        public long ID { get; set; }
        public long RutCliente { get; set; }
        public string DigitoCliente { get; set; }
        public string NombreCliente { get; set; }
        public string NombreEjecutivoComercial { get; set; }
        public string NombreEjecutivoFatoring { get; set; }
        public string Sucursal { get; set; }
        public string EstadoLinea { get; set; }
        public string TipoComision { get; set; }
        public string Observacion { get; set; }
        public long NroLinea { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaAprobación { get; set; }
        public DateTime FechaVigenteDesde { get; set; }
        public DateTime FechaVigenteHasta { get; set; }
        public double MontoSolicitada { get; set; }
        public double MontoDisponible { get; set; }
        public double MontoOcupado { get; set; }
        public double MontoAprobado { get; set; }
        public List<PorcentajeAnticipar> ListaPorcentajeAnticipar { get; set; } = new List<PorcentajeAnticipar>();
        public List<SubLineas> ListaSubLineas { get; set; } = new List<SubLineas>();
        public List<Pagadores> ListaPagadores { get; set; } = new List<Pagadores>();
        public List<Comision> ListaComision { get; set; } = new List<Comision>();
        public List<Gastos> ListaGastos { get; set; } = new List<Gastos>();
    }

    public class PorcentajeAnticipar
    {
        public string TipoProducto { get; set; }
        public double Porcentaje { get; set; }
        public string Verificacion { get; set; } //apc_ver_son
        public string Notificacion { get; set; }
        public string Cobranza { get; set; }
    }

    public class SubLineas
    {
        public string TipoProducto { get; set; }
        public double Linea { get; set; }
        public double Porcentaje { get; set; }
    }

    public class Pagadores
    {
        public long RutDeudor { get; set; }
        public string DigitoDeudor { get; set; }
        public string NombreDeudor { get; set; }
        public double MontoLinea { get; set; }
        public double PorcentajeLinea { get; set; }
    }

    public class Comision
    {
        public double Porcentaje { get; set; }
        public int IdMoneda { get; set; }
        public string Moneda { get; set; }
        public double Minimo { get; set; } //cli_com_min
        public double Maximo { get; set; } //cli_com_max
        public double MontoComision { get; set; }
        public double MontoComisionFlat { get; set; }

    }

    public class Gastos
    {
        public string TipoProducto { get; set; }
        public double Monto { get; set; }
        public string Estado { get; set; }
        public string descripcion { get; set; }
    }
}
