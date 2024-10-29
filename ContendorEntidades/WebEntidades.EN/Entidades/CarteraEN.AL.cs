using System;
using System.Collections.Generic;
using System.Text;

namespace WebEntidades.EN.Entidades
{
    /// <summary>
    /// Representa la estructura de la cartera.
    /// </summary>
    public class CarteraEN
    {
        public long ID { get; set; }
        public long RutDeudor { get; set; }
        public string DigitoDeudor { get; set; }
        public string NombreDeudor { get; set; }
        public long RutCliente { get; set; }
        public string DigitoCliente { get; set; }
        public string NombreCliente { get; set; }
        public long RutEmisor { get; set; }
        public string DigitoEmisor { get; set; }
        public string TipoDocto { get; set; }
        public string TipoDoctoCorta { get; set; }
        public int IdMoneda { get; set; }
        public string Moneda { get; set; }
        public double FactorCambioActual { get; set; }
        public long NroOperacion { get; set; }
        public string ConSinCesion { get; set; }
        public double TasaOperacion { get; set; }
        public DateTime FechaCurse { get; set; }
        public DateTime FechaOtorgada { get; set; }
        public double PorcentajeAnticipo { get; set; }
        public string AfectaExenta { get; set; }
        public string NroDocto { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVcto { get; set; }
        public DateTime FechaVctoOriginal { get; set; }
        public long NroProrrogas { get; set; }
        public long DiasMora { get; set; }
        public double TasasMora { get; set; }
        public double ValorDocto { get; set; }
        public double MontoInteres { get; set; }
        public double MontoAnticipo { get; set; }
        public double Saldo { get; set; }
        public double Deuda { get; set; }
        public string EstadoDocto { get; set; }
        public double MontoAbonado { get; set; }
        public string ConResponsabilidad { get; set; }
        public string CentroCosto { get; set; }
        public string Origen { get; set; }
        public string RechazoNotificacion { get; set; }
        public string Prorroga { get; set; }
        public DateTime FechaCompromiso { get; set; }
        public string EjecutivoComercial { get; set; }
        public string PlataformaComercial { get; set; }
        public string EjecutivoFactoring { get; set; }
        public string EjecutivoCobranza { get; set; }
        public string UltimaGestionCobranza { get; set; }
        public string EstadoGestionCobranza { get; set; }
        public string DocumentoCedido { get; set; }
        public string EstadoNotaCredito { get; set; }
        public string EstadoReclamo { get; set; }
        public DateTime UltimaFechaPago { get; set; }
        public string ConGarantia { get; set; }
        public string Mandante { get; set; }
        public string SistemaOrigen { get; set; }
    }
}

