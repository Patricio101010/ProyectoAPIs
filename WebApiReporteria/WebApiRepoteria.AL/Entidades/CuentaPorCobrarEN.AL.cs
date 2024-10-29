using System;

namespace WebApiRepoteria.AL.Entidades
{
    /// <summary>
    /// Representa la estructura de la cuenta por pagar.
    /// </summary>
    public class CuentaPorCobrarEN
    {
        public long ID { get; set; }
        public string TipoCuenta { get; set; }
        public int NumeroCuenta { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaPago { get; set; }
        public int IdMoneda { get; set; }
        public string Moneda { get; set; }
        public double FactorCambio { get; set; }
        public double ValorCuenta { get; set; }
        public string Descripcion { get; set; }
        public double SaldoCuenta { get; set; }
        public string Estado { get; set; }
        public int  CodigoEjecutivo { get; set; }
        public string  NombreEjecutivo { get; set; }
        public long RutCliente { get; set; }
        public string DigitoCliente { get; set; }
        public string NombreCliente { get; set; }
    }
}
