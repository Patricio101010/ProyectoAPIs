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
    public class PruebaController : ApiController
    {
        private readonly string Controlador = "AdministracionController";
        private string Metodo = "";

        private readonly ICuentaBLL ObjCuentaBLL = new CuentaBLL();

        [HttpGet]
        [Route("api/prueba/GetTodasCuentasPorCobrar")]
        public async Task<HttpResponseMessage> GetTodasCuentasPorCobrar([FromBody] ParametrosEntradaRequest parametrosEntrada)
        {
            ApiResponse<BLL.ServiceEntidades.CuentaPorCobrarEN> responsePorDefecto = new ApiResponse<BLL.ServiceEntidades.CuentaPorCobrarEN>
            {
                Success = false,
                ArchivoByteExcel = null,
                ArchivoBytePDF = null,
                Data = new List<BLL.ServiceEntidades.CuentaPorCobrarEN>(),
                StackTrace = null
            };

            Metodo = "GetTodasCuentasPorCobrar";
            if (parametrosEntrada == null)
            {
                responsePorDefecto.Message = "La solicitud no pudo ser procesada debido a un parámetro inválido.";
                responsePorDefecto.Error = "El cuerpo del parámetro es nulo. Esto indica que no se enviaron los datos requeridos en la solicitud o que hubo un problema en la estructura del JSON.";

                return Request.CreateResponse(HttpStatusCode.BadRequest, responsePorDefecto);
            }

            try
            {
                List<BLL.ServiceEntidades.CuentaPorCobrarEN> ListCuenta = await ObjCuentaBLL.getDevelveDatos(parametrosEntrada);
                if (ListCuenta.Count == 0)
                {
                    responsePorDefecto.Message = "No se encontraron registros de cuentas por cobrar.";
                    responsePorDefecto.Error = "No hay datos disponibles.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }


                responsePorDefecto.Success = true;
                responsePorDefecto.Message = "Los datos se han recuperado correctamente.";
                responsePorDefecto.Error = "No hay datos disponibles.";
                responsePorDefecto.Data = ListCuenta;
                responsePorDefecto.ArchivoByteExcel = null;
                responsePorDefecto.ArchivoBytePDF = null;

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