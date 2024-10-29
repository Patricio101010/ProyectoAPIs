using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRepoteria.AL.Entidades;

namespace WebApiRepoteria.BLL.Interfaz
{
    public interface ICuentaBLL
    {
        Task<List<ServiceEntidades.CuentaPorPagarEN>> GetDataCuentasPorPagarPorCliente(CuentasPorPagarRequest cuentaPorPagar);
        Task<List<ServiceEntidades.CuentaPorPagarEN>> GetDataTodasCuentasPorPagar(CuentasPorPagarRequest cuentaPorPagar);
        Task<List<ServiceEntidades.CuentaPorCobrarEN>> GetCuentasPorCobrarPorCliente(CuentasPorCobrarRequest cuentaPorCobrar);
        Task<List<ServiceEntidades.CuentaPorCobrarEN>> getDevelveDatos(ParametrosEntradaRequest cuentaPorCobrar);
    }
}
