using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiRepoteria.AL.Entidades;
using WebApiRepoteria.BLL.BusinessHelpers;
using WebApiRepoteria.BLL.Interfaz;
using WebApiRepoteria.DAL.Implementos;
using WebApiRepoteria.DAL.Interfaz;

namespace WebApiRepoteria.BLL.Implementos
{
    public class GestionBLL : IGestionBLL
    {
        private readonly RutinasGeneralesHelpersBLL ClsHelpers = new RutinasGeneralesHelpersBLL();
        private readonly IExecuteDAL ObjDAL = new ExecuteDAL();
        private readonly Elog ObjLog = new Elog();

        public async Task<List<ServiceEntidades.CarteraEN>> GetDataCarteraVigente()
        {
            string NombreProcedure = "sp_informe_de_cartera";

            DataTable GetData = await ObjDAL.ExecuteProcedureAsync(NombreProcedure);
            if (GetData.Rows.Count == 0)
            {
                return null; // Manejar el caso en el que no se obtienen datos
            }

            foreach (DataColumn column in GetData.Columns)
            {
                Console.WriteLine($"Columna: {column.ColumnName}, Tipo: {column.DataType}");
            }

            var Data = GetData.AsEnumerable().Select(r => new ServiceEntidades.CarteraEN
            {
                ID = r.Field<long?>("Num") ?? 0,
                AfectaExenta = r.Field<string>("AfectaExenta"),
                CentroCosto = r.Field<string>("centro_costo"),
                ConResponsabilidad = r.Field<string>("responsabilidad"),
                ConSinCesion = r.Field<string>("cesion"),
                DigitoCliente = r.Field<string>("DV_cliente"),
                DigitoDeudor = r.Field<string>("DV_deudor"),
                NombreCliente = r.Field<string>("Nombre_cliente"),
                NombreDeudor = r.Field<string>("Nombre_tercero"),
                NroDocto = r.Field<string>("Docto"),
                RechazoNotificacion = r.Field<string>("Rechazo_Notificacion"),
                TipoDocto = r.Field<string>("Tipo_docto"),
                TipoDoctoCorta = r.Field<string>("Tipo_FA"),
                EjecutivoComercial = r.Field<string>("Ejecutivo_Comercial"),
                SistemaOrigen = r.Field<string>("SistemaOrigen"),
                PlataformaComercial = r.Field<string>("Plataforma_Comercial"),
                EjecutivoFactoring = r.Field<string>("Ejecutivo_Factoring"),
                EjecutivoCobranza = r.Field<string>("Ejecutivo_Cobranza"),
                UltimaGestionCobranza = r.Field<string>("Ultima_gestion_Cobranza"),
                DocumentoCedido = r.Field<string>("DoctoCedido"),
                EstadoNotaCredito = r.Field<string>("NotaCredito"),
                ConGarantia = r.Field<string>("ConGarantia"),
                EstadoReclamo = r.Field<string>("Reclamo"),
                Mandante = r.Field<string>("Mandante"),
                Origen = r.Field<string>("origenOperacion"),
                Moneda = r.Field<string>("MON"),
                EstadoDocto = r.Field<string>("EstadoPago"),
                EstadoGestionCobranza = r.Field<string>("EstadoGestion"),
                Prorroga = r.Field<string>("Prorroga"),
                MontoAbonado = (double)r.Field<decimal>("MontoAbonado"),
                MontoInteres = (double)r.Field<decimal>("interesmora"),
                Deuda = (double)r.Field<decimal>("deuda"),
                Saldo = (double)r.Field<decimal>("Saldo"),
                FactorCambioActual = (double)r.Field<decimal>("FactorCambio"),
                MontoAnticipo = (double)r.Field<decimal>("MontoAnticipo"),
                ValorDocto = (double)r.Field<decimal>("Valor_Documento"),
                PorcentajeAnticipo = (double)r.Field<decimal>("PorcentajeAnticipo"),
                TasaOperacion = (double)r.Field<decimal>("Tasa_Negocio"),
                TasasMora = (double)r.Field<decimal>("tasaMora"),
                DiasMora = r.Field<int>("Dias_Mora"),
                IdMoneda = r.Field<int>("id_p_0023"),
                NroOperacion = (long)r.Field<decimal>("Num_Ope"),
                NroProrrogas = (long)r.Field<decimal>("Nro_Prorrogas"),
                RutCliente = long.Parse(r.Field<string>("Rut_cliente")),
                RutDeudor = long.Parse(r.Field<string>("Rut_deudor")),
                RutEmisor = (long)r.Field<int>("RutEmisor"),
                FechaCompromiso = r.Field<DateTime>("Fecha_Compromiso"),
                FechaCurse = r.Field<DateTime>("FechaCurse"),
                FechaOtorgada = r.Field<DateTime>("FechaOtorgado"),
                FechaVcto = r.Field<DateTime>("Vence"),
                FechaVctoOriginal = r.Field<DateTime>("Fecha_original"),
                UltimaFechaPago = r.Field<DateTime>("DOC_FUL_PGO"),
                FechaEmision = r.Field<DateTime>("FechaEmision"),


            }).ToList();

            return await Task.FromResult(Data);
        }

