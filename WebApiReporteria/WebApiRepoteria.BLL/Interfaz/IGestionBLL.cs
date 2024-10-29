
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRepoteria.AL.Entidades;

namespace WebApiRepoteria.BLL.Interfaz
{
    public interface IGestionBLL
    {
        Task<List<ServiceEntidades.CarteraEN>> GetDataCarteraVigente();
        Task<List<ServiceEntidades.DocumentoCompradoEN>> GetDataDocumentoComprado(DocumentoCompradoRequest documento);
        Task<List<ServiceEntidades.DocumentoDevengadoEN>> GetDataDocumentoDevengado(DocumentoDevengadoRequest documento);
        Task<List<ServiceEntidades.OperacionCursadaEN>> GetDataOperacionCursadas(OperacionCursadasRequest operacion);
        Task<List<ServiceEntidades.ExcedentesEN>> GetDataExcedentes(ExcedentesRequest excedentes);
    }
}
