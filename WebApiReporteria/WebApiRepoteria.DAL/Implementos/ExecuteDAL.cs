using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WebApiRepoteria.AL.Entidades;
using WebApiRepoteria.AL.Interfaz;
using WebApiRepoteria.AL.Seguridad;
using WebApiRepoteria.DAL.Interfaz;

namespace WebApiRepoteria.DAL.Implementos
{
    public class ExecuteDAL : IExecuteDAL
    {
        private readonly IEncriptaEN ObjEncripta = new EncriptaEN();
        private string CadenaConexion = "";

        public ExecuteDAL()
        {
            //CadenaConexion = ObjEncripta.Desencriptar(ConfigurationManager.ConnectionStrings["FactorConnection"].ToString());
            CadenaConexion = ConfigurationManager.ConnectionStrings["FactorConnection"].ToString();
        }

        public async Task<bool> ExecuteScriptAsync(string nombreProcedimiento)
        {
            try
            {
                using (var connection = new SqlConnection(CadenaConexion))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(nombreProcedimiento, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        // Ejecuta el comando
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        // Considera la operación exitosa si se afectaron filas
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Añade contexto a la excepción para facilitar la depuración
                throw new Exception("Error al ejecutar el procedimiento almacenado.", ex);
            }
        }

        public async Task<bool> ExecuteScriptAsync(string nombreProcedimiento, List<ModeloDataEN> modeloLista)
        {
            try
            {
                using (var connection = new SqlConnection(CadenaConexion))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(nombreProcedimiento, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 0;

                        // Añade parámetros al comando
                        foreach (var item in modeloLista)
                        {
                            command.Parameters.AddWithValue(item.NombreColumna, item.ValorString);
                        }

                        // Ejecuta el comando
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        // Considera la operación exitosa si se afectaron filas
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Añade contexto a la excepción para facilitar la depuración
                throw new Exception("Error al ejecutar el procedimiento almacenado.", ex);
            }
        }

        public async Task<DataTable> ExecuteProcedureAsync(string nombreProcedimiento)
        {
            DataTable resultTable = new DataTable();

            try
            {
                using (var connection = new SqlConnection(CadenaConexion))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(nombreProcedimiento, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 0;
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            // Llena el DataTable con los datos del procedimiento almacenado
                            adapter.Fill(resultTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar la excepción o agregar información adicional antes de lanzarla
                throw new Exception("Error al ejecutar el procedimiento almacenado.", ex);
            }

            return resultTable;
        }

        public async Task<DataTable> ExecuteProcedureAsync(string nombreProcedimiento, List<ModeloDataEN> modeloLista)
        {
            DataTable resultTable = new DataTable();

            try
            {
                using (var connection = new SqlConnection(CadenaConexion))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(nombreProcedimiento, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 0;

                        // Añade parámetros al comando
                        foreach (var item in modeloLista)
                        {
                            command.Parameters.AddWithValue(item.NombreColumna, item.ValorString);
                        }

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(resultTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Proporciona un mensaje descriptivo para ayudar en la depuración
                throw new Exception("Error al ejecutar el procedimiento almacenado.", ex);
            }

            return resultTable;
        }
    }
}
