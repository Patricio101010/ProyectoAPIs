using System.Collections.Generic;

namespace WebApiRepoteria.AL.Entidades
{
    /// <summary>
    /// Contiene la información del resultado al consumir
    /// </summary>
    /// <typeparam name="T">El tipo de los elementos entidad.</typeparam>
    /// 
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public List<T> Data { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
        public string StackTrace { get; set; }
        public string ArchivoByteExcel { get; set; }
        public string ArchivoBytePDF { get; set; }
    }
}
