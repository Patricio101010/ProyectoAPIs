
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiRepoteria.BLL.Interfaz
{
    public interface IGestionExportacionExcelBLL
    {
        Task<byte[]> GetCarteraVigente(List<ServiceEntidades.CarteraEN> data);
        Task<byte[]> GetDocumentoComprado(List<ServiceEntidades.DocumentoCompradoEN> data, DateTime fechaCurseDesde, DateTime fechaCurseHasta);
        Task<byte[]> GetDocumentoDevengado(List<ServiceEntidades.DocumentoDevengadoEN> data);
        Task<byte[]> GetOperacionCursadas(List<ServiceEntidades.OperacionCursadaEN> data, DateTime fechaCurseDesde, DateTime fechaCurseHasta);
        Task<byte[]> GetExcedentes(List<ServiceEntidades.ExcedentesEN> data);
    }
}