        public async Task<List<ServiceEntidades.DocumentoCompradoEN>> GetDataDocumentoComprado(DocumentoCompradoRequest documento)
        {
            string NombreProcedure = "spReporteDocumentosComprados";
            List<ModeloDataEN> GetModeloLista = new List<ModeloDataEN>{
                new ModeloDataEN { NombreColumna = "@RutCliente", ValorString = ClsHelpers.SetearCeroIzquierdaRut(documento.RutCliente) },
                new ModeloDataEN { NombreColumna = "@RutDeudor", ValorString = ClsHelpers.SetearCeroIzquierdaRut(documento.RutDeudor) },
                new ModeloDataEN { NombreColumna = "@TipoDocto", ValorString = documento.TipoDocto.ToString() ?? "0" },
                new ModeloDataEN { NombreColumna = "@FechaVctoDesde", ValorString = ClsHelpers.ConvertirFechaJuliana(documento.FechaDesde) },
                new ModeloDataEN { NombreColumna = "@FechaVctoHasta", ValorString = ClsHelpers.ConvertirFechaJuliana(documento.FechaHasta) },
                new ModeloDataEN { NombreColumna = "@NroOperacion", ValorString = documento.NroOperacion.ToString() ?? "0"},
                new ModeloDataEN { NombreColumna = "@FechaOtorgadoDesde", ValorString = ClsHelpers.ConvertirFechaJuliana(documento.FechaCurseDesde) },
                new ModeloDataEN { NombreColumna = "@FechaOtorgadoHasta", ValorString = ClsHelpers.ConvertirFechaJuliana(documento.FechaCurseHasta) },
                new ModeloDataEN { NombreColumna = "@NroDocto", ValorString = documento.NroDocto.ToString() ?? ""},
                new ModeloDataEN { NombreColumna = "@Estado", ValorString = documento.IdEstado.ToString() ?? "0"}
            };
            DataTable GetData = await ObjDAL.ExecuteProcedureAsync(NombreProcedure, GetModeloLista);
            if (GetData.Rows.Count == 0)
            {
                return null; // Manejar el caso en el que no se obtienen datos
            }

            //foreach (DataColumn column in GetData.Columns)
            //{
            //    ObjLog.generaArchivoLog($"Columna: {column.ColumnName}, Tipo: {column.DataType}");
            //}

            var Data = GetData.AsEnumerable().Select(r => new ServiceEntidades.DocumentoCompradoEN
            {
                AfectaExenta = r.Field<string>("AfectaExenta"),
                OrigenOperacion = r.Field<string>("origen"),
                Cesion = r.Field<string>("Cesion"),
                EstadoDocto = r.Field<string>("EstadoDocto"),
                Moneda = r.Field<string>("Moneda"),
                NombreCliente = r.Field<string>("Cliente"),
                NombreDeudor = r.Field<string>("Deudor"),
                NroDocto = r.Field<string>("NroDocto"),
                TipoDocto = r.Field<string>("TipoDocto"),
                TipoDoctoCorta = r.Field<string>("Tipo_FA"),
                FechaEmision = r.Field<DateTime>("FechaEmision"),
                FechaOtorgado = r.Field<DateTime>("FechaOtorgado"),
                FechaVcto = r.Field<DateTime>("FechaVcto"),
                IdMoneda = r.Field<int>("IDMoneda"),
                IDDoc = (long)r.Field<decimal>("IDDoc"),
                NroOperacion = (long)r.Field<decimal>("NroOperacion"),
                RutCliente = long.Parse(r.Field<string>("RutCliente")),
                RutDeudor = long.Parse(r.Field<string>("RutDeudor")),
                RutEmisor = long.Parse(r.Field<string>("emisor")),
                MontoAnticipo = (double)r.Field<decimal>("MontoAnticipo"),
                PorcentajeAnticipo = (double)r.Field<decimal>("PorcentajeAnticipo"),
                Comision = (double)r.Field<int>("Comision"),
                MontoDocto = (double)r.Field<decimal>("MontoDocto"),
                DiferenciaPrecio = (double)r.Field<decimal>("DiferenciaPrecio"),
                TasaNegocio = (double)r.Field<decimal>("TasaNegocio"),


            }).ToList();

            return await Task.FromResult(Data);
        }

