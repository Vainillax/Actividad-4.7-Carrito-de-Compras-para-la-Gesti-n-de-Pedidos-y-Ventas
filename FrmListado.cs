/*
 * ENCABEZADO DE AUTORÍA
 * Equipo N°: 3
 * Integrantes (orden alfabético por primer apellido):
 * 1. CONTRERAS Rodríguez Janis Isabel
 * 2. MACÍAS Cruz Meredith Miranda
 * 3. TORRES Barrera José Ángel
 */
using Biblioteca.Controllers;

namespace Biblioteca
{
    // Actividad 4.3: ventana de listado relacional. Muestra los libros con el NOMBRE de su categoría
    // (no el ID) leyendo de MariaDB (JOIN) o de MongoDB ($lookup), y la portada de cada libro al seleccionarlo.
    // Todos los controles (botones, tabla, PictureBox, Label de autoría) están creados en el Diseñador
    // (FrmListado.Designer.cs); aquí solo está la lógica.
    public partial class FrmListado : Form
    {
        private readonly ListadoController _controlador = new ListadoController();

        // Actividad 4.6: motor del último listado ("MariaDB" o "MongoDB"); se vuelve a listar con él tras borrar.
        private string _motorActual = string.Empty;

        public FrmListado()
        {
            InitializeComponent();
            Escribir("Módulo de listado listo. Pulsa \"Preparar datos de ejemplo\" la primera vez.");
        }

