using System.Web.Services;
using WebEntidades.EN.Entidades;

namespace WebEntidades
{
    /// <summary>
    /// Descripción breve de IncluyeEntidades
    /// </summary>
    /// 
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class IncluyeEntidades : WebService
    {
        
        [WebMethod]
        public CuentaPorCobrarEN ObtenerCuentaPorCobrar()
        {
            return new CuentaPorCobrarEN();
        }

        [WebMethod]
        public CuentaPorPagarEN ObtenerCuentaPorPagar()
        {
            return new CuentaPorPagarEN();
        }

        [WebMethod]
        public LineaFinanciamientoEN ObtenerLinea()
        {
            return new LineaFinanciamientoEN();
        }

        [WebMethod]
        public CarteraEN ObtenerCartera()
        {
            return new CarteraEN();
        }

        [WebMethod]
        public DocumentoCompradoEN ObtenerDocumento()
        {
            return new DocumentoCompradoEN();
        }

        [WebMethod]
        public DocumentoDevengadoEN ObtenerDocumentoDevengado()
        {
            return new DocumentoDevengadoEN();
        }

        [WebMethod]
        public OperacionCursadaEN ObtenerOperacionCursada()
        {
            return new OperacionCursadaEN();
        }


        [WebMethod]
        public ExcedentesEN ObtenerExcedentes()
        {
            return new ExcedentesEN();
        }
    }
}
