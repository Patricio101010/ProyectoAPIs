
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using WebApiRepoteria.AL.Entidades;
using WebApiRepoteria.AL.Interfaz;
using WebApiRepoteria.AL.Seguridad;
using WebApiRepoteria.BLL.BusinessHelpers;
using WebApiRepoteria.BLL.Interfaz;

namespace WebApiRepoteria.BLL.Implementos
{
    public class AdministracionExportarPDFBLL : IAdministracionExportarPDFBLL
    {
        //objeto
        readonly CuentaBLL ClsCuentaBLL = new CuentaBLL();
        readonly HelpersPDF  ClsHelpersPDF = new HelpersPDF();
        readonly RutinasGeneralesHelpersBLL ClsHelpers = new RutinasGeneralesHelpersBLL();
        private readonly IEncriptaEN ObjEncripta = new EncriptaEN();

        //variables privadas
        private string rutaImagen, rutaGuardarEXCEL;
        private bool registrarArchivo;

        public AdministracionExportarPDFBLL()
        {
            //rutaImagen = ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["rutaLogo"].ToString());
            //rutaGuardarEXCEL = ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["rutaGuardarEXCEL"].ToString());
            //registrarArchivo = bool.Parse(ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["registrarArchivo"].ToString()));

            rutaImagen = ConfigurationManager.AppSettings["rutaLogo"].ToString();
            rutaGuardarEXCEL = ConfigurationManager.AppSettings["rutaGuardarEXCEL"].ToString();
            registrarArchivo = bool.Parse(ConfigurationManager.AppSettings["registrarArchivo"].ToString());
        }

        public async Task<byte[]> GetCuentasPorCobrarPorCliente(List<ServiceEntidades.CuentaPorCobrarEN> cuentaPorPagar)
        {
            byte[] archivoByte = await GeneraArchivoPDFCuentaPorPagarPorCliente(cuentaPorPagar);

            return await Task.FromResult(archivoByte);
        }

        #region "Cuenta por pagar"

        private async Task<byte[]> GeneraArchivoPDFCuentaPorPagarPorCliente(List<ServiceEntidades.CuentaPorCobrarEN> DataCuentasPorPagar)
        {
            Byte[] archivoByte = null;
            var titleFontSubrayar = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 7);

            // Generación de PDF en memoria
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();
                document.Add(EscribirTitulo(titleFontSubrayar, boldFont, normalFont));
                document.Add(EscribirCabecera(boldFont, normalFont));
                document.Add(EscribirDetalle(boldFont, normalFont));
                document.Add(EscribirDetalle1(boldFont, normalFont));
                document.Close();

                archivoByte = ms.ToArray();

            }

            ClsHelpersPDF.GuardarArchivo(archivoByte);


