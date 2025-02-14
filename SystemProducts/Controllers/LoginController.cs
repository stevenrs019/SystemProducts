using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SystemProducts.Models;

namespace SystemProducts.Controllers
{
    public class LoginController : Controller
    {
        private readonly Database db = new Database(); // Contexto de BD como campo privado


        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string correo, string contraseña)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contraseña))
            {
                ViewBag.ErrorMessage = "Correo y contraseña son obligatorios.";
                return View();
            }

            

            var usuario = db.USUARIOS.FirstOrDefault(u => u.Correo == correo && u.Contraseña == contraseña);

            if (usuario != null)
            {
                // Guardar datos del usuario en sesión
                Session["IDUsuario"] = usuario.IDUsuario;
                Session["Nombre"] = usuario.Nombre;
                Session["Rol"] = usuario.Rol;

                return RedirectToAction("Index", "Home"); // Redirige a la gestión de productos
            }
            else
            {
                ViewBag.ErrorMessage = "Correo o contraseña incorrectos.";
                return View();
            }
        }

        // Método para cerrar sesión
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        // Método para encriptar la contraseña con SHA256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}