        // La fecha y hora del sistema debe verse en las capturas de evidencia.
        private void Escribir(string texto)
        {
            txtBitacora.AppendText($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {texto}{Environment.NewLine}");
        }

        // ================== PREPARAR DATOS (tablas + registros de ejemplo en los dos motores) ==================

        private void btnPreparar_Click(object? sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                if (_controlador.PrepararMariaDB())
                    Escribir("MariaDB: tablas 'generos' y 'libros' listas con datos de ejemplo.");
                else
                    Escribir("MariaDB: no se pudieron preparar los datos.");

                if (_controlador.PrepararMongoDB())
                    Escribir("MongoDB: colecciones 'generos' y 'libros' listas con datos de ejemplo.");
                else
                    Escribir("MongoDB: no se pudieron preparar los datos.");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // ================== LISTADO ==================

        private void btnListarMariaDB_Click(object? sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                Escribir("MariaDB: consulta ejecutada:" + Environment.NewLine + ListadoController.ConsultaJoin.Replace("\n", Environment.NewLine));
                List<LibroListado>? lista = _controlador.ListarMariaDB();
                if (lista == null)
                {
                    Escribir("MariaDB: el listado falló.");
                    return;
                }
                Mostrar(lista, "MariaDB (JOIN)");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnListarMongoDB_Click(object? sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                Escribir("MongoDB: pipeline ejecutado:" + Environment.NewLine + ListadoController.DescripcionPipeline.Replace("\n", Environment.NewLine));
                List<LibroListado>? lista = _controlador.ListarMongoDB();
                if (lista == null)
                {
                    Escribir("MongoDB: el listado falló.");
                    return;
                }
                Mostrar(lista, "MongoDB ($lookup)");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnLimpiar_Click(object? sender, EventArgs e)
        {
            dgvListado.DataSource = null;
            LiberarImagen();
            lblImagen.Text = "Selecciona un libro para ver su portada.";
            lblOrigen.Text = "Origen de datos: ninguno | Registros: 0";
        }

        // Vincula la lista al DataGridView. Las columnas se definieron en el Diseñador con encabezados
        // claros ("Categoría") y sin ningún ID técnico de llave foránea.
        private void Mostrar(List<LibroListado> lista, string motor)
        {
            dgvListado.DataSource = null;
            dgvListado.DataSource = lista;
            _motorActual = motor.StartsWith("MariaDB") ? "MariaDB" : "MongoDB";
            lblOrigen.Text = $"Origen de datos: {motor} | Registros: {lista.Count}";
            Escribir($"{motor}: {lista.Count} registro(s) listado(s).");

            if (lista.Count == 0)
            {
                LiberarImagen();
                lblImagen.Text = "No hay registros. Pulsa \"Preparar datos de ejemplo\".";
            }
        }

        // ================== ACTIVIDAD 4.5: ENVIAR EL REGISTRO AL FORMULARIO DE EDICIÓN ==================

        // Doble clic en una fila de la tabla: transfiere el libro al formulario Libros.
        private void dgvListado_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                EditarSeleccionado();
        }

        // Botón "Editar seleccionado" (creado en el Diseñador): hace lo mismo que el doble clic.
        private void btnEditar_Click(object? sender, EventArgs e)
        {
            EditarSeleccionado();
        }

        private void EditarSeleccionado()
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.5 (Edición y Actualización)
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            if (dgvListado.CurrentRow?.DataBoundItem is not LibroListado libro)
            {
                Escribir("Edición: seleccione primero un libro de la tabla.");
                return;
            }

            if (MdiParent is not FrmPrincipal principal)
            {
                Escribir("Edición: no se encontró la ventana principal para abrir el formulario Libros.");
                return;
            }

            Escribir($"Edición: se envía el libro ID {libro.Id} (\"{libro.Titulo}\") al formulario Libros.");
            principal.EditarLibroDesdeListado(libro.Id);
        }

        // ================== ACTIVIDAD 4.6: VISUALIZACIÓN Y BORRADO ==================

        // Devuelve el libro seleccionado en la tabla o avisa en la bitácora si no hay ninguno.
        private LibroListado? ObtenerSeleccionado(string accion)
        {
            if (dgvListado.CurrentRow?.DataBoundItem is LibroListado libro)
                return libro;

            Escribir($"{accion}: seleccione primero un libro de la tabla.");
            MessageBox.Show("Seleccione primero un libro de la tabla.", accion,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return null;
        }

        // El controlador recupera el objeto COMPLETO desde MariaDB (incluye género, estado e imagen)
        // para poblar la ventana modal de solo lectura.
        private LibroListado? RecuperarCompleto(int id, string accion)
        {
            LibroListado? completo = _controlador.BuscarLibroMariaDB(id);

            if (completo == null)
            {
                Escribir($"{accion}: el libro ID {id} no existe en MariaDB.");
                MessageBox.Show($"El libro ID {id} ya no existe en MariaDB. Vuelva a listar.", accion,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return completo;
        }

        // Muestra la ventana modal. Devuelve true solo si el usuario pulsó "Continuar con el borrado".
        private bool MostrarDetalle(LibroListado libro, string accion)
        {
            using (FrmLibroDetalle detalle = new FrmLibroDetalle())
            {
                detalle.Cargar(libro, accion);
                return detalle.ShowDialog(this) == DialogResult.OK;
            }
        }

        // Vuelve a listar con el mismo motor para que la tabla refleje el cambio.
        private void RefrescarListado()
        {
            if (_motorActual == "MariaDB")
                btnListarMariaDB_Click(null, EventArgs.Empty);
            else if (_motorActual == "MongoDB")
                btnListarMongoDB_Click(null, EventArgs.Empty);
        }

        // Botón "Ver detalle": solo consulta, no borra nada.
        private void btnVerDetalle_Click(object? sender, EventArgs e)
        {
            LibroListado? seleccionado = ObtenerSeleccionado("Ver detalle");
            if (seleccionado == null)
                return;

            LibroListado? completo = RecuperarCompleto(seleccionado.Id, "Ver detalle");
            if (completo == null)
                return;

            MostrarDetalle(completo, FrmLibroDetalle.Consulta);
            Escribir($"Ver detalle: se consultó el libro ID {completo.Id} (\"{completo.Titulo}\") en modo solo lectura.");
        }

        private async void btnBorradoLogico_Click(object? sender, EventArgs e)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.6 (Borrado Lógico y Borrado Físico)
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            LibroListado? seleccionado = ObtenerSeleccionado("Borrado lógico");
            if (seleccionado == null)
                return;

            LibroListado? libro = RecuperarCompleto(seleccionado.Id, "Borrado lógico");
            if (libro == null)
                return;

            // Paso previo obligatorio: ventana modal de solo lectura.
            if (!MostrarDetalle(libro, FrmLibroDetalle.BorradoLogico))
            {
                Escribir($"Borrado lógico: cancelado en la ventana de detalle (libro ID {libro.Id}).");
                return;
            }

            // Doble confirmación: solo con "Sí" se ejecuta la lógica de borrado.
            DialogResult respuesta = MessageBox.Show(
                $"¿Desea desactivar el libro ID {libro.Id} (\"{libro.Titulo}\")?\n\n" +
                "El registro seguirá guardado, pero dejará de aparecer en el listado.",
                "Confirmar borrado lógico",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
            {
                Escribir($"Borrado lógico: el usuario respondió No (libro ID {libro.Id}).");
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                ListadoController.ResultadoBorrado resultado = await _controlador.BorradoLogicoAsync(libro);
                Escribir("Borrado lógico: " + resultado.Mensaje);

                if (resultado.Exito)
                    RefrescarListado();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void btnBorradoFisico_Click(object? sender, EventArgs e)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.6 (Borrado Lógico y Borrado Físico)
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            LibroListado? seleccionado = ObtenerSeleccionado("Borrado físico");
            if (seleccionado == null)
                return;

            LibroListado? libro = RecuperarCompleto(seleccionado.Id, "Borrado físico");
            if (libro == null)
                return;

            // Paso previo obligatorio: ventana modal de solo lectura.
            if (!MostrarDetalle(libro, FrmLibroDetalle.BorradoFisico))
            {
                Escribir($"Borrado físico: cancelado en la ventana de detalle (libro ID {libro.Id}).");
                return;
            }

            // Doble confirmación: MessageBox Yes/No con icono de advertencia.
            DialogResult respuesta = MessageBox.Show(
                $"¿Eliminar DEFINITIVAMENTE el libro ID {libro.Id} (\"{libro.Titulo}\")?\n\n" +
                "Se borrará de MariaDB, de MongoDB y su imagen del disco. Esta acción no se puede deshacer.",
                "Confirmar borrado físico",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
            {
                Escribir($"Borrado físico: el usuario respondió No (libro ID {libro.Id}).");
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                // La portada mostrada aquí es una copia en memoria (no bloquea el archivo);
                // aun así se libera para no seguir mostrando una imagen que dejará de existir.
                LiberarImagen();

                ListadoController.ResultadoBorrado resultado = await _controlador.BorradoFisicoAsync(libro);
                Escribir("Borrado físico: " + resultado.Mensaje);

                if (resultado.Exito)
                    RefrescarListado();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // ================== IMAGEN DEL REGISTRO SELECCIONADO ==================

        private void dgvListado_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvListado.CurrentRow?.DataBoundItem is LibroListado libro)
                MostrarImagen(libro);
            else
                LiberarImagen();
        }

        // 1) Se arma la ruta física a partir de la ruta RELATIVA guardada en la base de datos.
        // 2) File.Exists comprueba que el archivo siga en el disco (si no, se avisa sin lanzar error).
        // 3) Image.FromStream sobre un FileStream dentro de "using": el archivo se lee, se copia a un
        //    Bitmap en memoria y el flujo se cierra enseguida, así el archivo no queda bloqueado
        //    (se puede borrar o reemplazar mientras la aplicación sigue abierta).
        private void MostrarImagen(LibroListado libro)
        {
            LiberarImagen();

            if (string.IsNullOrWhiteSpace(libro.RutaImagen))
            {
                lblImagen.Text = $"{libro.Titulo}\nEste libro no tiene imagen asignada.";
                return;
            }

            string rutaFisica = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, libro.RutaImagen);

            if (!File.Exists(rutaFisica))
            {
                lblImagen.Text = $"{libro.Titulo}\nImagen no encontrada: {libro.RutaImagen}";
                return;
            }

            try
            {
                using (FileStream flujo = new FileStream(rutaFisica, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (Image original = Image.FromStream(flujo))
                {
                    pbxPortada.Image = new Bitmap(original);
                }
                lblImagen.Text = $"{libro.Titulo}\n{libro.RutaImagen}";
            }
            catch (ArgumentException)
            {
                // Image.FromStream lanza esta excepción cuando el archivo no es una imagen válida.
                lblImagen.Text = $"{libro.Titulo}\nEl archivo no es una imagen válida.";
            }
            catch (IOException ex)
            {
                lblImagen.Text = $"{libro.Titulo}\nNo se pudo leer la imagen: {ex.Message}";
            }
        }

        // Libera la imagen anterior de la memoria antes de cargar otra.
        private void LiberarImagen()
        {
            pbxPortada.Image?.Dispose();
            pbxPortada.Image = null;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            LiberarImagen();
            base.OnFormClosed(e);
        }
    }
}
