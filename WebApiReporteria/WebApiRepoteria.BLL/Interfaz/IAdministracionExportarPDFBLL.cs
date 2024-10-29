
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRepoteria.AL.Entidades;

namespace WebApiRepoteria.BLL.Interfaz
{
    public interface IAdministracionExportarPDFBLL
    {
        Task<byte[]> GetCuentasPorCobrarPorCliente(List<ServiceEntidades.CuentaPorCobrarEN> cuentaPorPagar);
    }
}