        public async Task<List<ServiceEntidades.DocumentoDevengadoEN>> GetDataDocumentoDevengado(DocumentoDevengadoRequest documento)
        {
            string NombreProcedure = "sp_inf_devuelve_devengo_acumulado";
            List<ModeloDataEN> GetModeloLista = new List<ModeloDataEN>{
                new ModeloDataEN { NombreColumna = "@mes", ValorString = documento.anno.ToString() },
                new ModeloDataEN { NombreColumna = "@anho", ValorString =documento.mes.ToString() }
            };
            DataTable GetData = await ObjDAL.ExecuteProcedureAsync(NombreProcedure, GetModeloLista);
            if (GetData.Rows.Count == 0)
            {
                return null; // Manejar el caso en el que no se obtienen datos
            }

            foreach (DataColumn column in GetData.Columns)
            {
                ObjLog.generaArchivoLog($"Columna: {column.ColumnName}, Tipo: {column.DataType}");
            }

            var Data = GetData.AsEnumerable().Select(r => new ServiceEntidades.DocumentoDevengadoEN
            {
                CentroCosto = r.Field<string>("CentroCosto"),
                Cesión = r.Field<string>("Cesion"),
                Moneda = r.Field<string>("MONEDA"),
                TipoDocto = r.Field<string>("TIPO_DOCTO"),
                NroOperacion = (long)r.Field<decimal>("NRO_OPERACION"),
                DevengoDevuelto = (double)r.Field<decimal>("DEVENGO_DEVUELTO"),
                DevengoMoroso = (double)r.Field<decimal>("DEVENGO_MOROSO"),
                DevengoPagado = (double)r.Field<decimal>("DEVENGO_PAGADO"),
                DevengoVigente = (double)r.Field<decimal>("DEVENGO_VIGENTE"),
                DiferenciaPrecio = (double)r.Field<decimal>("DIFERENCIA_DE_PRECIO"),
                PorDevengar = (double)r.Field<decimal>("POR_DEVENGAR"),
                TotalMes = (double)r.Field<decimal>("TOTAL_MES")
            }).ToList();

            return await Task.FromResult(Data);
        }

