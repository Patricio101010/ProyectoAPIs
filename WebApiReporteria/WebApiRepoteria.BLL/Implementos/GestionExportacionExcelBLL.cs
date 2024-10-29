using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using WebApiRepoteria.AL.Interfaz;
using WebApiRepoteria.AL.Seguridad;
using WebApiRepoteria.BLL.BusinessHelpers;
using WebApiRepoteria.BLL.Interfaz;

namespace WebApiRepoteria.BLL.Implementos
{
    public class GestionExportacionExcelBLL : IGestionExportacionExcelBLL
    {
        //objeto
        readonly CuentaBLL ClsCuentaBLL = new CuentaBLL();
        readonly RutinasGeneralesHelpersBLL ClsHelpers = new RutinasGeneralesHelpersBLL();
        readonly HelpersExcel ClsHelpersExcel = new HelpersExcel();
        private readonly IEncriptaEN ObjEncripta = new EncriptaEN();

        //variables privadas
        private string rutaImagen, rutaGuardarEXCEL;
        private bool registrarArchivo;

        public GestionExportacionExcelBLL()
        {
            //rutaImagen = ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["rutaLogo"].ToString());
            //rutaGuardarEXCEL = ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["rutaGuardarEXCEL"].ToString());
            //registrarArchivo = bool.Parse(ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["registrarArchivo"].ToString()));

            rutaImagen = ConfigurationManager.AppSettings["rutaLogo"].ToString();
            rutaGuardarEXCEL = ConfigurationManager.AppSettings["rutaGuardarEXCEL"].ToString();
            registrarArchivo = bool.Parse(ConfigurationManager.AppSettings["registrarArchivo"].ToString());
        }

        #region "INFORME CARTERA"

        public async Task<byte[]> GetCarteraVigente(List<ServiceEntidades.CarteraEN> data)
        {
            byte[] archivoByte = await GeneraArchivoExcelCarteraVigente(data);

            return await Task.FromResult(archivoByte);
        }

        private async Task<byte[]> GeneraArchivoExcelCarteraVigente(List<ServiceEntidades.CarteraEN> data)
        {
            const int tituloRow = 3;
            const int headerRow = 5;
            Byte[] archivoByte = null;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Informe Cartera");
                ClsHelpersExcel.InsertarImagen(ws);
                ClsHelpersExcel.ConfigurarTitulo(ws, tituloRow, 10, "Informe de cartera vigente hasta el día de " + DateTime.Now.ToShortDateString());

                // La fila donde empieza el detalle
                int detalleStartRow = headerRow + 2;
                string[] headers = { "N°", "RUT DEUDOR", "DV", "NOMBRE DEUDOR", "RUT CLIENTE", "DV", "NOMBRE CLIENTE", "RUT EMISOR", "DV", "TIPO FA", "MONEDA", "FACTOR CAMBIO AL DIA HOY", "TIPO DOCTO.", "NRO.OPER.", "CON O SIN CESIÓN", "TASA OPERACIÓN", "FECHA CURSE", "FECHA OTORGADA", "PORC.ANTICIPO", "AFECTA O EXENTA", "NRO DOCTO.", "FECHA EMISIÓN", "FECHA VCTO", "FECHA ORIGINAL", "NRO PRÓRROGAS", "DÍAS MORA", "TASAS MORA", "VALOR DOCTO.", "VALOR DOCTO. $ AL DÍA HOY", "MONTO INTERÉS", "MONTO ANTICIPO", "MONTO ANTICIPO $ AL DÍA HOY", "SALDO", "SALDO $ AL DÍA HOY", "DEUDA", "DEUDA $ AL DÍA HOY", "ESTADO DOCTO.", "MONTO ABONADO", "MONTO ABONADO $ AL DÍA HOY", "CON RESPONSABILIDAD", "CENTRO DE COSTO", "ORIGEN", "RECHAZO NOTIFICACIÓN", "PRÓRROGA", "FECHA COMPROMISO", "EJECUTIVO COMERCIAL", "PLATAFORMA COMERCIAL", "EJECUTIVO FACTORING", "EJECUTIVO COBRANZA", "ÚLTIMA GESTIÓN COBRANZA", "ESTADO GESTIÓN COBRANZA", "DOCUMENTO CEDIDO", "ESTADO NOTA CRÉDITO", "ESTADO RECLAMO", "ULTIMA FECHA PAGO", "CON GARANTÍA", "MANDANTE", "SISTEMA ORIGEN" };
                ClsHelpersExcel.EscribirCabeceraDetalle(ws, ref detalleStartRow, headers);
                EscribirDetalle(ws, data, detalleStartRow);

                archivoByte = pck.GetAsByteArray();
                ClsHelpersExcel.GuardarArchivo(archivoByte);
            }

