using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UTF_system.Models
{
    public class Profesor : User
    {  
            public Profesor(int id, string nombre, string apellido, string password, Tipo tipo, string email ) : base(id, nombre, apellido, password, tipo, email)
        {
            
        }
           
        }
    }
