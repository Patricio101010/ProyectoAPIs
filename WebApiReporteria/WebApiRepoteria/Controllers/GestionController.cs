using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApiRepoteria.AL.Entidades;
using WebApiRepoteria.BLL.Implementos;
using WebApiRepoteria.BLL.Interfaz;


namespace WebApiRepoteria.Controllers
{
    public class GestionController : ApiController
    {
        private readonly string Controlador = "GestionController";
        private string Metodo = "";

        //private readonly ILogEN ObjLog = new LogEN();
        private readonly IGestionBLL ObjCarteraBLL = new GestionBLL();
        private readonly IGestionExportacionExcelBLL ObjExportarExcelBLL = new GestionExportacionExcelBLL();


        [HttpGet]
        [Route("api/gestion/GetObtenerCarteraVigente")]
        public async Task<HttpResponseMessage> GetObtenerCarteraVigente()
        {
            ApiResponse<BLL.ServiceEntidades.CarteraEN> responsePorDefecto = new ApiResponse<BLL.ServiceEntidades.CarteraEN>
            {
                Success = false,
                Data = new List<BLL.ServiceEntidades.CarteraEN>(),
                Message = null,
                Error = null,
                StackTrace = null,
                ArchivoByteExcel = null,
                ArchivoBytePDF = null
            };

            try
            {
                List<BLL.ServiceEntidades.CarteraEN> ListCatera = await ObjCarteraBLL.GetDataCarteraVigente();
                if (ListCatera.Count == 0)
                {
                    responsePorDefecto.Message = "No se encontraron registros de documento.";
                    responsePorDefecto.Error = "No hay datos disponibles.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                byte[] archivoByte = await ObjExportarExcelBLL.GetCarteraVigente(ListCatera);
                if (archivoByte == null || archivoByte.Length == 0)
                {
                    responsePorDefecto.Message = "No se pudo generar el archivo solicitado.";
                    responsePorDefecto.Error = "El archivo no está disponible o está vacío. Por favor, intente nuevamente más tarde.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                string archivoBase64Excel = Convert.ToBase64String(archivoByte);

                responsePorDefecto.Success = true;
                responsePorDefecto.Message = "Los datos se han recuperado correctamente.";
                responsePorDefecto.Data = ListCatera;
                responsePorDefecto.ArchivoByteExcel = archivoBase64Excel;

                return Request.CreateResponse(HttpStatusCode.OK, responsePorDefecto);
            }
            catch (Exception ex)
            {
                // Log de errores - descomentar para producción
                // ObjLog.GeneraArchivoLog(ex.Message, Controlador, Metodo);

                responsePorDefecto.Message = "Ocurrió un error al recuperar los datos.";
                responsePorDefecto.Error = ex.Message;
                responsePorDefecto.StackTrace = ex.StackTrace;

                return Request.CreateResponse(HttpStatusCode.InternalServerError, responsePorDefecto);
            }
        }

        [HttpGet]
        [Route("api/gestion/GetObtenerDocumentoComprado")]
        public async Task<HttpResponseMessage> GetObtenerDocumentoComprado([FromBody] DocumentoCompradoRequest documento)
        {
            ApiResponse<BLL.ServiceEntidades.DocumentoCompradoEN> responsePorDefecto = new ApiResponse<BLL.ServiceEntidades.DocumentoCompradoEN>
            {
                Success = false,
                Data = new List<BLL.ServiceEntidades.DocumentoCompradoEN>(),
                Message = null,
                Error = null,
                StackTrace = null,
                ArchivoByteExcel = null,
                ArchivoBytePDF = null
            };

            try
            {
                List<BLL.ServiceEntidades.DocumentoCompradoEN> ListCatera = await ObjCarteraBLL.GetDataDocumentoComprado(documento);
                if (ListCatera.Count == 0)
                {
                    responsePorDefecto.Message = "No se encontraron registros de cartera.";
                    responsePorDefecto.Error = "No hay datos disponibles.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                byte[] archivoByte = await ObjExportarExcelBLL.GetDocumentoComprado(ListCatera, documento.FechaCurseDesde, documento.FechaCurseHasta);
                if (archivoByte == null || archivoByte.Length == 0)
                {
                    responsePorDefecto.Message = "No se pudo generar el archivo solicitado.";
                    responsePorDefecto.Error = "El archivo no está disponible o está vacío. Por favor, intente nuevamente más tarde.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                string archivoBase64Excel = Convert.ToBase64String(archivoByte);

                responsePorDefecto.Success = true;
                responsePorDefecto.Message = "Los datos se han recuperado correctamente.";
                responsePorDefecto.Data = ListCatera;
                responsePorDefecto.ArchivoByteExcel = archivoBase64Excel;

                return Request.CreateResponse(HttpStatusCode.OK, responsePorDefecto);
            }
            catch (Exception ex)
            {
                // Log de errores - descomentar para producción
                // ObjLog.GeneraArchivoLog(ex.Message, Controlador, Metodo);

                responsePorDefecto.Message = "Ocurrió un error al recuperar los datos.";
                responsePorDefecto.Error = ex.Message;
                responsePorDefecto.StackTrace = ex.StackTrace;

                return Request.CreateResponse(HttpStatusCode.InternalServerError, responsePorDefecto);
            }
        }

        [HttpGet]
        [Route("api/gestion/GetObtenerDocumentoDevengado")]
        public async Task<HttpResponseMessage> GetObtenerDocumentoDevengado([FromBody] DocumentoDevengadoRequest documento)
        {
            ApiResponse<BLL.ServiceEntidades.DocumentoDevengadoEN> responsePorDefecto = new ApiResponse<BLL.ServiceEntidades.DocumentoDevengadoEN>
            {
                Success = false,
                Data = new List<BLL.ServiceEntidades.DocumentoDevengadoEN>(),
                Message = null,
                Error = null,
                StackTrace = null,
                ArchivoByteExcel = null,
                ArchivoBytePDF = null
            };

            try
            {
                List<BLL.ServiceEntidades.DocumentoDevengadoEN> ListCatera = await ObjCarteraBLL.GetDataDocumentoDevengado(documento);
                if (ListCatera.Count == 0)
                {
                    responsePorDefecto.Message = "No se encontraron registros de cartera.";
                    responsePorDefecto.Error = "No hay datos disponibles.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                byte[] archivoByte = await ObjExportarExcelBLL.GetDocumentoDevengado(ListCatera);
                if (archivoByte == null || archivoByte.Length == 0)
                {
                    responsePorDefecto.Message = "No se pudo generar el archivo solicitado.";
                    responsePorDefecto.Error = "El archivo no está disponible o está vacío. Por favor, intente nuevamente más tarde.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                string archivoBase64Excel = Convert.ToBase64String(archivoByte);

                responsePorDefecto.Success = true;
                responsePorDefecto.Message = "Los datos se han recuperado correctamente.";
                responsePorDefecto.Data = ListCatera;
                responsePorDefecto.ArchivoByteExcel = archivoBase64Excel;

                return Request.CreateResponse(HttpStatusCode.OK, responsePorDefecto);
            }
            catch (Exception ex)
            {
                // Log de errores - descomentar para producción
                // ObjLog.GeneraArchivoLog(ex.Message, Controlador, Metodo);

                responsePorDefecto.Message = "Ocurrió un error al recuperar los datos.";
                responsePorDefecto.Error = ex.Message;
                responsePorDefecto.StackTrace = ex.StackTrace;

                return Request.CreateResponse(HttpStatusCode.InternalServerError, responsePorDefecto);
            }
        }

        [HttpGet]
        [Route("api/gestion/GetObtenerOperacionCursadas")]
        public async Task<HttpResponseMessage> GetObtenerOperacionCursadas([FromBody] OperacionCursadasRequest operacion)
        {
            ApiResponse<BLL.ServiceEntidades.OperacionCursadaEN> responsePorDefecto = new ApiResponse<BLL.ServiceEntidades.OperacionCursadaEN>
            {
                Success = false,
                Data = new List<BLL.ServiceEntidades.OperacionCursadaEN>(),
                Message = null,
                Error = null,
                StackTrace = null,
                ArchivoByteExcel = null,
                ArchivoBytePDF = null
            };

            try
            {
                List<BLL.ServiceEntidades.OperacionCursadaEN> Listdata = await ObjCarteraBLL.GetDataOperacionCursadas(operacion);
                if (Listdata.Count == 0)
                {
                    responsePorDefecto.Message = "No se encontraron registros de cartera.";
                    responsePorDefecto.Error = "No hay datos disponibles.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                byte[] archivoByte = await ObjExportarExcelBLL.GetOperacionCursadas(Listdata, operacion.FechaDesde, operacion.FechaHasta);
                if (archivoByte == null || archivoByte.Length == 0)
                {
                    responsePorDefecto.Message = "No se pudo generar el archivo solicitado.";
                    responsePorDefecto.Error = "El archivo no está disponible o está vacío. Por favor, intente nuevamente más tarde.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                string archivoBase64Excel = Convert.ToBase64String(archivoByte);

                responsePorDefecto.Success = true;
                responsePorDefecto.Message = "Los datos se han recuperado correctamente.";
                responsePorDefecto.Data = Listdata;
                responsePorDefecto.ArchivoByteExcel = archivoBase64Excel;

                return Request.CreateResponse(HttpStatusCode.OK, responsePorDefecto);
            }
            catch (Exception ex)
            {
                // Log de errores - descomentar para producción
                // ObjLog.GeneraArchivoLog(ex.Message, Controlador, Metodo);

                responsePorDefecto.Message = "Ocurrió un error al recuperar los datos.";
                responsePorDefecto.Error = ex.Message;
                responsePorDefecto.StackTrace = ex.StackTrace;

                return Request.CreateResponse(HttpStatusCode.InternalServerError, responsePorDefecto);
            }
        }

        [HttpGet]
        [Route("api/gestion/GetObtenerExcedentes")]
        public async Task<HttpResponseMessage> GetObtenerExcedentes([FromBody] ExcedentesRequest excedentes)
        {
            ApiResponse<BLL.ServiceEntidades.ExcedentesEN> responsePorDefecto = new ApiResponse<BLL.ServiceEntidades.ExcedentesEN>
            {
                Success = false,
                Data = new List<BLL.ServiceEntidades.ExcedentesEN>(),
                Message = null,
                Error = null,
                StackTrace = null,
                ArchivoByteExcel = null,
                ArchivoBytePDF = null
            };

            try
            {
                List<BLL.ServiceEntidades.ExcedentesEN> Listdata = await ObjCarteraBLL.GetDataExcedentes(excedentes);
                if (Listdata.Count == 0)
                {
                    responsePorDefecto.Message = "No se encontraron registros de cartera.";
                    responsePorDefecto.Error = "No hay datos disponibles.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                //Quede aqui
                byte[] archivoByte = await ObjExportarExcelBLL.GetExcedentes(Listdata);
                if (archivoByte == null || archivoByte.Length == 0)
                {
                    responsePorDefecto.Message = "No se pudo generar el archivo solicitado.";
                    responsePorDefecto.Error = "El archivo no está disponible o está vacío. Por favor, intente nuevamente más tarde.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                string archivoBase64Excel = Convert.ToBase64String(archivoByte);

                responsePorDefecto.Success = true;
                responsePorDefecto.Message = "Los datos se han recuperado correctamente.";
                responsePorDefecto.Data = Listdata;
                responsePorDefecto.ArchivoByteExcel = archivoBase64Excel;

                return Request.CreateResponse(HttpStatusCode.OK, responsePorDefecto);
            }
            catch (Exception ex)
            {
                // Log de errores - descomentar para producción
                // ObjLog.GeneraArchivoLog(ex.Message, Controlador, Metodo);

                responsePorDefecto.Message = "Ocurrió un error al recuperar los datos.";
                responsePorDefecto.Error = ex.Message;
                responsePorDefecto.StackTrace = ex.StackTrace;

                return Request.CreateResponse(HttpStatusCode.InternalServerError, responsePorDefecto);
            }
        }
    }
}