
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRepoteria.AL.Entidades;

namespace WebApiRepoteria.BLL.Interfaz
{
    public interface IAdministracionExportarExcelBLL
    {
        Task<byte[]> GetCuentasPorPagar(List<ServiceEntidades.CuentaPorPagarEN> cuentaPorPagar);
        Task<byte[]> GetCuentasPorPagarPorCliente(List<ServiceEntidades.CuentaPorPagarEN> cuentaPorPagar);
        Task<byte[]> GetCuentasPorCobrar(List<ServiceEntidades.CuentaPorCobrarEN> cuentaPorCobrar);
        Task<byte[]> GetCuentasPorCobrarPorCliente(List<ServiceEntidades.CuentaPorCobrarEN> cuentaPorCobrar);
    }
}
