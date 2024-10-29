using System.Drawing; 
using System.IO;      
using OfficeOpenXml.Drawing; 
using OfficeOpenXml;  
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Threading.Tasks;

using WebApiRepoteria.AL.Entidades;
using WebApiRepoteria.BLL.BusinessHelpers;
using WebApiRepoteria.BLL.Interfaz;
using System.Configuration;
using WebApiRepoteria.AL.Interfaz;
using WebApiRepoteria.AL.Seguridad;
using System;

namespace WebApiRepoteria.BLL.Implementos
{
    public class AdministracionExportarExcelBLL : IAdministracionExportarExcelBLL
    {
        //objeto
        private readonly CuentaBLL ClsCuentaBLL = new CuentaBLL();
        private readonly RutinasGeneralesHelpersBLL ClsHelpers = new RutinasGeneralesHelpersBLL();
        private readonly HelpersExcel ClsHelpersExcel = new HelpersExcel();
        private readonly IEncriptaEN ObjEncripta = new EncriptaEN();

        //variables privadas
        private string rutaImagen, rutaGuardarEXCEL;
        private bool registrarArchivo;

        public AdministracionExportarExcelBLL()
        {
            //rutaImagen = ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["rutaLogo"].ToString());
            //rutaGuardarEXCEL = ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["rutaGuardarEXCEL"].ToString());
            //registrarArchivo = bool.Parse(ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["registrarArchivo"].ToString()));

            rutaImagen = ConfigurationManager.AppSettings["rutaLogo"].ToString();
            rutaGuardarEXCEL = ConfigurationManager.AppSettings["rutaGuardarEXCEL"].ToString();
            registrarArchivo = bool.Parse(ConfigurationManager.AppSettings["registrarArchivo"].ToString());
        }

        #region "CUENTAS POR PAGAR"

        #region "EXPORTAR A EXCEL LAS CUENTAS POR PAGAR POR CLIENTE"

        public async Task<byte[]> GetCuentasPorPagarPorCliente(List<ServiceEntidades.CuentaPorPagarEN> cuentaPorPagar)
        {
            //var Reporte = await ClsCuentaBLL.GetDataCuentasPorPagarPorClienteBLL(cuentaPorPagar);
            byte[] archivoByte = await GeneraArchivoExcelCuentaPorPagarPorCliente(cuentaPorPagar);

            return await Task.FromResult(archivoByte);
        }

        private async Task<byte[]> GeneraArchivoExcelCuentaPorPagarPorCliente(List<ServiceEntidades.CuentaPorPagarEN> DataCuentasPorPagar)
        {
            const int tituloRow = 3;
            const int headerRow = 7;
            Byte[] archivoByte = null;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("CUENTAS POR PAGAR");
                ClsHelpersExcel.InsertarImagen(ws);
                ClsHelpersExcel.ConfigurarTitulo(ws, tituloRow, 9, "TODAS LAS CUENTAS POR PAGAR");
                EscribirCabeceraReporteCuentaPorPagarPorCliente(ws, DataCuentasPorPagar, headerRow);

                int detalleStartRow = headerRow + 2; // La fila donde empieza el detalle
                string[] headers = { "NRO CUENTA", "FECHA", "MONEDA", "TIPO CUENTA", "DESCRIPCIÓN", "FACTOR CAMBIO ACTUAL", "MONTO", "MONTO $ AL DIA HOY", "ESTADO" };
                ClsHelpersExcel.EscribirCabeceraDetalle(ws, ref detalleStartRow, headers);
                EscribirDetalleCuentaPorPagarPorCliente(ws, DataCuentasPorPagar, detalleStartRow);

                archivoByte = pck.GetAsByteArray();
                ClsHelpersExcel.GuardarArchivo(archivoByte);
            }

            return await Task.FromResult(archivoByte);
        }

