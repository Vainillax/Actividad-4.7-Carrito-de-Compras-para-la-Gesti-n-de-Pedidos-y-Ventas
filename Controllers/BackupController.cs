/*
 * ENCABEZADO DE AUTORÍA
 * Equipo N°: 3
 * Integrantes (orden alfabético por primer apellido):
 * 1. CONTRERAS Rodríguez Janis Isabel
 * 2. MACÍAS Cruz Meredith Miranda
 * 3. TORRES Barrera José Ángel
 */
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Biblioteca.Models;

namespace Biblioteca.Controllers
{
    // Actividad 4.2: controlador utilitario que transforma colecciones de objetos en archivos
    // JSON / XML (serialización) y los vuelve a convertir en objetos (deserialización).
    // No muestra ventanas: si algo falla lanza la excepción y el formulario la presenta al usuario.
    public static class BackupController
    {
        // Carpeta de respaldos, RELATIVA a la ruta binaria de la aplicación (portable, sin rutas absolutas fijas).
        public const string CarpetaRespaldos = "Backups";

        public static string RutaCarpetaRespaldos =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CarpetaRespaldos);

        // ----- Opciones de System.Text.Json -----
        // WriteIndented = true genera un archivo legible con sangrías.
        // UnsafeRelaxedJsonEscaping evita que á, é, ñ se escriban como \u00E1, \u00E9, \u00F1.
        // IgnoreCycles evita la excepción por referencias circulares al exportar.
        private static readonly JsonSerializerOptions OpcionesExportar = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        // Al importar, un archivo con propiedades que la clase no tiene (por ejemplo, un JSON de Géneros
        // cargado como Autores) se rechaza en lugar de crear objetos con datos por defecto.
        private static readonly JsonSerializerOptions OpcionesImportar = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };

        // Misma lógica de la Actividad 3.6 (CopiarAlProyecto): ruta base del entorno de ejecución,
        // se crea la carpeta si no existe y se trabaja siempre con rutas relativas.
        public static string AsegurarCarpeta()
        {
            string carpeta = RutaCarpetaRespaldos;
            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);
            return carpeta;
        }

        // Nombre con fecha y hora para no pisar respaldos anteriores: Autores_20261114_153012.json
        private static string NuevaRuta(string nombreBase, string extension)
        {
            string marca = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return Path.Combine(AsegurarCarpeta(), $"{nombreBase}_{marca}{extension}");
        }

        // ================== EXPORTACIÓN (objetos -> archivo) ==================

        // JsonSerializer.Serialize convierte la lista completa en texto JSON con sangrías.
        // Devuelve la ruta del archivo creado.
        public static string ExportarJson<T>(List<T> coleccion, string nombreBase) where T : EntidadBase
        {
            string json = JsonSerializer.Serialize(coleccion, OpcionesExportar);

            string ruta = NuevaRuta(nombreBase, ".json");
            File.WriteAllText(ruta, json, new UTF8Encoding(false));
            return ruta;
        }

        // XmlSerializer tipado con la colección + StreamWriter (flujo de escritura) que vuelca
        // la estructura jerárquica de etiquetas al disco. Devuelve la ruta del archivo creado.
        public static string ExportarXml<T>(List<T> coleccion, string nombreBase) where T : EntidadBase
        {
            XmlSerializer serializador = new XmlSerializer(typeof(List<T>));

            string ruta = NuevaRuta(nombreBase, ".xml");
            try
            {
                using (StreamWriter escritor = new StreamWriter(ruta, false, new UTF8Encoding(false)))
                {
                    serializador.Serialize(escritor, coleccion);
                }
            }
            catch
            {
                // Si falla a la mitad no se deja un archivo incompleto en la carpeta de respaldos.
                if (File.Exists(ruta))
                    File.Delete(ruta);
                throw;
            }
            return ruta;
        }

        // ================== IMPORTACIÓN (archivo -> objetos) ==================

        // File.ReadAllText lee el texto crudo y JsonSerializer.Deserialize<List<T>>() reconstruye los objetos.
        // Lanza JsonException si el archivo está dañado o no cumple el esquema.
        public static List<T> ImportarJson<T>(string ruta) where T : EntidadBase
        {
            string contenido = File.ReadAllText(ruta, Encoding.UTF8);
            List<T>? lista = JsonSerializer.Deserialize<List<T>>(contenido, OpcionesImportar);
            return ValidarColeccion(lista);
        }

        // XmlSerializer.Deserialize reconstruye la lista desde un flujo de lectura.
        // Lanza InvalidOperationException si el XML está dañado o no cumple el esquema.
        public static List<T> ImportarXml<T>(string ruta) where T : EntidadBase
        {
            XmlSerializer serializador = new XmlSerializer(typeof(List<T>));

            // Una etiqueta que la clase no conoce (XML de otra colección) se rechaza.
            serializador.UnknownElement += (sender, e) =>
                throw new InvalidOperationException($"La etiqueta <{e.Element.Name}> no pertenece al esquema esperado.");

            using (StreamReader lector = new StreamReader(ruta, Encoding.UTF8))
            {
                object? resultado = serializador.Deserialize(lector);
                return ValidarColeccion(resultado as List<T>);
            }
        }

        // Revisiones de consistencia antes de tocar la colección activa del sistema.
        private static List<T> ValidarColeccion<T>(List<T>? lista) where T : EntidadBase
        {
            if (lista == null)
                throw new InvalidOperationException("El archivo no contiene una colección de registros.");

            HashSet<int> idsVistos = new HashSet<int>();
            foreach (T elemento in lista)
            {
                if (elemento == null)
                    throw new InvalidOperationException("El archivo contiene un registro vacío.");

                if (!idsVistos.Add(elemento.Id))
                    throw new InvalidOperationException($"El ID {elemento.Id} aparece repetido en el archivo.");
            }
            return lista;
        }

        // Reemplaza el contenido de la colección activa. Solo se llama cuando la deserialización
        // terminó sin errores, así un archivo corrupto nunca daña los datos que ya están en memoria.
        public static void Reemplazar<T>(List<T> destino, List<T> nuevos)
        {
            destino.Clear();
            destino.AddRange(nuevos);
        }
    }
}
