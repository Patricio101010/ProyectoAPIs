using System;
using System.Collections.Generic;
using System.Text;

namespace WebEntidades.EN.Entidades
{
    public class DocumentoCompradoEN
    {
        public long RutEmisor { get; set; }
        public long RutCliente { get; set; }
        public string NombreCliente { get; set; }
        public long RutDeudor { get; set; }
        public string NombreDeudor { get; set; }
        public string TipoDocto { get; set; }
        public string TipoDoctoCorta { get; set; }
        public string EstadoDocto { get; set; }
        public string NroDocto { get; set; }
        public DateTime  FechaOtorgado { get; set; }
        public long NroOperacion { get; set; }
        public string OrigenOperacion { get; set; }
        public double PorcentajeAnticipo { get; set; }
        public double MontoDocto { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVcto { get; set; }
        public double TasaNegocio { get; set; }
        public double Comision { get; set; }
        public double DiferenciaPrecio { get; set; }
        public double MontoAnticipo { get; set; }
        public long IDDoc { get; set; }
        public int IdMoneda { get; set; }
        public string Moneda { get; set; }
        public string Cesion { get; set; }
        public string AfectaExenta { get; set; }
    }
}