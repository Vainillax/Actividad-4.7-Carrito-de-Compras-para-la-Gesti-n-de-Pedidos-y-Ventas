/*
 * ENCABEZADO DE AUTORÍA
 * Equipo N°: 3
 * Integrantes (orden alfabético por primer apellido):
 * 1. CONTRERAS Rodríguez Janis Isabel
 * 2. MACÍAS Cruz Meredith Miranda
 * 3. TORRES Barrera José Ángel
 */
using System.Diagnostics;
using System.Text.Json;
using Biblioteca.Controllers;
using Biblioteca.Models;

namespace Biblioteca
{
    // Actividad 4.2: módulo de respaldo. Exporta e importa las colecciones del sistema en JSON y XML.
    // Todos los controles (botones, lista desplegable, tabla, cuadro de diálogo) están creados
    // en el Diseñador (FrmRespaldo.Designer.cs); aquí solo está la lógica.
    public partial class FrmRespaldo : Form
    {
        // Posición de cada colección en la lista desplegable cmbEntidad
        private const int IndiceAutores = 0;
        private const int IndiceGeneros = 1;

        public FrmRespaldo()
        {
            InitializeComponent();

            cmbEntidad.SelectedIndex = IndiceAutores;   // dispara cmbEntidad_SelectedIndexChanged
            Escribir("Carpeta de respaldos: " + BackupController.RutaCarpetaRespaldos);
        }

        // La fecha y hora del sistema debe verse en las capturas de evidencia.
        private void Escribir(string texto)
        {
            txtBitacora.AppendText($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {texto}{Environment.NewLine}");
        }

        // ================== VISTA: DataGridView vinculado a la colección en memoria ==================

        private void cmbEntidad_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ActualizarVista();
        }

        // Se vuelve a asignar DataSource (primero null) para que la tabla relea la lista completa.
        private void ActualizarVista()
        {
            dgvDatos.DataSource = null;

            switch (cmbEntidad.SelectedIndex)
            {
                case IndiceAutores:
                    dgvDatos.DataSource = Autores.ObtenerTodos();
                    lblTotal.Text = $"Registros en memoria: {Autores.ObtenerTodos().Count}";
                    break;

                case IndiceGeneros:
                    dgvDatos.DataSource = Genero.ObtenerTodos();
                    lblTotal.Text = $"Registros en memoria: {Genero.ObtenerTodos().Count}";
                    break;

                default:
                    lblTotal.Text = "Registros en memoria: 0";
                    break;
            }
        }

        // Las columnas se generan cuando el DataGridView termina de vincularse (puede ser después de asignar DataSource).
        // En Autores, Nombre y Codigo son alias de NombreCompleto e Id: se ocultan para no duplicar columnas.
        private void dgvDatos_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (cmbEntidad.SelectedIndex == IndiceAutores)
            {
                OcultarColumna("Nombre");
                OcultarColumna("Codigo");
            }
        }

        private void OcultarColumna(string nombre)
        {
            if (dgvDatos.Columns[nombre] != null)
                dgvDatos.Columns[nombre]!.Visible = false;
        }

        // ================== EXPORTACIÓN (serialización) ==================

        private void btnExportarJson_Click(object? sender, EventArgs e)
        {
            Exportar(true);
        }

        private void btnExportarXml_Click(object? sender, EventArgs e)
        {
            Exportar(false);
        }

