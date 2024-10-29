using System;
using System.Collections.Generic;
using System.Text;

namespace WebEntidades.EN.Entidades
{
    public class OperacionCursadaEN
	{
		public long RutCliente { get; set; }
		public string NombreCliente { get; set; }
		public DateTime FechaOtorgado { get; set; }
		public long NroOperacion { get; set; }
		public string OrigenOperacion { get; set; }
		public string CentroCosto { get; set; }
		public string TipoDocto { get; set; }
		public int IdMoneda { get; set; }
		public string Moneda { get; set; }
		public double FactorCambioActual { get; set; }
		public double Monto { get; set; }
		public double PorcentajeAnticipo { get; set; }
		public double MontoAnticipo { get; set; }
		public double TasaNegocio { get; set; }
		public double DiferenciaPrecio { get; set; }
		public double Gastos { get; set; }
		public double Recaudacion { get; set; }
		public double Excedentes { get; set; }
		public double MontoGiro { get; set; }
		public double Promedio { get; set; }
		public string Ejecutivo { get; set; }
		public string Cesion { get; set; }
	}
}
