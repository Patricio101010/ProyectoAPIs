using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApiRepoteria.AL.Entidades;
using WebApiRepoteria.AL.Interfaz;
using WebApiRepoteria.AL.Seguridad;
using WebApiRepoteria.BLL.BusinessHelpers;
using WebApiRepoteria.BLL.Interfaz;

namespace WebApiRepoteria.BLL.Implementos
{

    class LineaFinanciamientoPDF : ILineaFinanciamientoPDF
    {
        //objeto
        private readonly HelpersPDF  ClsHelpersPDF = new HelpersPDF();
        private readonly CuentaBLL ClsCuentaBLL = new CuentaBLL();
        private readonly RutinasGeneralesHelpersBLL ClsHelpers = new RutinasGeneralesHelpersBLL();
        private readonly IEncriptaEN ObjEncripta = new EncriptaEN();

        //variables privadas
        private string rutaImagen, rutaGuardarEXCEL;
        private bool registrarArchivo;

        public LineaFinanciamientoPDF()
        {
            //rutaImagen = ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["rutaLogo"].ToString());
            //rutaGuardarEXCEL = ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["rutaGuardarEXCEL"].ToString());
            //registrarArchivo = bool.Parse(ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["registrarArchivo"].ToString()));

            rutaImagen = ConfigurationManager.AppSettings["rutaLogo"].ToString();
            rutaGuardarEXCEL = ConfigurationManager.AppSettings["rutaGuardarEXCEL"].ToString();
            registrarArchivo = bool.Parse(ConfigurationManager.AppSettings["registrarArchivo"].ToString());
        }


        public async Task<byte[]> GetLineaFinanciamiento(ServiceEntidades.LineaFinanciamientoEN lineaFinanciamiento)
        {
            byte[] archivoByte = await GeneraArchivoPDFLineaFinanciamiento(lineaFinanciamiento);

            return await Task.FromResult(archivoByte);
        }

        #region "Linea financiamiento"

        private async Task<byte[]> GeneraArchivoPDFLineaFinanciamiento(ServiceEntidades.LineaFinanciamientoEN Data)
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
                document.Add(EscribirCabecera(boldFont, normalFont, Data));
                document.Add(EscribirDetalle(boldFont, normalFont, Data));
                document.Add(EscribirTabla(boldFont, normalFont, Data));
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

