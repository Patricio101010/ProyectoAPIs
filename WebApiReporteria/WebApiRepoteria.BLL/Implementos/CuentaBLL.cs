using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebApiRepoteria.AL.Entidades;
using WebApiRepoteria.BLL.BusinessHelpers;
using WebApiRepoteria.BLL.Interfaz;
using WebApiRepoteria.DAL.Implementos;
using WebApiRepoteria.DAL.Interfaz;

namespace WebApiRepoteria.BLL.Implementos
{
    public class CuentaBLL : ICuentaBLL
    {
        private readonly RutinasGeneralesHelpersBLL ClsHelpers = new RutinasGeneralesHelpersBLL();
        private readonly IExecuteDAL ObjDAL = new ExecuteDAL();

        public async Task<List<ServiceEntidades.CuentaPorCobrarEN>> getDevelveDatos(ParametrosEntradaRequest parametros)
        {
            string NombreProcedure = "spObtenerListaCuentaPorCobrar";
            List<ModeloDataEN> GetModeloLista = new List<ModeloDataEN>{
                new ModeloDataEN { NombreColumna = "@Registro", ValorString = parametros.registro.ToString() },
                new ModeloDataEN { NombreColumna = "@CantidadRegistro", ValorString =  parametros.CantidadRegistro.ToString() },
                new ModeloDataEN { NombreColumna = "@OrderColumn", ValorString =  parametros.OrderColumna.ToString() },
                new ModeloDataEN { NombreColumna = "@DireccionOrder", ValorString =  parametros.DireccionOrder },
            };

            DataTable GetData = await ObjDAL.ExecuteProcedureAsync(NombreProcedure, GetModeloLista);
            if (GetData.Rows.Count == 0)
            {
                return null; // Manejar el caso en el que no se obtienen datos
            }

            long Registro = 0;
            var Data = GetData.AsEnumerable().Select(r => new ServiceEntidades.CuentaPorCobrarEN
            {
                ID = ++Registro,
                NumeroCuenta = r.Field<long>("NroCuenta"),
                RutCliente = r.Field<long>("RutCliente"),
                NombreCliente = r.Field<string>("RazonSocial"),
                NombreEjecutivo = r.Field<string>("Ejecutivo"),
                TipoCuenta = r.Field<string>("TipoCuenta"),
                Fecha = r.Field<DateTime>("FechaCreacion"),
                FechaPago = r.Field<DateTime>("FechaPago"),
                IdMoneda = r.Field<int>("IdMoneda"),
                Moneda = r.Field<string>("Moneda"),
                ValorCuenta = (double)r.Field<decimal>("Monto"),
                SaldoCuenta = (double)r.Field<decimal>("Saldo"),
                Descripcion = r.Field<string>("Descripcion"),
                FactorCambio = (double)r.Field<decimal>("FactorCambio"),
                Estado = r.Field<string>("Estado"),
            }).ToList();

            return await Task.FromResult(Data);
        }

