using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UTF_system.Helpers;
using UTF_system.Models;

namespace UTF_system.Controllers
{
    public class ProfesoresController : Controller
    {
        // GET: Profesores
        public ActionResult Index()
        {
            ViewBag.success = TempData["success"];
            ViewBag.message = TempData["message"];

            return View();
        }

        public ActionResult Logout()
        {
            Session["id"] = null;
            Session["tipo"] = null;

            return RedirectToAction("Login", "Home");
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
                return RedirectToAction("RegistrarCalificacion", "Profesores");
            }

            if (!double.TryParse(calificacion, out nota))
            {
                TempData["message"] = "Nota no válida";
                return RedirectToAction("RegistrarCalificacion", "Profesores");
            }

            if (nota > 100 || nota < 1)
            {
                TempData["message"] = "Calificacion debe estar entre 1 y 100";
                return RedirectToAction("RegistrarCalificacion", "Profesores");
            }

            //Recuperar la de la base de datos
            Asignatura asignatura = DB.GetAsignatura(clave);

            if (asignatura == null)
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarCalificacion", "Profesores");
            }

            if (string.IsNullOrEmpty(asignatura.Clave))
            {
                TempData["message"] = "Asignatura no encontrada";
                return RedirectToAction("RegistrarCalificacion", "Profesores");
            }

            //Recuperar el estudiante
            User user = Models.User.SelectUserById(id);


            if (user == null)
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarCalificacion", "Profesores");
            }

            //El usuario no existe
            if (user.ID == 0)
            {
                TempData["message"] = "ID estudiante no encontrado";
                return RedirectToAction("RegistrarCalificacion", "Profesores");
            }

            //El usuario no es un estudiante
            if (user.Type != Models.User.Tipo.estudiante)
            {
                TempData["message"] = "ID estudiante no encontrado";
                return RedirectToAction("RegistrarCalificacion", "Profesores");
            }


            Calificacion Calificacion = DB.SelectCalificacion(user, asignatura);

            if (Calificacion == null)
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarCalificacion", "Profesores");
            }

            //El estudiante ya tiene una nota de esa asignatura
            if (Calificacion.Estudiante != null)
            {
                TempData["message"] = "Estudiante ya cuenta con dicha nota";
                return RedirectToAction("RegistrarCalificacion", "Profesores");
            }

            Calificacion = new Calificacion(user, asignatura, nota);

            //Anadir el  objeto calificacion a la base de datos
            if (!DB.AddCalificacion(Calificacion))
            {
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("RegistrarCalificacion", "Profesores");
            }

            TempData["message"] = "Calificacion registrada con exito";
            TempData["success"] = "true";

            return RedirectToAction("Index", "Profesores");
        }

        public ActionResult Estudiantes()
        {
            Estudiante[] estudiantes = DB.GetEstudiantes();
            if(estudiantes == null)
            {
                TempData["success"] = "false";
                TempData["message"] = "Ha ocurrido un problema";
                return RedirectToAction("Index", "Profesores");
            }

            ViewBag.estudiantes = estudiantes;
            return View();
        }

        public ActionResult Ranking()
        {
            Estudiante[] estudiantes = DB.GetEstudiantes();

            if(estudiantes == null)
            {
                TempData["success"] = "false";
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("Index", "Profesores");

            }

            for(int i = 0; i < estudiantes.Length; i++)
            {
                Calificacion[] calificaciones = DB.GetCalificacionesStudent(estudiantes[i].ID);
                if(calificaciones == null)
                {
                    TempData["success"] = "false";
                    TempData["message"] = "Ha ocurrido un error";
                    return RedirectToAction("Index", "Profesores");
                }

                estudiantes[i].Indice = Estudiante.CalcularIndice(calificaciones);
            }

            Array.Sort(estudiantes);
            Array.Reverse(estudiantes);
            ViewBag.home = "/Profesores/";
            ViewBag.estudiantes = estudiantes;
            return View();
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