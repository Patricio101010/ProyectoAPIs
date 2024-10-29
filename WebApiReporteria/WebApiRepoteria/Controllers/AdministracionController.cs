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
    public class AdministracionController : ApiController
    {
        private readonly string Controlador = "AdministracionController";
        private string Metodo = "";

        //private readonly ILogEN ObjLog = new LogEN();
        private readonly IAdministracionExportarExcelBLL ObjExportarExcelBLL = new AdministracionExportarExcelBLL();
        private readonly IAdministracionExportarPDFBLL ObjExportarPDFBLL = new AdministracionExportarPDFBLL();
        private readonly ICuentaBLL ObjCuentaBLL = new CuentaBLL();

        [HttpGet]
        [Route("api/Administracion/GetTodasCuentasPorCobrar")]
        public async Task<HttpResponseMessage> GetTodasCuentasPorCobrar([FromBody] CuentasPorCobrarRequest cuentaPorCobrar)
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
            if (cuentaPorCobrar == null)
            {
                responsePorDefecto.Message = "La solicitud no pudo ser procesada debido a un parámetro inválido.";
                responsePorDefecto.Error = "El cuerpo del parámetro es nulo. Esto indica que no se enviaron los datos requeridos en la solicitud o que hubo un problema en la estructura del JSON.";

                return Request.CreateResponse(HttpStatusCode.BadRequest, responsePorDefecto);
            }

            try
            {
                List<BLL.ServiceEntidades.CuentaPorCobrarEN> ListCuenta = await ObjCuentaBLL.GetCuentasPorCobrarPorCliente(cuentaPorCobrar);
                if (ListCuenta.Count == 0)
                {
                    responsePorDefecto.Message = "No se encontraron registros de cuentas por cobrar.";
                    responsePorDefecto.Error = "No hay datos disponibles.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                // Iniciar ambas tareas y almacenar sus resultados
                Task<byte[]> archivoByte1 = ObjExportarExcelBLL.GetCuentasPorCobrar(ListCuenta);
                Task<byte[]> archivoByte2 = ObjExportarPDFBLL.GetCuentasPorCobrarPorCliente(ListCuenta);

                byte[][] resultados = await Task.WhenAll(archivoByte1, archivoByte2);

                // Esperar a que ambas tareas finalicen
                byte[] archivoByteExcel = resultados[0];
                byte[] archivoBytePDF = resultados[1];

                int CasoError = 0;
                if (archivoByteExcel == null || archivoByteExcel.Length == 0)
                {
                    CasoError = 1;
                }
                else if (archivoBytePDF == null || archivoBytePDF.Length == 0)
                {
                    CasoError = 2;
                }
                else if (archivoByteExcel == null & archivoByteExcel.Length == 0 & archivoBytePDF == null & archivoBytePDF.Length == 0)
                {
                    CasoError = 3;
                }

                bool ArchivoConProblema = false;
                string archivoBase64Excel = "", archivoBase64PDF = "";

                switch (CasoError)
                {
                    case 0:
                        archivoBase64PDF = Convert.ToBase64String(archivoBytePDF);
                        archivoBase64Excel = Convert.ToBase64String(archivoByteExcel);

                        break;
                    case 1:
                        ArchivoConProblema = true;

                        responsePorDefecto.Message = "No se pudo generar el archivo solicitado.";
                        responsePorDefecto.Error = "El archivo no está disponible o está vacío. Por favor, intente nuevamente más tarde.";

                        archivoBase64PDF = Convert.ToBase64String(archivoBytePDF);
                        responsePorDefecto.ArchivoBytePDF = archivoBase64PDF;

                        break;
                    case 2:
                        ArchivoConProblema = true;

                        responsePorDefecto.Message = "No se pudo generar el archivo solicitado.";
                        responsePorDefecto.Error = "El archivo no está disponible o está vacío. Por favor, intente nuevamente más tarde.";

                        archivoBase64Excel = Convert.ToBase64String(archivoByteExcel);
                        responsePorDefecto.ArchivoByteExcel = archivoBase64Excel;

                        break;
                    case 3:
                        ArchivoConProblema = true;

                        responsePorDefecto.Message = "No se pudo generar el archivo solicitado.";
                        responsePorDefecto.Error = "El archivo no está disponible o está vacío. Por favor, intente nuevamente más tarde.";

                        break;
                }

                if (ArchivoConProblema)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                responsePorDefecto.Success = true;
                responsePorDefecto.Message = "Los datos se han recuperado correctamente.";
                responsePorDefecto.Error = "No hay datos disponibles.";
                responsePorDefecto.Data = ListCuenta;
                responsePorDefecto.ArchivoByteExcel = archivoBase64Excel;
                responsePorDefecto.ArchivoBytePDF = archivoBase64PDF;

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
        [Route("api/Administracion/GetCuentasPorCobrarPorCliente")]
        public async Task<HttpResponseMessage> GetCuentasPorCobrarPorCliente([FromBody] CuentasPorCobrarRequest cuentaPorCobrar)
        {
            ApiResponse<BLL.ServiceEntidades.CuentaPorCobrarEN> responsePorDefecto = new ApiResponse<BLL.ServiceEntidades.CuentaPorCobrarEN>
            {
                Success = false,
                Data = new List<BLL.ServiceEntidades.CuentaPorCobrarEN>(),
                Message = null,
                Error = null,
                StackTrace = null,
                ArchivoByteExcel = null,
                ArchivoBytePDF = null
            };

            Metodo = "GetCuentasPorCobrarPorCliente";
            if (cuentaPorCobrar == null)
            {
                responsePorDefecto.Message = "La solicitud no pudo ser procesada debido a un parámetro inválido.";
                responsePorDefecto.Error = "El cuerpo del parámetro es nulo. Esto indica que no se enviaron los datos requeridos en la solicitud o que hubo un problema en la estructura del JSON.";

                return Request.CreateResponse(HttpStatusCode.BadRequest, responsePorDefecto);
            }

            if (cuentaPorCobrar.RutCliente == 0)
            {
                responsePorDefecto.Message = "El parámetro 'RutCliente' es obligatorio.";
                responsePorDefecto.Error = "El valor del argumento 'RutCliente' no puede ser 0. Debe proporcionar un RUT válido para continuar con la solicitud.";

                return Request.CreateResponse(HttpStatusCode.BadRequest, responsePorDefecto);
            }

            try
            {
                List<BLL.ServiceEntidades.CuentaPorCobrarEN> ListCuenta = await ObjCuentaBLL.GetCuentasPorCobrarPorCliente(cuentaPorCobrar);
                if (ListCuenta.Count == 0)
                {
                    responsePorDefecto.Message = "No se encontraron registros de cuentas por cobrar.";
                    responsePorDefecto.Error = "No hay datos disponibles.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                // Obtén el archivo en formato byte[]
                var archivoByte = await ObjExportarExcelBLL.GetCuentasPorCobrarPorCliente(ListCuenta);
                if (archivoByte == null || archivoByte.Length == 0)
                {
                    responsePorDefecto.Message = "No se encontró el archivo o el archivo está vacío.";
                    responsePorDefecto.Error = "Ocurrió un problema al generar el archivo Excel. El archivo es nulo o su contenido está vacío.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                // Convertimos el byte[] a un string en base64 para poder enviarlo en JSON
                responsePorDefecto.Success = true;
                responsePorDefecto.Message = "Los datos se han recuperado correctamente.";
                responsePorDefecto.Data = ListCuenta;

                string archivoBase64 = Convert.ToBase64String(archivoByte);
                responsePorDefecto.ArchivoByteExcel = archivoBase64;

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
        [Route("api/Administracion/GetTodasCuentasPorPagar")]
        public async Task<HttpResponseMessage> GetTodasCuentasPorPagar([FromBody] CuentasPorPagarRequest cuentaPorPagar)
        {
            Metodo = "GetTodasCuentasPorPagar";
            ApiResponse<BLL.ServiceEntidades.CuentaPorPagarEN> responsePorDefecto = new ApiResponse<BLL.ServiceEntidades.CuentaPorPagarEN>
            {
                Success = false,
                Data = new List<BLL.ServiceEntidades.CuentaPorPagarEN>(),
                Message = null,
                Error = null,
                StackTrace = null,
                ArchivoByteExcel = null,
                ArchivoBytePDF = null
            };

            if (cuentaPorPagar == null)
            {
                responsePorDefecto.Message = "La solicitud no pudo ser procesada debido a un parámetro inválido.";
                responsePorDefecto.Error = "El cuerpo del parámetro es nulo. Esto indica que no se enviaron los datos requeridos en la solicitud o que hubo un problema en la estructura del JSON.";

                return Request.CreateResponse(HttpStatusCode.BadRequest, responsePorDefecto);
            }

            try
            {
                List<BLL.ServiceEntidades.CuentaPorPagarEN> ListCuenta = await ObjCuentaBLL.GetDataTodasCuentasPorPagar(cuentaPorPagar);
                if (ListCuenta.Count == 0)
                {
                    responsePorDefecto.Message = "No se encontraron registros de cuentas por pagar.";
                    responsePorDefecto.Error = "No hay datos disponibles.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                // Obtén el archivo en formato byte[]
                var archivoByte = await ObjExportarExcelBLL.GetCuentasPorPagar(ListCuenta);
                if (archivoByte == null || archivoByte.Length == 0)
                {
                    responsePorDefecto.Message = "No se encontró el archivo o el archivo está vacío.";
                    responsePorDefecto.Error = "Ocurrió un problema al generar el archivo Excel. El archivo es nulo o su contenido está vacío.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                // Convertimos el byte[] a un string en base64 para poder enviarlo en JSON

                responsePorDefecto.Success = true;
                responsePorDefecto.Message = "Los datos se han recuperado correctamente.";
                responsePorDefecto.Data = ListCuenta;
                string archivoBase64 = Convert.ToBase64String(archivoByte);
                responsePorDefecto.ArchivoByteExcel = archivoBase64;

                return Request.CreateResponse(HttpStatusCode.OK, responsePorDefecto);
            }
            catch (Exception ex)
            {
                // Log de errores - descomentar para producción
                // ObjLog.GeneraArchivoLog(ex.Message, Controlador, Metodo);

                responsePorDefecto.Message = "Ocurrió un error al recuperar los datos";
                responsePorDefecto.Error = ex.Message;
                responsePorDefecto.StackTrace = ex.StackTrace;

                return Request.CreateResponse(HttpStatusCode.InternalServerError, responsePorDefecto);
            }
        }

        [HttpGet]
        [Route("api/Administracion/GetCuentasPorPagarPorCliente")]
        public async Task<HttpResponseMessage> GetCuentasPorPagarPorCliente([FromBody] CuentasPorPagarRequest cuentaPorPagar)
        {
            Metodo = "GetCuentasPorPagarPorCliente";
            ApiResponse<BLL.ServiceEntidades.CuentaPorPagarEN> responsePorDefecto = new ApiResponse<BLL.ServiceEntidades.CuentaPorPagarEN>
            {
                Success = false,
                Data = new List<BLL.ServiceEntidades.CuentaPorPagarEN>(),
                Message = null,
                Error = null,
                StackTrace = null,
                ArchivoByteExcel = null,
                ArchivoBytePDF = null
            };

            if (cuentaPorPagar == null)
            {
                responsePorDefecto.Message = "La solicitud no pudo ser procesada debido a un parámetro inválido.";
                responsePorDefecto.Error = "El cuerpo del parámetro es nulo. Esto indica que no se enviaron los datos requeridos en la solicitud o que hubo un problema en la estructura del JSON.";

                return Request.CreateResponse(HttpStatusCode.BadRequest, responsePorDefecto);
            }

            if (cuentaPorPagar.RutCliente == 0)
            {
                responsePorDefecto.Message = "El parámetro 'RutCliente' es obligatorio.";
                responsePorDefecto.Error = "El valor del argumento 'RutCliente' no puede ser 0. Debe proporcionar un RUT válido para continuar con la solicitud.";

                return Request.CreateResponse(HttpStatusCode.BadRequest, responsePorDefecto);
            }

            try
            {
                List<BLL.ServiceEntidades.CuentaPorPagarEN> ListCuenta = await ObjCuentaBLL.GetDataTodasCuentasPorPagar(cuentaPorPagar);
                if (ListCuenta.Count == 0)
                {
                    responsePorDefecto.Message = "No se encontraron registros de cuentas por pagar.";
                    responsePorDefecto.Error = "No hay datos disponibles.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }


                // Obtén el archivo en formato byte[]
                //var archivoByte = await ObjExportarExcelBLL.GetDataCuentasPorPagarPorClienteBLL(ListCuenta);
                var archivoByte = await ObjExportarExcelBLL.GetCuentasPorPagarPorCliente(ListCuenta);
                if (archivoByte == null || archivoByte.Length == 0)
                {
                    responsePorDefecto.Message = "No se encontró el archivo o el archivo está vacío.";
                    responsePorDefecto.Error = "Ocurrió un problema al generar el archivo Excel. El archivo es nulo o su contenido está vacío.";

                    return Request.CreateResponse(HttpStatusCode.NotFound, responsePorDefecto);
                }

                // Convertimos el byte[] a un string en base64 para poder enviarlo en JSON
                responsePorDefecto.Success = true;
                responsePorDefecto.Message = "Los datos se han recuperado correctamente.";
                responsePorDefecto.Data = ListCuenta;

                string archivoBase64 = Convert.ToBase64String(archivoByte);
                responsePorDefecto.ArchivoByteExcel = archivoBase64;

                return Request.CreateResponse(HttpStatusCode.OK, responsePorDefecto);
            }
            catch (Exception ex)
            {
                // Log de errores - descomentar para producción
                // ObjLog.GeneraArchivoLog(ex.Message, Controlador, Metodo);

                responsePorDefecto.Message = "Ocurrió un error al recuperar los datos";
                responsePorDefecto.Error = ex.Message;
                responsePorDefecto.StackTrace = ex.StackTrace;

                return Request.CreateResponse(HttpStatusCode.InternalServerError, responsePorDefecto);
            }
        }

    }
}