# Actividad 4.7 — Carrito de Compras para la Gestión de Pedidos y Ventas (Biblioteca, Windows Forms)

## Qué se implementó
- **`FrmVentas`** (nuevo formulario, todos los controles creados en el Diseñador): terminal de ventas con búsqueda, cuadrícula de resultados, cuadrícula del carrito, total y bitácora con fecha y hora. Se abre con el botón **Carrito de compras** del menú principal (`btnCarrito`, creado en el Diseñador de `FrmPrincipal`).
- **Búsqueda por ID:** `BuscarPorIdMariaDB` / `BuscarPorIdMongoDB`. Devuelve un solo libro; si no existe avisa con un mensaje (sin excepciones por nulos).
- **Búsqueda por nombre:** `LIKE` con parámetro en MariaDB y expresión regular (`$regex`, sin distinguir mayúsculas) en MongoDB. Con la caja vacía lista todos los libros activos.
- **Solo se venden libros activos** (los de borrado lógico de la 4.6 no aparecen).
- **Añadir al carrito:** valida el stock y crea un objeto temporal `DetallePedido` dentro de una `List<DetallePedido>` (`Models/DetallePedido.cs`) que alimenta la cuadrícula del carrito. Si el libro ya está, suma la cantidad en la misma línea.
- **Subtotal automático:** `CalcularSubtotal` = Cantidad × Precio unitario. La **Cantidad es editable en la cuadrícula** y el subtotal y el total se recalculan solos (no hay botón "Recalcular").
- **Total:** `CalcularTotal` recorre la lista con `foreach` y suma los subtotales; se muestra en `lblTotalVenta` con formato de moneda.
- **Validación de existencias:** `ValidarCantidadYStock` compara contra `cantidad_copias` (MariaDB) / `CantidadCopias` (MongoDB) leído de la base de datos. Rechaza cantidades ≤ 0, libros sin existencias y cantidades mayores al stock con un `MessageBox` de advertencia.
- **Confirmar pedido:** `ValidarPedido` verifica que el carrito no esté vacío, que no haya líneas repetidas, que cantidades, precios y subtotales sean consistentes, que el stock siga alcanzando y que el total coincida con la suma. Con "Sí" el pedido queda validado y listo para la siguiente fase (guardado transaccional); el carrito se vacía.
- **Autoría:** `lblAutoria` está en el Diseñador de `FrmVentas` y todos los métodos de búsqueda y cálculo (Vista y Controlador) llevan el bloque de comentarios con los integrantes y el número de equipo.

## Archivos nuevos o modificados
- Nuevos: `FrmVentas.cs`, `FrmVentas.Designer.cs`, `Controllers/CarritoController.cs`, `Models/DetallePedido.cs`.
- Modificados: `FrmPrincipal.cs` y `FrmPrincipal.Designer.cs` (botón `btnCarrito`), `Biblioteca.csproj` (registro del formulario).

## Requisitos externos
1. MariaDB/MySQL local (por ejemplo XAMPP) con la base `biblioteca`.
2. MongoDB Community Server en `mongodb://localhost:27017`.

## Cómo probarla
1. Iniciar MySQL y MongoDB. Abrir `Biblioteca.slnx` en Visual Studio y ejecutar (entrar con el login).
2. Pulsar **Carrito de compras** (panel derecho). Pulsar **Preparar datos de ejemplo** la primera vez.
3. **Búsqueda por ID:** escribir `3` y pulsar *Buscar ID* → aparece "Fundación". Con `999` → mensaje de que no existe (no truena).
4. **Búsqueda por nombre:** escribir `fun` → *Buscar nombre* → coincidencia parcial. Cambiar a **MongoDB** y repetir.
5. **Añadir:** seleccionar un libro, Cantidad = 1, *Añadir al Carrito* → aparece en el carrito con su subtotal y el total se actualiza.
6. **Stock:** cada libro de ejemplo tiene 1 copia. Con Cantidad = 2 → *Añadir* → `MessageBox` "Stock insuficiente". Para probar varias unidades, en **Libros** (4.5) cambiar las *Copias* de un libro a 5 y actualizar.
7. **Cantidad editable:** en el carrito, cambiar la Cantidad de una línea: subtotal y total cambian solos. Escribir `0`, `-3` o texto → se rechaza. Poner más que el stock → advertencia y regresa la cantidad anterior.
8. **Quitar / Vaciar:** quitan líneas y el total baja.
9. **Confirmar pedido** con el carrito vacío → advertencia. Con libros → pregunta Sí/No y mensaje de pedido validado.

## Evidencias para el PDF (con fecha y hora del sistema visibles)
1. Búsqueda de un producto (por ID o nombre).
2. Adición exitosa al carrito con subtotal y total calculados.
3. Mensaje de error al intentar vender más de lo que hay en inventario.

## Autoría
Equipo N° 3:
1. CONTRERAS Rodríguez Janis Isabel
2. MACÍAS Cruz Meredith Miranda
3. TORRES Barrera José Ángel
