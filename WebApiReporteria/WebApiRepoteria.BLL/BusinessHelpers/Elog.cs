using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiRepoteria.BLL.BusinessHelpers
{
    public class Elog
    {
        string rutaArchivo;

        public Elog()
        {
            //rutaImagen = ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["rutaLogo"].ToString());
            //rutaGuardarEXCEL = ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["rutaGuardarEXCEL"].ToString());
            //registrarArchivo = bool.Parse(ObjEncripta.Desencriptar(ConfigurationManager.AppSettings["registrarArchivo"].ToString()));

            rutaArchivo = ConfigurationManager.AppSettings["rutaGuardarEXCEL"].ToString();
        }

        public void generaArchivoLog(string formatoInicial)
        {
            string mmensaje = formatoInicial + "; ";
            Console.WriteLine(mmensaje);

            System.IO.StreamWriter sw = new System.IO.StreamWriter(rutaArchivo + "log.log", true);
            sw.WriteLine(mmensaje);
            sw.Close();
        }
    }
}
