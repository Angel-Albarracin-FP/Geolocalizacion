using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mensajeria.Models
{
    public class Direccion
    {
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Ciudad { get; set; }
        public string Codigo_postal { get; set; }
        public string Provincia { get; set; }
        public string Pais { get; set; }
        public int Id { get; set; }
        public float Latitud { get; set; }
        public float Longitud { get; set; }
        public string Estado { get; set; }
    }
}
