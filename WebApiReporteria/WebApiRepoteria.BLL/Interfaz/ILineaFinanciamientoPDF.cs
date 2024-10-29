using System.Threading.Tasks;
using WebApiRepoteria.AL.Entidades;

namespace WebApiRepoteria.BLL.Interfaz
{
    public interface ILineaFinanciamientoPDF
    {
        Task<byte[]> GetLineaFinanciamiento(ServiceEntidades.LineaFinanciamientoEN lineaFinanciamiento);
    }
}