        private void Exportar(bool formatoJson)
        {
            string formato = formatoJson ? "JSON" : "XML";

            try
            {
                string? ruta = cmbEntidad.SelectedIndex switch
                {
                    IndiceAutores => ExportarLista(Autores.ObtenerTodos(), "Autores", formatoJson),
                    IndiceGeneros => ExportarLista(Genero.ObtenerTodos(), "Generos", formatoJson),
                    _ => null
                };

                if (ruta == null)
                    return;   // no había registros (ya se avisó al usuario)

                Escribir($"Exportación {formato} exitosa: {ruta}");
                MessageBox.Show($"La colección se exportó correctamente en formato {formato}.\n\nArchivo:\n{ruta}",
                    "Exportación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException)
            {
                MostrarError($"No tiene permisos para escribir en la carpeta de respaldos o el archivo es de solo lectura.", "Acceso denegado");
            }
            catch (IOException ex)
            {
                MostrarError($"No se pudo escribir el archivo (disco lleno, archivo bloqueado u otro error).\n\n{ex.Message}", "Error de entrada/salida");
            }
            catch (Exception ex) when (ex is JsonException || ex is InvalidOperationException || ex is NotSupportedException)
            {
                MostrarError($"No se pudo serializar la colección a {formato}.\n\n{DetalleDe(ex)}", "Error de serialización");
            }
        }

        private string? ExportarLista<T>(List<T> lista, string nombre, bool formatoJson) where T : EntidadBase
        {
            if (lista.Count == 0)
            {
                MessageBox.Show("No hay registros en memoria para exportar.\nRegistre datos en el módulo correspondiente o use el botón Cargar datos de ejemplo.",
                    "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            return formatoJson
                ? BackupController.ExportarJson(lista, nombre)
                : BackupController.ExportarXml(lista, nombre);
        }

        // ================== IMPORTACIÓN (deserialización) ==================

        private void btnImportarJson_Click(object? sender, EventArgs e)
        {
            Importar(true);
        }

        private void btnImportarXml_Click(object? sender, EventArgs e)
        {
            Importar(false);
        }

        private void Importar(bool formatoJson)
        {
            string formato = formatoJson ? "JSON" : "XML";

            // El cuadro de diálogo (ofdImportar, creado en el Diseñador) se abre en la carpeta de respaldos.
            try
            {
                ofdImportar.InitialDirectory = BackupController.AsegurarCarpeta();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // Si no se puede crear la carpeta, el cuadro se abre en la ubicación por defecto.
                ofdImportar.InitialDirectory = string.Empty;
            }

            ofdImportar.Filter = formatoJson
                ? "Archivos JSON (*.json)|*.json|Todos los archivos (*.*)|*.*"
                : "Archivos XML (*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            ofdImportar.Title = $"Importar respaldo {formato}";
            ofdImportar.FileName = string.Empty;

            if (ofdImportar.ShowDialog(this) != DialogResult.OK)
                return;

            string ruta = ofdImportar.FileName;

            // Todo el bloque de deserialización va dentro de try/catch: un archivo dañado nunca cierra la aplicación.
            try
            {
                int importados = cmbEntidad.SelectedIndex switch
                {
                    IndiceAutores => ImportarLista(Autores.ObtenerTodos(), "Autores", formatoJson, ruta),
                    IndiceGeneros => ImportarLista(Genero.ObtenerTodos(), "Géneros", formatoJson, ruta),
                    _ => -1
                };

                if (importados < 0)
                {
                    Escribir($"Importación {formato} cancelada por el usuario.");
                    return;
                }

                ActualizarVista();   // la tabla se vincula de inmediato a la colección reconstruida
                Escribir($"Importación {formato} exitosa: {importados} registro(s) desde {Path.GetFileName(ruta)}");
                MessageBox.Show($"Se importaron {importados} registro(s) desde:\n{ruta}",
                    "Importación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (JsonException ex)
            {
                MostrarError($"El archivo JSON está dañado o no cumple con el esquema esperado.\nNo se modificó ningún dato del sistema.\n\nDetalle: {ex.Message}", "Archivo JSON no válido");
            }
            catch (InvalidOperationException ex)
            {
                // XmlSerializer envuelve cualquier error del XML en InvalidOperationException
                MostrarError($"El archivo no cumple con la estructura esperada ({formato}).\nNo se modificó ningún dato del sistema.\n\nDetalle: {DetalleDe(ex)}", "Archivo no válido");
            }
            catch (ArgumentException ex)
            {
                // El modelo rechazó un valor del archivo (por ejemplo ID menor a 1 o edad fuera de rango)
                MostrarError($"El archivo contiene datos que el sistema no acepta.\nNo se modificó ningún dato del sistema.\n\nDetalle: {ex.Message}", "Datos no válidos");
            }
            catch (FileNotFoundException)
            {
                MostrarError("El archivo seleccionado ya no existe.", "Archivo no encontrado");
            }
            catch (UnauthorizedAccessException)
            {
                MostrarError("No tiene permisos para leer el archivo seleccionado.", "Acceso denegado");
            }
            catch (IOException ex)
            {
                MostrarError($"No se pudo leer el archivo (está en uso u ocurrió otro error).\n\n{ex.Message}", "Error de entrada/salida");
            }
        }

        // Devuelve el número de registros importados, o -1 si el usuario canceló.
        // La colección activa solo se reemplaza cuando el archivo se leyó completo y sin errores.
        private int ImportarLista<T>(List<T> destino, string entidad, bool formatoJson, string ruta) where T : EntidadBase
        {
            List<T> nuevos = formatoJson
                ? BackupController.ImportarJson<T>(ruta)
                : BackupController.ImportarXml<T>(ruta);

            if (destino.Count > 0)
            {
                DialogResult respuesta = MessageBox.Show(
                    $"El archivo contiene {nuevos.Count} registro(s) de {entidad}.\n\nSe reemplazarán los {destino.Count} registro(s) que hay actualmente en memoria. ¿Desea continuar?",
                    "Confirmar importación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes)
                    return -1;
            }

            BackupController.Reemplazar(destino, nuevos);
            return nuevos.Count;
        }

        // ================== APOYO ==================

        // Registros de muestra para probar el módulo sin capturar datos a mano (solo agrega los IDs que no existen).
        private void btnDatosEjemplo_Click(object? sender, EventArgs e)
        {
            int agregados = 0;

            Autores[] autores =
            {
                new Autores(101, "Gabriel García Márquez", 87, "gabo@macondo.com", "Colombiana", "autor_default.png", true),
                new Autores(102, "Isabel Allende", 84, "isabel@casa.com", "Chilena", "autor_default.png", true),
                new Autores(103, "Julio Cortázar", 69, "julio@rayuela.com", "Argentina", "autor_default.png", false)
            };
            foreach (Autores a in autores)
            {
                if (!Autores.ObtenerTodos().Exists(x => x.Id == a.Id))
                {
                    Autores.ObtenerTodos().Add(a);
                    agregados++;
                }
            }

            Genero[] generos =
            {
                new Genero(201, "Ciencia ficción", "Futuros posibles y tecnología", "genero_default.png", true),
                new Genero(202, "Terror", "Libros de suspenso y miedo", "genero_default.png", true),
                new Genero(203, "Novela histórica", "Ficción ambientada en el pasado", "genero_default.png", true)
            };
            foreach (Genero g in generos)
            {
                if (!Genero.ObtenerTodos().Exists(x => x.Id == g.Id))
                {
                    Genero.ObtenerTodos().Add(g);
                    agregados++;
                }
            }

            ActualizarVista();
            Escribir($"Datos de ejemplo: {agregados} registro(s) agregado(s) (IDs 101-103 de Autores y 201-203 de Géneros).");
        }

        // Abre la carpeta de respaldos en el Explorador de archivos (útil para las capturas de evidencia).
        private void btnAbrirCarpeta_Click(object? sender, EventArgs e)
        {
            try
            {
                string carpeta = BackupController.AsegurarCarpeta();
                Process.Start(new ProcessStartInfo { FileName = carpeta, UseShellExecute = true });
                Escribir("Carpeta de respaldos abierta: " + carpeta);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.ComponentModel.Win32Exception)
            {
                MostrarError($"No se pudo abrir la carpeta de respaldos.\n\n{ex.Message}", "Error");
            }
        }

        private void MostrarError(string mensaje, string titulo)
        {
            Escribir($"ERROR - {titulo}");
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Mensaje más claro: XmlSerializer deja la causa real en InnerException.
        private static string DetalleDe(Exception ex)
        {
            return ex.InnerException != null ? $"{ex.Message} {ex.InnerException.Message}" : ex.Message;
        }
    }
}
