using System;
using System.Collections.Generic;

namespace WebApiRepoteria.AL.Entidades
{
    /// <summary>
    /// Contiene la información de un reporte con una cabecera y un detalle asociado.
    /// </summary>
    /// <typeparam name="T">El tipo de los elementos en el detalle del reporte.</typeparam>
    public class RepositorioEN
    {
        /// <summary>
        /// Representa la cabecera y los detalles de un reporte.
        /// </summary>
        /// <typeparam name="T">El tipo de los elementos en el detalle del reporte.</typeparam>

        public class ReporteConDetalleSimple<T>
        {

            public long Rut { get; set; }
            public string Digito { get; set; }
            public string Nombre { get; set; }
            public DateTime Fecha { get; set; }
            public List<T> Detalle { get; set; } = new List<T>();
        }

        public class ReporteConDobleDetalle<T,TY>
        {

            public long Rut { get; set; }
            public string Digito { get; set; }
            public string Nombre { get; set; }
            public DateTime Fecha { get; set; }
            public List<T> DetalleA { get; set; } = new List<T>();
            public List<TY> DetalleB { get; set; } = new List<TY>();
        }
    }
}
