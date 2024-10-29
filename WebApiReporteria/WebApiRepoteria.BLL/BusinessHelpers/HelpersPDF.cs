
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Configuration;
using System.IO;
using WebApiRepoteria.AL.Interfaz;
using WebApiRepoteria.AL.Seguridad;

namespace WebApiRepoteria.BLL.BusinessHelpers
{
    public class HelpersPDF
    {
        private readonly IEncriptaEN ObjEncripta = new EncriptaEN();

        //variables privadas
        private string rutaImagen, rutaGuardarEXCEL;
        private bool registrarArchivo;

        public HelpersPDF()
        {
            //rutaImagen = ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["rutaLogo"].ToString());
            //rutaGuardarEXCEL = ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["rutaGuardarEXCEL"].ToString());
            //registrarArchivo = bool.Parse(ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["registrarArchivo"].ToString()));

            rutaImagen = ConfigurationManager.AppSettings["rutaLogo"].ToString();
            rutaGuardarEXCEL = ConfigurationManager.AppSettings["rutaGuardarEXCEL"].ToString();
            registrarArchivo = bool.Parse(ConfigurationManager.AppSettings["registrarArchivo"].ToString());
        }

        #region "configuracion"

        public void GuardarArchivo(byte[] archivoBytes)
        {
            if (registrarArchivo)
            {
                string rutaAbsoluta = Path.GetFullPath(rutaGuardarEXCEL + "ArchivoPDF.PDF");
                string directorio = Path.GetDirectoryName(rutaAbsoluta);

                if (!Directory.Exists(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                // Verifica si el archivo existe, y si es así, lo elimina
                if (File.Exists(rutaAbsoluta))
                {
                    File.Delete(rutaAbsoluta);
                }

                File.WriteAllBytes(rutaAbsoluta, archivoBytes);
            }
        }

        public PdfPCell CrearEspacioBlanco(int colspan)
        {
            return new PdfPCell(new Phrase(" "))
            {
                Colspan = colspan,
                Border = 0,
                SpaceCharRatio = 800
            };
        }

        public PdfPTable CrearTabla(int numberColumns)
        {
            return new PdfPTable(numberColumns)
            {
                DefaultCell = { Border = PdfPCell.TOP_BORDER },
                WidthPercentage = 95
            };
        }

        public PdfPCell CrearImageCelda(int colspan, iTextSharp.text.Image ImagenLogo, int rowSpan = 2)
        {
            return new PdfPCell(ImagenLogo, false)
            {
                Colspan = colspan,
                Rowspan = rowSpan,
                Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_TOP,
                MinimumHeight = 30
            };
        }

        public PdfPCell CrearTextoCelda(string text, Font font, int colspan = 1, int rowSpan = 1, int horizontalAlig = Element.ALIGN_CENTER, int verticalAlig = Element.ALIGN_MIDDLE, int border = 0)
        {
            return new PdfPCell(new Phrase(text, font))
            {
                Colspan = colspan,
                Rowspan = rowSpan,
                Border = border,
                HorizontalAlignment = horizontalAlig,
                VerticalAlignment = verticalAlig
            };
        }

        public PdfPCell CrearTituloCabecera(string text, Font font, int colspan = 1, int rowSpan = 1, int horizontalAlig = Element.ALIGN_CENTER, int verticalAlig = Element.ALIGN_MIDDLE, int border = 0)
        {
            return new PdfPCell(new Phrase(text, font))
            {
                Colspan = colspan,
                Rowspan = rowSpan,
                Border = border,
                HorizontalAlignment = horizontalAlig,
                VerticalAlignment = verticalAlig,
                BackgroundColor = new iTextSharp.text.BaseColor(220, 220, 220)
            };
        }

        #endregion 
    }
}
