using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using WebApiRepoteria.AL.Interfaz;
using WebApiRepoteria.AL.Seguridad;

namespace WebApiRepoteria.BLL.BusinessHelpers
{
    public class HelpersExcel
    {
        //objeto
        private readonly IEncriptaEN ObjEncripta = new EncriptaEN();

        //variables privadas
        private string rutaImagen, rutaGuardarEXCEL; private bool registrarArchivo;
        public HelpersExcel()
        {
            //rutaImagen = ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["rutaLogo"].ToString());
            //rutaGuardarEXCEL = ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["rutaGuardarEXCEL"].ToString());
            //registrarArchivo = bool.Parse(ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["registrarArchivo"].ToString()));

            rutaImagen = ConfigurationManager.AppSettings["rutaLogo"].ToString();
            rutaGuardarEXCEL = ConfigurationManager.AppSettings["rutaGuardarEXCEL"].ToString();
            registrarArchivo = bool.Parse(ConfigurationManager.AppSettings["registrarArchivo"].ToString());
        }

        public void GuardarArchivo(byte[] archivoBytes)
        {
            if (registrarArchivo)
            {
                string rutaAbsoluta = Path.GetFullPath(rutaGuardarEXCEL + "Ejemplo.xlsx");
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

        public void ConfigurarTitulo(ExcelWorksheet ws, int row, int columnaFinal, string titulo)
        {
            ws.Cells[row, 2].Value = titulo;
            using (var rng = ws.Cells[row, 2, row, columnaFinal])
            {
                rng.Merge = true;
                rng.Style.Font.Bold = true;
                rng.Style.Font.Size = 14;
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
        }

        public void InsertarImagen(ExcelWorksheet ws)
        {
            // Agregar imagen a la cabecera
            if (!string.IsNullOrEmpty(rutaImagen) && File.Exists(rutaImagen))
            {
                string strLogoPath = ConfigurationManager.AppSettings["RutaLogo"];
                System.Drawing.Image Logo = System.Drawing.Image.FromFile(rutaImagen);
                var Picture = ws.Drawings.AddPicture("Logo", Logo);
                Picture.SetPosition(0, 1, 0, 0);
                Picture.SetSize(100, 100);

            }
        }

        public void GuardarArchivoDesdeBytes(byte[] archivoBytes, string rutaArchivo)
        {
            File.WriteAllBytes(rutaArchivo, archivoBytes);
        }

        public void EscribirCabeceraDetalle(ExcelWorksheet ws, ref int fila, string[] headers)
        {
            for (int col = 1; col <= headers.Length; col++)
            {
                ws.Cells[fila, col].Value = headers[col - 1];
                AplicarEstiloEncabezodoDetalle(ws.Cells[fila, col]);
            }
            fila++;
        }

        public void AplicarEstiloEncabezodoDetalle(ExcelRange range)
        {
            range.Style.Font.Bold = true;
            range.Style.Font.Size = 12;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
        }

        public void AplicarBorder(ExcelRange range)
        {
            //range.Style.Font.Bold = true;
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;      // Borde superior
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;   // Borde inferior
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;     // Borde izquierdo
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;    // Borde derecho
        }

        public void EscribirCabeceraReporteSoloFecha(ExcelWorksheet ws, int row)
        {
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells[row, 1].Value = "Fecha: ";
            ws.Cells[row, 2].Value = DateTime.Now.ToLongDateString();
        }

    }
}
