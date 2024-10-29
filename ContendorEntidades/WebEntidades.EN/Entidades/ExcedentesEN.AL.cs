using System;
using System.Collections.Generic;
using System.Text;

namespace WebEntidades.EN.Entidades
{
    public class ExcedentesEN
    {
        public long RutCliente { get; set; }
        public string NombreCliente { get; set; }
        public long RutDeudor { get; set; }
        public string NombreDeudor { get; set; }
        public long NroOperacion { get; set; }
        public string NroDocto { get; set; }
        public string TipoDocto { get; set; }
        public double MontoAnticipo { get; set; }
        public double MontoPagado { get; set; }
        public DateTime FechaVcto { get; set; }
        public DateTime FechaPago { get; set; }
        public DateTime FechaEntregado { get; set; }
        public double MontoExcedente { get; set; }
        public int IdMoneda { get; set; }
        public string Moneda { get; set; }
        public long NroPago { get; set; }

    }
}
