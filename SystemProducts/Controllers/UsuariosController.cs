using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SystemProducts.Data;
using SystemProducts.Models;

namespace SystemProducts.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly Database db = new Database(); // Contexto de BD como campo privado

        // GET: Usuarios
        public ActionResult Index()
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                var usuarios = db.USUARIOS.ToList();
                return View(usuarios);
            }
            catch (Exception ex)
            {
                // Loggear el error si es necesario
                ViewBag.ErrorMessage = "Ocurrió un error al obtener los usuarios.";
                return View(new List<USUARIOS>()); // Retorna una lista vacía en caso de error
            }
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // Inicializa un nuevo usuario con valores por defecto
            var usuario = new USUARIOS
            {
                Estado = true // Estado activo por defecto
            };
            return View(usuario);
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(USUARIOS usuario)
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    usuario.Estado = usuario.Estado ?? true;
                    usuario.FechaRegistro = DateTime.Now;

                    db.USUARIOS.Add(usuario);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Usuario creado correctamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al crear el usuario.";
                }
            }
            return View(usuario);
        }


        // GET: Usuarios/Edit/{id}
        public ActionResult Edit(int? id)
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            USUARIOS usuario = db.USUARIOS.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }
        // GET: Usuarios/Details/{id}
        public ActionResult Details(int? id)
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            USUARIOS usuario = db.USUARIOS.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(USUARIOS usuario)
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioExistente = db.USUARIOS.Find(usuario.IDUsuario);
                    if (usuarioExistente == null)
                    {
                        TempData["ErrorMessage"] = "Usuario no encontrado.";
                        return RedirectToAction("Index");
                    }

                    usuarioExistente.Nombre = usuario.Nombre;
                    usuarioExistente.Apellido = usuario.Apellido;
                    usuarioExistente.Correo = usuario.Correo;
                    usuarioExistente.Rol = usuario.Rol;
                    usuarioExistente.Estado = usuario.Estado;

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
                    return RedirectToAction("Index");
                }
                catch
                {
                    TempData["ErrorMessage"] = "Error al actualizar el usuario.";
                }
            }
            return View(usuario);
        }
        // GET: Usuarios/Delete/{id}
        public ActionResult Delete(int? id)
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            USUARIOS usuario = db.USUARIOS.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                USUARIOS usuario = db.USUARIOS.Find(id);
                if (usuario != null)
                {
                    db.USUARIOS.Remove(usuario);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Usuario eliminado correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Usuario no encontrado.";
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Error al eliminar el usuario.";
            }

            return RedirectToAction("Index");
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