            return await Task.FromResult(archivoByte);
        }

        private void EscribirDetalle(ExcelWorksheet ws, List<ServiceEntidades.CarteraEN> data, int startRow)
        {
            int row = startRow;
            bool HabilitaTotalDolar = false;
            double TotalMtoDoctoPesos = 0.00, TotalMtoDoctoDolar = 0.00, TotalInteresMoraDolar = 0.00, TotalInteresMoraPesos = 0.00, TotalMtoAnticipoDolar = 0.00, TotalMtoAnticipoPesos = 0.00, TotalSaldoDolar = 0.00, TotalSaldoPesos = 0.00, TotalDeudaDolar = 0.00, TotalDeudaPesos = 0.00, TotalAbonadoDolar = 0.00, TotalAbonadoPesos = 0.00;

            // Recorrer cada detalle y escribir en la hoja
            foreach (var item in data)
            {
                double valorDoctoENPesos = ClsHelpers.CalcularMontoEnPesos(item.ValorDocto, item.IdMoneda, item.FactorCambioActual);
                double montoAnticipoENPesos = ClsHelpers.CalcularMontoEnPesos(item.MontoAnticipo, item.IdMoneda, item.FactorCambioActual);
                double saldoENPesos = ClsHelpers.CalcularMontoEnPesos(item.Saldo, item.IdMoneda, item.FactorCambioActual);
                double deudaENPesos = ClsHelpers.CalcularMontoEnPesos(item.Deuda + item.MontoInteres, item.IdMoneda, item.FactorCambioActual);
                double montoAbonadoENPesos = ClsHelpers.CalcularMontoEnPesos(item.MontoAbonado, item.IdMoneda, item.FactorCambioActual);

                ws.Cells[row, 1].Value = item.ID.ToString();//N°
                ws.Cells[row, 2].Value = item.RutDeudor;//	RUT DEUDOR
                ws.Cells[row, 3].Value = item.DigitoDeudor;//DV
                ws.Cells[row, 4].Value = item.NombreDeudor;// NOMBRE DEUDOR
                ws.Cells[row, 5].Value = item.RutCliente;// RUT CLIENTE
                ws.Cells[row, 6].Value = item.DigitoCliente;//DV
                ws.Cells[row, 7].Value = item.NombreCliente;//NOMBRE CLIENTE
                ws.Cells[row, 8].Value = item.RutEmisor == 0 ? "" : ClsHelpers.ObtieneFormatoRut(item.RutEmisor);//RUT EMISOR
                ws.Cells[row, 9].Value = item.RutEmisor == 0 ? "" : ClsHelpers.ObtenerdigitoVerificador(item.RutEmisor);// DV
                ws.Cells[row, 10].Value = item.TipoDocto;//TIPO FA
                ws.Cells[row, 11].Value = item.Moneda;//MONEDA
                ws.Cells[row, 12].Value = item.IdMoneda == 1 ? "" : ClsHelpers.ObtieneFormatoDecimal(item.FactorCambioActual);//FACTOR CAMBIO AL DIA HOY
                ws.Cells[row, 13].Value = item.TipoDoctoCorta;//TIPO DOCTO.
                ws.Cells[row, 14].Value = item.NroOperacion;//NRO.OPER.;
                ws.Cells[row, 15].Value = item.ConSinCesion == "N" ? "Sin cesión" : "Con cesión";//CON O SIN CESIÓN
                ws.Cells[row, 16].Value = ClsHelpers.ObtieneFormatoDecimal(item.TasaOperacion);//TASA OPERACIÓN
                ws.Cells[row, 17].Value = ClsHelpers.ObtenerFecha(item.FechaCurse);//FECHA CURSE
                ws.Cells[row, 18].Value = item.FechaOtorgada.ToShortDateString();//FECHA OTORGADA
                ws.Cells[row, 19].Value = ClsHelpers.ObtieneFormatoPorcentaje(item.PorcentajeAnticipo);//PORC.ANTICIPO
                ws.Cells[row, 20].Value = item.AfectaExenta;//AFECTA O EXENTA
                ws.Cells[row, 21].Value = item.NroDocto;//NRO DOCTO.
                ws.Cells[row, 22].Value = item.FechaEmision.ToShortDateString();//FECHA EMISIÓN
                ws.Cells[row, 23].Value = item.FechaVcto.ToShortDateString();//FECHA VCTO
                ws.Cells[row, 24].Value = item.FechaVctoOriginal.ToShortDateString();//FECHA ORIGINAL
                ws.Cells[row, 25].Value = item.NroProrrogas;//NRO PRÓRROGAS
                ws.Cells[row, 26].Value = item.DiasMora < 0 ? 0 : item.DiasMora;//DÍAS MORA
                ws.Cells[row, 27].Value = item.TasasMora; // TASAS MORA
                ws.Cells[row, 28].Value = item.ValorDocto;//VALOR DOCTO.
                ws.Cells[row, 29].Value = item.IdMoneda == 1 ? "" : ClsHelpers.ObtieneFormatoMiles(valorDoctoENPesos);//VALOR DOCTO. $ AL DÍA HOY
                ws.Cells[row, 30].Value = item.MontoInteres;//MONTO INTERÉS
                ws.Cells[row, 31].Value = item.MontoAnticipo;//MONTO ANTICIPO
                ws.Cells[row, 32].Value = item.IdMoneda == 1 ? "" : ClsHelpers.ObtieneFormatoMiles(montoAnticipoENPesos);//MONTO ANTICIPO $ AL DÍA HOY
                ws.Cells[row, 33].Value = item.Saldo;//SALDO
                ws.Cells[row, 34].Value = item.IdMoneda == 1 ? "" : ClsHelpers.ObtieneFormatoMiles(saldoENPesos);//SALDO $ AL DÍA HOY
                ws.Cells[row, 35].Value = item.Deuda + Math.Round(item.MontoInteres, 2);//DEUDA
                ws.Cells[row, 36].Value = item.IdMoneda == 1 ? "" : ClsHelpers.ObtieneFormatoMiles(deudaENPesos);//DEUDA $ AL DÍA HOY
                ws.Cells[row, 37].Value = item.EstadoDocto == "" ? "" : item.EstadoDocto;// ESTADO DOCTO.
                ws.Cells[row, 38].Value = item.MontoAbonado;// MONTO ABONADO
                ws.Cells[row, 39].Value = item.IdMoneda == 1 ? "" : ClsHelpers.ObtieneFormatoMiles(montoAbonadoENPesos);//MONTO ABONADO $ AL DÍA HOY
                ws.Cells[row, 40].Value = item.ConResponsabilidad;//CON RESPONSABILIDAD
                ws.Cells[row, 41].Value = item.CentroCosto;//CENTRO DE COSTO
                ws.Cells[row, 42].Value = item.Origen;//ORIGEN
                ws.Cells[row, 43].Value = item.RechazoNotificacion;//RECHAZO NOTIFICACIÓN
                ws.Cells[row, 44].Value = item.Prorroga;//PRÓRROGA
                ws.Cells[row, 45].Value = ClsHelpers.ObtenerFecha(item.FechaCompromiso);//FECHA COMPROMISO
                ws.Cells[row, 46].Value = item.EjecutivoComercial;//EJECUTIVO COMERCIAL
                ws.Cells[row, 47].Value = item.PlataformaComercial;//PLATAFORMA COMERCIAL
                ws.Cells[row, 48].Value = item.EjecutivoFactoring;//EJECUTIVO FACTORING
                ws.Cells[row, 49].Value = item.EjecutivoCobranza;// EJECUTIVO COBRANZA
                ws.Cells[row, 50].Value = item.UltimaGestionCobranza;//ÚLTIMA GESTIÓN COBRANZA
                ws.Cells[row, 51].Value = item.EstadoGestionCobranza;//ESTADO GESTIÓN COBRANZA
                ws.Cells[row, 52].Value = ClsHelpers.ObtenerEstadoDocumentoCedidor(item.DocumentoCedido);//DOCUMENTO CEDIDO
                ws.Cells[row, 53].Value = item.EstadoNotaCredito;//ESTADO NOTA CRÉDITO
                ws.Cells[row, 54].Value = item.EstadoReclamo;//ESTADO RECLAMO
                ws.Cells[row, 55].Value = ClsHelpers.ObtenerFecha(item.UltimaFechaPago);//ULTIMA FECHA PAGO
                ws.Cells[row, 56].Value = item.ConGarantia;//CON GARANTÍA
                ws.Cells[row, 57].Value = item.Mandante;//MANDANTE
                ws.Cells[row, 58].Value = item.SistemaOrigen;// SISTEMA ORIGEN

                string formatoMoneda = ClsHelpers.DevuelveFormatoMoneda(item.IdMoneda);
                string formatoMonedaPesos = ClsHelpers.DevuelveFormatoMoneda(1);
                string formatoMonedaDecimal = ClsHelpers.DevuelveFormatoMoneda(2);
                ws.Cells[row, 2].Style.Numberformat.Format = formatoMonedaPesos;
                ws.Cells[row, 5].Style.Numberformat.Format = formatoMonedaPesos;
                ws.Cells[row, 8].Style.Numberformat.Format = formatoMonedaPesos;
                ws.Cells[row, 29].Style.Numberformat.Format = formatoMonedaPesos;
                ws.Cells[row, 32].Style.Numberformat.Format = formatoMonedaPesos;
                ws.Cells[row, 34].Style.Numberformat.Format = formatoMonedaPesos;
                ws.Cells[row, 36].Style.Numberformat.Format = formatoMonedaPesos;
                ws.Cells[row, 39].Style.Numberformat.Format = formatoMonedaPesos;
                ws.Cells[row, 28].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 30].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 31].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 33].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 35].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 38].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 12].Style.Numberformat.Format = formatoMonedaDecimal;
                ws.Cells[row, 15].Style.Numberformat.Format = formatoMonedaDecimal;
                ws.Cells[row, 18].Style.Numberformat.Format = formatoMonedaDecimal;
                ws.Cells[row, 22, row, 24].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoFecha();

                // Ajustar el alto de la fila (opcional)
                ws.Row(row).CustomHeight = true;
                ws.Row(row).Height = 20;

                ClsHelpersExcel.AplicarBorder(ws.Cells[row, 1, row, 58]);

                // Incrementar la fila para la próxima iteración
                row++;


                if (item.IdMoneda == 1)
                {
                    TotalMtoDoctoPesos += item.ValorDocto;
                    TotalInteresMoraPesos += item.MontoInteres;
                    TotalMtoAnticipoPesos += item.MontoAnticipo;
                    TotalSaldoPesos += item.Saldo;
                    TotalDeudaPesos += item.Deuda + item.MontoInteres;
                    TotalAbonadoPesos += item.MontoAbonado;
                }
                else
                {
                    HabilitaTotalDolar = true;
                    TotalMtoDoctoDolar += item.ValorDocto;
                    TotalInteresMoraDolar += item.MontoInteres;
                    TotalMtoAnticipoDolar += item.MontoAnticipo;
                    TotalSaldoDolar += item.Saldo;
                    TotalDeudaDolar += item.Deuda + item.MontoInteres;
                    TotalAbonadoDolar += item.MontoAbonado;
                }
            }

            ws.Cells[row, 26].Value = "Total en Pesos:";
            ws.Cells[row, 28].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1);
            ws.Cells[row, 28].Value = TotalMtoDoctoPesos;
            ws.Cells[row, 30, row, 31].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1);
            ws.Cells[row, 30].Value = TotalInteresMoraPesos;
            ws.Cells[row, 31].Value = TotalMtoAnticipoPesos;
            ws.Cells[row, 33].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1);
            ws.Cells[row, 33].Value = TotalSaldoPesos;
            ws.Cells[row, 35].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1);
            ws.Cells[row, 35].Value = TotalDeudaPesos;
            ws.Cells[row, 38].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1);
            ws.Cells[row, 38].Value = TotalAbonadoPesos;

            row++;

            if (HabilitaTotalDolar)
            {
                ws.Cells[row, 26].Value = "Total en Dólar:";
                ws.Cells[row, 28].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1);
                ws.Cells[row, 28].Value = TotalMtoDoctoDolar;
                ws.Cells[row, 30, row, 31].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1);
                ws.Cells[row, 30].Value = TotalInteresMoraDolar;
                ws.Cells[row, 31].Value = TotalMtoAnticipoDolar;
                ws.Cells[row, 33].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1);
                ws.Cells[row, 33].Value = TotalSaldoDolar;
                ws.Cells[row, 35].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1);
                ws.Cells[row, 35].Value = TotalDeudaDolar;
                ws.Cells[row, 38].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1);
                ws.Cells[row, 38].Value = TotalAbonadoDolar;
            }

            // Ajustar el ancho de las columnas (opcional)
            for (int col = 1; col < 58; col++)
            {
                ws.Column(col).AutoFit();
            }
        }

        #endregion 

        #region "INFORME DOCUMENTO COMPRADO"

        public async Task<byte[]> GetDocumentoComprado(List<ServiceEntidades.DocumentoCompradoEN> data, DateTime fechaCurseDesde, DateTime fechaCurseHasta)
        {
            byte[] archivoByte = await GeneraArchivoExcelDocumentoComprado(data, fechaCurseDesde, fechaCurseHasta);

            return await Task.FromResult(archivoByte);
        }

        private async Task<byte[]> GeneraArchivoExcelDocumentoComprado(List<ServiceEntidades.DocumentoCompradoEN> data, DateTime fechaCurseDesde, DateTime fechaCurseHasta)
        {
            const int tituloRow = 3;
            const int headerRow = 6;
            Byte[] archivoByte = null;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("DOCUMENTOS COMPRADOS");
                ClsHelpersExcel.InsertarImagen(ws);
                ClsHelpersExcel.ConfigurarTitulo(ws, tituloRow, 10, "DOCUMENTOS COMPRADOS ");

                // La fila donde empieza el detalle
                int detalleStartRow = headerRow + 2;
                string[] headers = { "R.U.T. CLIENTE", "CLIENTE", "R.U.T. DEUDOR", "DEUDOR", "RUT EMISOR", "TIPO DOCTO", "AFECTA O EXENTA", "ID DOCTO", "Nº DOCTO", "FECHA EMISIÓN", "FECHA VCTO.", "MONTO DOCTOS.", "Nº OPERAC.", "MONEDA", "FEC. OPERAC.", "CON O SIN CECIÓN", "TASA OP	COMISIÓN", "ORIGEN OPERAC.", "DIF.PRECIO", "% ANTICIPO", "ANT.BRUTO" };
                EscribirCabeceraReporteDocumentoComprado(ws, fechaCurseDesde, fechaCurseHasta, headerRow);
                ClsHelpersExcel.EscribirCabeceraDetalle(ws, ref detalleStartRow, headers);
                EscribirDetalle(ws, data, detalleStartRow);

                archivoByte = pck.GetAsByteArray();
                ClsHelpersExcel.GuardarArchivo(archivoByte);
            }

            return await Task.FromResult(archivoByte);
        }

        private void EscribirCabeceraReporteDocumentoComprado(ExcelWorksheet ws, DateTime fechaCurseDesde, DateTime fechaCurseHasta, int row)
        {
            //var primerdatos = data[0]; // Obtener el primer cliente de la lista
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, 1].Value = "Período: ";
            ws.Cells[row, 2].Value = fechaCurseDesde.ToShortDateString() + " - " + fechaCurseHasta.ToShortDateString();
            ws.Cells[row, 5, row, 8].Merge = true;
        }

        private void EscribirDetalle(ExcelWorksheet ws, List<ServiceEntidades.DocumentoCompradoEN> data, int startRow)
        {
            int row = startRow;

            // Recorrer cada detalle y escribir en la hoja
            foreach (var item in data)
            {
                //double valorDoctoENPesos = ClsHelpers.CalcularMontoEnPesos(item.ValorDocto, item.IdMoneda, item.FactorCambioActual);

                ws.Cells[row, 1].Value = ClsHelpers.ObtieneFormatoRut(item.RutCliente, ClsHelpers.ObtenerdigitoVerificador(item.RutCliente));//"R.U.T. CLIENTE"
                ws.Cells[row, 2].Value = item.NombreCliente;//"CLIENTE"
                ws.Cells[row, 3].Value = ClsHelpers.ObtieneFormatoRut(item.RutDeudor, ClsHelpers.ObtenerdigitoVerificador(item.RutDeudor));//"R.U.T. DEUDOR"
                ws.Cells[row, 4].Value = item.NombreDeudor;//"DEUDOR"
                ws.Cells[row, 5].Value = ClsHelpers.ObtieneFormatoRut(item.RutEmisor, ClsHelpers.ObtenerdigitoVerificador(item.RutEmisor));//"R.U.T. EMISOR"
                ws.Cells[row, 6].Value = item.TipoDocto;//"TIPO DOCTO"
                ws.Cells[row, 7].Value = item.AfectaExenta;//"AFECTA O EXENTA"
                ws.Cells[row, 8].Value = item.IDDoc;//"ID DOCTO"
                ws.Cells[row, 9].Value = item.NroDocto;//"Nº DOCTO"
                ws.Cells[row, 10].Value = item.FechaEmision.ToShortDateString();//"FECHA EMISIÓN"
                ws.Cells[row, 11].Value = item.FechaVcto.ToShortDateString();//"FECHA VCTO."
                ws.Cells[row, 12].Value = item.MontoDocto;//"MONTO DOCTOS."
                ws.Cells[row, 13].Value = item.NroOperacion;//"Nº OPERAC."
                ws.Cells[row, 14].Value = item.Moneda;//"MONEDA"
                ws.Cells[row, 15].Value = item.FechaOtorgado.ToShortDateString();//"FEC. OPERAC."
                ws.Cells[row, 16].Value = item.Cesion == "N" ? "Sin cesión" : "Con cesión";//"CON O SIN CECIÓN"
                ws.Cells[row, 17].Value = ClsHelpers.ObtieneFormatoDecimal(item.TasaNegocio);//"TASA OP	COMISIÓN"
                ws.Cells[row, 18].Value = item.OrigenOperacion;//"ORIGEN OPERAC."
                ws.Cells[row, 19].Value = item.DiferenciaPrecio;//"DIF.PRECIO"
                ws.Cells[row, 20].Value = ClsHelpers.ObtieneFormatoPorcentaje(item.PorcentajeAnticipo);//"% ANTICIPO"
                ws.Cells[row, 21].Value = item.MontoAnticipo;//"ANT.BRUTO"

                string formatoMoneda = ClsHelpers.DevuelveFormatoMoneda(item.IdMoneda);
                string formatoMonedaDecimal = ClsHelpers.DevuelveFormatoMoneda(2);

                ws.Cells[row, 12].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 19].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 21].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 17].Style.Numberformat.Format = formatoMonedaDecimal;
                ws.Cells[row, 10, row, 11].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoFecha();
                ws.Cells[row, 15].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoFecha();

                // Ajustar el alto de la fila (opcional)
                ws.Row(row).CustomHeight = true;
                ws.Row(row).Height = 20;

                ClsHelpersExcel.AplicarBorder(ws.Cells[row, 1, row, 21]);

                // Incrementar la fila para la próxima iteración
                row++;
            }

            // Ajustar el ancho de las columnas (opcional)
            for (int col = 1; col <= 21; col++)
            {
                ws.Column(col).AutoFit();
            }
        }


        #endregion

        #region "INFORME DOCUMENTO DEVENGADO"

        public async Task<byte[]> GetDocumentoDevengado(List<ServiceEntidades.DocumentoDevengadoEN> data)
        {
            byte[] archivoByte = await GeneraArchivoExcelDocumentoDevengado(data);

            return await Task.FromResult(archivoByte);
        }

        private async Task<byte[]> GeneraArchivoExcelDocumentoDevengado(List<ServiceEntidades.DocumentoDevengadoEN> data)
        {
            const int tituloRow = 3;
            const int headerRow = 6;
            Byte[] archivoByte = null;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("DOCUMENTOS DEVENGADO");
                ClsHelpersExcel.InsertarImagen(ws);
                ClsHelpersExcel.ConfigurarTitulo(ws, tituloRow, 10, "DOCUMENTOS DEVENGADO");

                // La fila donde empieza el detalle
                int detalleStartRow = headerRow + 2;

                string[] headers = { "TIPO DOCTO", "MONEDA", "NRO OPERACIÓN", "CENTRO COSTO", "DIFERENCIA PRECIO", "POR DEVENGAR", "DEVENGO VIGENTE", "DEVENGO MOROSO", "DEVENGO PAGADO", " DEVENGO DEVUELTO", " TOTAL MES", "CON O SIN CESIÓN" };
                ClsHelpersExcel.EscribirCabeceraDetalle(ws, ref detalleStartRow, headers);
                EscribirDetalle(ws, data, detalleStartRow);

                archivoByte = pck.GetAsByteArray();
                ClsHelpersExcel.GuardarArchivo(archivoByte);
            }

            return await Task.FromResult(archivoByte);
        }

        private void EscribirDetalle(ExcelWorksheet ws, List<ServiceEntidades.DocumentoDevengadoEN> data, int startRow)
        {
            int row = startRow;

            // Recorrer cada detalle y escribir en la hoja
            foreach (var item in data)
            {
                ws.Cells[row, 1].Value = item.TipoDocto;// "TIPO DOCTO"
                ws.Cells[row, 2].Value = item.Moneda;// "MONEDA"
                ws.Cells[row, 3].Value = item.NroOperacion;// "NRO OPERACIÓN"
                ws.Cells[row, 4].Value = item.CentroCosto;// "CENTRO COSTO"
                ws.Cells[row, 5].Value = item.DiferenciaPrecio;// "DIFERENCIA PRECIO"
                ws.Cells[row, 6].Value = item.PorDevengar;// "POR DEVENGAR"
                ws.Cells[row, 7].Value = item.DevengoVigente;// "DEVENGO VIGENTE"
                ws.Cells[row, 8].Value = item.DevengoMoroso;// "DEVENGO MOROSO"
                ws.Cells[row, 9].Value = item.DevengoPagado;// "DEVENGO PAGADO"
                ws.Cells[row, 10].Value = item.DevengoDevuelto;// " DEVENGO DEVUELTO"
                ws.Cells[row, 11].Value = item.TotalMes;// " TOTAL MES"
                ws.Cells[row, 12].Value = item.Cesión;// "CON O SIN CESIÓN"

                string formatoMoneda = ClsHelpers.DevuelveFormatoMoneda(item.IdMoneda);

                ws.Cells[row, 5, row, 11].Style.Numberformat.Format = formatoMoneda;

                // Ajustar el alto de la fila (opcional)
                ws.Row(row).CustomHeight = true;
                ws.Row(row).Height = 20;

                ClsHelpersExcel.AplicarBorder(ws.Cells[row, 1, row, 12]);

                // Incrementar la fila para la próxima iteración
                row++;
            }

            // Ajustar el ancho de las columnas (opcional)
            for (int col = 1; col <= 12; col++)
            {
                ws.Column(col).AutoFit();
            }
        }

        #endregion

        #region "INFORME OPERACIONES CURSADAS"

        public async Task<byte[]> GetOperacionCursadas(List<ServiceEntidades.OperacionCursadaEN> data, DateTime fechaCurseDesde, DateTime fechaCurseHasta)
        {
            byte[] archivoByte = await GeneraArchivoExcelOperacionCursadas(data, fechaCurseDesde, fechaCurseHasta);

            return await Task.FromResult(archivoByte);
        }

        private async Task<byte[]> GeneraArchivoExcelOperacionCursadas(List<ServiceEntidades.OperacionCursadaEN> data, DateTime fechaCurseDesde, DateTime fechaCurseHasta)
        {
            const int tituloRow = 3;
            const int headerRow = 6;
            Byte[] archivoByte = null;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("OPERACIÓN CURSADAS");
                ClsHelpersExcel.InsertarImagen(ws);
                ClsHelpersExcel.ConfigurarTitulo(ws, tituloRow, 10, "OPERACIÓN CURSADAS");

                // La fila donde empieza el detalle
                int detalleStartRow = headerRow + 2;
                EscribirCabeceraReporteOperacionCursada(ws, fechaCurseDesde, fechaCurseHasta, headerRow);
                string[] headers = { "RUT CLIENTE", "CLIENTE", "FECHA OTORGADO", "NRO OPERACION", "ORIGEN OPERACION", "CENTRO COSTO", "TIPO DOCTO", "MONEDA", "FACTOR CAMBIO", "MONTO", "MONTO $ AL DIA HOY", "PORC. ANTICIPO", "MONTO ANTICIPO", "MONTO ANTICIPO $ AL DIA HOY", "TASA NEGOCIO", "DIFERENCIA PRECIO", "DIFERENCIA PRECIO $ AL DIA HOY", "GASTO", "GASTO $ AL DIA HOY", "RECAUDACIÓN", "RECAUDACIÓN $ AL DIA HOY", "EXCEDENTE", "EXCEDENTE $ AL DIA HOY", "MONTO GIRO", "MONTO GIRO $ AL DIA HOY", "PLA. PROMEDIO", "EJECUTIVO", "CON O SIN CESIÓN" };
                ClsHelpersExcel.EscribirCabeceraDetalle(ws, ref detalleStartRow, headers);
                EscribirDetalle(ws, data, detalleStartRow);

                archivoByte = pck.GetAsByteArray();
                ClsHelpersExcel.GuardarArchivo(archivoByte);
            }

            return await Task.FromResult(archivoByte);
        }

        private void EscribirDetalle(ExcelWorksheet ws, List<ServiceEntidades.OperacionCursadaEN> data, int startRow)
        {
            int row = startRow;


            // Recorrer cada detalle y escribir en la hoja
            foreach (var item in data)
            {
                double montoENPesos = ClsHelpers.CalcularMontoEnPesos(item.Monto, item.IdMoneda, item.FactorCambioActual);
                double montoAnticipoENPesos = ClsHelpers.CalcularMontoEnPesos(item.MontoAnticipo, item.IdMoneda, item.FactorCambioActual);
                double diferenciaPrecioENPesos = ClsHelpers.CalcularMontoEnPesos(item.DiferenciaPrecio, item.IdMoneda, item.FactorCambioActual);
                double gastoENPesos = ClsHelpers.CalcularMontoEnPesos(item.Gastos, item.IdMoneda, item.FactorCambioActual);
                double recaudacionENPesos = ClsHelpers.CalcularMontoEnPesos(item.Recaudacion, item.IdMoneda, item.FactorCambioActual);
                double excedenteENPesos = ClsHelpers.CalcularMontoEnPesos(item.Excedentes, item.IdMoneda, item.FactorCambioActual);
                double montoGiroENPesos = ClsHelpers.CalcularMontoEnPesos(item.MontoGiro, item.IdMoneda, item.FactorCambioActual);

                ws.Cells[row, 1].Value = ClsHelpers.ObtieneFormatoRut(item.RutCliente, ClsHelpers.ObtenerdigitoVerificador(item.RutCliente));// "RUT CLIENTE"
                ws.Cells[row, 2].Value = item.NombreCliente;// "CLIENTE"
                ws.Cells[row, 3].Value = item.FechaOtorgado.ToShortDateString();// "FECHA OTORGADO"
                ws.Cells[row, 4].Value = item.NroOperacion;//"NRO OPERACION"
                ws.Cells[row, 5].Value = item.OrigenOperacion;//"ORIGEN OPERACION"
                ws.Cells[row, 6].Value = item.CentroCosto;//"CENTRO COSTO"
                ws.Cells[row, 7].Value = item.TipoDocto;//"TIPO DOCTO"
                ws.Cells[row, 8].Value = item.Moneda;//"MONEDA"
                ws.Cells[row, 9].Value = item.FactorCambioActual;//"FACTOR CAMBIO"
                ws.Cells[row, 10].Value = item.Monto;//"MONTO"
                ws.Cells[row, 11].Value = montoENPesos;//"MONTO $ AL DIA HOY"
                ws.Cells[row, 12].Value = ClsHelpers.ObtieneFormatoPorcentaje(item.PorcentajeAnticipo);//"PORC. ANTICIPO"
                ws.Cells[row, 13].Value = item.MontoAnticipo;//"MONTO ANTICIPO"
                ws.Cells[row, 14].Value = montoAnticipoENPesos;//"MONTO ANTICIPO $ AL DIA HOY"
                ws.Cells[row, 15].Value = item.TasaNegocio;//"TASA NEGOCIO"
                ws.Cells[row, 16].Value = item.DiferenciaPrecio;//"DIFERENCIA PRECIO"
                ws.Cells[row, 17].Value = diferenciaPrecioENPesos;//"DIFERENCIA PRECIO $ AL DIA HOY"
                ws.Cells[row, 18].Value = item.Gastos;//"GASTO"
                ws.Cells[row, 19].Value = gastoENPesos;//"GASTO $ AL DIA HOY"
                ws.Cells[row, 20].Value = item.Recaudacion;//"RECAUDACIÓN"
                ws.Cells[row, 21].Value = recaudacionENPesos;//"RECAUDACIÓN $ AL DIA HOY"
                ws.Cells[row, 22].Value = item.Excedentes;//"EXCEDENTE"
                ws.Cells[row, 23].Value = excedenteENPesos;//"EXCEDENTE $ AL DIA HOY"
                ws.Cells[row, 24].Value = item.MontoGiro;//"MONTO GIRO"
                ws.Cells[row, 25].Value = montoGiroENPesos;//"MONTO GIRO $ AL DIA HOY"
                ws.Cells[row, 26].Value = item.Promedio;//"PLA. PROMEDIO"
                ws.Cells[row, 27].Value = item.Ejecutivo;//"EJECUTIVO"
                ws.Cells[row, 28].Value = item.Cesion == "N" ? "Sin cesión" : "Con cesión";//"CON O SIN CESIÓN" }

                string formatoMoneda = ClsHelpers.DevuelveFormatoMoneda(item.IdMoneda);
                string formatoMonedaPesos = ClsHelpers.DevuelveFormatoMoneda(1);
                string formatoMonedaDecimal = ClsHelpers.DevuelveFormatoMoneda(2);

                ws.Cells[row, 3].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoFecha();
                ws.Cells[row, 9].Style.Numberformat.Format = formatoMonedaDecimal;
                ws.Cells[row, 15].Style.Numberformat.Format = formatoMonedaDecimal;
                ws.Cells[row, 10].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 11].Style.Numberformat.Format = formatoMonedaPesos;
                ws.Cells[row, 13].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 14].Style.Numberformat.Format = formatoMonedaPesos;
                ws.Cells[row, 16].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 17].Style.Numberformat.Format = formatoMonedaPesos;
                ws.Cells[row, 18].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 19].Style.Numberformat.Format = formatoMonedaPesos;
                ws.Cells[row, 20].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 21].Style.Numberformat.Format = formatoMonedaPesos;
                ws.Cells[row, 22].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 23].Style.Numberformat.Format = formatoMonedaPesos;
                ws.Cells[row, 24].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 25].Style.Numberformat.Format = formatoMonedaPesos;

                // Ajustar el alto de la fila (opcional)
                ws.Row(row).CustomHeight = true;
                ws.Row(row).Height = 20;

                ClsHelpersExcel.AplicarBorder(ws.Cells[row, 1, row, 28]);

                // Incrementar la fila para la próxima iteración
                row++;
            }

            // Ajustar el ancho de las columnas (opcional)
            for (int col = 1; col <= 28; col++)
            {
                ws.Column(col).AutoFit();
            }
        }

        private void EscribirCabeceraReporteOperacionCursada(ExcelWorksheet ws, DateTime fechaCurseDesde, DateTime fechaCurseHasta, int row)
        {
            //var primerdatos = data[0]; // Obtener el primer cliente de la lista
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, 1].Value = "Período: ";
            ws.Cells[row, 2].Value = fechaCurseDesde.ToShortDateString() + " - " + fechaCurseHasta.ToShortDateString();
            ws.Cells[row, 5, row, 8].Merge = true;
        }

        #endregion


        #region "INFORME OPERACIONES CURSADAS"

        public async Task<byte[]> GetExcedentes(List<ServiceEntidades.ExcedentesEN> data)
        {
            byte[] archivoByte = null;
            //byte[] archivoByte = await GeneraArchivoExcelGetExcedentes(data, fechaCurseDesde, fechaCurseHasta);

            return await Task.FromResult(archivoByte);
        }
        #endregion

    }

}