        public async Task<List<ServiceEntidades.CuentaPorPagarEN>> GetDataCuentasPorPagarPorCliente(CuentasPorPagarRequest cuentaPorPagar)
        {
            string NombreProcedure = "sp_ObtenerCuentasPorPagarCliente";
            List<ModeloDataEN> GetModeloLista = new List<ModeloDataEN>{
                new ModeloDataEN { NombreColumna = "@RutCliente", ValorString = ClsHelpers.SetearCeroIzquierdaRut(cuentaPorPagar.RutCliente) },
                new ModeloDataEN { NombreColumna = "@FechaDesde", ValorString = ClsHelpers.ConvertirFechaJuliana(cuentaPorPagar.FechaDesde) },
                new ModeloDataEN { NombreColumna = "@FechaHasta", ValorString = ClsHelpers.ConvertirFechaJuliana(cuentaPorPagar.FechaHasta) },
                new ModeloDataEN { NombreColumna = "@IdMoneda", ValorString = cuentaPorPagar.IdMoneda.ToString() ?? string.Empty},
                new ModeloDataEN { NombreColumna = "@IdEstado", ValorString = cuentaPorPagar.IdEstado.ToString() ?? string.Empty},
                new ModeloDataEN { NombreColumna = "@NroCuenta", ValorString = cuentaPorPagar.NumeroCuenta.ToString() ?? string.Empty }
            };

            DataTable GetData = await ObjDAL.ExecuteProcedureAsync(NombreProcedure, GetModeloLista);
            if (GetData.Rows.Count == 0)
            {
                return null; // Manejar el caso en el que no se obtienen datos
            }

            long Registro = 0;
            var Data = GetData.AsEnumerable().Select(r => new ServiceEntidades.CuentaPorPagarEN
            {
                ID = ++Registro,
                NumeroCuenta = (int)r.Field<decimal>("nroCuenta"),
                Estado = r.Field<string>("estado"),
                Moneda = r.Field<string>("moneda"),
                Descripcion = r.Field<string>("descripcion"),
                FactorCambio = (double)r.Field<decimal>("factorCambio"),
                Fecha = r.Field<DateTime>("fechaCreacion"),
                IdMoneda = r.Field<int>("idMoneda"),
                TipoCuenta = r.Field<string>("tipoCuenta"),
                ValorCuenta = (double)r.Field<decimal>("valorCuenta"),
                NombreCliente = r.Field<string>("tipoCuenta"),
                DigitoCliente = r.Field<string>("tipoCuenta"),
                RutCliente = long.Parse(r.Field<string>("tipoCuenta"))

            }).ToList();

            return await Task.FromResult(Data);
        }

        public async Task<List<ServiceEntidades.CuentaPorPagarEN>> GetDataTodasCuentasPorPagar(CuentasPorPagarRequest cuentaPorPagar)
        {
            string NombreProcedure = "sp_ObtenerTodasCuentasPorPagar";
            List<ModeloDataEN> GetModeloLista = new List<ModeloDataEN>{
                new ModeloDataEN { NombreColumna = "@RutCliente", ValorString = ClsHelpers.SetearCeroIzquierdaRut(cuentaPorPagar.RutCliente) },
                new ModeloDataEN { NombreColumna = "@FechaDesde", ValorString = ClsHelpers.ConvertirFechaJuliana(cuentaPorPagar.FechaDesde) },
                new ModeloDataEN { NombreColumna = "@FechaHasta", ValorString = ClsHelpers.ConvertirFechaJuliana(cuentaPorPagar.FechaHasta) },
                new ModeloDataEN { NombreColumna = "@IdMoneda", ValorString = cuentaPorPagar.IdMoneda.ToString() ?? string.Empty},
                new ModeloDataEN { NombreColumna = "@IdEstado", ValorString = cuentaPorPagar.IdEstado.ToString() ?? string.Empty},
                new ModeloDataEN { NombreColumna = "@NroCuenta", ValorString = cuentaPorPagar.NumeroCuenta.ToString() ?? string.Empty }
            };

            DataTable GetData = await ObjDAL.ExecuteProcedureAsync(NombreProcedure, GetModeloLista);
            if (GetData.Rows.Count == 0)
            {
                return null; // Manejar el caso en el que no se obtienen datos
            }

            long Registro = 0;
            var Data = GetData.AsEnumerable().Select(r => new ServiceEntidades.CuentaPorPagarEN
            {
                ID = ++Registro,
                NumeroCuenta = (int)r.Field<decimal>("nroCuenta"),
                Estado = r.Field<string>("estado"),
                Moneda = r.Field<string>("moneda"),
                Descripcion = r.Field<string>("descripcion"),
                FactorCambio = (double)r.Field<decimal>("factorCambio"),
                Fecha = r.Field<DateTime>("fechaCreacion"),
                FechaPago = r.Field<DateTime>("fechaPago"),
                IdMoneda = r.Field<int>("idMoneda"),
                //SaldoCuenta = (double)r.Field<decimal>("SaldoCuenta"),
                TipoCuenta = r.Field<string>("tipoCuenta"),
                ValorCuenta = (double)r.Field<decimal>("valorCuenta"),
                CodigoEjecutivo = (int)r.Field<int>("codigoEjecutivo"),
                NombreEjecutivo = r.Field<string>("nombreEjecutivo"),
                NroDocto = r.Field<string>("nroDocto"),
                NroPago = (long)r.Field<decimal>("nroPago"),
                RutCliente = long.Parse(r.Field<string>("rutCliente")),
                DigitoCliente = r.Field<string>("digitoCliente"),
                NombreCliente = r.Field<string>("nombreCliente"),
                RutDeudor = long.Parse(r.Field<string>("rutDeudor")),
                DigitoDeudor = r.Field<string>("digitoDeudor"),
                NombreDeudor = r.Field<string>("razonSocialDeudor")
            }).ToList();

            return await Task.FromResult(Data);
        }

