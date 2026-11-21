
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
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Biblioteca.Controllers
{
    public class LibroListado
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal PrecioRentaDiaria { get; set; }
        public string RutaImagen { get; set; } = string.Empty;
        public int IdGenero { get; set; }
        public bool EsActivo { get; set; }
        public bool EsNovedad { get; set; }

        // Cantidad de copias físicas registradas.
        public int CantidadCopias { get; set; } = 1;
    }

    public class LibroDocumento
    {
        [BsonId]
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal PrecioRentaDiaria { get; set; }

        public int IdGenero { get; set; }
        public string RutaImagen { get; set; } = string.Empty;
        public bool EsActivo { get; set; }

        [BsonElement("EsNovedad")]
        public bool EsNovedad { get; set; }

        // Se guarda en MongoDB como CantidadCopias.
        [BsonElement("CantidadCopias")]
        public int CantidadCopias { get; set; } = 1;
    }

    public class ListadoController
    {
        public const string CarpetaImagenes = "ImagenesDatos";

        private readonly GeneroController _generos =
            new GeneroController();

        public const string ConsultaJoin =
            "SELECT l.id, l.titulo, " +
            "COALESCE(g.nombre, 'Sin categoría') AS categoria, " +
            "l.precio_renta, l.ruta_imagen, l.es_novedad, " +
            "l.cantidad_copias " +
            "FROM libros AS l " +
            "LEFT JOIN generos AS g ON l.id_genero = g.id " +
            "WHERE l.es_activo = 1 " +
            "ORDER BY l.id";

        private record LibroEjemplo(
            int Id, string Titulo, decimal Precio, int IdGenero);

        private static List<Genero> GenerosEjemplo() =>
            new List<Genero>
            {
                new Genero(1, "Terror",
                    "Libros de suspenso y miedo",
                    "terror.png", true),

                new Genero(2, "Ciencia ficción",
                    "Historias de futuro, espacio y tecnología",
                    "ciencia_ficcion.png", true),

                new Genero(3, "Fantasía",
                    "Mundos mágicos y aventuras épicas",
                    "fantasia.png", true),

                new Genero(4, "Historia",
                    "Obras sobre hechos y personajes del pasado",
                    "historia.png", true)
            };

        private static readonly LibroEjemplo[] LibrosEjemplo =
        {
            new LibroEjemplo(1, "Drácula", 25.00m, 1),
            new LibroEjemplo(2, "Frankenstein", 22.50m, 1),
            new LibroEjemplo(3, "Fundación", 30.00m, 2),
            new LibroEjemplo(4, "Solaris", 28.00m, 2),
            new LibroEjemplo(5, "El Hobbit", 35.00m, 3),
            new LibroEjemplo(6, "Breve historia de México", 20.00m, 4)
        };

        private static string CrearPortadaEjemplo(
            int id, string titulo)
        {
            string relativa = Path.Combine(
                CarpetaImagenes,
                $"libro_ejemplo_{id}.png");

            string absoluta = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                relativa);

            if (File.Exists(absoluta))
                return relativa;

            Directory.CreateDirectory(
                Path.GetDirectoryName(absoluta)!);

            Color[] paleta =
            {
                Color.FromArgb(120, 40, 40),
                Color.FromArgb(40, 70, 120),
                Color.FromArgb(40, 110, 80),
                Color.FromArgb(110, 80, 30),
                Color.FromArgb(80, 50, 110),
                Color.FromArgb(50, 90, 100)
            };

            using (Bitmap portada = new Bitmap(300, 420))
            using (Graphics g = Graphics.FromImage(portada))
            using (SolidBrush fondo =
                new SolidBrush(paleta[id % paleta.Length]))
            using (Pen marco = new Pen(Color.White, 3))
            using (Font fuente =
                new Font("Segoe UI", 22, FontStyle.Bold))
            using (StringFormat centrado = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.SmoothingMode =
                    System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                g.TextRenderingHint =
                    System.Drawing.Text.TextRenderingHint.AntiAlias;

                g.FillRectangle(fondo, 0, 0, 300, 420);
                g.DrawRectangle(marco, 12, 12, 276, 396);

                g.DrawString(
                    titulo,
                    fuente,
                    Brushes.White,
                    new RectangleF(24, 24, 252, 372),
                    centrado);

                portada.Save(
                    absoluta,
                    System.Drawing.Imaging.ImageFormat.Png);
            }

            return relativa;
        }

        // ================================ MariaDB ================================

        private bool AsegurarColumnaNovedad(
            MySqlConnection conexion)
        {
            const string sql = @"
                ALTER TABLE libros
                ADD COLUMN IF NOT EXISTS
                es_novedad TINYINT(1) NOT NULL DEFAULT 0";

            using MySqlCommand comando =
                new MySqlCommand(sql, conexion);

            comando.ExecuteNonQuery();
            return true;
        }

        private bool AsegurarColumnaCopias(
            MySqlConnection conexion)
        {
            const string sql = @"
                ALTER TABLE libros
                ADD COLUMN IF NOT EXISTS
                cantidad_copias INT NOT NULL DEFAULT 1";

            using MySqlCommand comando =
                new MySqlCommand(sql, conexion);

            comando.ExecuteNonQuery();
            return true;
        }

        public bool PrepararMariaDB()
        {
            MySqlConnection? conexion =
                ConexionMariaDB.ObtenerConexion();

            if (conexion == null)
                return false;

            try
            {
                if (!_generos.CrearTablaMariaDB())
                    return false;

                foreach (Genero genero in GenerosEjemplo())
                {
                    if (!_generos.InsertarMariaDB(genero))
                        return false;
                }

                const string sqlTabla = @"
                    CREATE TABLE IF NOT EXISTS libros (
                        id INT PRIMARY KEY,
                        titulo VARCHAR(150) NOT NULL,
                        precio_renta DECIMAL(10,2) NOT NULL,
                        id_genero INT NOT NULL,
                        ruta_imagen VARCHAR(255),
                        es_activo TINYINT(1) NOT NULL,
                        es_novedad TINYINT(1) NOT NULL DEFAULT 0,
                        cantidad_copias INT NOT NULL DEFAULT 1,
                        CONSTRAINT fk_libros_genero
                        FOREIGN KEY (id_genero)
                        REFERENCES generos(id)
                    )";

                using (MySqlCommand crear =
                    new MySqlCommand(sqlTabla, conexion))
                {
                    crear.ExecuteNonQuery();
                }

                AsegurarColumnaNovedad(conexion);
                AsegurarColumnaCopias(conexion);

                const string sqlInsert = @"
                    INSERT INTO libros
                    (id, titulo, precio_renta, id_genero,
                     ruta_imagen, es_activo, es_novedad,
                     cantidad_copias)
                    VALUES
                    (@id, @titulo, @precio, @genero,
                     @ruta, 1, 0, 1)
                    ON DUPLICATE KEY UPDATE
                        id = id";   // Actividad 4.6: si el libro ya existe no se pisan sus datos editados

                foreach (LibroEjemplo libro in LibrosEjemplo)
                {
                    using (MySqlCommand comando =
                        new MySqlCommand(sqlInsert, conexion))
                    {
                        comando.Parameters.AddWithValue(
                            "@id", libro.Id);

                        comando.Parameters.AddWithValue(
                            "@titulo", libro.Titulo);

                        comando.Parameters.AddWithValue(
                            "@precio", libro.Precio);

                        comando.Parameters.AddWithValue(
                            "@genero", libro.IdGenero);

                        comando.Parameters.AddWithValue(
                            "@ruta",
                            CrearPortadaEjemplo(
                                libro.Id, libro.Titulo));

                        comando.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    $"No se pudieron preparar los datos en MariaDB: {ex.Message}",
                    "Error SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        public bool CrearTablaLibrosMariaDB()
        {
            MySqlConnection? conexion =
                ConexionMariaDB.ObtenerConexion();

            if (conexion == null)
                return false;

            try
            {
                const string sql = @"
                    CREATE TABLE IF NOT EXISTS libros (
                        id INT PRIMARY KEY,
                        titulo VARCHAR(150) NOT NULL,
                        precio_renta DECIMAL(10,2) NOT NULL,
                        id_genero INT NOT NULL,
                        ruta_imagen VARCHAR(255) NOT NULL,
                        es_activo TINYINT(1) NOT NULL,
                        es_novedad TINYINT(1) NOT NULL DEFAULT 0,
                        cantidad_copias INT NOT NULL DEFAULT 1,
                        CONSTRAINT fk_libros_genero
                        FOREIGN KEY (id_genero)
                        REFERENCES generos(id)
                    )";

                using (MySqlCommand comando =
                    new MySqlCommand(sql, conexion))
                {
                    comando.ExecuteNonQuery();
                }

                AsegurarColumnaNovedad(conexion);
                AsegurarColumnaCopias(conexion);

                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    "No se pudo preparar la tabla de libros: " +
                    ex.Message,
                    "Error SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        public List<LibroListado>? ListarMariaDB()
        {
            MySqlConnection? conexion =
                ConexionMariaDB.ObtenerConexion();

            if (conexion == null)
                return null;

            List<LibroListado> resultado =
                new List<LibroListado>();

            try
            {
                using (MySqlCommand comando =
                    new MySqlCommand(ConsultaJoin, conexion))
                using (MySqlDataReader lector =
                    comando.ExecuteReader())
                {
                    int colRuta =
                        lector.GetOrdinal("ruta_imagen");

                    while (lector.Read())
                    {
                        resultado.Add(new LibroListado
                        {
                            Id = lector.GetInt32("id"),
                            Titulo = lector.GetString("titulo"),
                            Categoria = lector.GetString("categoria"),

                            PrecioRentaDiaria =
                                lector.GetDecimal("precio_renta"),

                            RutaImagen = lector.IsDBNull(colRuta)
                                ? string.Empty
                                : lector.GetString("ruta_imagen"),

                            EsNovedad =
                                lector.GetBoolean("es_novedad"),

                            CantidadCopias =
                                lector.GetInt32("cantidad_copias")
                        });
                    }
                }

                return resultado;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    $"No se pudo listar desde MariaDB: {ex.Message}\n\n" +
                    "Pulsa primero \"Preparar datos de ejemplo\" " +
                    "si las tablas aún no existen.",
                    "Error SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return null;
            }
        }

        public LibroListado? BuscarLibroMariaDB(int id)
        {
            MySqlConnection? conexion =
                ConexionMariaDB.ObtenerConexion();

            if (conexion == null)
                return null;

            const string sql = @"
                SELECT
                    l.id,
                    l.titulo,
                    COALESCE(g.nombre, 'Sin categoría') AS categoria,
                    l.precio_renta,
                    l.ruta_imagen,
                    l.id_genero,
                    l.es_activo,
                    l.es_novedad,
                    l.cantidad_copias
                FROM libros AS l
                LEFT JOIN generos AS g
                    ON l.id_genero = g.id
                WHERE l.id = @id
                LIMIT 1";

            try
            {
                using (MySqlCommand comando =
                    new MySqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader lector =
                        comando.ExecuteReader())
                    {
                        if (!lector.Read())
                            return null;

                        int colRuta =
                            lector.GetOrdinal("ruta_imagen");

                        return new LibroListado
                        {
                            Id = lector.GetInt32("id"),
                            Titulo = lector.GetString("titulo"),
                            Categoria = lector.GetString("categoria"),

                            PrecioRentaDiaria =
                                lector.GetDecimal("precio_renta"),

                            RutaImagen = lector.IsDBNull(colRuta)
                                ? string.Empty
                                : lector.GetString("ruta_imagen"),

                            IdGenero = lector.GetInt32("id_genero"),
                            EsActivo = lector.GetBoolean("es_activo"),
                            EsNovedad = lector.GetBoolean("es_novedad"),

                            CantidadCopias =
                                lector.GetInt32("cantidad_copias")
                        };
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    "Error al buscar el libro en MariaDB: " +
                    ex.Message,
                    "Error de búsqueda",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return null;
            }
        }

        public bool InsertarLibroMariaDB(LibroDocumento libro)
        {
            MySqlConnection? conexion =
                ConexionMariaDB.ObtenerConexion();

            if (conexion == null)
                return false;

            const string sql = @"
                INSERT INTO libros
                (id, titulo, precio_renta, id_genero,
                 ruta_imagen, es_activo, es_novedad,
                 cantidad_copias)
                VALUES
                (@id, @titulo, @precio, @genero,
                 @ruta, @activo, @novedad, @copias)";

            try
            {
                using MySqlCommand comando =
                    new MySqlCommand(sql, conexion);

                comando.Parameters.AddWithValue("@id", libro.Id);
                comando.Parameters.AddWithValue(
                    "@titulo", libro.Titulo);

                comando.Parameters.AddWithValue(
                    "@precio", libro.PrecioRentaDiaria);

                comando.Parameters.AddWithValue(
                    "@genero", libro.IdGenero);

                comando.Parameters.AddWithValue(
                    "@ruta", libro.RutaImagen);

                comando.Parameters.AddWithValue(
                    "@activo", libro.EsActivo);

                comando.Parameters.AddWithValue(
                    "@novedad", libro.EsNovedad);

                comando.Parameters.AddWithValue(
                    "@copias", libro.CantidadCopias);

                comando.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    $"No se pudo insertar el libro en MariaDB: {ex.Message}",
                    "Error SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        public bool ActualizarLibroMariaDB(LibroDocumento libro)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.5 (Edición y Actualización)
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            MySqlConnection? conexion =
                ConexionMariaDB.ObtenerConexion();

            if (conexion == null)
                return false;

            const string sqlExiste =
                "SELECT COUNT(*) FROM libros WHERE id = @id";

            const string sqlActualizar = @"
                UPDATE libros SET
                    titulo = @titulo,
                    precio_renta = @precio,
                    id_genero = @genero,
                    ruta_imagen = @ruta,
                    es_activo = @activo,
                    es_novedad = @novedad,
                    cantidad_copias = @copias
                WHERE id = @id";

            try
            {
                using (MySqlCommand comprobar =
                    new MySqlCommand(sqlExiste, conexion))
                {
                    comprobar.Parameters.AddWithValue(
                        "@id", libro.Id);

                    long existe = Convert.ToInt64(
                        comprobar.ExecuteScalar());

                    if (existe == 0)
                        return false;
                }

                using (MySqlCommand comando =
                    new MySqlCommand(sqlActualizar, conexion))
                {
                    comando.Parameters.AddWithValue("@id", libro.Id);
                    comando.Parameters.AddWithValue(
                        "@titulo", libro.Titulo);

                    comando.Parameters.AddWithValue(
                        "@precio", libro.PrecioRentaDiaria);

                    comando.Parameters.AddWithValue(
                        "@genero", libro.IdGenero);

                    comando.Parameters.AddWithValue(
                        "@ruta", libro.RutaImagen);

                    comando.Parameters.AddWithValue(
                        "@activo", libro.EsActivo);

                    comando.Parameters.AddWithValue(
                        "@novedad", libro.EsNovedad);

                    comando.Parameters.AddWithValue(
                        "@copias", libro.CantidadCopias);

                    comando.ExecuteNonQuery();
                }

                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    "No se pudo actualizar el libro en MariaDB: " +
                    ex.Message,
                    "Error SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        public bool RevertirLibroMariaDB(int id)
        {
            MySqlConnection? conexion =
                ConexionMariaDB.ObtenerConexion();

            if (conexion == null)
                return false;

            using MySqlCommand comando =
                new MySqlCommand(
                    "DELETE FROM libros WHERE id = @id",
                    conexion);

            comando.Parameters.AddWithValue("@id", id);

            return comando.ExecuteNonQuery() > 0;
        }

        public List<Genero> ObtenerGenerosMariaDB()
        {
            MySqlConnection? conexion =
                ConexionMariaDB.ObtenerConexion();

            if (conexion == null)
                return new List<Genero>();

            const string sql = @"
                SELECT id, nombre, descripcion,
                       ruta_imagen, es_activo
                FROM generos
                WHERE es_activo = 1
                ORDER BY nombre";

            List<Genero> resultado =
                new List<Genero>();

            using (MySqlCommand comando =
                new MySqlCommand(sql, conexion))
            using (MySqlDataReader lector =
                comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    resultado.Add(new Genero(
                        lector.GetInt32("id"),
                        lector.GetString("nombre"),
                        lector.GetString("descripcion"),

                        lector.IsDBNull(
                            lector.GetOrdinal("ruta_imagen"))
                            ? string.Empty
                            : lector.GetString("ruta_imagen"),

                        lector.GetBoolean("es_activo")));
                }
            }

            return resultado;
        }

        // ================================ MongoDB ================================

        private IMongoCollection<LibroDocumento> ColeccionLibros()
        {
            return ConexionMongoDB.ObtenerBaseDatos()
                .GetCollection<LibroDocumento>("libros");
        }

        public bool PrepararMongoDB()
        {
            try
            {
                foreach (Genero genero in GenerosEjemplo())
                {
                    if (!_generos.InsertarMongoDB(genero))
                        return false;
                }

                foreach (LibroEjemplo libro in LibrosEjemplo)
                {
                    LibroDocumento documento =
                        new LibroDocumento
                        {
                            Id = libro.Id,
                            Titulo = libro.Titulo,
                            PrecioRentaDiaria = libro.Precio,
                            IdGenero = libro.IdGenero,

                            RutaImagen = CrearPortadaEjemplo(
                                libro.Id, libro.Titulo),

                            EsActivo = true,
                            EsNovedad = false,
                            CantidadCopias = 1
                        };

                    var actualizacion =
                        Builders<LibroDocumento>.Update
                            .SetOnInsert(
                                x => x.Titulo,
                                documento.Titulo)

                            .SetOnInsert(
                                x => x.PrecioRentaDiaria,
                                documento.PrecioRentaDiaria)

                            .SetOnInsert(
                                x => x.IdGenero,
                                documento.IdGenero)

                            .SetOnInsert(
                                x => x.RutaImagen,
                                documento.RutaImagen)

                            .SetOnInsert(
                                x => x.EsActivo,
                                documento.EsActivo)

                            .SetOnInsert(
                                x => x.EsNovedad,
                                documento.EsNovedad)

                            .SetOnInsert(
                                x => x.CantidadCopias,
                                documento.CantidadCopias);

                    ColeccionLibros().UpdateOne(
                        x => x.Id == documento.Id,
                        actualizacion,
                        new UpdateOptions
                        {
                            IsUpsert = true
                        });
                }

                // Completa el campo en documentos antiguos.
                var filtroSinCopias =
                    Builders<LibroDocumento>.Filter
                        .Exists(x => x.CantidadCopias, false);

                var completarCopias =
                    Builders<LibroDocumento>.Update
                        .Set(x => x.CantidadCopias, 1);

                ColeccionLibros().UpdateMany(
                    filtroSinCopias,
                    completarCopias);

                return true;
            }
            catch (Exception ex) when (
                ex is MongoException ||
                ex is TimeoutException)
            {
                MessageBox.Show(
                    $"No se pudieron preparar los datos en MongoDB: {ex.Message}",
                    "Error MongoDB",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        public const string DescripcionPipeline =
            "db.libros.aggregate([\n" +
            "  { $match: { EsActivo: true } },\n" +
            "  { $lookup: { from: \"generos\", localField: \"IdGenero\", foreignField: \"_id\", as: \"genero\" } },\n" +
            "  { $unwind: { path: \"$genero\", preserveNullAndEmptyArrays: true } },\n" +
            "  { $sort: { _id: 1 } }\n" +
            "])";

        public List<LibroListado>? ListarMongoDB()
        {
            try
            {
                IMongoCollection<BsonDocument> libros =
                    ConexionMongoDB.ObtenerBaseDatos()
                    .GetCollection<BsonDocument>("libros");

                BsonDocument[] etapas =
                {
                    // Actividad 4.6: el borrado lógico oculta los registros inactivos de la lista.
                    new BsonDocument("$match", new BsonDocument("EsActivo", true)),

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

                    new BsonDocument(
                        "$sort",
                        new BsonDocument("_id", 1))
                };

                PipelineDefinition<BsonDocument, BsonDocument>
                    pipeline =
                    PipelineDefinition<BsonDocument, BsonDocument>
                    .Create(etapas);

                List<BsonDocument> documentos =
                    libros.Aggregate(pipeline).ToList();

                List<LibroListado> resultado =
                    new List<LibroListado>();

                foreach (BsonDocument doc in documentos)
                {
                    string categoria = "Sin categoría";

                    if (doc.TryGetValue(
                        "genero", out BsonValue genero)
                        && genero.IsBsonDocument)
                    {
                        categoria = genero.AsBsonDocument
                            .GetValue(
                                "Nombre",
                                "Sin categoría").AsString;
                    }

                    resultado.Add(new LibroListado
                    {
                        Id = doc["_id"].ToInt32(),

                        Titulo = doc.GetValue(
                            "Titulo", "").AsString,

                        Categoria = categoria,

                        PrecioRentaDiaria = doc.GetValue(
                            "PrecioRentaDiaria", 0).ToDecimal(),

                        RutaImagen = doc.GetValue(
                            "RutaImagen", "").AsString,

                        EsNovedad = doc.GetValue(
                            "EsNovedad", false).ToBoolean(),

                        CantidadCopias = doc.GetValue(
                            "CantidadCopias", 1).ToInt32()
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
                    $"No se pudo listar desde MongoDB: {ex.Message}\n\n" +
                    "Verifica que el servicio esté iniciado y pulsa " +
                    "\"Preparar datos de ejemplo\".",
                    "Error MongoDB",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return null;
            }
        }

        public async Task<bool> InsertarLibroMongoDBAsync(
            LibroDocumento libro)
        {
            try
            {
                await ColeccionLibros().InsertOneAsync(libro);
                return true;
            }
            catch (Exception ex) when (
                ex is MongoException ||
                ex is TimeoutException)
            {
                MessageBox.Show(
                    $"No se pudo insertar el libro en MongoDB: {ex.Message}",
                    "Error MongoDB",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        public async Task<bool> ActualizarLibroMongoDBAsync(
            LibroDocumento libro)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.5 (Edición y Actualización)
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            try
            {
                // Filtro por identificador: el Id del libro se guarda como _id ([BsonId]).
                FilterDefinition<LibroDocumento> filtro =
                    Builders<LibroDocumento>.Filter.Eq(d => d.Id, libro.Id);

                // Reemplazo del documento completo con la nueva versión del objeto.
                ReplaceOneResult resultado =
                    await ColeccionLibros().ReplaceOneAsync(filtro, libro);

                return resultado.MatchedCount > 0;
            }
            catch (Exception ex) when (
                ex is MongoException ||
                ex is TimeoutException)
            {
                MessageBox.Show(
                    "No se pudo actualizar el libro en MongoDB: " +
                    ex.Message,
                    "Error MongoDB",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        // ============================ ACTIVIDAD 4.6: BORRADO ============================

        // Resultado de una operación de borrado: éxito y mensaje listo para mostrar y registrar en la bitácora.
        public record ResultadoBorrado(bool Exito, string Mensaje);

        private static LibroDocumento ADocumento(LibroListado libro)
        {
            return new LibroDocumento
            {
                Id = libro.Id,
                Titulo = libro.Titulo,
                PrecioRentaDiaria = libro.PrecioRentaDiaria,
                IdGenero = libro.IdGenero,
                RutaImagen = libro.RutaImagen,
                EsActivo = libro.EsActivo,
                EsNovedad = libro.EsNovedad,
                CantidadCopias = libro.CantidadCopias
            };
        }

        // ---------- Borrado lógico (Soft Delete): UPDATE / $set, el registro sigue en el disco de la base ----------

        public bool CambiarEstadoMariaDB(int id, bool activo)
        {
            MySqlConnection? conexion = ConexionMariaDB.ObtenerConexion();

            if (conexion == null)
                return false;

            const string sql = "UPDATE libros SET es_activo = @activo WHERE id = @id";

            try
            {
                using MySqlCommand comando = new MySqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@activo", activo);
                comando.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

                return comando.ExecuteNonQuery() > 0;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    "No se pudo cambiar el estado del libro en MariaDB: " + ex.Message,
                    "Error SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        // Devuelve cuántos documentos coincidieron (0 = no existía) o -1 si hubo un error.
        public async Task<long> CambiarEstadoMongoDBAsync(int id, bool activo)
        {
            try
            {
                UpdateResult resultado = await ColeccionLibros().UpdateOneAsync(
                    Builders<LibroDocumento>.Filter.Eq(d => d.Id, id),
                    Builders<LibroDocumento>.Update.Set(d => d.EsActivo, activo));

                return resultado.MatchedCount;
            }
            catch (Exception ex) when (
                ex is MongoException ||
                ex is TimeoutException)
            {
                MessageBox.Show(
                    "No se pudo cambiar el estado del libro en MongoDB: " + ex.Message,
                    "Error MongoDB",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return -1;
            }
        }

        public async Task<ResultadoBorrado> BorradoLogicoAsync(LibroListado original)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.6 (Borrado Lógico y Borrado Físico)
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            // En lugar de DELETE se ejecuta un UPDATE que cambia el estado a "Falso" (es_activo = 0 / EsActivo = false).
            if (!CambiarEstadoMariaDB(original.Id, false))
                return new ResultadoBorrado(false,
                    "No se marcó el libro como inactivo: no se encontró o falló MariaDB.");

            long mongo = await CambiarEstadoMongoDBAsync(original.Id, false);

            if (mongo < 0)
            {
                // Compensación: se devuelve MariaDB a su estado anterior para que ambos motores coincidan.
                bool restaurado = CambiarEstadoMariaDB(original.Id, original.EsActivo);

                return new ResultadoBorrado(false, restaurado
                    ? "MongoDB falló; se restauró el estado original en MariaDB. No se borró nada."
                    : "MariaDB quedó inactivo pero MongoDB falló. Revise ambos registros.");
            }

            // Sincronía con la lista en memoria usada por otros módulos.
            Libros? enMemoria = Libros.ObtenerTodos().Find(l => l.Id == original.Id);
            if (enMemoria != null)
                enMemoria.EsActivo = false;

            string aviso = mongo == 0
                ? " (MongoDB no tenía un documento con ese _id)."
                : ".";

            return new ResultadoBorrado(true,
                $"Borrado lógico aplicado al libro ID {original.Id}: es_activo = 0 en MariaDB y EsActivo = false en MongoDB" + aviso);
        }

        // ---------- Borrado físico (Hard Delete): DELETE / DeleteOne + archivo de imagen ----------

        // DELETE parametrizado por llave primaria: un solo registro, nunca borrados masivos.
        public bool EliminarLibroMariaDB(int id)
        {
            MySqlConnection? conexion = ConexionMariaDB.ObtenerConexion();

            if (conexion == null)
                return false;

            const string sql = "DELETE FROM libros WHERE id = @id";

            try
            {
                using MySqlCommand comando = new MySqlCommand(sql, conexion);
                comando.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

                return comando.ExecuteNonQuery() == 1;
            }
            catch (MySqlException ex) when (ex.Number == 1451 || ex.Number == 1217)
            {
                // Llave foránea activa: otro registro depende de este libro.
                MessageBox.Show(
                    "El libro no puede eliminarse físicamente porque otros registros dependen de él " +
                    "(integridad referencial). Use el borrado lógico.\n\n" + ex.Message,
                    "Integridad referencial",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    "No se pudo eliminar el libro en MariaDB: " + ex.Message,
                    "Error SQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        // Devuelve cuántos documentos se borraron (0 = ya no existía) o -1 si hubo un error.
        public async Task<long> EliminarLibroMongoDBAsync(int id)
        {
            try
            {
                DeleteResult resultado = await ColeccionLibros().DeleteOneAsync(
                    Builders<LibroDocumento>.Filter.Eq(d => d.Id, id));

                return resultado.DeletedCount;
            }
            catch (Exception ex) when (
                ex is MongoException ||
                ex is TimeoutException)
            {
                MessageBox.Show(
                    "No se pudo eliminar el libro en MongoDB: " + ex.Message,
                    "Error MongoDB",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return -1;
            }
        }

        // Indica si OTRO libro de MariaDB usa la misma imagen (en ese caso no se debe borrar el archivo).
        private bool ImagenUsadaPorOtroLibro(string rutaRelativa, int idExcluido)
        {
            MySqlConnection? conexion = ConexionMariaDB.ObtenerConexion();

            if (conexion == null)
                return true; // sin certeza, mejor conservar el archivo

            try
            {
                using MySqlCommand comando = new MySqlCommand(
                    "SELECT COUNT(*) FROM libros WHERE ruta_imagen = @ruta AND id <> @id",
                    conexion);

                comando.Parameters.AddWithValue("@ruta", rutaRelativa);
                comando.Parameters.AddWithValue("@id", idExcluido);

                return Convert.ToInt64(comando.ExecuteScalar()) > 0;
            }
            catch (MySqlException)
            {
                return true;
            }
        }

        // Destrucción física de la imagen. Se llama DESPUÉS de confirmar el borrado en ambas bases.
        // Validaciones: ruta dentro de ImagenesDatos, archivo existente, no compartido con otro libro
        // y no bloqueado por otro proceso. Devuelve un mensaje para la bitácora.
        public string EliminarImagenDeDisco(string? rutaRelativa, int idLibro)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.6 (Borrado Lógico y Borrado Físico)
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            if (string.IsNullOrWhiteSpace(rutaRelativa))
                return "El libro no tenía imagen asociada; no hay archivo que borrar.";

            string baseApp = AppDomain.CurrentDomain.BaseDirectory;

            string carpetaImagenes = Path.GetFullPath(
                Path.Combine(baseApp, CarpetaImagenes)) + Path.DirectorySeparatorChar;

            string rutaCompleta = Path.GetFullPath(Path.Combine(baseApp, rutaRelativa));

            // Solo se tocan archivos de la carpeta de imágenes de la aplicación.
            if (!rutaCompleta.StartsWith(carpetaImagenes, StringComparison.OrdinalIgnoreCase))
                return $"La imagen \"{rutaRelativa}\" está fuera de la carpeta {CarpetaImagenes}; no se borró.";

            if (!File.Exists(rutaCompleta))
                return $"La imagen \"{rutaRelativa}\" ya no existía en el disco.";

            if (ImagenUsadaPorOtroLibro(rutaRelativa, idLibro))
                return $"La imagen \"{rutaRelativa}\" se conservó porque otro libro la usa (o no se pudo verificar).";

            try
            {
                // Si otro proceso tiene el archivo abierto, esta apertura exclusiva lanza IOException.
                using (new FileStream(rutaCompleta, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                }

                File.Delete(rutaCompleta);
                return $"Imagen eliminada del disco: {rutaRelativa}";
            }
            catch (IOException)
            {
                return $"La imagen \"{rutaRelativa}\" está en uso por otro proceso; el registro se eliminó pero el archivo se conservó.";
            }
            catch (UnauthorizedAccessException ex)
            {
                return $"Sin permisos para borrar \"{rutaRelativa}\": {ex.Message}";
            }
        }

        public async Task<ResultadoBorrado> BorradoFisicoAsync(LibroListado original)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.6 (Borrado Lógico y Borrado Físico)
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            // 1) Antes de borrar se conserva la ruta relativa de la imagen asociada.
            string rutaImagen = original.RutaImagen;

            // 2) MariaDB primero: es el motor con llaves foráneas; si falla no se toca nada más.
            if (!EliminarLibroMariaDB(original.Id))
                return new ResultadoBorrado(false,
                    "No se eliminó el libro: MariaDB lo rechazó o ya no existía. No se modificó MongoDB ni el disco.");

            // 3) MongoDB: DeleteOne por _id.
            long mongo = await EliminarLibroMongoDBAsync(original.Id);

            if (mongo < 0)
            {
                // Compensación: se vuelve a insertar el libro en MariaDB con sus datos originales.
                bool restaurado = InsertarLibroMariaDB(ADocumento(original));

                return new ResultadoBorrado(false, restaurado
                    ? "MongoDB falló; se restauró el libro en MariaDB. No se borró nada."
                    : "MariaDB eliminó el libro pero MongoDB falló y no se pudo restaurar. Revise ambas bases.");
            }

            // 4) Sincronía con la lista en memoria usada por otros módulos.
            Libros.ObtenerTodos().RemoveAll(l => l.Id == original.Id);
            LibroAutor.QuitarVinculosDeLibro(original.Id);

            // 5) Con la eliminación confirmada en las bases, se destruye el archivo de imagen.
            string mensajeImagen = EliminarImagenDeDisco(rutaImagen, original.Id);

            string avisoMongo = mongo == 0
                ? " MongoDB no tenía un documento con ese _id."
                : string.Empty;

            return new ResultadoBorrado(true,
                $"Borrado físico del libro ID {original.Id}: eliminado de MariaDB y MongoDB.{avisoMongo} {mensajeImagen}");
        }
    }
}
