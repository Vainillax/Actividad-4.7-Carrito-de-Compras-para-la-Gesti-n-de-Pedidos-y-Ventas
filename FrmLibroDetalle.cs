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
    // Actividad 4.6: ventana MODAL de solo lectura, basada en el formulario de captura de Libros.
    // Muestra el registro completo (incluida su imagen) como paso previo obligatorio antes de borrar.
    // Todos los controles (TextBox con ReadOnly = true, PictureBox, Label de autoría y botones)
    // están creados en el Diseñador (FrmLibroDetalle.Designer.cs); aquí solo está la lógica.
    // DialogResult.OK = el usuario decidió continuar; Cancel = canceló o solo estaba consultando.
    public partial class FrmLibroDetalle : Form
    {
        public const string Consulta = "Consulta";
        public const string BorradoLogico = "Borrado lógico";
        public const string BorradoFisico = "Borrado físico";

        public FrmLibroDetalle()
        {
            InitializeComponent();
        }

        // Puebla la vista con el objeto completo recuperado por el controlador.
        public void Cargar(LibroListado libro, string accion)
        {
            Text = $"Detalle del libro - {accion} (solo lectura)";

            txtId.Text = libro.Id.ToString();
            txtTitulo.Text = libro.Titulo;
            txtCategoria.Text = libro.Categoria;
            txtPrecio.Text = libro.PrecioRentaDiaria.ToString("C2");
            txtEstado.Text = libro.EsActivo ? "Activo" : "Inactivo";
            txtNovedad.Text = libro.EsNovedad ? "Sí" : "No";
            txtCopias.Text = libro.CantidadCopias.ToString();
            txtRuta.Text = libro.RutaImagen;

            // Bloqueo de edición: ningún dato puede modificarse en esta ventana.
            foreach (Control control in Controls)
            {
                if (control is TextBox cuadro)
                    cuadro.ReadOnly = true;
            }

            switch (accion)
            {
                case BorradoLogico:
                    lblAviso.Text = "Revise el registro. Al continuar se marcará como INACTIVO " +
                        "(es_activo = 0 / EsActivo = false): dejará de verse en el listado, pero seguirá guardado.";
                    break;

                case BorradoFisico:
                    lblAviso.Text = "Revise el registro. Al continuar se ELIMINARÁ DE FORMA PERMANENTE " +
                        "de MariaDB y MongoDB, y su imagen se borrará del disco.";
                    break;

                default:
                    lblAviso.Text = "Vista de solo lectura: los datos no pueden modificarse.";
                    btnContinuar.Visible = false;
                    btnCancelar.Text = "Cerrar";
                    break;
            }

            MostrarImagen(libro);
        }

        // Misma técnica del listado: se lee el archivo con un FileStream dentro de "using" y se copia
        // a un Bitmap en memoria, así el archivo no queda bloqueado y se puede borrar después.
        private void MostrarImagen(LibroListado libro)
        {
            LiberarImagen();

            if (string.IsNullOrWhiteSpace(libro.RutaImagen))
            {
                lblImagen.Text = "Este libro no tiene imagen asignada.";
                return;
            }

            string rutaFisica = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, libro.RutaImagen);

            if (!File.Exists(rutaFisica))
            {
                lblImagen.Text = "Imagen no encontrada en el disco.";
                return;
            }

            try
            {
                using (FileStream flujo = new FileStream(rutaFisica, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (Image original = Image.FromStream(flujo))
                {
                    pbxPortada.Image = new Bitmap(original);
                }
                lblImagen.Text = "Portada del libro";
            }
            catch (ArgumentException)
            {
                lblImagen.Text = "El archivo no es una imagen válida.";
            }
            catch (IOException ex)
            {
                lblImagen.Text = "No se pudo leer la imagen: " + ex.Message;
            }
        }

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
