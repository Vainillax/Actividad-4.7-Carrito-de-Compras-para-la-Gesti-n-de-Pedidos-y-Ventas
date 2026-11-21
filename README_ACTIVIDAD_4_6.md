# Actividad 4.6 — Visualización en solo lectura, Borrado Lógico y Borrado Físico (Biblioteca, Windows Forms)

## Qué se implementó
- **Ventana modal de solo lectura (`FrmLibroDetalle`):** basada en el formulario de captura de Libros. Todos los `TextBox` tienen `ReadOnly = true`. Muestra ID, título, categoría (nombre), renta por día, estado, novedad, copias, ruta y la **imagen** del libro. Se abre con `ShowDialog()` y es el paso previo obligatorio antes de cualquier borrado.
- **Botones nuevos en `FrmListado` (creados en el Diseñador):** `btnVerDetalle` (solo consulta), `btnBorradoLogico` y `btnBorradoFisico`.
- **Carga de contexto:** el controlador recupera el objeto completo con `BuscarLibroMariaDB(id)` y puebla la ventana modal.
- **Borrado lógico (Soft Delete):** `ListadoController.BorradoLogicoAsync` ejecuta `UPDATE libros SET es_activo = 0 WHERE id = @id` en MariaDB y `$set: { EsActivo: false }` en MongoDB. No se borra nada.
- **Filtrado en listas:** `ConsultaJoin` ahora lleva `WHERE l.es_activo = 1` y el pipeline de MongoDB inicia con `$match: { EsActivo: true }`, por eso el registro desaparece de la vista pero sigue guardado.
- **Borrado físico (Hard Delete):** `ListadoController.BorradoFisicoAsync`:
  1. Guarda la ruta relativa de la imagen antes de borrar.
  2. MariaDB: `DELETE FROM libros WHERE id = @id` con parámetro `Int32` ligado a la llave primaria; captura los errores 1451/1217 (llave foránea) y avisa al usuario.
  3. MongoDB: `DeleteOneAsync` con filtro por `_id`.
  4. Si MongoDB falla, el libro se reinserta en MariaDB (compensación) para que ambos motores coincidan.
  5. Con el borrado confirmado, `EliminarImagenDeDisco` ejecuta `File.Delete(rutaCompleta)` solo si: la ruta está dentro de `ImagenesDatos`, el archivo existe, ningún otro libro lo usa y no está bloqueado por otro proceso (se prueba abriéndolo con `FileShare.None`).
- **Seguridad:** antes de ejecutar cualquier borrado hay un `MessageBox` con botones Yes/No e icono de advertencia; solo con "Sí" se ejecuta la lógica.
- **Autoría:** `lblAutoria` está en el Diseñador de `FrmListado` y de `FrmLibroDetalle`, y los métodos de borrado llevan el bloque de comentarios con los integrantes y el número de equipo.

## Requisitos externos
1. MariaDB/MySQL local (por ejemplo XAMPP) con la base `biblioteca`.
2. MongoDB Community Server en `mongodb://localhost:27017`.

## Cómo probarla
1. Iniciar MySQL y MongoDB. Abrir `Biblioteca.slnx` en Visual Studio y ejecutar (entrar con el login).
2. Abrir **Listado relacional** → "Preparar datos de ejemplo" → "Listar MariaDB (JOIN)".
3. **Ver detalle:** seleccionar un libro y pulsar "Ver detalle". Los campos no se pueden escribir.
4. **Borrado lógico:** seleccionar un libro → "Borrado lógico" → "Continuar con el borrado" → "Sí". El libro desaparece de la tabla. En phpMyAdmin: `SELECT id, titulo, es_activo FROM biblioteca.libros;` (queda en 0). En Compass: `{ EsActivo: false }`.
5. **Borrado físico:** seleccionar otro libro → "Borrado físico" → "Continuar" → "Sí". En phpMyAdmin y Compass el registro ya no existe y el archivo `ImagenesDatos\libro_ejemplo_N.png` se borra de la carpeta de la aplicación (`bin\Debug\...\ImagenesDatos`).
6. Pulsar "No" o "Cancelar" en cualquier paso: no cambia nada.
7. Para repetir pruebas: "Preparar datos de ejemplo" vuelve a crear los libros 1 a 6 borrados físicamente (y sus portadas). Un libro con borrado lógico se reactiva desde **Libros** (buscar por ID, Estado = Activo, Actualizar).

## Autoría
Equipo N° 3:
1. CONTRERAS Rodríguez Janis Isabel
2. MACÍAS Cruz Meredith Miranda
3. TORRES Barrera José Ángel
