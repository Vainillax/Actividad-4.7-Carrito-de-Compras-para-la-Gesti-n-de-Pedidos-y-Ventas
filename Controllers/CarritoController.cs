/*
 * ENCABEZADO DE AUTORÍA
 * Equipo N°: 3
 * Integrantes (orden alfabético por primer apellido):
 * 1. CONTRERAS Rodríguez Janis Isabel
 * 2. MACÍAS Cruz Meredith Miranda
 * 3. TORRES Barrera José Ángel
 */
using Biblioteca.Data;
using Biblioteca.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Biblioteca.Controllers
{
    // Actividad 4.7: lógica del Carrito de Compras.
    //  - Búsqueda por ID (exacta) y por nombre (LIKE en MariaDB / expresión regular en MongoDB).
    //  - Cálculo de subtotales y total (recorrido de la lista temporal).
    //  - Validación de existencias (stock = cantidad_copias) y validación final del pedido.
    // Solo se venden libros ACTIVOS (es_activo = 1 / EsActivo = true), así que los que tienen
    // borrado lógico (Actividad 4.6) no aparecen en las búsquedas.
    public class CarritoController
    {
        public const string MotorMariaDB = "MariaDB";
        public const string MotorMongoDB = "MongoDB";

        public record ResultadoValidacion(bool Valido, string Mensaje);

        // ================================ MariaDB: consultas ================================

        private const string ConsultaBase =
            "SELECT l.id, l.titulo, " +
            "COALESCE(g.nombre, 'Sin categoría') AS categoria, " +
            "l.precio_renta, l.ruta_imagen, l.es_novedad, " +
            "l.cantidad_copias " +
            "FROM libros AS l " +
            "LEFT JOIN generos AS g ON l.id_genero = g.id " +
            "WHERE l.es_activo = 1 ";

        public const string ConsultaPorId =
            ConsultaBase + "AND l.id = @id";

        public const string ConsultaPorNombre =
            ConsultaBase + "AND l.titulo LIKE @nombre ORDER BY l.titulo";

        // Los comodines % y _ que escriba el usuario se tratan como texto normal.
        private static string EscaparLike(string texto)
        {
            return texto
                .Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_");
        }

        private static List<LibroListado> LeerLibros(MySqlCommand comando)
        {
            List<LibroListado> resultado = new List<LibroListado>();

            using (MySqlDataReader lector = comando.ExecuteReader())
            {
                int colRuta = lector.GetOrdinal("ruta_imagen");

                while (lector.Read())
                {
                    resultado.Add(new LibroListado
                    {
                        Id = lector.GetInt32("id"),
                        Titulo = lector.GetString("titulo"),
                        Categoria = lector.GetString("categoria"),
                        PrecioRentaDiaria = lector.GetDecimal("precio_renta"),
                        RutaImagen = lector.IsDBNull(colRuta)
                            ? string.Empty
                            : lector.GetString("ruta_imagen"),
                        EsActivo = true,
                        EsNovedad = lector.GetBoolean("es_novedad"),
                        CantidadCopias = lector.GetInt32("cantidad_copias")
                    });
                }
            }

            return resultado;
        }

        // Búsqueda por ID en MariaDB. Devuelve una lista vacía si no existe (nunca null por "no encontrado");
        // devuelve null solo si hubo un error de conexión o de SQL.
        public List<LibroListado>? BuscarPorIdMariaDB(int id)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.7 (Carrito de Compras) - Búsqueda por ID
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            MySqlConnection? conexion = ConexionMariaDB.ObtenerConexion();
            if (conexion == null)
                return null;

            try
            {
                using (MySqlCommand comando = new MySqlCommand(ConsultaPorId, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    return LeerLibros(comando);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    $"No se pudo buscar por ID en MariaDB: {ex.Message}\n\n" +
                    "Pulsa \"Preparar datos de ejemplo\" si las tablas aún no existen.",
                    "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Búsqueda por nombre en MariaDB con el operador LIKE (coincidencias parciales).
        public List<LibroListado>? BuscarPorNombreMariaDB(string texto)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.7 (Carrito de Compras) - Búsqueda por nombre (LIKE)
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            MySqlConnection? conexion = ConexionMariaDB.ObtenerConexion();
            if (conexion == null)
                return null;

            try
            {
                using (MySqlCommand comando = new MySqlCommand(ConsultaPorNombre, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", "%" + EscaparLike(texto) + "%");
                    return LeerLibros(comando);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    $"No se pudo buscar por nombre en MariaDB: {ex.Message}\n\n" +
                    "Pulsa \"Preparar datos de ejemplo\" si las tablas aún no existen.",
                    "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // ================================ MongoDB: pipelines ================================

        public const string DescripcionPipelinePorId =
            "db.libros.aggregate([\n" +
            "  { $match: { EsActivo: true, _id: <ID> } },\n" +
            "  { $lookup: { from: \"generos\", localField: \"IdGenero\", foreignField: \"_id\", as: \"genero\" } },\n" +
            "  { $unwind: { path: \"$genero\", preserveNullAndEmptyArrays: true } }\n" +
            "])";

        public const string DescripcionPipelinePorNombre =
            "db.libros.aggregate([\n" +
            "  { $match: { EsActivo: true, Titulo: { $regex: \"<texto>\", $options: \"i\" } } },\n" +
            "  { $lookup: { from: \"generos\", localField: \"IdGenero\", foreignField: \"_id\", as: \"genero\" } },\n" +
            "  { $unwind: { path: \"$genero\", preserveNullAndEmptyArrays: true } },\n" +
            "  { $sort: { Titulo: 1 } }\n" +
            "])";

        // Ejecuta el pipeline ($match + $lookup + $unwind + $sort) con el filtro recibido.
        private static List<LibroListado>? EjecutarPipelineMongoDB(BsonDocument filtro)
        {
            try
            {
                IMongoCollection<BsonDocument> libros =
                    ConexionMongoDB.ObtenerBaseDatos().GetCollection<BsonDocument>("libros");

                BsonDocument[] etapas =
                {
                    new BsonDocument("$match", filtro),

                    new BsonDocument("$lookup", new BsonDocument
                    {
                        { "from", "generos" },
                        { "localField", "IdGenero" },
                        { "foreignField", "_id" },
                        { "as", "genero" }
                    }),

                    new BsonDocument("$unwind", new BsonDocument
                    {
                        { "path", "$genero" },
                        { "preserveNullAndEmptyArrays", true }
                    }),

                    new BsonDocument("$sort", new BsonDocument("Titulo", 1))
                };

                PipelineDefinition<BsonDocument, BsonDocument> pipeline =
                    PipelineDefinition<BsonDocument, BsonDocument>.Create(etapas);

                List<BsonDocument> documentos = libros.Aggregate(pipeline).ToList();
                List<LibroListado> resultado = new List<LibroListado>();

                foreach (BsonDocument doc in documentos)
                {
                    string categoria = "Sin categoría";

                    if (doc.TryGetValue("genero", out BsonValue genero) && genero.IsBsonDocument)
                    {
                        categoria = genero.AsBsonDocument
                            .GetValue("Nombre", "Sin categoría").AsString;
                    }

                    resultado.Add(new LibroListado
                    {
                        Id = doc["_id"].ToInt32(),
                        Titulo = doc.GetValue("Titulo", "").AsString,
                        Categoria = categoria,
                        PrecioRentaDiaria = doc.GetValue("PrecioRentaDiaria", 0).ToDecimal(),
                        RutaImagen = doc.GetValue("RutaImagen", "").AsString,
                        EsActivo = true,
                        EsNovedad = doc.GetValue("EsNovedad", false).ToBoolean(),
                        CantidadCopias = doc.GetValue("CantidadCopias", 1).ToInt32()
                    });
                }

                return resultado;
            }
            catch (Exception ex) when (
                ex is MongoException ||
                ex is TimeoutException ||
                ex is FormatException ||
                ex is InvalidCastException)
            {
                MessageBox.Show(
                    $"No se pudo buscar en MongoDB: {ex.Message}\n\n" +
                    "Verifica que el servicio esté iniciado y pulsa \"Preparar datos de ejemplo\".",
                    "Error MongoDB", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public List<LibroListado>? BuscarPorIdMongoDB(int id)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.7 (Carrito de Compras) - Búsqueda por ID
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            BsonDocument filtro = new BsonDocument
            {
                { "EsActivo", true },
                { "_id", id }
            };

            return EjecutarPipelineMongoDB(filtro);
        }

        // Búsqueda por nombre en MongoDB con expresión regular (sin distinguir mayúsculas).
        public List<LibroListado>? BuscarPorNombreMongoDB(string texto)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.7 (Carrito de Compras) - Búsqueda por nombre (regex)
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            // Regex.Escape evita que caracteres como ( o * del usuario rompan la expresión.
            BsonDocument filtro = new BsonDocument
            {
                { "EsActivo", true },
                { "Titulo", new BsonRegularExpression(Regex.Escape(texto), "i") }
            };

            return EjecutarPipelineMongoDB(filtro);
        }

        // ================================ Stock ================================

        // Devuelve las existencias actuales (cantidad_copias) leyéndolas de la base de datos,
        // o null si el libro no existe, está inactivo o hubo un error.
        public int? ObtenerStock(int idLibro, string motor)
        {
            return motor == MotorMongoDB
                ? ObtenerStockMongoDB(idLibro)
                : ObtenerStockMariaDB(idLibro);
        }

        private static int? ObtenerStockMariaDB(int idLibro)
        {
            MySqlConnection? conexion = ConexionMariaDB.ObtenerConexion();
            if (conexion == null)
                return null;

            try
            {
                const string sql =
                    "SELECT cantidad_copias FROM libros WHERE id = @id AND es_activo = 1 LIMIT 1";

                using (MySqlCommand comando = new MySqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idLibro);
                    object? valor = comando.ExecuteScalar();

                    if (valor == null || valor == DBNull.Value)
                        return null;

                    return Convert.ToInt32(valor);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"No se pudo consultar el stock en MariaDB: {ex.Message}",
                    "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private static int? ObtenerStockMongoDB(int idLibro)
        {
            try
            {
                IMongoCollection<BsonDocument> libros =
                    ConexionMongoDB.ObtenerBaseDatos().GetCollection<BsonDocument>("libros");

                BsonDocument filtro = new BsonDocument
                {
                    { "_id", idLibro },
                    { "EsActivo", true }
                };

                BsonDocument? doc = libros.Find(filtro).FirstOrDefault();
                if (doc == null)
                    return null;

                return doc.GetValue("CantidadCopias", 1).ToInt32();
            }
            catch (Exception ex) when (
                ex is MongoException ||
                ex is TimeoutException ||
                ex is FormatException ||
                ex is InvalidCastException)
            {
                MessageBox.Show($"No se pudo consultar el stock en MongoDB: {ex.Message}",
                    "Error MongoDB", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // ================================ Cálculos ================================

        // Subtotal de una línea del carrito: Cantidad x Precio unitario.
        public static decimal CalcularSubtotal(int cantidad, decimal precioUnitario)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.7 (Carrito de Compras) - Cálculo de subtotal
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            return cantidad * precioUnitario;
        }

        // Acumulado total: recorre la lista temporal y suma todos los subtotales.
        public static decimal CalcularTotal(IEnumerable<DetallePedido> detalles)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.7 (Carrito de Compras) - Cálculo del total
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            decimal total = 0m;

            foreach (DetallePedido detalle in detalles)
                total += detalle.Subtotal;

            return total;
        }

        // ================================ Validaciones ================================

        // Antes de añadir (o de cambiar la cantidad de una línea) se comprueba:
        //   cantidad > 0  y  (cantidad ya en el carrito + cantidad solicitada) <= stock disponible en la BD.
        public ResultadoValidacion ValidarCantidadYStock(
            int idLibro, string titulo, int cantidadSolicitada, int cantidadYaEnCarrito, string motor)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.7 (Carrito de Compras) - Validación de existencias
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            if (cantidadSolicitada <= 0)
                return new ResultadoValidacion(false,
                    "La cantidad debe ser mayor a cero. No se aceptan cantidades negativas.");

            int? stock = ObtenerStock(idLibro, motor);

            if (stock == null)
                return new ResultadoValidacion(false,
                    $"No se pudo verificar el stock de \"{titulo}\" (ID {idLibro}). " +
                    "El libro ya no existe o está inactivo.");

            if (stock.Value <= 0)
                return new ResultadoValidacion(false,
                    $"Sin existencias: \"{titulo}\" no tiene copias disponibles.");

            int totalSolicitado = cantidadYaEnCarrito + cantidadSolicitada;

            if (totalSolicitado > stock.Value)
            {
                string detalleCarrito = cantidadYaEnCarrito > 0
                    ? $" (ya hay {cantidadYaEnCarrito} en el carrito)"
                    : string.Empty;

                return new ResultadoValidacion(false,
                    $"Stock insuficiente para \"{titulo}\": solicitó {totalSolicitado}{detalleCarrito} " +
                    $"y solo hay {stock.Value} disponible(s).");
            }

            return new ResultadoValidacion(true, string.Empty);
        }

        // Validación final antes de la fase de guardado transaccional.
        public ResultadoValidacion ValidarPedido(
            IList<DetallePedido> carrito, decimal totalMostrado, string motor)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.7 (Carrito de Compras) - Validación del pedido
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            if (carrito == null || carrito.Count == 0)
                return new ResultadoValidacion(false,
                    "El carrito está vacío. Añada al menos un libro antes de confirmar.");

            HashSet<int> vistos = new HashSet<int>();

            foreach (DetallePedido linea in carrito)
            {
                if (!vistos.Add(linea.IdLibro))
                    return new ResultadoValidacion(false,
                        $"El libro ID {linea.IdLibro} aparece repetido en el carrito.");

                if (linea.Cantidad <= 0)
                    return new ResultadoValidacion(false,
                        $"La cantidad de \"{linea.Titulo}\" no es válida ({linea.Cantidad}).");

                if (linea.PrecioUnitario < 0)
                    return new ResultadoValidacion(false,
                        $"El precio de \"{linea.Titulo}\" no es válido.");

                if (linea.Subtotal != CalcularSubtotal(linea.Cantidad, linea.PrecioUnitario))
                    return new ResultadoValidacion(false,
                        $"El subtotal de \"{linea.Titulo}\" no coincide con Cantidad x Precio.");

                // Se vuelve a leer el stock: pudo cambiar desde que se añadió al carrito.
                ResultadoValidacion stock = ValidarCantidadYStock(
                    linea.IdLibro, linea.Titulo, linea.Cantidad, 0, motor);

                if (!stock.Valido)
                    return stock;
            }

            if (CalcularTotal(carrito) != totalMostrado)
                return new ResultadoValidacion(false,
                    "El total mostrado no coincide con la suma de los subtotales. Revise el carrito.");

            return new ResultadoValidacion(true, string.Empty);
        }
    }
}
