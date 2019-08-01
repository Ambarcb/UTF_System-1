using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace UTF_system.Models
{
    public class User
    {
        public enum Tipo{admin = 1, profesor = 2, estudiante = 3}
            
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Password { get; set; }
        public Tipo Type { get; set; }
        public string Email { get; set; }

        public User(int id, string nombre, string apellido, string password, Tipo tipo, string email)
        {
            this.ID = id;
            this.Nombre = nombre;
            this.Apellido = apellido;
            this.Password = password;
            this.Type = tipo;
            this.Email = email;
        }

        public static User SelectUserById(int id)
        {
            //Si es un error abriendo la base de datos, devuelve null
            //Si no se encontro el usuario, devuelve 0
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch(Exception ex)
            {
                return null;
            }
                
            string query = String.Format("SELECT * FROM users_table WHERE id = {0}", id);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            SqlDataReader dr;

            try
            {
                dr = sqlCommand.ExecuteReader();
            }
            catch(Exception ex)
            {
                return null;
            }

            
            User user = null;

            if (dr.Read()) 
            {
                
                string nombre = dr["nombre"].ToString();
                string apellido = dr["apellido"].ToString();
                string password = dr["password"].ToString();
                string email = dr["email"].ToString();
                Tipo tipo = (Tipo)int.Parse(dr["tipo"].ToString());
                user = new User(id, nombre, apellido, password, tipo, email);
                sqlConnection.Close();
            }
            else
                return new User(0,"","","",0,"");

            return user;

        }
    }
}