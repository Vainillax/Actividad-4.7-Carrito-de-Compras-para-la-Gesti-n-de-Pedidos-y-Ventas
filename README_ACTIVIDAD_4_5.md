# Actividad 4.5 — Edición y Actualización de Datos e Imágenes (Biblioteca, Windows Forms)

## Qué se implementó
- **Transferencia de estado desde la consulta:** en `FrmListado` (listado relacional de la Actividad 4.3) se agregó el botón `btnEditar` ("Editar seleccionado"), creado en `FrmListado.Designer.cs`, y el evento `CellDoubleClick` de `dgvListado`. Ambos llaman a `EditarSeleccionado()`, que envía el ID del libro a `FrmPrincipal.EditarLibroDesdeListado(int)`.
- **Precarga del formulario:** `FrmPrincipal` abre (o trae al frente) `FrmLibros` y llama a `FrmLibros.CargarLibroParaEdicion(int)`. Este método consulta el libro por ID en MariaDB (`BuscarLibroMariaDB`) y llena los controles: título, costo, estado, novedad, copias y ruta de imagen. La vista previa de la imagen actual aparece en el `PictureBox` (rutina de la Actividad 3.6).
- **ComboBox dependiente (matching):** `cmb_Genero_frmLibros` conserva `DisplayMember = Nombre` y `ValueMember = Id`; al cargar el libro se asigna su `IdGenero` a `SelectedValue`, de modo que el usuario ve el nombre del género y el sistema mantiene el Id. Si el género del libro no está activo, se avisa y se marca el campo.
- **Actualización en MariaDB:** `ListadoController.ActualizarLibroMariaDB` ejecuta un `UPDATE ... WHERE id = @id` parametrizado con `ExecuteNonQuery()`.
- **Actualización en MongoDB:** `ListadoController.ActualizarLibroMongoDBAsync` usa un filtro por `_id` (`Builders<LibroDocumento>.Filter.Eq`) y `ReplaceOneAsync` con el documento completo.
- **Cambio de imagen:** si el usuario elige una imagen nueva, `ObtenerRutaImagen` (rutina de la Actividad 3.6) la copia a `ImagenesDatos`; la ruta del modelo solo cambia si la copia tuvo éxito. Cuando ambos motores ya guardaron la ruta nueva, se borra del disco la imagen anterior para no dejar archivos huérfanos.
- **Validaciones:** antes de actualizar se vuelven a validar ID, título, costo, tipo de recurso, copias y selección de género, en la misma forma que al guardar.
- **Sincronización:** si MongoDB falla después de actualizar MariaDB, se restauran en MariaDB los datos originales del libro. Esta compensación es una corrección técnica adicional, no un requisito de la actividad.
- **Autoría:** `lblAutoria` está en el Diseñador de `FrmLibros` y de `FrmListado`, y los métodos de actualización llevan el bloque de comentarios de autoría.

## Requisitos externos
Se requieren servicios de base de datos (código trabajado en Visual Studio Community):
1. MariaDB/MySQL local (por ejemplo, el servicio MySQL de XAMPP) con la base `biblioteca`.
2. MongoDB Community Server en `mongodb://localhost:27017`.

phpMyAdmin y MongoDB Compass no son necesarios para ejecutar el programa; solo sirven para comprobar los cambios y tomar las evidencias.

## Cómo probarla
1. Iniciar MySQL (XAMPP) y MongoDB. Abrir `Biblioteca.slnx` en Visual Studio y ejecutar.
2. Si la base está vacía: ejecutar `Database/Actividad_4_4_MariaDB.sql` en phpMyAdmin, o abrir **Listado relacional** y pulsar "Preparar datos de ejemplo".
3. En **Listado relacional** pulsar "Listar MariaDB (JOIN)", seleccionar un libro y hacer **doble clic** (o pulsar "Editar seleccionado").
4. Se abre **Libros** con los datos, el género y la imagen del registro. Capturar este estado: son los datos originales.
5. Cambiar título/costo/género y, con el botón "..." o "Cargar Imagen", elegir otra imagen. Pulsar **Actualizar** (barra superior) y confirmar.
6. Comprobar el cambio en phpMyAdmin (`SELECT * FROM biblioteca.libros WHERE id = ...;`) y en MongoDB Compass (`{ _id: ... }`). Las dos bases deben mostrar los mismos datos y la misma `ruta_imagen` / `RutaImagen`.

## Autoría
Equipo N° 3:
1. CONTRERAS Rodríguez Janis Isabel
2. MACÍAS Cruz Meredith Miranda
3. TORRES Barrera José Ángel