        private void EscribirCabeceraReporteCuentaPorPagarPorCliente(ExcelWorksheet ws, List<ServiceEntidades.CuentaPorPagarEN> data, int row)
        {
            var primerdatos = data[0]; // Obtener el primer cliente de la lista
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, 1].Value = "Rut: ";
            ws.Cells[row, 2].Value = ClsHelpers.ObtieneFormatoRut(primerdatos.RutCliente, primerdatos.DigitoCliente);
            ws.Cells[row, 4].Value = "Nombre Cliente: ";
            ws.Cells[row, 5].Value = primerdatos.NombreCliente;
            ws.Cells[row, 5, row, 8].Merge = true;
        }

        private void EscribirDetalleCuentaPorPagarPorCliente(ExcelWorksheet ws, List<ServiceEntidades.CuentaPorPagarEN> detalles, int startRow)
        {
            int row = startRow;

            // Recorrer cada detalle y escribir en la hoja
            foreach (var item in detalles)
            {
                double montoENPesos = ClsHelpers.CalcularMontoEnPesos(item.ValorCuenta, item.IdMoneda, item.FactorCambio);
                string formatoMoneda = ClsHelpers.DevuelveFormatoMoneda(item.IdMoneda);

                ws.Cells[row, 1].Value = item.NumeroCuenta;
                ws.Cells[row, 2].Value = item.Fecha;
                ws.Cells[row, 2].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoFecha();
                ws.Cells[row, 3].Value = item.Moneda;
                ws.Cells[row, 4].Value = item.TipoCuenta;
                ws.Cells[row, 5].Value = item.Descripcion;

                // Manejo de FactorCambio
                ws.Cells[row, 6].Value = item.FactorCambio == 0.00 ? "" : ClsHelpers.ObtieneFormatoDecimal(item.FactorCambio);
                ws.Cells[row, 6].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(2); // Usar formato para el caso 1

                // Aplicar formato y valor a celdas monetarias
                ws.Cells[row, 7].Value = item.ValorCuenta;
                ws.Cells[row, 7].Style.Numberformat.Format = formatoMoneda;

                ws.Cells[row, 8].Value = montoENPesos;
                ws.Cells[row, 8].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1); // Usar formato para el caso 1

                ws.Cells[row, 9].Value = item.Estado;

                // Ajustar el alto de la fila (opcional)
                ws.Row(row).CustomHeight = true;
                ws.Row(row).Height = 20;

                ClsHelpersExcel.AplicarBorder(ws.Cells[row, 1, row, 9]);

                // Incrementar la fila para la próxima iteración
                row++;
            }

            // Ajustar el ancho de las columnas (opcional)
            for (int col = 1; col <= 10; col++)
            {
                ws.Column(col).AutoFit();
            }
        }

        #endregion

        #region "EXPORTAR A EXCEL TODAS LAS CUENTAS POR PAGAR"

        public async Task<byte[]> GetCuentasPorPagar(List<ServiceEntidades.CuentaPorPagarEN> cuentaPorPagar)
        {
            byte[] archivoByte = await GeneraArchivoExcelTodasCuentaPorPagar(cuentaPorPagar);

            return await Task.FromResult(archivoByte);
        }

        private async Task<byte[]> GeneraArchivoExcelTodasCuentaPorPagar(List<ServiceEntidades.CuentaPorPagarEN> DataCuentasPorPagar)
        {
            const int tituloRow = 3;
            const int headerRow = 7;
            Byte[] archivoByte = null;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("CUENTAS POR PAGAR");
                ClsHelpersExcel.InsertarImagen(ws);
                ClsHelpersExcel.ConfigurarTitulo(ws, tituloRow, 17, "TODAS LAS CUENTAS POR PAGAR");
                ClsHelpersExcel.EscribirCabeceraReporteSoloFecha(ws, headerRow);

                int detalleStartRow = headerRow + 2; // La fila donde empieza el detalle
                string[] headers = { "RUT CLIENTE", "RAZÓN SOCIAL", "EJECUTIVO", "NRO CUENTA", "FECHA", "MONEDA", "TIPO CUENTA", "DESCRIPCIÓN", "FACTOR CAMBIO ACTUAL", "MONTO", "MONTO $ AL DIA HOY", "FECHA PAGO", "NRO PAGO", "NRO DOCTO", "RUT DEUDOR", "RAZÓN SOCIAL", "ESTADO" };
                ClsHelpersExcel.EscribirCabeceraDetalle(ws, ref detalleStartRow, headers);
                EscribirDetalleTodasCuentaPorPagar(ws, DataCuentasPorPagar, detalleStartRow);

                archivoByte = pck.GetAsByteArray();
                ClsHelpersExcel.GuardarArchivo(archivoByte);
            }

            return await Task.FromResult(archivoByte);
        }

        private void EscribirDetalleTodasCuentaPorPagar(ExcelWorksheet ws, List<ServiceEntidades.CuentaPorPagarEN> detalles, int startRow)
        {
            int row = startRow;

            // Recorrer cada detalle y escribir en la hoja
            foreach (var item in detalles)
            {
                double montoENPesos = ClsHelpers.CalcularMontoEnPesos(item.ValorCuenta, item.IdMoneda, item.FactorCambio);
                string formatoMoneda = ClsHelpers.DevuelveFormatoMoneda(item.IdMoneda);

                ws.Cells[row, 1].Value = ClsHelpers.ObtieneFormatoRut(item.RutCliente, item.DigitoCliente);
                ws.Cells[row, 2].Value = item.NombreCliente;
                ws.Cells[row, 3].Value = item.NombreEjecutivo;
                ws.Cells[row, 4].Value = item.NumeroCuenta;
                ws.Cells[row, 5].Value = item.Fecha.ToShortDateString();
                ws.Cells[row, 5].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(2); // Usar formato para el caso 1
                ws.Cells[row, 6].Value = item.Moneda;
                ws.Cells[row, 7].Value = item.TipoCuenta;
                ws.Cells[row, 8].Value = item.Descripcion;

                // Manejo de FactorCambio
                ws.Cells[row, 9].Value = item.FactorCambio == 0.00 ? "" : ClsHelpers.ObtieneFormatoDecimal(item.FactorCambio);
                ws.Cells[row, 9].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(2);

                // Aplicar formato y valor a celdas monetarias
                ws.Cells[row, 10].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 10].Value = item.ValorCuenta;

                ws.Cells[row, 11].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1); // Usar formato para el caso 1
                ws.Cells[row, 11].Value = montoENPesos;

                ws.Cells[row, 12].Value = item.FechaPago.ToShortDateString();
                ws.Cells[row, 12].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoFecha();
                ws.Cells[row, 13].Value = item.NroPago;
                ws.Cells[row, 14].Value = item.NroDocto;
                ws.Cells[row, 15].Value = ClsHelpers.ObtieneFormatoRut(item.RutDeudor, item.DigitoDeudor);
                ws.Cells[row, 16].Value = item.NombreDeudor;
                ws.Cells[row, 17].Value = item.Estado;

                // Ajustar el alto de la fila (opcional)
                ws.Row(row).CustomHeight = true;
                ws.Row(row).Height = 20;

                ClsHelpersExcel.AplicarBorder(ws.Cells[row, 1, row, 9]);

                // Incrementar la fila para la próxima iteración
                row++;
            }

            // Ajustar el ancho de las columnas (opcional)
            for (int col = 1; col <= 17; col++)
            {
                ws.Column(col).AutoFit();
            }
        }

        #endregion 

        #endregion 

        #region "CUENTAS POR COBRAR"

        #region "EXPORTAR A EXCEL TODAS LAS CUENTAS POR COBRAR"

        public async Task<byte[]> GetCuentasPorCobrar(List<ServiceEntidades.CuentaPorCobrarEN> data)
        {
            byte[] archivoByte = await GeneraArchivoExcelTodasCuentaPorCobrar(data);

            return await Task.FromResult(archivoByte);
        }

        private async Task<byte[]> GeneraArchivoExcelTodasCuentaPorCobrar(List<ServiceEntidades.CuentaPorCobrarEN> DataCuentasPorPagar)
        {
            const int tituloRow = 2;
            const int headerRow = 7;
            Byte[] archivoByte = null;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("CUENTAS POR COBRAR");
                ClsHelpersExcel.InsertarImagen(ws);
                ClsHelpersExcel.ConfigurarTitulo(ws, tituloRow, 14, "TODAS LAS CUENTAS POR COBRAR");
                ClsHelpersExcel.EscribirCabeceraReporteSoloFecha(ws, headerRow);

                int detalleStartRow = headerRow + 2; // La fila donde empieza el detalle
                string[] headers = { "IDENTIFICACIÓN", "RAZÓN SOCIAL", "EJECUTIVO", "NRO CUENTA", "FECHA", "TIPO CUENTA", "MONEDA", "DESCRIPCIÓN", "FACTOR CAMBIO", "MONTO", "MONTO $ AL DIA HOY", "SALDO", "SALDO $ AL DIA HOY", "ESTADO" };
                ClsHelpersExcel.EscribirCabeceraDetalle(ws, ref detalleStartRow, headers);
                EscribirDetalleTodasCuentaPorCobrar(ws, DataCuentasPorPagar, detalleStartRow);

                archivoByte = pck.GetAsByteArray();
                ClsHelpersExcel.GuardarArchivo(archivoByte);
            }

            return await Task.FromResult(archivoByte);
        }

        private void EscribirDetalleTodasCuentaPorCobrar(ExcelWorksheet ws, List<ServiceEntidades.CuentaPorCobrarEN> detalles, int startRow)
        {
            int row = startRow;

            // Recorrer cada detalle y escribir en la hoja
            foreach (var item in detalles)
            {
                double montoENPesos = ClsHelpers.CalcularMontoEnPesos(item.ValorCuenta, item.IdMoneda, item.FactorCambio);
                double saldoENPesos = ClsHelpers.CalcularMontoEnPesos(item.SaldoCuenta, item.IdMoneda, item.FactorCambio);
                string formatoMoneda = ClsHelpers.DevuelveFormatoMoneda(item.IdMoneda);

                ws.Cells[row, 1].Value = ClsHelpers.ObtieneFormatoRut(item.RutCliente, item.DigitoCliente);
                ws.Cells[row, 2].Value = item.NombreCliente;
                ws.Cells[row, 3].Value = item.NombreEjecutivo;
                ws.Cells[row, 4].Value = item.NumeroCuenta;
                ws.Cells[row, 5].Value = item.Fecha.ToShortDateString();
                ws.Cells[row, 5].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoFecha();
                ws.Cells[row, 6].Value = item.TipoCuenta;
                ws.Cells[row, 7].Value = item.Moneda;
                ws.Cells[row, 8].Value = item.Descripcion;

                // Manejo de FactorCambio
                ws.Cells[row, 9].Value = item.FactorCambio == 0.00 ? "" : ClsHelpers.ObtieneFormatoDecimal(item.FactorCambio);
                ws.Cells[row, 9].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(2); // Usar formato para el caso 1

                // Aplicar formato y valor a celdas monetarias
                ws.Cells[row, 10].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 10].Value = item.ValorCuenta;

                ws.Cells[row, 11].Value = montoENPesos;
                ws.Cells[row, 11].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1); // Usar formato para el caso 1

                ws.Cells[row, 12].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 12].Value = item.SaldoCuenta;

                ws.Cells[row, 13].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 13].Value = saldoENPesos;
                ws.Cells[row, 14].Value = item.Estado;

                // Ajustar el alto de la fila (opcional)
                ws.Row(row).CustomHeight = true;
                ws.Row(row).Height = 20;

                ClsHelpersExcel.AplicarBorder(ws.Cells[row, 1, row, 14]);

                // Incrementar la fila para la próxima iteración
                row++;
            }

            // Ajustar el ancho de las columnas (opcional)
            for (int col = 1; col <= 14; col++)
            {
                ws.Column(col).AutoFit();
            }
        }

        #endregion

        #region "Exportar a Excel las cuentas por cobrar por cliente"

        public async Task<byte[]> GetCuentasPorCobrarPorCliente(List<ServiceEntidades.CuentaPorCobrarEN> cuentaPorPagar)
        {
            byte[] archivoByte = await GeneraArchivoExcelCuentaPorCobrarPorCliente(cuentaPorPagar);

            return await Task.FromResult(archivoByte);
        }

        private async Task<byte[]> GeneraArchivoExcelCuentaPorCobrarPorCliente(List<ServiceEntidades.CuentaPorCobrarEN> DataCuentasPorPagar)
        {
            const int tituloRow = 3;
            const int headerRow = 7;
            Byte[] archivoByte = null;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("CUENTAS POR COBRAR");
                ClsHelpersExcel.InsertarImagen(ws);
                ClsHelpersExcel.ConfigurarTitulo(ws, tituloRow, 11, "TODAS LAS CUENTAS POR COBRAR");
                EscribirCabeceraReporteCuentaPorCobrarPorCliente(ws, DataCuentasPorPagar, headerRow);

                int detalleStartRow = headerRow + 2; // La fila donde empieza el detalle
                string[] headers = { "NRO CUENTA", "FECHA", "MONEDA", "TIPO CTA", "DESCRIPCIÓN", "FACTOR CAMBIO ACTUAL", "MONTO", "MONTO $ AL DIA HOY", "SALDO", "SALDO $ AL DIA HOY", "ESTADO" };
                ClsHelpersExcel.EscribirCabeceraDetalle(ws, ref detalleStartRow, headers);
                EscribirDetalleCuentaPorCobrarPorCliente(ws, DataCuentasPorPagar, detalleStartRow);

                archivoByte = pck.GetAsByteArray();
                ClsHelpersExcel.GuardarArchivo(archivoByte);
            }

            return await Task.FromResult(archivoByte);
        }

        private void EscribirCabeceraReporteCuentaPorCobrarPorCliente(ExcelWorksheet ws, List<ServiceEntidades.CuentaPorCobrarEN> data, int row)
        {
            var primerdatos = data[0]; // Obtener el primer cliente de la lista

            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, 1].Value = "Rut: ";
            ws.Cells[row, 2].Value = ClsHelpers.ObtieneFormatoRut(primerdatos.RutCliente, primerdatos.DigitoCliente);
            ws.Cells[row, 4].Value = "Nombre Cliente: ";
            ws.Cells[row, 5].Value = primerdatos.NombreCliente;
            ws.Cells[row, 5, row, 8].Merge = true;
        }

        private void EscribirDetalleCuentaPorCobrarPorCliente(ExcelWorksheet ws, List<ServiceEntidades.CuentaPorCobrarEN> detalles, int startRow)
        {
            int row = startRow;

            // Recorrer cada detalle y escribir en la hoja
            foreach (var item in detalles)
            {
                double montoENPesos = ClsHelpers.CalcularMontoEnPesos(item.ValorCuenta, item.IdMoneda, item.FactorCambio);
                double saldoENPesos = ClsHelpers.CalcularMontoEnPesos(item.SaldoCuenta, item.IdMoneda, item.FactorCambio);
                string formatoMoneda = ClsHelpers.DevuelveFormatoMoneda(item.IdMoneda);

                ws.Cells[row, 1].Value = item.NumeroCuenta;
                ws.Cells[row, 2].Value = item.Fecha.ToShortDateString();
                ws.Cells[row, 2].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoFecha();
                ws.Cells[row, 3].Value = item.Moneda;
                ws.Cells[row, 4].Value = item.TipoCuenta;
                ws.Cells[row, 5].Value = item.Descripcion;

                // Manejo de FactorCambio
                ws.Cells[row, 6].Value = item.FactorCambio == 0.00 ? "" : ClsHelpers.ObtieneFormatoDecimal(item.FactorCambio);
                ws.Cells[row, 6].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(2); // Usar formato para el caso 1

                // Aplicar formato y valor a celdas monetarias
                ws.Cells[row, 7].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 7].Value = item.ValorCuenta;

                ws.Cells[row, 8].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1); // Usar formato para el caso 1
                ws.Cells[row, 8].Value = montoENPesos;

                ws.Cells[row, 9].Style.Numberformat.Format = formatoMoneda;
                ws.Cells[row, 9].Value = item.SaldoCuenta;

                ws.Cells[row, 10].Style.Numberformat.Format = ClsHelpers.DevuelveFormatoMoneda(1);
                ws.Cells[row, 10].Value = saldoENPesos;

                ws.Cells[row, 11].Value = item.Estado;

                // Ajustar el alto de la fila (opcional)
                ws.Row(row).CustomHeight = true;
                ws.Row(row).Height = 20;

                ClsHelpersExcel.AplicarBorder(ws.Cells[row, 1, row, 11]);

                // Incrementar la fila para la próxima iteración
                row++;
            }

            // Ajustar el ancho de las columnas (opcional)
            for (int col = 1; col <= 11; col++)
            {
                ws.Column(col).AutoFit();
            }
        }

        #endregion

        #endregion

    }
}