        public async Task<List<ServiceEntidades.CuentaPorCobrarEN>> GetCuentasPorCobrarPorCliente(CuentasPorCobrarRequest cuentaPorCobrar)
        {
            string NombreProcedure = "sp_ObtenerCuentasPorCobrarCliente";
            List<ModeloDataEN> GetModeloLista = new List<ModeloDataEN>{
                new ModeloDataEN { NombreColumna = "@RutCliente", ValorString = ClsHelpers.SetearCeroIzquierdaRut(cuentaPorCobrar.RutCliente) },
                new ModeloDataEN { NombreColumna = "@FechaDesde", ValorString = ClsHelpers.ConvertirFechaJuliana(cuentaPorCobrar.FechaDesde) },
                new ModeloDataEN { NombreColumna = "@FechaHasta", ValorString = ClsHelpers.ConvertirFechaJuliana(cuentaPorCobrar.FechaHasta) },
                new ModeloDataEN { NombreColumna = "@IdMoneda", ValorString = cuentaPorCobrar.IdMoneda.ToString() ?? string.Empty},
                new ModeloDataEN { NombreColumna = "@IdEstado", ValorString = cuentaPorCobrar.IdEstado.ToString() ?? string.Empty},
                new ModeloDataEN { NombreColumna = "@NroCuenta", ValorString = cuentaPorCobrar.NumeroCuenta.ToString() ?? string.Empty }
            };

            DataTable GetData = await ObjDAL.ExecuteProcedureAsync(NombreProcedure, GetModeloLista);
            if (GetData.Rows.Count == 0)
            {
                return null; // Manejar el caso en el que no se obtienen datos
            }

            long Registro = 0;
            var Data = GetData.AsEnumerable().Select(r => new ServiceEntidades.CuentaPorCobrarEN
            {
                ID = ++Registro,
                NumeroCuenta = (int)r.Field<decimal>("nroCuenta"),
                Estado = r.Field<string>("estado"),
                Moneda = r.Field<string>("moneda"),
                Descripcion = r.Field<string>("descripcion"),
                FactorCambio = (double)r.Field<decimal>("factorCambio"),
                Fecha = r.Field<DateTime>("fechaCreacion"),
                FechaPago = r.Field<DateTime>("fechaPago"),
                IdMoneda = r.Field<int>("idMoneda"),
                SaldoCuenta = (double)r.Field<decimal>("saldoCuenta"),
                TipoCuenta = r.Field<string>("tipoCuenta"),
                ValorCuenta = (double)r.Field<decimal>("valorCuenta"),
                CodigoEjecutivo = (int)r.Field<int>("codigoEjecutivo"),
                NombreEjecutivo = r.Field<string>("nombreEjecutivo"),
                RutCliente = long.Parse(r.Field<string>("rutCliente")),
                DigitoCliente = r.Field<string>("digitoCliente"),
                NombreCliente = r.Field<string>("nombreCliente")
            }).ToList();

            return await Task.FromResult(Data);
        }

    }
}
