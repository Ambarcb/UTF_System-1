using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UTF_system.Models
{
    public class Estudiante : User, IComparable<Estudiante>
    {
        public string Carrera { get; set; }
        public double Indice { get; set; }

        public Estudiante(int id, string nombre, string apellido, string password, Tipo tipo, string carrera, string email) : base(id,nombre,apellido, password,tipo,email)
        {

            this.Carrera = carrera;
            this.Indice = 0.0;
        }

        public static double CalcularIndice(Calificacion[] calificaciones)
        {
            double creditosAcumulados = 0;
            double puntosHonor = 0;

            foreach(Calificacion calificacion in calificaciones)
            {
                creditosAcumulados += calificacion.Asignatura.Creditos;
                puntosHonor += calificacion.Asignatura.Creditos * Calificacion.GetPuntosHonor(calificacion.Letra);
            }

            if (creditosAcumulados == 0)
                return 0;
            
            return  puntosHonor*1.0 / creditosAcumulados*1.0;
        }

        public int CompareTo(Estudiante other)
        {
            return this.Indice.CompareTo(other.Indice);
        }
    }
}