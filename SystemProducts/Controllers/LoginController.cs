﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SystemProducts.Models;
using ResendEmailClient;
using System.Threading.Tasks;

namespace SystemProducts.Controllers
{
    public class LoginController : Controller
    {
        private readonly ArquitecturaSoftwareEntities db = new ArquitecturaSoftwareEntities(); // Contexto de BD como campo privado
        private readonly ResendService resendService = new ResendService("re_JjyGu1j2_26iBkMi96bSHzBpsTcbL87Cg");

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
                // Enviar correo de inicio de sesión
                Task.Run(async () =>
                {
                    try
                    {
                        // 1. Ruta al template
                        var templatePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Views/Shared/EmailTemplates/LoginSuccess.html");

                        // 2. Leer HTML
                        var htmlTemplate = System.IO.File.ReadAllText(templatePath);

                        // 3. Reemplazar contenido dinámico
                        var htmlFinal = htmlTemplate
                            .Replace("{NOMBRE}", usuario.Nombre)
                            .Replace("{FECHA}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                            .Replace("{ANIO}", DateTime.Now.Year.ToString());

                        // 4. Enviar
                        await resendService.SendEmailAsync(
                            "onboarding@resend.dev",
                correo,
                "Inicio de sesión exitoso",
                htmlFinal
                        );
                    }
                    catch (Exception ex)
                    {
                        // Logueá el error o ignorá según tus necesidades
                        System.Diagnostics.Debug.WriteLine("Error enviando correo: " + ex.Message);
                    }
                });
                if (usuario.Rol == "Cliente")
                {
                    return RedirectToAction("Presentacion", "Productos");
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // Redirige a la gestión de productos

                }

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