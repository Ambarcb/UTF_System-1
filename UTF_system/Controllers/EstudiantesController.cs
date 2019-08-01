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
    public class EstudiantesController : Controller
    {
        // GET: Estudiantes
        public ActionResult Index()
        {
            int id = int.Parse(Session["id"].ToString());
            Estudiante estudiante = DB.GetEstudiante(id);

            if(estudiante == null)
            {
                TempData["success"] = true;
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("Logout", "Admin");
            }

            Calificacion[] calificaciones = DB.GetCalificacionesStudent(estudiante.ID);
            if (calificaciones == null)
            {
                TempData["success"] = false;
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("Logout", "Admin");
            }

            ViewBag.success = "true";
            ViewBag.message = "Bienvenid@, " + estudiante.Nombre + " " + estudiante.Apellido;
            estudiante.Indice = Estudiante.CalcularIndice(calificaciones);
            ViewBag.estudiante = estudiante;
            return View();
        }

        public ActionResult Logout()
        {
            Session["id"] = null;
            Session["tipo"] = null;

            return RedirectToAction("Login", "Home");
        }

        public ActionResult Asignaturas()
        {
            int id = int.Parse(Session["id"].ToString());
            Calificacion[] calificaciones = DB.GetCalificacionesStudent(id);

            if(calificaciones == null)
            {
                TempData["success"] = "false";
                TempData["message"] = "Ha ocurrido un error";
            }

            ViewBag.calificaciones = calificaciones;
            return View();

        }

        public ActionResult Calificaciones()
        {
            int id = int.Parse(Session["id"].ToString());
            Estudiante estudiante = DB.GetEstudiante(id);

            if (estudiante == null)
            {
                TempData["success"] = true;
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("Index", "Estudiantes");
            }

            Calificacion[] calificaciones = DB.GetCalificacionesStudent(estudiante.ID);
            if (calificaciones == null)
            {
                TempData["success"] = false;
                TempData["message"] = "Ha ocurrido un error";
                return RedirectToAction("Index", "Estudiantes");
            }

            int totalCreditos = 0;
            int totalPuntosHonor = 0;
            foreach (Calificacion c in calificaciones)
            {
                totalCreditos += c.Asignatura.Creditos;
                totalPuntosHonor += c.Asignatura.Creditos * c.ValorNota;
            }
                

            estudiante.Indice = Estudiante.CalcularIndice(calificaciones);
            ViewBag.totalPuntosHonor = totalPuntosHonor;
            ViewBag.totalCreditos = totalCreditos;
            ViewBag.calificaciones = calificaciones;
            ViewBag.estudiante = estudiante;
            return View();
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
            ViewBag.home = "/Estudiantes/z";
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