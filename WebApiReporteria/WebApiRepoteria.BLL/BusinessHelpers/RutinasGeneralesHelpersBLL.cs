using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using System;
using System.Globalization;

namespace WebApiRepoteria.BLL.BusinessHelpers
{
    public class RutinasGeneralesHelpersBLL
    {
        public string DevuelveFormatoFecha()
        {
            return "dd/MM/yyyy";
        }
        public string DevuelveFormatoMoneda(int idMoneda)
        {
            string FormatoMoneda;

            switch (idMoneda)
            {
                case 1: // Ejemplo: Peso Chileno
                    FormatoMoneda = "#,##0";
                    break;
                case 2: // Ejemplo: Dólar Americano
                case 3: // Ejemplo: Euro
                    FormatoMoneda = "#,##0.00";
                    break;

                // Agrega más monedas según sea necesario
                default:
                    FormatoMoneda = "#,##0.0000"; // Formato por defecto
                    break;
            }

            return FormatoMoneda;
        }
        public double CalcularMontoEnPesos(double valor, int idMoneda, double factorCambio)
        {
            double montoENPesos = valor;
            if (idMoneda != 1)
            {
                montoENPesos *= factorCambio;
            }
            return montoENPesos;
        }

        public string SetearCeroIzquierdaRut(long rut)
        {
            string rutString = "";
            if (rut.ToString() != "0")
            {
                rutString = rut.ToString().PadLeft(12, '0');
            }

            return rutString;
        }

        public string ObtenerEstadoDocumentoCedidor(string doctoCedido)
        {
            string estadoDocumentoCedido = "";
            switch (doctoCedido)
            {
                case "N":
                    estadoDocumentoCedido = "NO";
                    break;

                case "S":
                    estadoDocumentoCedido = "SI";
                    break;

                case "R":
                    estadoDocumentoCedido = "RECEDIDO";
                    break;
            }
            return estadoDocumentoCedido;
        }

        public string ObtenerFecha(DateTime fecha)
        {
            string fechaString = "";
            if (ConvertirFechaJuliana(fecha) != "19000101")
            {
                fechaString = fecha.ToString(DevuelveFormatoFecha());
            }
            return fechaString;
        }

        public string ConvertirFechaJuliana(DateTime fecha)
        {
            return fecha.ToString("yyyyMMdd");
        }

        public string ObtieneFormatoDecimal(double valor)
        {
            return valor.ToString("N2");
        }

        public string ObtieneFormatoPorcentaje(double valor)
        {
            return valor.ToString("N2") + " %";
        }

        public string ObtieneFormatoRut(long rut, string digito)
        {
            return rut.ToString("N0") + "-" + digito;
        }

        public string ObtieneFormatoRut(long rut)
        {
            return rut.ToString("N0");
        }


        public string ObtieneFormatoMiles(double valor)
        {
            return valor.ToString("N0");
        }

        public string ObtenerFormatoMoneda(int idMoneda, double monto)
        {
            CultureInfo cultura;
            string FormatoMoneda;

            switch (idMoneda)
            {
                case 1: // Ejemplo: Peso Chileno
                    cultura = new CultureInfo("es-CL");
                    FormatoMoneda = "#,##0";
                    break;
                case 2: // Ejemplo: Dólar Americano
                    cultura = new CultureInfo("en-US");
                    FormatoMoneda = "#,##0.00";
                    break;
                case 3: // Ejemplo: Euro
                    FormatoMoneda = "#,##0.0000"; // Formato por defecto
                    FormatoMoneda = "#,##0.00";
                    cultura = new CultureInfo("fr-FR");
                    break;
                // Agrega más monedas según sea necesario
                default:
                    FormatoMoneda = "#,##0.00"; // Formato por defecto
                    cultura = CultureInfo.InvariantCulture;
                    break;
            }

            return string.Format(cultura, FormatoMoneda, monto);
        }


        public string ObtenerdigitoVerificador(long rut)
        {
            long rutAux = rut;

            long rut1 = (long)rutAux;
            long Digito;
            int Contador;
            long Multiplo;
            long Acumulador;
            string RutDigito;

            Contador = 2;
            Acumulador = 0;

            while (rut1 != 0)
            {
                Multiplo = (rut1 % 10) * Contador;
                Acumulador = Acumulador + Multiplo;
                rut1 = rut1 / 10;
                Contador = Contador + 1;

                if (Contador == 8)
                {
                    Contador = 2;
                }

            }

            Digito = 11 - (Acumulador % 11);
            RutDigito = Digito.ToString().Trim();

            if (Digito == 10)
            {
                RutDigito = "K";
            }

            if (Digito == 11)
            {
                RutDigito = "0";
            }

            return RutDigito;
        }
    }
}