            return await Task.FromResult(archivoByte);
        }


        private PdfPTable EscribirTitulo(Font titleFontSubrayar, Font boldFont, Font normalFont)
        {

            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(rutaImagen);
            logo.ScalePercent(7.1F);

            int numberColumns = 12;
            var TablaTitulo = ClsHelpersPDF.CrearTabla(numberColumns);

            TablaTitulo.AddCell(ClsHelpersPDF.CrearImageCelda(3, logo));
            TablaTitulo.AddCell(ClsHelpersPDF.CrearTextoCelda("Linea de Financiamiento.", titleFontSubrayar, 7, 2, Element.ALIGN_LEFT));

            TablaTitulo.AddCell(ClsHelpersPDF.CrearTextoCelda("Fecha: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_TOP));
            TablaTitulo.AddCell(ClsHelpersPDF.CrearTextoCelda(DateTime.Now.ToShortDateString(), normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_TOP));

            TablaTitulo.AddCell(ClsHelpersPDF.CrearTextoCelda("Hora: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_TOP));
            TablaTitulo.AddCell(ClsHelpersPDF.CrearTextoCelda(DateTime.Now.ToShortTimeString(), normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_TOP));

            return TablaTitulo;
        }

        private PdfPTable EscribirCabecera(Font boldFont, Font normalFont)
        {
            int numberColumns = 10;
            var TablaCabecera = ClsHelpersPDF.CrearTabla(numberColumns);
            TablaCabecera.AddCell(ClsHelpersPDF.CrearEspacioBlanco(numberColumns));

            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("Identificación: ", boldFont, 1, 1, Element.ALIGN_RIGHT));
            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("16.414.623-5", normalFont, 1, 1, Element.ALIGN_LEFT));

            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("Razón social cliente: ", boldFont, 1, 1, Element.ALIGN_RIGHT));
            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("CLIENTE                            DESARROLLO", normalFont, 4, 1, Element.ALIGN_LEFT));

            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("Ejecutivo Bco: ", boldFont, 1, 1, Element.ALIGN_RIGHT));
            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("CAMILA         FUENTES", normalFont, 2, 1, Element.ALIGN_LEFT));

            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("Sucursal: ", boldFont, 1, 1, Element.ALIGN_RIGHT));
            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("CASA MATRIZ", normalFont, 1, 1, Element.ALIGN_LEFT));

            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("Ejecutivo: ", boldFont, 1, 1, Element.ALIGN_RIGHT));
            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("CAMILA         FUENTES", normalFont, 7, 1, Element.ALIGN_LEFT));

            return TablaCabecera;
        }

        private PdfPTable EscribirDetalle1(Font boldFont, Font normalFont)
        {
            int numberColumns = 14;
            int border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER;
            var TablaDetalle = ClsHelpersPDF.CrearTabla(numberColumns);

            TablaDetalle.AddCell(ClsHelpersPDF.CrearEspacioBlanco(numberColumns));

            // fila 9
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("PORCENTAJE ANTICIPAR", boldFont, 5, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("SUBLINEAS (PRODUCTOS)", boldFont, 3, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("PAGADORES", boldFont, 4, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));

            //  fila 10
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("T.P.", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera(" % ", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Ver.", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Not.", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Cob.", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Producto", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Línea", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("%", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Identificación", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Razón Social", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Sub Linea", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("%", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            //  fila 11
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            // fila 12
            TablaDetalle.AddCell(ClsHelpersPDF.CrearEspacioBlanco(numberColumns));

            // fila 13
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("COMISIÓN", boldFont, 6, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("GASTOS", boldFont, 7, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));


            // fila 14
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("% COMISIÓN", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("MONEDA", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("MÍNIMO", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("MÁXIMO MONTO", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("COMISIÓN", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("COMISION FLAT", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("TIPO DOCUMENTO", boldFont, 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("MONTO", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("ESTADO", boldFont, 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("DESCRIPCIÓN", boldFont, 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));

            // fila 14
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont, 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont, 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont, 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));




            return TablaDetalle;
        }

        private PdfPTable EscribirDetalle(Font boldFont, Font normalFont)
        {
            int numberColumns = 8;
            int border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER;
            var TablaDetalle = ClsHelpersPDF.CrearTabla(numberColumns);

            TablaDetalle.AddCell(ClsHelpersPDF.CrearEspacioBlanco(numberColumns));

            // fila 1
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("DATOS LÍNEA", boldFont, 4, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("TIPO COMISIÓN", boldFont, 3, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));

            // fila 2
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Nro linea: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("xxxxxxxx", normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Fecha Solicitud: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("17/07/2024", normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Tipo Comisión: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Normal", normalFont, 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER));

            // fila 3
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Línea Solicitada: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("999.999.999.999", normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Línea Disponible: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("999.999.999.999", normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Tipo Comisión: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("OK", normalFont, 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER));

            // fila 4
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Estado de Linea: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("VIGENTE", normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Línea Ocupada: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("999.999.999.999", normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("  ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("  ", normalFont, 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            // fila 5
            TablaDetalle.AddCell(ClsHelpersPDF.CrearEspacioBlanco(numberColumns));

            // fila 6
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("APROBACIÓN COMITE", boldFont, 8, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));

            // fila 7
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Fecha Resolución: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("17/07/2024", normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Fecha Desde: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("17/07/2024", normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Fecha Hasta: ", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("30/03/2035", normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Monto Aprobado: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("999.999.999.999", normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            return TablaDetalle;
        }

        #endregion



    }
}