        private PdfPTable EscribirCabecera(Font boldFont, Font normalFont, ServiceEntidades.LineaFinanciamientoEN Data)
        {
            int numberColumns = 10;
            var TablaCabecera = ClsHelpersPDF.CrearTabla(numberColumns);
            TablaCabecera.AddCell(ClsHelpersPDF.CrearEspacioBlanco(numberColumns));

            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("Identificación: ", boldFont, 1, 1, Element.ALIGN_RIGHT));
            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtieneFormatoRut(Data.RutCliente, Data.DigitoCliente), normalFont, 1, 1, Element.ALIGN_LEFT));

            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("Razón social cliente: ", boldFont, 1, 1, Element.ALIGN_RIGHT));
            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.NombreCliente, normalFont, 4, 1, Element.ALIGN_LEFT));

            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("Ejecutivo Bco: ", boldFont, 1, 1, Element.ALIGN_RIGHT));
            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.NombreEjecutivoFatoring, normalFont, 2, 1, Element.ALIGN_LEFT));

            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("Sucursal: ", boldFont, 1, 1, Element.ALIGN_RIGHT));
            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.Sucursal, normalFont, 1, 1, Element.ALIGN_LEFT));

            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda("Ejecutivo: ", boldFont, 1, 1, Element.ALIGN_RIGHT));
            TablaCabecera.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.NombreEjecutivoComercial, normalFont, 7, 1, Element.ALIGN_LEFT));

            return TablaCabecera;
        }
        private PdfPTable EscribirDetalle(Font boldFont, Font normalFont, ServiceEntidades.LineaFinanciamientoEN Data)
        {
            int numberColumns = 8;
            int border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER;
            var TablaDetalle = ClsHelpersPDF.CrearTabla(numberColumns);

            TablaDetalle.AddCell(ClsHelpersPDF.CrearEspacioBlanco(numberColumns));

            // TITULOS DE TABLA
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("DATOS LÍNEA", boldFont, 4, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("TIPO COMISIÓN", boldFont, 3, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));

            // TITULOS DE CABECERA DE TABLA
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Nro linea: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.NroLinea.ToString(), normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Fecha Solicitud: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.FechaSolicitud.ToShortDateString(), normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER));

            // ESPACION EN BLANCO
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            // ESCRIBIR EL DETALLE DE LAS TABLAS
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Tipo Comisión: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.TipoComision, normalFont, 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Línea Solicitada: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtenerFormatoMoneda(1, Data.MontoSolicitada), normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Línea Disponible: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtenerFormatoMoneda(1, Data.MontoDisponible), normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER));

            // ESPACION EN BLANCO
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Observación: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.Observacion, normalFont, 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Estado de Linea: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.EstadoLinea, normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Línea Ocupada: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtenerFormatoMoneda(1, Data.MontoOcupado), normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            // ESPACION EN BLANCO
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont, 3));

            //TablaDetalle.AddCell(CrearTextoCelda("  ", boldFont, 3, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER));
            //TablaDetalle.AddCell(CrearTextoCelda("  ", normalFont, 2, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            // SALTO LINEA
            TablaDetalle.AddCell(ClsHelpersPDF.CrearEspacioBlanco(numberColumns));

            // TITULOS DE TABLA
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("APROBACIÓN COMITE", boldFont, 8, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));

            // ESCRIBIR EL DETALLE DE LAS TABLAS
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Fecha Resolución: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.FechaAprobación.ToShortDateString(), normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Fecha Desde: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.FechaVigenteDesde.ToShortDateString(), normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Fecha Hasta: ", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.FechaVigenteHasta.ToShortDateString(), normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("Monto Aprobado: ", boldFont, 1, 1, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtenerFormatoMoneda(1, Data.MontoAprobado), normalFont, 1, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            return TablaDetalle;
        }

        private PdfPTable EscribirTabla(Font boldFont, Font normalFont, ServiceEntidades.LineaFinanciamientoEN Data)
        {
            int numberColumns = 14;
            int border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER;
            var TablaDetalle = ClsHelpersPDF.CrearTabla(numberColumns);

            TablaDetalle.AddCell(ClsHelpersPDF.CrearEspacioBlanco(numberColumns));

            int CantudadRegistroAnticipar = Data.ListaPorcentajeAnticipar.Count;
            int CantudadRegistroPagadores = Data.ListaPagadores.Count;
            int CantudadRegistroSubLineas = Data.ListaSubLineas.Count;

            int CantidadRegistroMaximo = 0;
            if (CantudadRegistroAnticipar > CantudadRegistroPagadores)
            {
                if (CantudadRegistroAnticipar > CantudadRegistroSubLineas)
                {
                    CantidadRegistroMaximo = CantudadRegistroAnticipar;
                }
                else
                {
                    CantidadRegistroMaximo = CantudadRegistroSubLineas;
                }
            }
            else
            {
                if (CantudadRegistroPagadores > CantudadRegistroSubLineas)
                {
                    CantidadRegistroMaximo = CantudadRegistroPagadores;
                }
                else
                {
                    CantidadRegistroMaximo = CantudadRegistroSubLineas;
                }
            }


            // TITULOS DE TABLA
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("PORCENTAJE ANTICIPAR", boldFont, 5, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("SUBLINEAS (PRODUCTOS)", boldFont, 3, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("PAGADORES", boldFont, 4, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE, PdfPCell.BOTTOM_BORDER));

            // TITULOS DE CABECERA DE TABLA
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("T.P.", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera(" % ", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Ver.", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Not.", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Cob.", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            // ESPACION EN BLANCO
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Producto", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Línea", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("%", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            // ESPACION EN BLANCO
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Identificación", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Razón Social", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("Sub Linea", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("%", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));

            // BUCLE PARA ESCRIBIR EL DETALLE DE LAS TABLAS
            for (int i = 0; i < CantidadRegistroMaximo; i++)
            {
                if (i < CantudadRegistroAnticipar)
                {
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.ListaPorcentajeAnticipar[i].TipoProducto, boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtieneFormatoPorcentaje(Data.ListaPorcentajeAnticipar[i].Porcentaje), normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.ListaPorcentajeAnticipar[i].Verificacion, normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.ListaPorcentajeAnticipar[i].Notificacion, normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.ListaPorcentajeAnticipar[i].Cobranza, normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
                }

                // ESPACION EN BLANCO
                TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

                if (i < CantudadRegistroSubLineas)
                {
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.ListaSubLineas[i].TipoProducto, normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtenerFormatoMoneda(1, Data.ListaSubLineas[i].Linea), normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtieneFormatoPorcentaje(Data.ListaSubLineas[i].Porcentaje), normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
                }

                // ESPACION EN BLANCO
                TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

                if (i < CantudadRegistroPagadores)
                {
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtieneFormatoRut(Data.ListaPagadores[i].RutDeudor, Data.ListaPagadores[i].DigitoDeudor), normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.ListaPagadores[i].NombreDeudor, normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtenerFormatoMoneda(1, Data.ListaPagadores[i].MontoLinea), normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtieneFormatoPorcentaje(Data.ListaPagadores[i].PorcentajeLinea), normalFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER));
                }
            }

            // ESPACION EN BLANCO
            TablaDetalle.AddCell(ClsHelpersPDF.CrearEspacioBlanco(numberColumns));

            // TITULOS DE TABLA
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("COMISIÓN", boldFont, 6, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            // ESPACION EN BLANCO
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda("GASTOS", boldFont, 7, 1, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE));

            // TITULOS DE CABECERA DE TABLA
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("% COMISIÓN", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("MONEDA", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("MÍNIMO", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("MÁXIMO MONTO", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("COMISIÓN", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("COMISION FLAT", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));

            // ESPACION EN BLANCO
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("TIPO DOCUMENTO", boldFont, 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("MONTO", boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("ESTADO", boldFont, 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
            TablaDetalle.AddCell(ClsHelpersPDF.CrearTituloCabecera("DESCRIPCIÓN", boldFont, 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));

            int CantudadRegistroComision = Data.ListaComision.Count;
            int CantudadRegistroGastos = Data.ListaGastos.Count;
            CantidadRegistroMaximo = 0;
            if (CantudadRegistroComision > CantudadRegistroGastos)
            {
                CantidadRegistroMaximo = CantudadRegistroComision;
            }
            else
            {
                CantidadRegistroMaximo = CantudadRegistroGastos;
            }

            // BUCLE PARA ESCRIBIR EL DETALLE DE LAS TABLAS
            for (int i = 0; i < CantidadRegistroMaximo; i++)
            {
                if (i < CantudadRegistroComision)
                {
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtieneFormatoPorcentaje(Data.ListaComision[i].Porcentaje), boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.ListaComision[i].Moneda, boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtenerFormatoMoneda(Data.ListaComision[i].IdMoneda, Data.ListaComision[i].Minimo), boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtenerFormatoMoneda(Data.ListaComision[i].IdMoneda, Data.ListaComision[i].Maximo), boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtenerFormatoMoneda(Data.ListaComision[i].IdMoneda, Data.ListaComision[i].MontoComision), boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtenerFormatoMoneda(Data.ListaComision[i].IdMoneda, Data.ListaComision[i].MontoComisionFlat), boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
                }

                // ESPACION EN BLANCO
                TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(" ", boldFont));

                if (i < CantudadRegistroGastos)
                {
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.ListaGastos[i].TipoProducto, boldFont, 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(ClsHelpers.ObtieneFormatoDecimal(Data.ListaGastos[i].Monto), boldFont, 1, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.ListaGastos[i].Estado, boldFont, 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
                    TablaDetalle.AddCell(ClsHelpersPDF.CrearTextoCelda(Data.ListaGastos[i].descripcion, boldFont, 2, 1, Element.ALIGN_CENTER, Element.ALIGN_MIDDLE, border));
                }
            }

            return TablaDetalle;
        }


        #endregion

    }
}
