
/*
 * ENCABEZADO DE AUTORÍA
 * Equipo N°: 3
 * Integrantes:
 * 1. CONTRERAS Rodriguez Janis Isabel
 * 2. TORRES Barrera Jose Angel
 * 3. MACÍAS Cruz Meredith Miranda
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Biblioteca
{
    // Actividad 3.1: FrmBase concentra los manejadores de eventos reutilizables.
    // Los formularios heredados solo los conectan a sus controles (DRY).
    public partial class FrmBase : Form
    {
        public FrmBase()
        {
            InitializeComponent();

            // El ID es común a todos los formularios heredados.
            // Solo acepta números, pero no obliga a escribirlo al cambiar de control.
            txtId.ShortcutsEnabled = false;
            txtId.KeyPress += SoloNumeros_KeyPress;
            txtId.Validating -= TextoObligatorio_Validating;
        }

        // ================== KEYPRESS (filtros de teclado) ==================

        // Solo letras: acepta letras (con acentos y ñ), espacio y teclas de control.
        protected void SoloLetras_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }
        }

        // Solo números enteros: acepta dígitos y teclas de control.
        protected void SoloNumeros_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }
        }

        // Números con precisión: dígitos, teclas de control y un solo punto decimal.
        protected void Decimal_KeyPress(object? sender, KeyPressEventArgs e)
        {
            TextBox txt = (TextBox)sender!;

            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
                return;

            if (e.KeyChar == '.' && (!txt.Text.Contains('.') || txt.SelectedText.Contains('.')))
                return;

            e.Handled = true;
            System.Media.SystemSounds.Beep.Play();
        }

        // Campos sin espacios.
        protected void SinEspacios_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }
        }

        // ================== VALIDATING ==================

        // TextBox obligatorio.
        // Muestra el error, pero permite abandonar el control.
        protected void TextoObligatorio_Validating(object? sender, CancelEventArgs e)
        {
            TextBox txt = (TextBox)sender!;

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                errorProvider1.SetError(txt, "Este campo es obligatorio.");
            }
            else
            {
                errorProvider1.SetError(txt, "");
            }
        }

        // ComboBox obligatorio.
        // Muestra el error, pero permite abandonar el control.
        protected void ComboObligatorio_Validating(object? sender, CancelEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender!;

            if (cmb.SelectedIndex == -1)
            {
                errorProvider1.SetError(cmb, "Seleccione una opción de la lista.");
            }
            else
            {
                errorProvider1.SetError(cmb, "");
            }
        }

        // Decimal obligatorio: no vacío, con formato numérico válido y mayor que cero.
        protected void DecimalObligatorio_Validating(object? sender, CancelEventArgs e)
        {
            TextBox txt = (TextBox)sender!;
            string mensaje = "";

            if (string.IsNullOrWhiteSpace(txt.Text))
                mensaje = "Este campo es obligatorio.";
            else if (!decimal.TryParse(txt.Text.Trim(), out decimal valor))
                mensaje = "Ingrese un número válido (ejemplo: 150.50).";
            else if (valor <= 0)
                mensaje = "El valor debe ser mayor que cero.";

            errorProvider1.SetError(txt, mensaje);
        }

        // Correo obligatorio: no vacío, con "@" y un punto después de la arroba.
        protected void Correo_Validating(object? sender, CancelEventArgs e)
        {
            TextBox txt = (TextBox)sender!;
            string correo = txt.Text.Trim();
            string mensaje = "";

            if (correo == "")
            {
                mensaje = "Este campo es obligatorio.";
            }
            else
            {
                int arroba = correo.IndexOf('@');

                if (arroba <= 0 ||
                    correo.IndexOf('.', arroba) <= arroba + 1 ||
                    correo.EndsWith("."))
                {
                    mensaje = "Ingrese un correo válido (ejemplo: nombre@dominio.com).";
                }
            }

            errorProvider1.SetError(txt, mensaje);
        }

        // NumericUpDown con monto obligatorio mayor que cero.
        protected void MontoPositivo_Validating(object? sender, CancelEventArgs e)
        {
            NumericUpDown nup = (NumericUpDown)sender!;

            if (nup.Value <= 0)
            {
                errorProvider1.SetError(nup, "El monto debe ser mayor que cero.");
            }
            else
            {
                errorProvider1.SetError(nup, "");
            }
        }

        // ================== CONEXIÓN AUTOMÁTICA PARA TODAS LAS PESTAÑAS ==================

        private static readonly string[] _letras =
        {
            "nombre", "apellido", "pais", "nacionalidad", "motivo", "descripcion"
        };

        private static readonly string[] _numeros =
        {
            "edad", "prestados", "copias", "cantidad", "anio", "telefono", "dias"
        };

        private static readonly string[] _decimales =
        {
            "costo", "precio", "monto", "salario", "tarifa"
        };

        private static readonly string[] _correos =
        {
            "correo", "email", "mail"
        };

        private static readonly HashSet<string> _excluidos =
            new HashSet<string>
            {
                "cmb_Libros_frmEdi",
                "txtBuscarDetalle"
            };

        private bool _validacionesConectadas = false;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!_validacionesConectadas)
            {
                _validacionesConectadas = true;
                ConectarValidacionesAutomaticas(this);
                ConectarDialogoImagen(this);
            }

            AjustarParaMdi();
        }

        // ================== ACTIVIDAD 3.5: FORMULARIO COMO VENTANA HIJA MDI ==================

        private void AjustarParaMdi()
        {
            if (MdiParent == null)
                return;

            int alto = 0;

            foreach (Control c in pnlFormularioBase.Controls)
                alto = Math.Max(alto, c.Bottom);

            int ancho = pnlFormularioBase.Width;

            pnlFormularioBase.Dock = DockStyle.None;
            pnlFormularioBase.Location = new System.Drawing.Point(0, 0);
            pnlFormularioBase.Size = new System.Drawing.Size(ancho, alto + 10);

            AutoScroll = true;
        }

        private static bool Contiene(string nombre, string[] claves)
        {
            foreach (string clave in claves)
            {
                if (nombre.Contains(clave))
                    return true;
            }

            return false;
        }

        private void ConectarValidacionesAutomaticas(Control contenedor)
        {
            foreach (Control c in contenedor.Controls)
            {
                if (c.HasChildren)
                    ConectarValidacionesAutomaticas(c);

                // El ID no se valida automáticamente.
                if (c == txtId ||
                    (c.Tag as string) == "sinvalidar" ||
                    _excluidos.Contains(c.Name))
                {
                    continue;
                }

                string n = c.Name.ToLowerInvariant();

                if (c is TextBox txt)
                {
                    // Cajas de solo lectura e información no se validan.
                    if (txt.ReadOnly ||
                        n.Contains("info") ||
                        n.Contains("rutaima") ||
                        n.Contains("buscar"))
                    {
                        continue;
                    }

                    // ----- KeyPress -----

                    txt.KeyPress -= SoloLetras_KeyPress;
                    txt.KeyPress -= SoloNumeros_KeyPress;
                    txt.KeyPress -= Decimal_KeyPress;
                    txt.KeyPress -= SinEspacios_KeyPress;

                    bool esCorreo = Contiene(n, _correos);
                    bool esDecimal = Contiene(n, _decimales);

                    if (esCorreo)
                    {
                        txt.KeyPress += SinEspacios_KeyPress;
                    }
                    else if (esDecimal)
                    {
                        txt.KeyPress += Decimal_KeyPress;
                        txt.ShortcutsEnabled = false;
                    }
                    else if (Contiene(n, _numeros))
                    {
                        txt.KeyPress += SoloNumeros_KeyPress;
                        txt.ShortcutsEnabled = false;
                    }
                    else if (Contiene(n, _letras))
                    {
                        txt.KeyPress += SoloLetras_KeyPress;
                        txt.ShortcutsEnabled = false;
                    }

                    // ----- Validating -----

                    txt.Validating -= TextoObligatorio_Validating;
                    txt.Validating -= DecimalObligatorio_Validating;
                    txt.Validating -= Correo_Validating;

                    if (esCorreo)
                        txt.Validating += Correo_Validating;
                    else if (esDecimal)
                        txt.Validating += DecimalObligatorio_Validating;
                    else
                        txt.Validating += TextoObligatorio_Validating;
                }
                else if (c is ComboBox cmb)
                {
                    cmb.Validating -= ComboObligatorio_Validating;
                    cmb.Validating += ComboObligatorio_Validating;
                }
                else if (c is NumericUpDown nup && n.Contains("mon"))
                {
                    nup.Validating -= MontoPositivo_Validating;
                    nup.Validating += MontoPositivo_Validating;
                }
            }
        }

        // ================== ACTIVIDAD 3.3: DIÁLOGO DE ARCHIVO ==================

        private PictureBox? pbxFotografia;
        private Button? btnCargarFoto;
        private OpenFileDialog? ofdImagen;

        private const string CarpetaImagenes = "ImagenesDatos";

        private string _rutaOrigenImagen = string.Empty;
        private bool _asignandoRuta = false;

        private void ConectarDialogoImagen(Control contenedor)
        {
            Button? boton = null;
            TextBox? ruta = null;

            BuscarControlesImagen(contenedor, ref boton, ref ruta);

            if (ruta == null)
                return;

            pbxFotografia = BuscarControl<PictureBox>(contenedor, "pbxFotografia");
            btnCargarFoto = BuscarControl<Button>(contenedor, "btnCargarFoto");
            ofdImagen = BuscarDialogoImagen();

            if (pbxFotografia != null)
            {
                pbxFotografia.SizeMode = PictureBoxSizeMode.StretchImage;
                pbxFotografia.BringToFront();
            }

            if (btnCargarFoto != null)
            {
                btnCargarFoto.Tag = ruta;
                btnCargarFoto.Click -= BotonImagen_Click;
                btnCargarFoto.Click += BotonImagen_Click;
                btnCargarFoto.BringToFront();
            }

            if (boton != null)
            {
                boton.Tag = ruta;
                boton.Click -= BotonImagen_Click;
                boton.Click += BotonImagen_Click;

                new ToolTip().SetToolTip(boton, "Cargar Imagen");
            }

            ruta.TextChanged -= RutaImagen_TextChanged;
            ruta.TextChanged += RutaImagen_TextChanged;
        }

        private static T? BuscarControl<T>(Control contenedor, string nombre)
            where T : Control
        {
            foreach (Control c in contenedor.Controls)
            {
                if (c is T encontrado &&
                    c.Name.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                {
                    return encontrado;
                }

                if (c.HasChildren)
                {
                    T? interno = BuscarControl<T>(c, nombre);

                    if (interno != null)
                        return interno;
                }
            }

            return null;
        }

        private OpenFileDialog BuscarDialogoImagen()
        {
            foreach (FieldInfo campo in GetType().GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic))
            {
                if (campo.FieldType == typeof(OpenFileDialog) &&
                    campo.GetValue(this) is OpenFileDialog dialogo)
                {
                    return dialogo;
                }
            }

            return new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Archivos de imagen (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Seleccionar imagen"
            };
        }

        private void BuscarControlesImagen(
            Control contenedor,
            ref Button? boton,
            ref TextBox? ruta)
        {
            foreach (Control c in contenedor.Controls)
            {
                string n = c.Name.ToLowerInvariant();

                if (c is Button b && n.Contains("imagen"))
                    boton = b;
                else if (c is TextBox t && n.Contains("rutaima"))
                    ruta = t;

                if (c.HasChildren)
                    BuscarControlesImagen(c, ref boton, ref ruta);
            }
        }

        private void BotonImagen_Click(object? sender, EventArgs e)
        {
            if (sender is not Button boton ||
                boton.Tag is not TextBox ruta ||
                ofdImagen == null)
            {
                return;
            }

            ofdImagen.Filter =
                "Archivos de imagen (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            ofdImagen.InitialDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (ofdImagen.ShowDialog() != DialogResult.OK)
                return;

            string rutaOrigen = ofdImagen.FileName;

            try
            {
                MostrarVistaPrevia(rutaOrigen);
                _rutaOrigenImagen = rutaOrigen;

                _asignandoRuta = true;
                ruta.Text = Path.GetFileName(rutaOrigen);
                _asignandoRuta = false;

                errorProvider1.SetError(ruta, "");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(
                    "El archivo de imagen ya no existe en la ruta seleccionada.",
                    "Archivo no encontrado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "No tiene permisos para leer el archivo seleccionado.",
                    "Acceso denegado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show(
                    "El archivo está bloqueado o no se pudo leer.\n\n" + ex.Message,
                    "Error de entrada/salida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show(
                    "El archivo seleccionado no es una imagen válida.",
                    "Formato no válido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                _asignandoRuta = false;
            }
        }

        private void MostrarVistaPrevia(string rutaOrigen)
        {
            if (pbxFotografia == null)
                return;

            using (Image original = Image.FromFile(rutaOrigen))
            {
                Image copia = new Bitmap(original);

                pbxFotografia.SizeMode = PictureBoxSizeMode.StretchImage;

                LimpiarVistaPrevia();

                pbxFotografia.Image = copia;
            }
        }

        private void LimpiarVistaPrevia()
        {
            if (pbxFotografia == null)
                return;

            Image? anterior = pbxFotografia.Image;

            pbxFotografia.Image = null;

            anterior?.Dispose();
        }

        private void RutaImagen_TextChanged(object? sender, EventArgs e)
        {
            if (_asignandoRuta || sender is not TextBox ruta)
                return;

            _rutaOrigenImagen = string.Empty;

            LimpiarVistaPrevia();

            string rutaRelativa = ruta.Text.Trim();

            if (rutaRelativa.Length == 0)
                return;

            try
            {
                string rutaCompleta = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    rutaRelativa);

                if (File.Exists(rutaCompleta))
                    MostrarVistaPrevia(rutaCompleta);
            }
            catch (Exception ex) when (
                ex is IOException ||
                ex is UnauthorizedAccessException ||
                ex is OutOfMemoryException ||
                ex is ArgumentException)
            {
                LimpiarVistaPrevia();
            }
        }

        protected string ObtenerRutaImagen(TextBox ruta)
        {
            if (_rutaOrigenImagen.Length == 0)
                return ruta.Text.Trim();

            string? rutaRelativa = CopiarAlProyecto(
                _rutaOrigenImagen,
                CarpetaImagenes);

            if (rutaRelativa == null)
            {
                throw new OperationCanceledException(
                    "No se pudo copiar la imagen; el registro no se guardó.");
            }

            _rutaOrigenImagen = string.Empty;

            _asignandoRuta = true;
            ruta.Text = rutaRelativa;
            _asignandoRuta = false;

            return rutaRelativa;
        }

        protected string? CopiarAlProyecto(string rutaOrigen, string carpeta)
        {
            try
            {
                string rutaBase = AppDomain.CurrentDomain.BaseDirectory;
                string carpetaDestino = Path.Combine(rutaBase, carpeta);

                if (!Directory.Exists(carpetaDestino))
                    Directory.CreateDirectory(carpetaDestino);

                string nombreUnico =
                    Guid.NewGuid().ToString() + Path.GetExtension(rutaOrigen);

                string rutaDestino = Path.Combine(
                    carpetaDestino,
                    nombreUnico);

                File.Copy(rutaOrigen, rutaDestino, true);

                return Path.Combine(carpeta, nombreUnico);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(
                    "El archivo seleccionado ya no existe. Vuelva a cargarlo.",
                    "Archivo no encontrado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "No tiene permisos para escribir en la carpeta \"" +
                    carpeta + "\" o el archivo es de solo lectura.",
                    "Acceso denegado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show(
                    "No se pudo copiar el archivo (disco lleno, archivo bloqueado u otro error).\n\n" +
                    ex.Message,
                    "Error de entrada/salida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            return null;
        }

        // ================== MENÚS DE SELECCIÓN ==================

        protected static void CargarSelector(
            ComboBox combo,
            IEnumerable<string> opciones)
        {
            List<string> nuevas = new List<string>(opciones);

            bool igual = combo.Items.Count == nuevas.Count;

            for (int i = 0; igual && i < nuevas.Count; i++)
                igual = combo.Items[i]?.ToString() == nuevas[i];

            if (igual)
                return;

            string elegido = combo.Text;

            combo.BeginUpdate();
            combo.Items.Clear();

            foreach (string o in nuevas)
                combo.Items.Add(o);

            combo.EndUpdate();

            combo.SelectedIndex =
                elegido.Length > 0 ? combo.FindStringExact(elegido) : -1;
        }

        protected static int? IdDeSelector(ComboBox combo)
        {
            string texto = combo.Text.Trim();

            int guion = texto.IndexOf(" - ");

            string numero = guion > 0
                ? texto.Substring(0, guion)
                : texto;

            return int.TryParse(numero, out int id)
                ? id
                : (int?)null;
        }

        protected static void SeleccionarPorId(ComboBox combo, int id)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i]?.ToString()?.StartsWith(id + " - ") == true)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }

            combo.SelectedIndex = -1;
        }

        // ================== APOYO PARA GUARDAR / ACTUALIZAR ==================

        // Valida el formulario y comprueba los errores activos.
        // Los controles pueden perder el foco, pero los errores siguen impidiendo guardar.
        protected bool FormularioValido()
        {
            this.ValidateChildren();

            bool hayErrores = HayErroresActivos(this);

            if (hayErrores)
                MostrarErrorValidacion();

            return !hayErrores;
        }

        protected void MostrarErrorValidacion()
        {
            TextBox? info = BuscarCuadroInfo(this);

            if (info != null)
            {
                info.Text =
                    "⚠️ Hay campos con errores.\r\n" +
                    "Corrija los campos marcados con el icono rojo antes de continuar.";
            }
        }

        private TextBox? BuscarCuadroInfo(Control contenedor)
        {
            foreach (Control c in contenedor.Controls)
            {
                if (c is TextBox t && t.Name.StartsWith("txb_Info"))
                    return t;

                TextBox? encontrado = BuscarCuadroInfo(c);

                if (encontrado != null)
                    return encontrado;
            }

            return null;
        }

        // Comprueba si existe algún error activo en el formulario.
        protected bool HayErroresActivos(Control contenedor)
        {
            foreach (Control control in contenedor.Controls)
            {
                if (errorProvider1.GetError(control) != "")
                    return true;

                if (control.HasChildren && HayErroresActivos(control))
                    return true;
            }

            return false;
        }
    }
}
