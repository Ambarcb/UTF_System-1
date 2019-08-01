using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using System.Web.Security;
using UTF_system.Helpers;
using UTF_system.Models;

namespace UTF_system.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            //Si no hay nadie logueado
            if (Session["id"] == null)
                return RedirectToAction("Login", "Home");

            ViewBag.message = TempData["message"];
            ViewBag.success = TempData["success"];
            return View();
        }

        public ActionResult Logout()
        {
            Session["id"] = null;
            Session["tipo"] = null;

            return RedirectToAction("Login", "Home");
        }


        public ActionResult Ranking()
        {
            Estudiante[] estudiantes = DB.GetEstudiantes();

            if (estudiantes == null)
            {
                TempData["success"] = "false";
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("Index", "Profesores");

            }

            for (int i = 0; i < estudiantes.Length; i++)
            {
                Calificacion[] calificaciones = DB.GetCalificacionesStudent(estudiantes[i].ID);
                if (calificaciones == null)
                {
                    TempData["success"] = "false";
                    TempData["message"] = "Ha ocurrido un error";
                    return RedirectToAction("Index", "Profesores");
                }

                estudiantes[i].Indice = Estudiante.CalcularIndice(calificaciones);
            }

            Array.Sort(estudiantes);
            Array.Reverse(estudiantes);
            ViewBag.estudiantes = estudiantes;
            ViewBag.home = "/Admin/";
            return View();
        }

        public ActionResult RegistrarEstudiante()
        {
            string[] carreras = DB.GetCarreras();

            ViewBag.carreras = carreras;
            ViewBag.message = TempData["message"];
            return View();
        }

        [HttpPost]
        public ActionResult RegistrarEstudiante(string ID, string nombre, string apellido, string carrera, string email)
        {
            /*
            int id;

            //Not a valid ID
            if (!int.TryParse(ID, out id) || ID.Length != 7)
            {
                TempData["message"] = "ID inválido";
                return RedirectToAction("RegistrarEstudiante");
            }

            User estudiante = Models.User.SelectUserById(id);
            
            //Error en DB
            if (estudiante == null)
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarEstudiante");
            }
            //ID ya existe
            else if (estudiante.ID != 0)
            {
                TempData["message"] = "Estudiante con dicho ID ya existe";
                return RedirectToAction("RegistrarEstudiante");
            }
            */
            User estudiante = new Estudiante(0,nombre, apellido, Helpers.Util.GeneratePassword(), Models.User.Tipo.estudiante, carrera, email);

            int id = DB.AddUser(estudiante);
            if ( id == 0)
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarEstudiante");
            }

            estudiante.ID = id;
            if (!DB.AddStudent((Estudiante)estudiante))
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarEstudiante");
            }

            try
            {
                //Configuring webMail class to send emails  
                //gmail smtp server  
                WebMail.SmtpServer = "smtp.gmail.com";
                //gmail port to send emails  
                WebMail.SmtpPort = 587;
                WebMail.SmtpUseDefaultCredentials = true;
                //sending emails with secure protocol  
                WebMail.EnableSsl = true;
                //EmailId used to send emails from application  
                WebMail.UserName = "utecnicasfundamentales@gmail.com";
                WebMail.Password = "FinalWork3";

                //Sender email address.  
                WebMail.From = "utecnicasfundamentales@gmail.com";

                string body = String.Format("Bienvenido a la familia de Tecnicas fundamentales. Aqui estan tus credenciales:\n id: {0}\nContraseña: {1}", estudiante.ID, estudiante.Password);
                //Send email  
                WebMail.Send(to: email, subject: "Bienvenido!", body: body, cc: estudiante.Email, bcc: estudiante.Email, isBodyHtml: true);

            }
            catch (Exception)
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarProfesor");

            }

            TempData["message"] = "Estudiante registrado con exito";
            TempData["success"] = "true";

            return RedirectToAction("Index");
        }



        public ActionResult RegistrarAsignatura()
        {
            ViewBag.message = TempData["message"];
            return View();
        }
        [HttpPost]
        public ActionResult RegistrarAsignatura(string clave, string Creditos, string nombre)
        {
            int creditos;

            if (!int.TryParse(Creditos, out creditos))
            {
                TempData["message"] = "Creditos no válidos";
                return RedirectToAction("RegistrarAsignatura");
            }

            if (creditos > 5 || creditos < 1)
            {
                TempData["message"] = "Creditos deben ser entre 1 y 5";
                return RedirectToAction("RegistrarAsignatura");
            }

            Asignatura asignatura = DB.GetAsignatura(clave);

            //Problema abriendo la base de datos
            if (asignatura == null)
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarAsignatura");
            }
            //Asignatura ya existe
            else if (asignatura.Clave != "")
            {
                TempData["message"] = "Asignatura ya ha sido registrada";
                return RedirectToAction("RegistrarAsignatura");
            }

            asignatura = new Asignatura(clave, nombre, creditos);

            //Si hubo un problema anadiendo la asignatura
            if (!DB.AddAsignatura(asignatura))
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarAsignatura");
            }

            TempData["success"] = "true";
            TempData["message"] = "Asignatura registrada con exito";
            return RedirectToAction("Index", "Admin");

        }

        public ActionResult RegistrarProfesor()
        {
            ViewBag.message = TempData["message"];
            return View();
        }

        [HttpPost]
        public ActionResult RegistrarProfesor(string nombre, string apellido, string email)
        {
            
            /*
            User user = Models.User.SelectUserById(id);

            //No se pudo verificar si el usuario ya existe
            if (user == null)
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarProfesor");
            }
            //Usuario ya existe
            else if (user.ID != 0)
            {
                TempData["message"] = "Usuario con dicho ID ya existe";
                return RedirectToAction("RegistrarProfesor");
            }

    */
            User user = new User(1, nombre, apellido, Helpers.Util.GeneratePassword() , Models.User.Tipo.profesor, email);

            int id = DB.AddUser(user);
            //Error insertando usuario
            if (id == 0)
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarProfesor");
            }
            user.ID = id;
            try
            {
                //Configuring webMail class to send emails  
                //gmail smtp server  
                WebMail.SmtpServer = "smtp.gmail.com";
                //gmail port to send emails  
                WebMail.SmtpPort = 587;
                WebMail.SmtpUseDefaultCredentials = true;
                //sending emails with secure protocol  
                WebMail.EnableSsl = true;
                //EmailId used to send emails from application  
                WebMail.UserName = "utecnicasfundamentales@gmail.com";
                WebMail.Password = "FinalWork3";

                //Sender email address.  
                WebMail.From = "utecnicasfundamentales@gmail.com";

                string body = String.Format("Bienvenido a la familia de Tecnicas fundamentales. Aqui estan tus credenciales:\n id: {0}\nContraseña: {1}", user.ID, user.Password);
                //Send email  
                WebMail.Send(to: email, subject: "Bienvenido!", body: body, cc: user.Email, bcc: user.Email, isBodyHtml: true);
                
            }
            catch (Exception)
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarProfesor");

            }

            TempData["message"] = "Profesor registrado con exito";
            TempData["success"] = "true";
            return RedirectToAction("Index");

        }

        public ActionResult RegistrarCalificacion()
        {
            ViewBag.message = TempData["message"];
            return View();
        }

        [HttpPost]
        public ActionResult RegistrarCalificacion(string clave, string idEstudiante, string calificacion)
        {
            int id;
            double nota;

            if (!int.TryParse(idEstudiante, out id))
            {
                TempData["message"] = "ID no válido";
                return RedirectToAction("RegistrarCalificacion", "Admin");
            }

            if (!double.TryParse(calificacion, out nota))
            {
                TempData["message"] = "Nota no válida";
                return RedirectToAction("RegistrarCalificacion", "Admin");
            }

            if (nota > 100 || nota < 1)
            {
                TempData["message"] = "Calificacion debe estar entre 1 y 100";
                return RedirectToAction("RegistrarCalificacion", "Admin");
            }

            //Recuperar la de la base de datos
            Asignatura asignatura = DB.GetAsignatura(clave);

            if (asignatura == null)
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarCalificacion", "Admin");
            }

            if (string.IsNullOrEmpty(asignatura.Clave))
            {
                TempData["message"] = "Asignatura no encontrada";
                return RedirectToAction("RegistrarCalificacion", "Admin");
            }

            //Recuperar el estudiante
            User user = Models.User.SelectUserById(id);


            if (user == null)
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarCalificacion", "Admin");
            }

            //El usuario no existe
            if (user.ID == 0)
            {
                TempData["message"] = "ID estudiante no encontrado";
                return RedirectToAction("RegistrarCalificacion", "Admin");
            }

            //El usuario no es un estudiante
            if (user.Type != Models.User.Tipo.estudiante)
            {
                TempData["message"] = "ID estudiante no encontrado";
                return RedirectToAction("RegistrarCalificacion", "Admin");
            }


            Calificacion Calificacion = DB.SelectCalificacion(user, asignatura);

            if (Calificacion == null)
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarCalificacion", "Admin");
            }

            //El estudiante ya tiene una nota de esa asignatura
            if (Calificacion.Estudiante != null)
            {
                TempData["message"] = "Estudiante ya cuenta con dicha nota";
                return RedirectToAction("RegistrarCalificacion", "Admin");
            }

            Calificacion = new Calificacion(user, asignatura, nota);

            //Anadir el  objeto calificacion a la base de datos
            if (!DB.AddCalificacion(Calificacion))
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarCalificacion", "Admin");
            }

            TempData["message"] = "Calificacion registrada con exito";
            TempData["success"] = "true";

            return RedirectToAction("Index", "Admin");
        }

        public ActionResult Estudiantes()
        {
            Estudiante[] estudiantes = DB.GetEstudiantes();

            if (estudiantes == null)
            {
                TempData["success"] = "false";
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.estudiantes = estudiantes;

            return View();


        }

        public ActionResult Asignaturas()
        {
            Asignatura[] asignaturas = DB.GetAsignaturas();

            //TODO
            if (asignaturas == null)
            {
                TempData["message"] = "Ha ocurrido un error";
                TempData["success"] = "false";
                return RedirectToAction("Index","Admin");
            }
            
            ViewBag.asignaturas = asignaturas;
            return View();
        }

        public ActionResult Calificaciones()
        {
            Calificacion[] calificaciones = DB.GetCalificacions();

            if (calificaciones == null)
            {
                TempData["message"] = "Ha ocurrido un error";
                TempData["success"] = "false";
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.calificaciones = calificaciones;
            return View();
        }

        public ActionResult Profesores()
        {
            User[] profesores = DB.GetProfesores();

            if (profesores == null)
            {
                TempData["success"] = "false";
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.profesores = profesores;
            return View();
        }

        public ActionResult EliminarEstudiante(string ID)
        {
            int id = int.Parse(ID);

            //Asegurarse de que el estudiante exista
            User user = Models.User.SelectUserById(id);

            if (user == null)
            {
                TempData["success"] = "false";
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("Index", "Admin");
            }
            else if (user.ID == 0)
            {
                TempData["success"] = "false";
                TempData["message"] = "ID no válido";
                return RedirectToAction("Index", "Admin");
            }
            else if (user.Type != Models.User.Tipo.estudiante)
            {
                TempData["success"] = "false";
                TempData["message"] = "ID no válido";
                return RedirectToAction("Index", "Admin");
            }



            if (!DB.DeleteStudent(id) || !DB.DeleteUser(id) || !DB.DeleteCalificacionStudent(id))
            {
                TempData["success"] = "false";
                TempData["message"] = "No se ha podido eliminar el estudiante";
                return RedirectToAction("Index", "Admin");
            }

            TempData["success"] = "true";
            TempData["message"] = "Estudiante eliminado";
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult ModificarEstudiante(int id)
        {
            string[] carreras = DB.GetCarreras();
            ViewBag.carreras = carreras;

            User user = Models.User.SelectUserById(id);
            if (user == null)
            {
                TempData["success"] = "false";
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("Index", "Admin");
            }
            else if (user.ID == 0)
            {
                TempData["success"] = "false";
                TempData["message"] = "Estudiante no encontrado";
                return RedirectToAction("Index", "Admin");
            }
            else if (user.Type != Models.User.Tipo.estudiante)
            {
                TempData["success"] = "false";
                TempData["message"] = "ID no válido";
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.estudiante = user;
            ViewBag.message = TempData["message"];
            return View();
        }

        [HttpPost]
        public ActionResult ModificarEstudiante(int id, string nombre, string apellido, string carrera)
        {
            if (!DB.UpdateStudent(id, nombre, apellido, carrera))
            {
                TempData["message"] = "No se pudo modificar el usuario";
                ViewBag.success = "false";
                return RedirectToAction("ModificarEstudiante", "Admin", new { id = id });
            }

            TempData["success"] = "true";
            TempData["message"] = "Estudiante modificado con exito";
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult EliminarAsignatura(string clave)
        {
            if(!DB.DeleteAsignatura(clave))
            {
                TempData["success"] = "false";
                TempData["message"] = "Ha ocurrido un error";
            }

            TempData["success"] = "true";
            TempData["message"] = "Asignatura eliminada con exito";
            return RedirectToAction("Index", "Admin");
        }


        public ActionResult ModificarAsignatura(string clave)
        {
            Asignatura asignatura = DB.GetAsignatura(clave);
            if(asignatura == null)
            {
                TempData["success"] = "false";
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("Index", "Admin");
            }
            else if(asignatura.Clave == "")
            {
                TempData["success"] = "false";
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("Index", "Admin");

            }

            ViewBag.asignatura = asignatura;
            return View();
        }

        [HttpPost]
        public ActionResult ModificarAsignatura(string clave, string nombre, string Creditos)
        {
            int creditos;

            if(!int.TryParse(Creditos, out creditos))
            {
                TempData["message"] = "Créditos inválidos";
                TempData["success"] = "false";
                return View();
            }

            if(creditos > 5 || creditos < 1)
            {
                TempData["message"] = "Créditos deben estar entre 1 y 5";
                TempData["success"] = "false";
                return View();
            }

            if(!DB.ModificarAsignatura(clave,nombre,creditos))
            {
                TempData["message"] = "Ha ocurrido un error";
                TempData["success"] = "false";
                return View();
            }

            TempData["message"] = "Asignatura modificada con exito";
            TempData["success"] = "true";
            return RedirectToAction("Index", "Admin");
        }


        public ActionResult EliminarCalificacion(int idEstudiante, string clave)
        {
            if(!DB.EliminarCalificacion(clave,idEstudiante))
            {
                TempData["success"] = "false";
                TempData["message"] = "Ha ocurrido un error";
            }

            TempData["success"] = "true";
            TempData["message"] = "Calificación eliminada con éxito";
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult ModificarCalificacion(int idEstudiante, string clave)
        {
            ViewBag.idEstudiante = idEstudiante;
            ViewBag.clave = clave;
            ViewBag.message = TempData["message"];
            return View();
        }
        
        [HttpPost]
        public ActionResult ModificarCalificacion(string calificacion, string clave, int idEstudiante)
        {
            double nota;

            if(!double.TryParse(calificacion, out nota))
            {
                TempData["sucess"] = "false";
                TempData["message"] = "Nota inválida";
                ViewBag.idEstudiante = idEstudiante;
                ViewBag.clave = clave;

                return RedirectToAction("ModificarCalificacion", "Admin", new { idEstudiante = idEstudiante, clave = clave });
            }
            else if(nota > 100 || nota < 0)
            {
                TempData["sucess"] = "false";
                TempData["message"] = "Nota debe ser entre 0 y 100";
                ViewBag.idEstudiante = idEstudiante;
                ViewBag.clave = clave;

                return RedirectToAction("ModificarCalificacion", "Admin", new { idEstudiante = idEstudiante, clave = clave });
            }

            string letra = Util.GetLetra(nota);

            if(!DB.ModificarCalificacion(idEstudiante, clave,(int)Math.Ceiling(nota),letra))
            {
                TempData["sucess"] = "false";
                TempData["message"] = "Ha ocurrido un error";
                ViewBag.idEstudiante = idEstudiante;
                ViewBag.clave = clave;

                return RedirectToAction("ModificarCalificacion", "Admin", new { idEstudiante = idEstudiante, clave = clave });
            }

            TempData["success"] = "true";
            TempData["message"] = "Calificación modificada con éxito";
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult EliminarProfesor(int id)
        {
            if(!DB.DeleteUser(id))
            {
                TempData["success"] = "false";
                TempData["message"] = "Ha ocurrido un error";

                return RedirectToAction("Index", "Admin");
                
            }

            TempData["success"] = "true";
            TempData["message" +
                ""] = "Profesor eliminado con éxito";
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult ModificarProfesor(int id)
        {
            
            User profesor = Models.User.SelectUserById(id);
            if(profesor == null)
            {
                TempData["message"] = "Ha ocurrido un error";
                TempData["success"] = "false";
                return RedirectToAction("Index", "Admin");
            }
            else if(profesor.ID == 0)
            {
                TempData["message"] = "Ha ocurrido un error";
                TempData["success"] = "false";
                return RedirectToAction("Index", "Admin");
            }


            ViewBag.profesor = profesor;
            return View();
        }

        [HttpPost]
        public ActionResult ModificarProfesor(int idProfesor, string nombre, string apellido)
        {
            if(!DB.UpdateUser(idProfesor,nombre,apellido))
            {
                TempData["success"] = "false";
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("Index", "Admin");

            }

            TempData["success"] = "true";
            TempData["message"] = "Profesor modificado con éxito";
            return RedirectToAction("Index","Admin");
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["id"] != null)
                base.OnActionExecuting(filterContext);
            else
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Home" }));
        }
        


    }
}