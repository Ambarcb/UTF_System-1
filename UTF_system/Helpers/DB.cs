using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using UTF_system.Models;

namespace UTF_system.Helpers
{
    public class DB
    {
        public static string[] GetCarreras()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return null;
            }

            string query = "SELECT * FROM carreras_table";

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            SqlDataReader dr;

            try
            {
                dr = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return null;
            }

            List<string> carreras = new List<string>();

            while (dr.Read())
            {
                carreras.Add((string)dr["clave"]);

            }

            sqlConnection.Close();
            return carreras.ToArray();
        }

        public static int AddUser(User user)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return 0;
            }

            string query = String.Format("INSERT INTO users_table VALUES('{0}','{1}','{2}',{3},'{4}'); SELECT SCOPE_IDENTITY();", user.Nombre, user.Apellido, user.Password, (int)user.Type, user.Email);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            int id = 0;
            try
            {
                id = Convert.ToInt32(sqlCommand.ExecuteScalar());
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return 0;
            }

            
            sqlConnection.Close();
            return id;

        }

        public static bool AddStudent(Estudiante estudiante)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            string query = String.Format("INSERT INTO estudiantes_table VALUES({0},'{1}')", estudiante.ID, estudiante.Carrera);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return false;
            }

            sqlConnection.Close();
            return true;
        }

        public static Asignatura GetAsignatura(string clave)
        {
            //Si es un error abriendo la base de datos, devuelve null
            //Si no se encontro el usuario, devuelve 0
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return null;
            }

            string query = String.Format("SELECT * FROM asignaturas_table WHERE clave = '{0}'", clave);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            SqlDataReader dr;

            try
            {
                dr = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {

                return null;
            }


            Asignatura asignatura = null;

            if (dr.Read())
            {

                //dr["clave"].ToString();
                string nombre = dr["nombre"].ToString();
                int creditos = int.Parse(dr["creditos"].ToString());
                sqlConnection.Close();
                asignatura = new Asignatura(clave, nombre, creditos);
            }
            else
            {
                sqlConnection.Close();
                return new Asignatura("", "", 0);
            }

                
            sqlConnection.Close();
            return asignatura;

        }

        public static bool AddAsignatura(Asignatura asignatura)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            

            string query = String.Format("INSERT INTO asignaturas_table VALUES('{0}','{1}', {2})", asignatura.Clave, asignatura.Nombre, asignatura.Creditos);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return false;
            }

            sqlConnection.Close();
            return true;
        }

        public static bool AddCalificacion(Calificacion calificacion)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            string query = String.Format("INSERT INTO calificaciones_table VALUES({0},'{1}', '{2}', {3})", calificacion.Nota, calificacion.Letra, calificacion.Asignatura.Clave, calificacion.Estudiante.ID);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return false;
            }

            sqlConnection.Close();
            return true;
        }

        public static Calificacion SelectCalificacion(User estudiante, Asignatura asignatura)
        {
            //Si es un error abriendo la base de datos, devuelve null
            //Si no se encontro el usuario, devuelve 0
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return null;
            }

            string query = String.Format("SELECT * FROM calificaciones_table WHERE id_estudiante = {0} AND clave_asignatura = '{1}'", estudiante.ID, asignatura.Clave);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            SqlDataReader dr;

            try
            {
                dr = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                return null;
            }


            Calificacion calificacion = null;

            if (dr.Read())
            {

                //dr["clave"].ToString();

                double nota = double.Parse(dr["nota"].ToString());
                string letra = dr["letra"].ToString();
                string clave_asignatura = dr["clave_asignatura"].ToString();
                int id_estudiante = int.Parse(dr["id_estudiante"].ToString());

                sqlConnection.Close();
                calificacion = new Calificacion(estudiante, asignatura, nota);
            }
            else
            {
                sqlConnection.Close();
                return new Calificacion(null, null, 0);
            }
            sqlConnection.Close();
            return calificacion;
        }

        public static Asignatura[] GetAsignaturas()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return null;
            }

            string query = "SELECT * FROM asignaturas_table";

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            SqlDataReader dr;

            try
            {
                dr = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return null;
            }

            List<Asignatura> asignaturas = new List<Asignatura>();

            while (dr.Read())
            {
                string clave = (string)dr["clave"];
                string nombre = dr["nombre"].ToString();
                int creditos = int.Parse(dr["creditos"].ToString());
                asignaturas.Add(new Asignatura(clave, nombre, creditos));
            }

            sqlConnection.Close();
            return asignaturas.ToArray();
        }

        public static Estudiante[] GetEstudiantes()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return null;
            }

            string query = "SELECT * FROM estudiantes_table";

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            SqlDataReader dr;

            try
            {
                dr = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return null;
            }

            List<Estudiante> estudiantes = new List<Estudiante>();

            while (dr.Read())
            {
                string clave = (string)dr["clave_carrera"];
                int id = int.Parse(dr["id"].ToString());
                User user = User.SelectUserById(id);
                Estudiante estudiante = new Estudiante(user.ID, user.Nombre, user.Apellido, user.Password, user.Type, clave, user.Email);
                if (user == null)
                {
                    sqlConnection.Close();
                    return null;
                }
                    

                estudiantes.Add(estudiante);
            }

            sqlConnection.Close();
            return estudiantes.ToArray();
        }

        public static Calificacion[] GetCalificacions()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return null;
            }

            string query = "SELECT * FROM calificaciones_table";

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            SqlDataReader dr;

            try
            {
                dr = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return null;
            }

            List<Calificacion> calificaciones = new List<Calificacion>();

            while (dr.Read())
            {
                double nota = double.Parse(dr["nota"].ToString());
                string clave = dr["clave_asignatura"].ToString();
                int idEstudiante = int.Parse(dr["id_estudiante"].ToString());

                User user = User.SelectUserById(idEstudiante);

                if (user == null)
                {
                    
                    sqlConnection.Close();
                    return null;
                }
                    
                Asignatura asignatura = DB.GetAsignatura(clave);
                if (asignatura == null)
                {
                    sqlConnection.Close();
                    return null;

                }
                    
                calificaciones.Add(new Calificacion(user, asignatura, nota));


            }

            sqlConnection.Close();
            return calificaciones.ToArray();
        }

        public static User[] GetProfesores()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return null;
            }

            string query = "SELECT * FROM users_table WHERE tipo = " + (int)Models.User.Tipo.profesor;

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            SqlDataReader dr;

            try
            {
                dr = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return null;
            }

            List<User> profesores = new List<User>();

            while (dr.Read())
            {

                string nombre = dr["nombre"].ToString();
                string apellido = (string)dr["apellido"];
                int id = int.Parse(dr["id"].ToString());
                string email = dr["email"].ToString();
                profesores.Add(new User(id, nombre, apellido, "", Models.User.Tipo.profesor,email));
            }

            sqlConnection.Close();
            return profesores.ToArray();
        }

        public static bool DeleteStudent(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            string query = String.Format("DELETE FROM estudiantes_table WHERE id = {0}", id);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return false;
            }

            sqlConnection.Close();
            return true;
        }

        public static bool DeleteUser(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            string query = String.Format("DELETE FROM users_table WHERE id = {0}", id);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return false;
            }

            sqlConnection.Close();
            return true;
        }

        public static bool DeleteCalificacionStudent(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            string query = String.Format("DELETE FROM calificaciones_table WHERE id_estudiante = {0}", id);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return false;
            }

            sqlConnection.Close();
            return true;
        }

        public static bool UpdateUser(int id, string nombre, string apellido)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            string query = String.Format("UPDATE users_table SET nombre = '{0}', apellido = '{1}' WHERE id = {2}", nombre, apellido, id);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return false;
            }

            sqlConnection.Close();
            return true;
        }

        public static bool UpdateStudent(int id, string nombre, string apellido, string carrera)
        {
            if (!UpdateUser(id, nombre, apellido))
                return false;

            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            string query = String.Format("UPDATE estudiantes_table SET clave_carrera = '{0}' WHERE id = {1}", carrera, id);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return false;
            }

            sqlConnection.Close();
            return true;
        }

        public static bool DeleteCalificacionAsignatura(string clave)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            string query = String.Format("DELETE FROM calificaciones_table WHERE clave_asignatura = '{0}'", clave);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return false;
            }

            sqlConnection.Close();
            return true;
        }

        public static bool DeleteAsignatura(string clave)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            string query = String.Format("DELETE FROM asignaturas_table WHERE clave = '{0}'", clave);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return false;
            }

            if (!DB.DeleteCalificacionAsignatura(clave))
                return false;
            sqlConnection.Close();
            return true;
        }

        public static bool ModificarAsignatura(string clave, string nombre, int creditos)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            string query = String.Format("UPDATE asignaturas_table SET nombre = '{0}', creditos = {1} WHERE clave = '{2}'", nombre, creditos, clave);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return false;
            }

            sqlConnection.Close();
            return true;
        }

        public static bool EliminarCalificacion(string clave, int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            string query = String.Format("DELETE FROM calificaciones_table WHERE id_estudiante = {0} AND clave_asignatura = {1}", id, clave);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return false;
            }

            sqlConnection.Close();
            return true;
        }

        public static bool ModificarCalificacion(int id, string clave, int nota, string letra)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }

            string query = String.Format("UPDATE calificaciones_table SET nota = '{0}', letra = '{1}' WHERE clave_asignatura = '{2}' AND id_estudiante = {3}", (int)nota, letra, clave, id);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return false;
            }

            sqlConnection.Close();
            return true;
        }


        public static Calificacion[] GetCalificacionesStudent(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return null;
            }

            string query = "SELECT * FROM calificaciones_table WHERE id_estudiante = " + id;

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            SqlDataReader dr;

            try
            {
                dr = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                sqlConnection.Close();
                return null;
            }

            List<Calificacion> calificaciones = new List<Calificacion>();

            while (dr.Read())
            {
                double nota = double.Parse(dr["nota"].ToString());
                string clave = dr["clave_asignatura"].ToString();
                int idEstudiante = int.Parse(dr["id_estudiante"].ToString());

                User user = User.SelectUserById(idEstudiante);

                if (user == null)
                    return null;
                Asignatura asignatura = DB.GetAsignatura(clave);
                if (asignatura == null)
                    return null;
                calificaciones.Add(new Calificacion(user, asignatura, nota));


            }
            sqlConnection.Close();
            return calificaciones.ToArray();
        }

        public static Estudiante GetEstudiante(int id)
        {
            User user = User.SelectUserById(id);

            if (user == null)
                return null;
            
            //Si es un error abriendo la base de datos, devuelve null
            //Si no se encontro el usuario, devuelve 0
            string connectionString = ConfigurationManager.ConnectionStrings["db_connection"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                return null;
            }

            string query = String.Format("SELECT * FROM estudiantes_table WHERE id = '{0}'", id);

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            SqlDataReader dr;

            try
            {
                dr = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {

                return null;
            }


            Estudiante estudiante = null;

            if (dr.Read())
            {

               
                string carrera = dr["clave_carrera"].ToString();
                estudiante = new Estudiante(id, user.Nombre, user.Apellido, "", User.Tipo.estudiante, carrera, user.Email);
            }
            else
            {
                sqlConnection.Close();
                return new Estudiante(0,"","","",0,"","");
            }


            sqlConnection.Close();
            return estudiante;
        }

        
        

    }
}