using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UTF_system.Models
{
    public class Asignatura
    {
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public int Creditos { get; set; }

        public Asignatura(string clave, string nombre, int creditos)
        {
            this.Clave = clave;
            this.Nombre = nombre;
            this.Creditos = creditos;
        }
    }

    
}