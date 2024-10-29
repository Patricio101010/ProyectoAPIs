using System;
using System.Collections.Generic;
using System.Text;

namespace WebEntidades.EN.Entidades
{
    public class DocumentoDevengadoEN
    {
        //             
        public string TipoDocto { get; set; }
        public int IdMoneda { get; set; }
        public string Moneda { get; set; }
        public long NroOperacion { get; set; }
        public string CentroCosto { get; set; }
        public double DiferenciaPrecio { get; set; }
        public double PorDevengar { get; set; }
        public double DevengoVigente { get; set; }
        public double DevengoMoroso { get; set; }
        public double DevengoPagado { get; set; }
        public double DevengoDevuelto { get; set; }
        public double TotalMes { get; set; }
        public string Cesión { get; set; }

    }
}
