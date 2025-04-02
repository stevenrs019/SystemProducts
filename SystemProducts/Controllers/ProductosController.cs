using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemProducts.Data;
using SystemProducts.Models;

namespace SystemProducts.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ArquitecturaSoftwareEntities db = new ArquitecturaSoftwareEntities(); // Contexto de BD como campo privado

        // Verifica sesión antes de mostrar productos (Protección para administradores)
        public ActionResult Index()
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var productos = db.PRODUCTOS.Include("CATEGORIAS_PRODUCTOS").ToList();
            return View(productos);
        }

        // Mostrar la presentación de productos para los clientes
        public ActionResult Presentacion()
        {
            return View(db.PRODUCTOS.ToList());
        }

        // 3️⃣ Crear un nuevo producto (GET)
        [HttpGet]
        // 🔹 GET: Productos/Create
        public ActionResult Create()
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // 🔹 Inicializar ViewBag con la lista de categorías
            ViewBag.IDCategoria = new SelectList(db.CATEGORIAS_PRODUCTOS, "IDCategoria", "NombreCategoria");

            // 🔹 Asegurar que se pasa un modelo vacío para evitar problemas de `null`
            return View(new PRODUCTOS());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PRODUCTOS producto)
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (!ModelState.IsValid)
            {
                // Si hay error de validación, recargar la lista de categorías
                ViewBag.IDCategoria = new SelectList(db.CATEGORIAS_PRODUCTOS, "IDCategoria", "NombreCategoria", producto.IDCategoria);
                return View(producto);
            }

            db.PRODUCTOS.Add(producto);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Producto creado correctamente.";
            return RedirectToAction("Index");
        }



        [HttpPost]
        public ActionResult UploadImage()
        {
            try
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    string path = Server.MapPath("~/Content/Img/" + fileName);
                    file.SaveAs(path);

                    return Json(new { success = true, imageUrl = "/Content/Img/" + fileName });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Error al subir la imagen." });
        }

        // GET: Productos/Edit/{id} - Carga la vista con el producto seleccionado
        // 🔹 GET: Productos/Edit/{id} - Carga la vista con el producto seleccionado
        public ActionResult Edit(int? id)
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (id == null)
            {
                return HttpNotFound();
            }

            // 🔹 Incluir la relación con CATEGORIAS_PRODUCTOS para obtener IDCategoria correctamente
            PRODUCTOS producto = db.PRODUCTOS.Include("CATEGORIAS_PRODUCTOS").FirstOrDefault(p => p.IDProducto == id);

            if (producto == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDCategoria = new SelectList(db.CATEGORIAS_PRODUCTOS, "IDCategoria", "NombreCategoria", producto.IDCategoria);
            return View(producto);
        }

        // 🔹 POST: Productos/Edit/{id} - Guarda los cambios en la base de datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PRODUCTOS producto, HttpPostedFileBase ImagenFile)
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (!ModelState.IsValid)
            {
                // Recargar lista de categorías para el dropdown en la vista si hay error
                ViewBag.IDCategoria = new SelectList(db.CATEGORIAS_PRODUCTOS, "IDCategoria", "NombreCategoria", producto.IDCategoria);
                return View(producto);
            }

            var productoExistente = db.PRODUCTOS.Find(producto.IDProducto);
            if (productoExistente == null)
            {
                return HttpNotFound();
            }

            // 🔹 Actualizar los valores del producto
            productoExistente.Nombre = producto.Nombre;
            productoExistente.IDCategoria = producto.IDCategoria; // Ahora usa IDCategoria
            productoExistente.Precio = producto.Precio;
            productoExistente.Stock = producto.Stock;
            productoExistente.Descripcion = producto.Descripcion;

            // 🔹 Manejo de imagen (Si el usuario subió una nueva)
            if (ImagenFile != null && ImagenFile.ContentLength > 0)
            {
                string fileName = Guid.NewGuid().ToString() + "_" + System.IO.Path.GetFileName(ImagenFile.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/Content/Img/"), fileName);
                ImagenFile.SaveAs(path);

                // Guardar la ruta en la base de datos
                productoExistente.Imagen = "/Content/Img/" + fileName;
            }

            db.SaveChanges();
            TempData["SuccessMessage"] = "Producto actualizado correctamente.";
            return RedirectToAction("Index");
        }



        // 🔹 7️⃣ Detalles de un producto
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            PRODUCTOS producto = db.PRODUCTOS.Include("CATEGORIAS_PRODUCTOS").FirstOrDefault(p => p.IDProducto == id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            return View(producto);
        }
        // 🔹 GET: Productos/Delete/{id} - Muestra la confirmación de eliminación
        public ActionResult Delete(int? id)
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (id == null)
            {
                return HttpNotFound();
            }

            PRODUCTOS producto = db.PRODUCTOS.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            return View(producto);
        }

        // 🔹 POST: Productos/DeleteConfirmed/{id} - Elimina el producto de la base de datos
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["IDUsuario"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            PRODUCTOS producto = db.PRODUCTOS.Find(id);
            if (producto != null)
            {
                db.PRODUCTOS.Remove(producto);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Producto eliminado correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error al eliminar el producto.";
            }

            return RedirectToAction("Index");
        }

    }
}
