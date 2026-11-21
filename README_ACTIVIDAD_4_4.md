# Actividad 4.4 — Biblioteca (Windows Forms)

## Qué se implementó
- ComboBox de géneros en `FrmLibros`, declarado en `FrmLibros.Designer.cs` como control real del formulario.
- El ComboBox carga los géneros activos desde MariaDB al abrir el formulario.
- `DisplayMember = Nombre` muestra el texto y `ValueMember = Id` conserva la llave que procesa el sistema.
- La inserción de libros usa `INSERT INTO` parametrizado en MariaDB.
- El mismo registro se convierte en `LibroDocumento` y se inserta en MongoDB con `InsertOneAsync`.
- Se validan ID, título, precio, tipo de recurso, copias/archivo digital y selección de categoría.
- Se conserva la rutina de la Actividad 3.6: la imagen se copia al proyecto y se almacena la ruta relativa en ambos motores.
- Si la inserción documental falla después de insertar en MariaDB, se intenta revertir el registro SQL para evitar datos desincronizados.

## Requisitos externos
El código se trabaja en **Visual Studio Community**. Para que el guardado funcione sí se requieren servicios externos de base de datos:
1. MariaDB/MySQL local (por ejemplo, el servicio MySQL de XAMPP).
2. MongoDB Community Server local, escuchando en `mongodb://localhost:27017`.

phpMyAdmin y MongoDB Compass no son necesarios para ejecutar el programa; se utilizan para inspeccionar y tomar las evidencias que solicita la actividad.

## Preparación
1. Abrir `Biblioteca.slnx` o `Biblioteca.csproj` en Visual Studio.
2. Iniciar MariaDB y MongoDB.
3. Ejecutar `Database/Actividad_4_4_MariaDB.sql` en phpMyAdmin. La aplicación espera la base `biblioteca`, usuario `root`, contraseña vacía y puerto `3306`, de acuerdo con `Data/ConexionMariaDB.cs`.
4. Confirmar que los géneros estén activos. El ComboBox muestra únicamente registros con `es_activo = 1`.
5. Restaurar paquetes NuGet si Visual Studio lo solicita (`MySql.Data` y `MongoDB.Driver`).
6. Iniciar la aplicación, entrar al módulo Libros, llenar los datos, seleccionar el género del ComboBox y cargar la imagen. Guardar.
7. Verificar el registro en phpMyAdmin (`biblioteca.libros`) y MongoDB Compass (`biblioteca` → `libros`). Ambos deben mostrar el mismo ID, `IdGenero` y `RutaImagen`.

## Conexiones
- MariaDB: `localhost:3306`, base `biblioteca`, usuario `root`, contraseña vacía. Si tu instalación tiene otra contraseña o puerto, modificar `Data/ConexionMariaDB.cs`.
- MongoDB: `mongodb://localhost:27017`, base `biblioteca`, colección `libros`. Si tu servidor usa otra dirección, modificar `Data/ConexionMongoDB.cs`.

## Nota importante sobre evidencia y sincronización
La inserción en dos motores distintos no puede formar una única transacción distribuida. El proyecto incluye una compensación: si MongoDB falla después del INSERT en MariaDB, intenta borrar el registro SQL recién insertado. Si también falla esa reversión, el mensaje lo indica para revisión manual. Antes de entregar, tomar capturas reales de ejecución y de ambos administradores; no se incluyen capturas simuladas.

## Autoría
Equipo N° 3:
1. CONTRERAS Rodríguez Janis Isabel
2. MACÍAS Cruz Meredith Miranda
3. TORRES Barrera José Ángel
