
/*
 * ENCABEZADO DE AUTORÍA
 * Equipo N°: 3
 * Integrantes (orden alfabético por primer apellido):
 * 1. CONTRERAS Rodríguez Janis Isabel
 * 2. MACÍAS Cruz Meredith Miranda
 * 3. TORRES Barrera José Ángel
 */

using Biblioteca.Models;
using Biblioteca.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Biblioteca
{
    public partial class FrmLibros : FrmBase, IPanelCRUD
    {
        public FrmLibros()
        {
            InitializeComponent();
            ConfigurarValidaciones();
            ConfigurarRecurso();
            CargarGenerosDependientes();
        }

        private const string CarpetaArchivos = "ArchivosDigitales";

        private readonly ListadoController _listadoController =
            new ListadoController();

        private string _rutaOrigenArchivo = string.Empty;

        // Actividad 4.5: copia del registro tal como se cargó para editarlo. Sirve para
        // restaurar MariaDB si MongoDB falla y para borrar la imagen anterior al cambiarla.
        private LibroListado? _libroOriginal;

        // ================== CARGA DE GÉNEROS ==================

        private void CargarGenerosDependientes()
        {
            try
            {
                if (!_listadoController.CrearTablaLibrosMariaDB())
                    return;

                List<Genero> generos =
                    _listadoController.ObtenerGenerosMariaDB();

                cmb_Genero_frmLibros.DataSource = null;
                cmb_Genero_frmLibros.DisplayMember = nameof(Genero.Nombre);
                cmb_Genero_frmLibros.ValueMember = nameof(Genero.Id);
                cmb_Genero_frmLibros.DataSource = generos;

                errorProvider1.SetError(cmb_Genero_frmLibros, string.Empty);

                if (generos.Count == 0)
                {
                    txb_Info_frmLibros.Text =
                        "No hay géneros registrados. Registre primero las categorías en el módulo Géneros.";
                }
            }
            catch (Exception ex)
            {
                txb_Info_frmLibros.Text =
                    "No fue posible cargar las categorías: " + ex.Message;
            }
        }

        // ================== ARCHIVOS DIGITALES ==================

        private string? GuardarArchivoDigital()
        {
            if (_rutaOrigenArchivo.Length == 0)
                return txb_Archivo_frmLibros.Text.Trim();

            string? rutaRelativa =
                CopiarAlProyecto(_rutaOrigenArchivo, CarpetaArchivos);

            if (rutaRelativa == null)
                return null;

            _rutaOrigenArchivo = string.Empty;
            txb_Archivo_frmLibros.Text = rutaRelativa;

            return rutaRelativa;
        }

        // ================== RECURSOS FÍSICOS Y DIGITALES ==================

        private void ConfigurarRecurso()
        {
            cmb_Tipo_frmLibros.Items.Clear();
            cmb_Tipo_frmLibros.Items.AddRange(
                new object[] { "Físico", "Digital" });

            cmb_Tipo_frmLibros.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cmb_Tipo_frmLibros.SelectedIndex = 0;

            cmb_Tipo_frmLibros.SelectedIndexChanged +=
                (s, e) => ActualizarCamposSegunTipo();

            txb_Copias_frmLibros.KeyPress += SoloNumeros_KeyPress;
            txb_Copias_frmLibros.ShortcutsEnabled = false;
            txb_Archivo_frmLibros.ReadOnly = true;

            btn_Archivo_frmLibros.Click += btn_Archivo_Click;
            btn_Descargar_frmLibros.Click += btn_Descargar_Click;

            CargarAutoresDisponibles();
            ActualizarCamposSegunTipo();

            if (SesionSistema.EsInvitado)
                AplicarSoloLectura();
        }

        private void ActualizarCamposSegunTipo()
        {
            bool esDigital =
                cmb_Tipo_frmLibros.SelectedItem?.ToString() == "Digital";

            txb_Copias_frmLibros.Enabled = !esDigital;
            txb_Archivo_frmLibros.Enabled = esDigital;
            btn_Archivo_frmLibros.Enabled = esDigital;

            if (esDigital)
                txb_Copias_frmLibros.Clear();
        }

        private void CargarAutoresDisponibles()
        {
            clb_Autores_frmLibros.Items.Clear();

            foreach (Autores a in Autores.ObtenerTodos())
                clb_Autores_frmLibros.Items.Add(a);

            clb_Autores_frmLibros.CheckOnClick = true;
        }

        private void AplicarSoloLectura()
        {
            foreach (Control c in Controls)
                BloquearControles(c);

            txtId.Enabled = true;
            btn_Descargar_frmLibros.Enabled = true;
        }

        private void BloquearControles(Control contenedor)
        {
            foreach (Control c in contenedor.Controls)
            {
                if (c is TextBox ||
                    c is ComboBox ||
                    c is Button ||
                    c is CheckedListBox)
                {
                    c.Enabled = false;
                }

                BloquearControles(c);
            }
        }

        private void btn_Archivo_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog dialogo = new OpenFileDialog();

            dialogo.Title = "Seleccionar archivo digital";
            dialogo.Filter =
                "Libros digitales (*.pdf;*.epub)|*.pdf;*.epub|" +
                "Todos los archivos (*.*)|*.*";

            if (dialogo.ShowDialog() == DialogResult.OK)
            {
                _rutaOrigenArchivo = dialogo.FileName;
                txb_Archivo_frmLibros.Text =
                    Path.GetFileName(dialogo.FileName);

                txb_Info_frmLibros.Text =
                    "Archivo seleccionado. Se copiará a la carpeta \"" +
                    CarpetaArchivos + "\" al guardar el registro.";
            }
        }

        private void btn_Descargar_Click(object? sender, EventArgs e)
        {
            string ruta = txb_Archivo_frmLibros.Text.Trim();

            if (cmb_Tipo_frmLibros.SelectedItem?.ToString() != "Digital"
                || ruta.Length == 0)
            {
                txb_Info_frmLibros.Text =
                    "Este libro no tiene un archivo digital disponible.";
                return;
            }

            try
            {
                bool esEnlace =
                    Uri.TryCreate(ruta, UriKind.Absolute, out Uri? uri)
                    && (uri.Scheme == Uri.UriSchemeHttp
                    || uri.Scheme == Uri.UriSchemeHttps);

                string destino = ruta;

                if (!esEnlace)
                {
                    if (_rutaOrigenArchivo.Length > 0)
                        destino = _rutaOrigenArchivo;
                    else if (!Path.IsPathRooted(ruta))
                        destino = Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory, ruta);
                }

                Process.Start(new ProcessStartInfo(destino)
                {
                    UseShellExecute = true
                });
            }
            catch
            {
                txb_Info_frmLibros.Text =
                    "No se pudo abrir el archivo. Verifique que la ruta o el enlace sean correctos.";
            }
        }

        // ================== VALIDACIÓN DE RECURSOS ==================

        private bool RecursoValido()
        {
            bool valido = true;

            errorProvider1.SetError(txb_Copias_frmLibros, "");
            errorProvider1.SetError(txb_Archivo_frmLibros, "");

            if (cmb_Tipo_frmLibros.SelectedItem?.ToString() == "Digital")
            {
                if (string.IsNullOrWhiteSpace(txb_Archivo_frmLibros.Text))
                {
                    errorProvider1.SetError(
                        txb_Archivo_frmLibros,
                        "Seleccione el archivo o enlace del recurso digital.");

                    valido = false;
                }
            }
            else if (!int.TryParse(
                txb_Copias_frmLibros.Text.Trim(), out int copias)
                || copias < 1)
            {
                errorProvider1.SetError(
                    txb_Copias_frmLibros,
                    "Indique cuántas copias físicas hay (mínimo 1).");

                valido = false;
            }

            if (!valido)
            {
                txb_Info_frmLibros.Text =
                    "Hay campos con errores. Corrija los campos marcados.";
            }

            return valido;
        }

        private void VincularAutoresSeleccionados(int idLibro)
        {
            LibroAutor.QuitarVinculosDeLibro(idLibro);

            foreach (object item in clb_Autores_frmLibros.CheckedItems)
            {
                if (item is Autores autor)
                    LibroAutor.Vincular(idLibro, autor.Id);
            }
        }

        // ================== VALIDACIONES ==================

        private void ConfigurarValidaciones()
        {
            txb_Titulo_frmLibros.Validating -= TextoObligatorio_Validating;
            txb_Titulo_frmLibros.Validating += TextoObligatorio_Validating;

            txb_Costo_frmLibros.KeyPress -= Decimal_KeyPress;
            txb_Costo_frmLibros.KeyPress += Decimal_KeyPress;

            txb_Costo_frmLibros.Validating -= DecimalObligatorio_Validating;
            txb_Costo_frmLibros.Validating += DecimalObligatorio_Validating;

            txb_Costo_frmLibros.ShortcutsEnabled = false;

            cmb_Novedad_frmLibros.Validating -= ComboObligatorio_Validating;
            cmb_Novedad_frmLibros.Validating += ComboObligatorio_Validating;

            cmb_Estado_frmLibros.Validating -= ComboObligatorio_Validating;
            cmb_Estado_frmLibros.Validating += ComboObligatorio_Validating;
        }

        private static bool EsSi(string? texto)
        {
            return texto == "Si" || texto == "Sí";
        }

        private bool ObtenerNovedadSeleccionada()
        {
            return EsSi(cmb_Novedad_frmLibros.SelectedItem?.ToString());
        }

        private bool ObtenerEstadoSeleccionado()
        {
            return cmb_Estado_frmLibros.SelectedItem?.ToString() == "Activo";
        }

        private bool ObtenerGeneroSeleccionado(out int idGenero)
        {
            idGenero = 0;

            return cmb_Genero_frmLibros.SelectedValue != null
                && int.TryParse(
                    cmb_Genero_frmLibros.SelectedValue.ToString(),
                    out idGenero);
        }

        private bool ObtenerId(out int id)
        {
            id = 0;

            if (!int.TryParse(txtId.Text.Trim(), out id))
            {
                errorProvider1.SetError(txtId, "Ingrese un ID numérico válido.");
                txb_Info_frmLibros.Text =
                    "Ingrese un ID numérico válido para continuar.";
                return false;
            }

            errorProvider1.SetError(txtId, "");
            return true;
        }

        // ================== GUARDAR ==================

        public async void EjecutarGuardar()
        {
            bool hayErrores =
                !ValidateChildren() || HayErroresActivos(this);

            if (hayErrores)
            {
                txb_Info_frmLibros.Text =
                    "Hay campos con errores. Corrija los campos marcados antes de guardar.";
                return;
            }

            if (!ObtenerId(out int id))
                return;

            if (!RecursoValido())
                return;

            if (!ObtenerGeneroSeleccionado(out int idGenero))
            {
                txb_Info_frmLibros.Text =
                    "Seleccione una categoría válida.";

                errorProvider1.SetError(
                    cmb_Genero_frmLibros,
                    "La categoría es obligatoria.");

                return;
            }

            try
            {
                string titulo = txb_Titulo_frmLibros.Text.Trim();

                if (!decimal.TryParse(
                    txb_Costo_frmLibros.Text.Trim(), out decimal costo)
                    || costo <= 0)
                {
                    txb_Info_frmLibros.Text =
                        "Ingrese un costo válido mayor que cero.";
                    return;
                }

                bool esNovedad = ObtenerNovedadSeleccionada();
                bool estado = ObtenerEstadoSeleccionado();

                string rutaImagen =
                    ObtenerRutaImagen(txb_RutaIma_frmLibros);

                bool esDigital =
                    cmb_Tipo_frmLibros.SelectedItem?.ToString() == "Digital";

                TipoRecurso tipo = esDigital
                    ? TipoRecurso.Digital
                    : TipoRecurso.Fisico;

                int copias = esDigital
                    ? 0
                    : int.Parse(txb_Copias_frmLibros.Text.Trim());

                string? archivo = string.Empty;

                if (esDigital)
                {
                    archivo = GuardarArchivoDigital();

                    if (archivo == null)
                        return;
                }

                LibroDocumento documento = new LibroDocumento
                {
                    Id = id,
                    Titulo = titulo,
                    PrecioRentaDiaria = costo,
                    IdGenero = idGenero,
                    RutaImagen = rutaImagen,
                    EsActivo = estado,
                    EsNovedad = esNovedad,
                    CantidadCopias = copias
                };

                if (!_listadoController.CrearTablaLibrosMariaDB())
                    return;

                bool guardadoMariaDB =
                    _listadoController.InsertarLibroMariaDB(documento);

                if (!guardadoMariaDB)
                    return;

                bool guardadoMongoDB =
                    await _listadoController.InsertarLibroMongoDBAsync(documento);

                if (!guardadoMongoDB)
                {
                    bool revertido =
                        _listadoController.RevertirLibroMariaDB(id);

                    txb_Info_frmLibros.Text = revertido
                        ? "MongoDB rechazó el registro; se revirtió también la inserción en MariaDB."
                        : "Falló MongoDB y no se pudo revertir MariaDB. Revise ambas bases antes de repetir el ID.";

                    return;
                }

                Libros nuevoLibro = new Libros(
                    id, titulo, costo, copias, esNovedad,
                    rutaImagen, estado, tipo, archivo);

                nuevoLibro.InsertarRegistro(nuevoLibro);

                VincularAutoresSeleccionados(id);

                txb_Info_frmLibros.Text =
                    "Libro guardado correctamente en MariaDB y MongoDB.";

                LimpiarCampos();
                CargarGenerosDependientes();
            }
            catch (Exception ex)
            {
                txb_Info_frmLibros.Text =
                    "Ocurrió un error al guardar: " + ex.Message;
            }
        }

        // ================== BUSCAR ==================

        public void EjecutarBuscar(string id, ErrorProvider alerta)
        {
            alerta.Clear();
            errorProvider1.Clear();

            if (string.IsNullOrWhiteSpace(id))
            {
                alerta.SetError(
                    txtId,
                    "Ingrese un ID de libro para realizar la búsqueda.");
                return;
            }

            if (!int.TryParse(id.Trim(), out int idBuscado))
            {
                alerta.SetError(txtId, "El ID debe ser un número entero.");
                return;
            }

            try
            {
                LibroListado? libroEncontrado =
                    _listadoController.BuscarLibroMariaDB(idBuscado);

                if (libroEncontrado == null)
                {
                    _libroOriginal = null;

                    txb_Info_frmLibros.Text =
                        "No se encontró ningún libro registrado con ese ID.";
                    return;
                }

                // Actividad 4.5: se conserva el registro original antes de editarlo.
                _libroOriginal = libroEncontrado;

                txtId.Text = libroEncontrado.Id.ToString();
                txb_Titulo_frmLibros.Text = libroEncontrado.Titulo;

                txb_Costo_frmLibros.Text =
                    libroEncontrado.PrecioRentaDiaria.ToString();

                txb_RutaIma_frmLibros.Text = libroEncontrado.RutaImagen;

                cmb_Genero_frmLibros.SelectedValue =
                    libroEncontrado.IdGenero;

                cmb_Estado_frmLibros.SelectedItem =
                    libroEncontrado.EsActivo ? "Activo" : "Inactivo";

                string valorNovedad =
                    libroEncontrado.EsNovedad ? "Sí" : "No";

                int indiceNovedad =
                    cmb_Novedad_frmLibros.FindStringExact(valorNovedad);

                if (indiceNovedad == -1 && valorNovedad == "Sí")
                {
                    indiceNovedad =
                        cmb_Novedad_frmLibros.FindStringExact("Si");
                }

                cmb_Novedad_frmLibros.SelectedIndex = indiceNovedad;

                cmb_Tipo_frmLibros.SelectedItem = "Físico";

                // Recuperar la cantidad de copias desde MariaDB.
                txb_Copias_frmLibros.Text =
                    libroEncontrado.CantidadCopias.ToString();

                txb_Archivo_frmLibros.Clear();
                _rutaOrigenArchivo = string.Empty;

                ActualizarCamposSegunTipo();
                CargarAutoresDisponibles();

                // Validación visual (Actividad 4.5): el ComboBox debe mostrar el NOMBRE del
                // género del libro mientras SelectedValue conserva su Id.
                bool generoCoincide =
                    ObtenerGeneroSeleccionado(out int idCargado)
                    && idCargado == libroEncontrado.IdGenero;

                if (!generoCoincide)
                {
                    errorProvider1.SetError(
                        cmb_Genero_frmLibros,
                        "El género de este libro no está en la lista (puede estar inactivo). Seleccione otro.");

                    txb_Info_frmLibros.Text =
                        "Libro encontrado, pero su género no está activo. Seleccione otro género antes de actualizar.";
                    return;
                }

                string avisoImagen = string.Empty;

                if (!string.IsNullOrWhiteSpace(libroEncontrado.RutaImagen)
                    && !File.Exists(Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        libroEncontrado.RutaImagen)))
                {
                    avisoImagen =
                        " La imagen registrada no se encontró en el disco.";
                }

                txb_Info_frmLibros.Text =
                    "Libro encontrado correctamente. Se cargaron sus datos, su género y su imagen; " +
                    "edite lo necesario y pulse Actualizar." + avisoImagen;
            }
            catch (Exception ex)
            {
                alerta.SetError(txtId, "Error de búsqueda: " + ex.Message);
            }
        }

        // ================== ACTUALIZAR ==================

        public async void EjecutarActualizar()
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.5 (Edición y Actualización)
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            if (!FormularioValido())
                return;

            if (!ObtenerId(out int id))
                return;

            if (!RecursoValido())
                return;

            if (!ObtenerGeneroSeleccionado(out int idGenero))
            {
                txb_Info_frmLibros.Text =
                    "Seleccione una categoría válida.";
                return;
            }

            try
            {
                bool esDigital =
                    cmb_Tipo_frmLibros.SelectedItem?.ToString() == "Digital";

                string? archivoDigital = string.Empty;

                if (esDigital)
                {
                    archivoDigital = GuardarArchivoDigital();

                    if (archivoDigital == null)
                        return;
                }

                string rutaImagen =
                    ObtenerRutaImagen(txb_RutaIma_frmLibros);

                if (!decimal.TryParse(
                    txb_Costo_frmLibros.Text.Trim(), out decimal costo)
                    || costo <= 0)
                {
                    txb_Info_frmLibros.Text =
                        "Ingrese un costo válido mayor que cero.";
                    return;
                }

                bool esNovedad = ObtenerNovedadSeleccionada();
                bool estado = ObtenerEstadoSeleccionado();

                int copias = esDigital
                    ? 0
                    : int.Parse(txb_Copias_frmLibros.Text.Trim());

                string titulo = txb_Titulo_frmLibros.Text.Trim();

                LibroDocumento documento = new LibroDocumento
                {
                    Id = id,
                    Titulo = titulo,
                    PrecioRentaDiaria = costo,
                    IdGenero = idGenero,
                    RutaImagen = rutaImagen,
                    EsActivo = estado,
                    EsNovedad = esNovedad,
                    CantidadCopias = copias
                };

                // Registro tal como se cargó (solo si corresponde al mismo ID).
                LibroListado? original =
                    (_libroOriginal != null && _libroOriginal.Id == id)
                        ? _libroOriginal
                        : null;

                // Primero se comprueba y actualiza MariaDB.
                if (!_listadoController.ActualizarLibroMariaDB(documento))
                {
                    txb_Info_frmLibros.Text =
                        "No se encontró el libro o no se pudieron actualizar los datos en MariaDB.";
                    return;
                }

                // Después se actualiza el documento en MongoDB.
                bool actualizadoMongo =
                    await _listadoController.ActualizarLibroMongoDBAsync(documento);

                if (!actualizadoMongo)
                {
                    // Compensación: se restauran en MariaDB los datos originales.
                    bool restaurado = original != null
                        && _listadoController.ActualizarLibroMariaDB(
                            DocumentoDesdeOriginal(original));

                    txb_Info_frmLibros.Text = restaurado
                        ? "MongoDB no pudo actualizar el registro; se restauraron los datos originales en MariaDB para que ambos motores coincidan."
                        : "MariaDB se actualizó, pero MongoDB no. Revise ambos registros para sincronizarlos.";
                    return;
                }

                // Actualizar o crear el objeto en memoria.
                Libros? libro =
                    Libros.ObtenerTodos().Find(l => l.Id == id);

                if (libro == null)
                {
                    libro = new Libros(
                        id,
                        titulo,
                        costo,
                        copias,
                        esNovedad,
                        rutaImagen,
                        estado,
                        esDigital ? TipoRecurso.Digital : TipoRecurso.Fisico,
                        esDigital ? archivoDigital! : string.Empty);

                    libro.InsertarRegistro(libro);
                }
                else
                {
                    libro.Titulo = titulo;
                    libro.PrecioRentaDiaria = costo;
                    libro.EsNovedad = esNovedad;
                    libro.RutaImagen = rutaImagen;
                    libro.EsActivo = estado;

                    libro.TipoRecurso =
                        esDigital ? TipoRecurso.Digital : TipoRecurso.Fisico;

                    libro.RutaArchivoDigital =
                        esDigital ? archivoDigital! : string.Empty;

                    libro.AjustarEjemplares(copias);
                }

                VincularAutoresSeleccionados(id);

                // Sincronía con el disco: la imagen anterior ya no la usa ningún motor.
                string avisoImagenAnterior =
                    EliminarImagenAnterior(original?.RutaImagen, rutaImagen);

                txb_Info_frmLibros.Text =
                    "Libro actualizado correctamente en MariaDB y MongoDB."
                    + avisoImagenAnterior;

                LimpiarCampos();
            }
            catch (Exception ex)
            {
                txb_Info_frmLibros.Text =
                    "Error al actualizar: " + ex.Message;
            }
        }

        // ================== ACTIVIDAD 4.5: EDICIÓN DESDE EL LISTADO ==================

        // Recibe el ID del libro elegido en FrmListado (doble clic o botón "Editar seleccionado"),
        // consulta el registro por ID en MariaDB y precarga el formulario: textos, número,
        // ComboBox de género (SelectedValue), estado, novedad y la imagen actual en el PictureBox.
        public void CargarLibroParaEdicion(int id)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.5 (Edición y Actualización)
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            txtId.Text = id.ToString();
            EjecutarBuscar(id.ToString(), errorProvider1);
        }

        private static LibroDocumento DocumentoDesdeOriginal(LibroListado original)
        {
            return new LibroDocumento
            {
                Id = original.Id,
                Titulo = original.Titulo,
                PrecioRentaDiaria = original.PrecioRentaDiaria,
                IdGenero = original.IdGenero,
                RutaImagen = original.RutaImagen,
                EsActivo = original.EsActivo,
                EsNovedad = original.EsNovedad,
                CantidadCopias = original.CantidadCopias
            };
        }

        // Si el usuario cambió la imagen y ambos motores ya guardaron la ruta nueva,
        // se borra del disco el archivo anterior para no dejar archivos huérfanos.
        // Solo se tocan archivos de la carpeta de imágenes de la aplicación.
        private static string EliminarImagenAnterior(
            string? rutaAnterior, string rutaNueva)
        {
            if (string.IsNullOrWhiteSpace(rutaAnterior)
                || rutaAnterior == rutaNueva)
            {
                return string.Empty;
            }

            string prefijo =
                ListadoController.CarpetaImagenes + Path.DirectorySeparatorChar;

            if (!rutaAnterior.StartsWith(
                prefijo, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            try
            {
                string rutaFisica = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, rutaAnterior);

                if (File.Exists(rutaFisica))
                    File.Delete(rutaFisica);

                return string.Empty;
            }
            catch (Exception ex) when (
                ex is IOException || ex is UnauthorizedAccessException)
            {
                return Environment.NewLine +
                    "No se pudo borrar la imagen anterior: " + ex.Message;
            }
        }

        // ================== ELIMINAR ==================

        public void EjecutarEliminar(StatusStrip barraEstado)
        {
            try
            {
                string idAEliminar = txtId.Text.Trim();

                if (string.IsNullOrWhiteSpace(idAEliminar) ||
                    !int.TryParse(idAEliminar, out int id))
                {
                    txb_Info_frmLibros.Text =
                        "Debe especificar un ID numérico válido.";
                    return;
                }

                DialogResult respuesta = MessageBox.Show(
                    $"¿Está seguro de eliminar el libro con ID {idAEliminar}?",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes)
                    return;

                Libros libroAuxiliar = new Libros();
                libroAuxiliar.EliminarRegistro(idAEliminar);

                if (barraEstado != null && barraEstado.Items.Count > 0)
                {
                    barraEstado.Items[0].Text =
                        $"Libro con ID [{idAEliminar}] fue eliminado correctamente.";
                }

                LimpiarCampos();

                txb_Info_frmLibros.Text =
                    "El registro ha sido eliminado.";
            }
            catch (Exception ex)
            {
                txb_Info_frmLibros.Text =
                    "Error al eliminar: " + ex.Message;
            }
        }

        // ================== LIMPIAR ==================

        private void LimpiarCampos()
        {
            errorProvider1.Clear();
            _libroOriginal = null;

            txtId.Clear();
            txb_Titulo_frmLibros.Clear();
            txb_Costo_frmLibros.Clear();
            txb_RutaIma_frmLibros.Clear();
            txb_Copias_frmLibros.Clear();
            txb_Archivo_frmLibros.Clear();

            _rutaOrigenArchivo = string.Empty;

            cmb_Tipo_frmLibros.SelectedIndex = 0;

            CargarAutoresDisponibles();

            if (cmb_Novedad_frmLibros.Items.Count > 0)
                cmb_Novedad_frmLibros.SelectedIndex = 0;

            if (cmb_Estado_frmLibros.Items.Count > 0)
                cmb_Estado_frmLibros.SelectedIndex = 0;
        }
    }
}
