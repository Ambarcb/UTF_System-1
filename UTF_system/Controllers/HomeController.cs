using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UTF_system.Models;

namespace UTF_system.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Si no hay nadie logueado
            if (Session["id"] == null)
                return RedirectToAction("Login");
            else if ((Models.User.Tipo)Session["tipo"] == Models.User.Tipo.admin)
            {
                AdminController controller = new AdminController();
                controller.ControllerContext = ControllerContext;
                return controller.Index();
            }
            else if ((Models.User.Tipo)Session["tipo"] == Models.User.Tipo.profesor)
            {
                ProfesoresController controller = new ProfesoresController();
                controller.ControllerContext = ControllerContext;
                return controller.Index();
            }
            else if ((Models.User.Tipo)Session["tipo"] == Models.User.Tipo.estudiante)
            {
                return RedirectToAction("Index", "Estudiantes");
            }

            return RedirectToAction("Login");

        }
        

        public ActionResult Login()
        {
            
            ViewBag.message = TempData["message"];
            return View();
        }

        [HttpPost]
        public ActionResult Login(string id, string password)
        {
            

            int ID = 0;

            //No se inserto un numero
            if(!int.TryParse(id, out ID))
            {
                TempData["message"] = "ID invalido";
                return RedirectToAction("Login");
            }

            Models.User user = Models.User.SelectUserById(ID);

            //Hubo un problema con la base de datos
            if(user == null)
            {
                TempData["message"] = "Ha ocurrido un problema";
                return RedirectToAction("Login");
            }
            //No existe el usuario
            else if(user.ID == 0)
            {
                TempData["message"] = "Usuario o contrasena incorrecto";
                return RedirectToAction("Login");
                
            }

            //Si la contrasena no es la correcta
            if (password != user.Password)
            {
                TempData["message"] = "Usuario o contrasena incorrecto";
                return RedirectToAction("Login");
            }
            
            Session["id"] = id;
            Session["tipo"] = user.Type;

            TempData["message"] = String.Format("Bienvenid@ {0} {1}", user.Nombre, user.Apellido);
            TempData["success"] = "true";
            if (user.Type == Models.User.Tipo.admin)
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (user.Type == Models.User.Tipo.profesor)
            {
                return RedirectToAction("Index", "Profesores");
            }
            else if (user.Type == Models.User.Tipo.estudiante)
            {
                return RedirectToAction("Index", "Estudiantes");
            }
            
            return Content(user.Nombre);
        }

        
    }
}