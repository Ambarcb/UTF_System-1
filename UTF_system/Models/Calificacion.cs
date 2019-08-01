using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UTF_system.Models
{
    public class Calificacion
    {
        public User Estudiante { get; set; }
        public Asignatura Asignatura { get; set; }
        public double Nota { get; set; }
        public string Letra { get; set; }
        public int ValorNota { get; set; }

        public Calificacion(User estudiante, Asignatura asignatura, double nota)
        {
            this.Estudiante = estudiante;
            this.Asignatura = asignatura;
            this.Nota = nota;
            

            if(nota >= 90)
            {
                this.Letra = "A";
                
            }
            else if(nota >= 80)
            {
                this.Letra = "B";
            }
            else if(nota >= 70)
            {
                this.Letra = "C";
            }
            else if(nota >= 60)
            {
                this.Letra = "D";
            }
            else
            {
                this.Letra = "F";
            }

            //En realidad GetPuntosHonor devuelve el valor de cada letra
            this.ValorNota = GetPuntosHonor(this.Letra);
        }

        public static int GetPuntosHonor(string letra)
        {
            int output = 0;
            switch(letra)
            {
                case "A":
                    output = 4;
                    break;
                case "B":
                    output = 3;
                    break;
                case "C":
                    output = 2;
                    break;
                case "D":
                    output = 1;
                    break;
                default:
                    output = 0;
                    break;
            }

            return output;
        }
    }
}