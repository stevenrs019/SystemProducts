using System;
using System.Linq;
using System.Web.Mvc;
using SystemProducts.Models;

namespace SystemProducts.Controllers
{
    public class RegistroController : Controller
    {
        private readonly ArquitecturaSoftwareEntities db = new ArquitecturaSoftwareEntities();

        // GET: Registro/Crear
        public ActionResult Crear()
        {
            var nuevoUsuario = new USUARIOS
            {
                Estado = true // Activo por defecto
            };
            return View("Formulario", nuevoUsuario); // Muestra la vista llamada Formulario.cshtml
        }

        // POST: Registro/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(USUARIOS usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    usuario.Estado = usuario.Estado ?? true;
                    usuario.FechaRegistro = DateTime.Now;

                    db.USUARIOS.Add(usuario);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "¡Registro exitoso! Ahora podés iniciar sesión.";
                    return RedirectToAction("Index", "Login");
                }
                catch
                {
                    TempData["ErrorMessage"] = "Error al registrar el usuario.";
                }
            }

            return View("Formulario", usuario);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