        public async Task<List<ServiceEntidades.OperacionCursadaEN>> GetDataOperacionCursadas(OperacionCursadasRequest operacion)
        {
            string NombreProcedure = "SpRetornaOperacionesCursadas";
            List<ModeloDataEN> GetModeloLista = new List<ModeloDataEN>{
                new ModeloDataEN { NombreColumna = "@RutCliente", ValorString = ClsHelpers.SetearCeroIzquierdaRut(operacion.RutCliente) },
                new ModeloDataEN { NombreColumna = "@IdSucursal", ValorString = operacion.IdSucursal .ToString() },
                new ModeloDataEN { NombreColumna = "@IdEjecutivo", ValorString = operacion.IdEjecutivo .ToString() },
                new ModeloDataEN { NombreColumna = "@TipoDocumento", ValorString = operacion.TipoDocto .ToString() },
                new ModeloDataEN { NombreColumna = "@fechaDesde", ValorString =  ClsHelpers.ConvertirFechaJuliana(operacion.FechaDesde) },
                new ModeloDataEN { NombreColumna = "@fechaHasta", ValorString = ClsHelpers.ConvertirFechaJuliana(operacion.FechaHasta) }
            };

            DataTable GetData = await ObjDAL.ExecuteProcedureAsync(NombreProcedure, GetModeloLista);
            if (GetData.Rows.Count == 0)
            {
                return null; // Manejar el caso en el que no se obtienen datos
            }

            foreach (DataColumn column in GetData.Columns)
            {
                ObjLog.generaArchivoLog($"Columna: {column.ColumnName}, Tipo: {column.DataType}");
            }

            var Data = GetData.AsEnumerable().Select(r => new ServiceEntidades.OperacionCursadaEN
            {
                NombreCliente = r.Field<string>("NombreCliente"),
                TipoDocto = r.Field<string>("TipoDocto"),
                CentroCosto = r.Field<string>("CentroCosto"),
                Cesion = r.Field<string>("Cesion"),
                RutCliente = long.Parse(r.Field<string>("RutCliente")),
                Ejecutivo = r.Field<string>("ejecutivo"),
                Moneda = r.Field<string>("Moneda"),
                OrigenOperacion = r.Field<string>("OrigenOperacion"),
                NroOperacion = (long)r.Field<decimal>("NroOperacion"),
                FechaOtorgado = r.Field<DateTime>("FechaOtorgado"),
                DiferenciaPrecio = (double)r.Field<decimal>("DiferenciaPrecio"),
                Excedentes = (double)r.Field<decimal>("Excedente"),
                FactorCambioActual = (double)r.Field<decimal>("FactorCambio"),
                Gastos = (double)r.Field<decimal>("GastoOperacion"),
                Monto = (double)r.Field<decimal>("MontoOperacion"),
                MontoAnticipo = (double)r.Field<decimal>("MontoAnticipo"),
                MontoGiro = (double)r.Field<decimal>("MontoGiro"),
                PorcentajeAnticipo = (double)r.Field<decimal>("PorcentajeAnticipo"),
                Promedio = (double)r.Field<decimal>("Promedio"),
                Recaudacion = (double)r.Field<decimal>("Recaudacion"),
                TasaNegocio = (double)r.Field<decimal>("TasaNegocio")
            }).ToList();

            return await Task.FromResult(Data);
        }

        public async Task<List<ServiceEntidades.ExcedentesEN>> GetDataExcedentes(ExcedentesRequest excedentes)
        {
            string NombreProcedure = "SpRetornaOperacionesCursadas";
            List<ModeloDataEN> GetModeloLista = new List<ModeloDataEN>{
                new ModeloDataEN { NombreColumna = "@RutCliente", ValorString = ClsHelpers.SetearCeroIzquierdaRut(excedentes.RutCliente) },
                new ModeloDataEN { NombreColumna = "@TipoDocumento", ValorString = excedentes.Vigente == true ? "0":"1" },
            };

            DataTable GetData = await ObjDAL.ExecuteProcedureAsync(NombreProcedure, GetModeloLista);
            if (GetData.Rows.Count == 0)
            {
                return null; // Manejar el caso en el que no se obtienen datos
            }

            foreach (DataColumn column in GetData.Columns)
            {
                ObjLog.generaArchivoLog($"Columna: {column.ColumnName}, Tipo: {column.DataType}");
            }

            var Data = GetData.AsEnumerable().Select(r => new ServiceEntidades.ExcedentesEN
            {
                NombreCliente = r.Field<string>("NombreCliente"),
                TipoDocto = r.Field<string>("TipoDocto"),
                Moneda = r.Field<string>("Moneda"),
                NombreDeudor = r.Field<string>("NombreDeudor"),
                NroDocto = r.Field<string>("NroDocto"),
                FechaEntregado = r.Field<DateTime>("FechaEntrega"),
                FechaPago = r.Field<DateTime>("FechaPago"),
                FechaVcto = r.Field<DateTime>("FechaVcto"),
                IdMoneda = r.Field<int>("IdMoneda"),
                NroOperacion = (long)r.Field<decimal>("NroOperacion"),
                NroPago = (long)r.Field<decimal>("NroPago"),
                RutDeudor = long.Parse(r.Field<string>("RutDeudor")),
                RutCliente = long.Parse(r.Field<string>("RutCliente")),
                MontoAnticipo = (double)r.Field<decimal>("ValorDocumento"),
                MontoExcedente = (double)r.Field<decimal>("SaldoExcedente"),
                MontoPagado = (double)r.Field<decimal>("MontoPagado")
            }).ToList();

            return await Task.FromResult(Data);
        }


    }
}
