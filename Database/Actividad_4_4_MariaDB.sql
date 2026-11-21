-- Preparación de MariaDB para la Actividad 4.4
-- Ejecutar en phpMyAdmin o MariaDB antes de iniciar la aplicación.

CREATE DATABASE IF NOT EXISTS biblioteca
  CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE biblioteca;

CREATE TABLE IF NOT EXISTS generos (
    id INT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion VARCHAR(255) NOT NULL,
    ruta_imagen VARCHAR(255),
    es_activo TINYINT(1) NOT NULL DEFAULT 1
);

INSERT INTO generos (id, nombre, descripcion, ruta_imagen, es_activo)
VALUES
 (1, 'Terror', 'Libros de suspenso y miedo', 'terror.png', 1),
 (2, 'Ciencia ficción', 'Historias de futuro, espacio y tecnología', 'ciencia_ficcion.png', 1),
 (3, 'Fantasía', 'Mundos mágicos y aventuras épicas', 'fantasia.png', 1),
 (4, 'Historia', 'Obras sobre hechos y personajes del pasado', 'historia.png', 1)
ON DUPLICATE KEY UPDATE
 nombre = VALUES(nombre),
 descripcion = VALUES(descripcion),
 ruta_imagen = VALUES(ruta_imagen),
 es_activo = VALUES(es_activo);

CREATE TABLE IF NOT EXISTS libros (
    id INT PRIMARY KEY,
    titulo VARCHAR(150) NOT NULL,
    precio_renta DECIMAL(10,2) NOT NULL,
    id_genero INT NOT NULL,
    ruta_imagen VARCHAR(255) NOT NULL,
    es_activo TINYINT(1) NOT NULL,
    CONSTRAINT fk_libros_genero FOREIGN KEY (id_genero) REFERENCES generos(id)
);
