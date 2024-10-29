using WebApiRepoteria.AL.Entidades;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace WebApiRepoteria.DAL.Interfaz
{
    public interface IExecuteDAL
    {
        Task<DataTable> ExecuteProcedureAsync(string NombreProcedimiento);
        Task<DataTable> ExecuteProcedureAsync(string NombreProcedimiento, List<ModeloDataEN> modeloLista);
    }
}
