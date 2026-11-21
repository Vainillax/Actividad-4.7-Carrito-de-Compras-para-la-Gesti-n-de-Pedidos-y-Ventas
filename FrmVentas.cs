/*
 * ENCABEZADO DE AUTORÍA
 * Equipo N°: 3
 * Integrantes (orden alfabético por primer apellido):
 * 1. CONTRERAS Rodríguez Janis Isabel
 * 2. MACÍAS Cruz Meredith Miranda
 * 3. TORRES Barrera José Ángel
 */
using Biblioteca.Controllers;
using Biblioteca.Models;
using System.ComponentModel;

namespace Biblioteca
{
    // Actividad 4.7: terminal de ventas (Carrito de Compras).
    // 1) Se busca un libro por ID (exacto) o por nombre (LIKE / regex) en MariaDB o MongoDB.
    // 2) "Añadir al Carrito" valida el stock y crea un objeto temporal DetallePedido dentro de una List<DetallePedido>.
    // 3) El subtotal (Cantidad x Precio) y el total se recalculan solos cada vez que cambia el carrito.
    // 4) "Confirmar pedido" hace la validación final antes de la fase de guardado transaccional.
    // Todos los controles (botones, cuadrículas, cajas de texto, Label de autoría y Label del total) están
    // creados en el Diseñador (FrmVentas.Designer.cs); aquí solo está la lógica.
    public partial class FrmVentas : Form
    {
        private readonly CarritoController _controlador = new CarritoController();
        private readonly ListadoController _preparacion = new ListadoController();

        // Colección temporal de la capa de Vista: los productos elegidos antes de la persistencia.
        private readonly List<DetallePedido> _carrito = new List<DetallePedido>();

        // Envoltura que avisa a la cuadrícula cuando se agrega, quita o modifica una línea.
        // Trabaja sobre la MISMA lista _carrito (no es una copia).
        private readonly BindingList<DetallePedido> _vistaCarrito;

        private string MotorActual =>
            rdoMongoDB.Checked ? CarritoController.MotorMongoDB : CarritoController.MotorMariaDB;

        public FrmVentas()
        {
            InitializeComponent();

            _vistaCarrito = new BindingList<DetallePedido>(_carrito);
            dgvCarrito.DataSource = _vistaCarrito;

            ActualizarTotal();
            Escribir("Terminal de ventas lista. Pulsa \"Preparar datos de ejemplo\" la primera vez.");
        }

        // La fecha y hora del sistema debe verse en las capturas de evidencia.
        private void Escribir(string texto)
        {
            txtBitacora.AppendText($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {texto}{Environment.NewLine}");
        }

        // ================== PREPARAR DATOS ==================

        private void btnPreparar_Click(object? sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                if (_preparacion.PrepararMariaDB())
                    Escribir("MariaDB: tablas 'generos' y 'libros' listas con datos de ejemplo.");
                else
                    Escribir("MariaDB: no se pudieron preparar los datos.");

                if (_preparacion.PrepararMongoDB())
                    Escribir("MongoDB: colecciones 'generos' y 'libros' listas con datos de ejemplo.");
                else
                    Escribir("MongoDB: no se pudieron preparar los datos.");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // ================== BÚSQUEDA DINÁMICA ==================

        private void txtBuscarId_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnBuscarId.PerformClick();
            }
        }

        private void txtBuscarNombre_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnBuscarNombre.PerformClick();
            }
        }

        // Búsqueda por ID: al escribir un código exacto devuelve el objeto único.
        private void btnBuscarId_Click(object? sender, EventArgs e)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.7 (Carrito de Compras) - Búsqueda por ID
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            string texto = txtBuscarId.Text.Trim();

            if (!int.TryParse(texto, out int id) || id <= 0)
            {
                Escribir($"Búsqueda por ID: \"{texto}\" no es un ID válido.");
                MessageBox.Show("Escriba un ID numérico válido (número entero mayor a cero).",
                    "Búsqueda por ID", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtBuscarId.Focus();
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                string motor = MotorActual;
                Escribir($"{motor}: búsqueda por ID {id}." + Environment.NewLine +
                    (motor == CarritoController.MotorMariaDB
                        ? CarritoController.ConsultaPorId
                        : CarritoController.DescripcionPipelinePorId.Replace("\n", Environment.NewLine)));

                List<LibroListado>? lista = motor == CarritoController.MotorMariaDB
                    ? _controlador.BuscarPorIdMariaDB(id)
                    : _controlador.BuscarPorIdMongoDB(id);

                if (lista == null)
                {
                    Escribir($"{motor}: la búsqueda falló.");
                    return;
                }

                MostrarResultados(lista, motor);

                if (lista.Count == 0)
                {
                    // Caso "no se encontró": se avisa sin lanzar excepciones por valores nulos.
                    Escribir($"{motor}: no existe un libro activo con ID {id}.");
                    MessageBox.Show($"No se encontró ningún libro activo con el ID {id}.",
                        "Búsqueda por ID", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // Búsqueda por nombre: coincidencias parciales con LIKE (MariaDB) o expresión regular (MongoDB).
        private void btnBuscarNombre_Click(object? sender, EventArgs e)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.7 (Carrito de Compras) - Búsqueda por nombre (Like)
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            string texto = txtBuscarNombre.Text.Trim();

            Cursor = Cursors.WaitCursor;
            try
            {
                string motor = MotorActual;
                string criterio = texto.Length == 0 ? "(vacío: todos los libros activos)" : $"\"{texto}\"";
                Escribir($"{motor}: búsqueda por nombre {criterio}." + Environment.NewLine +
                    (motor == CarritoController.MotorMariaDB
                        ? CarritoController.ConsultaPorNombre
                        : CarritoController.DescripcionPipelinePorNombre.Replace("\n", Environment.NewLine)));

                List<LibroListado>? lista = motor == CarritoController.MotorMariaDB
                    ? _controlador.BuscarPorNombreMariaDB(texto)
                    : _controlador.BuscarPorNombreMongoDB(texto);

                if (lista == null)
                {
                    Escribir($"{motor}: la búsqueda falló.");
                    return;
                }

                MostrarResultados(lista, motor);

                if (lista.Count == 0)
                {
                    Escribir($"{motor}: ningún libro activo coincide con {criterio}.");
                    MessageBox.Show($"Ningún libro activo coincide con {criterio}.",
                        "Búsqueda por nombre", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnLimpiarBusqueda_Click(object? sender, EventArgs e)
        {
            dgvResultados.DataSource = null;
            txtBuscarId.Clear();
            txtBuscarNombre.Clear();
            lblResultados.Text = "Resultados de la búsqueda:";
            nudCantidad.Value = 1;
            txtBuscarId.Focus();
        }

        // Vincula los resultados a la cuadrícula de búsqueda (columnas definidas en el Diseñador).
        private void MostrarResultados(List<LibroListado> lista, string motor)
        {
            dgvResultados.DataSource = null;
            dgvResultados.DataSource = lista;
            lblResultados.Text = $"Resultados de la búsqueda ({motor}): {lista.Count} libro(s)";
            Escribir($"{motor}: {lista.Count} libro(s) encontrado(s).");

            if (lista.Count > 0)
            {
                dgvResultados.ClearSelection();
                dgvResultados.Rows[0].Selected = true;
                dgvResultados.CurrentCell = dgvResultados.Rows[0].Cells[0];
            }
        }

        // ================== CARRITO ==================

        private void dgvResultados_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                AnadirSeleccionado();
        }

        private void btnAnadir_Click(object? sender, EventArgs e)
        {
            AnadirSeleccionado();
        }

        // Selección y adición: crea un objeto temporal DetallePedido y lo agrega a la List<DetallePedido>.
        private void AnadirSeleccionado()
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.7 (Carrito de Compras) - Añadir al carrito
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            if (dgvResultados.CurrentRow?.DataBoundItem is not LibroListado libro)
            {
                Escribir("Añadir: seleccione primero un libro de los resultados de la búsqueda.");
                MessageBox.Show("Busque un libro y selecciónelo en la tabla de resultados.",
                    "Añadir al carrito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int cantidad = (int)nudCantidad.Value;
            DetallePedido? existente = _carrito.FirstOrDefault(d => d.IdLibro == libro.Id);
            int yaEnCarrito = existente?.Cantidad ?? 0;

            // Validación de existencias contra la base de datos ANTES de tocar el carrito.
            CarritoController.ResultadoValidacion validacion = _controlador.ValidarCantidadYStock(
                libro.Id, libro.Titulo, cantidad, yaEnCarrito, MotorActual);

            if (!validacion.Valido)
            {
                Escribir("Añadir: rechazado. " + validacion.Mensaje);
                MessageBox.Show(validacion.Mensaje, "Existencias insuficientes",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (existente != null)
            {
                // El libro ya estaba en el carrito: se acumula la cantidad en la misma línea.
                existente.Cantidad += cantidad;
                existente.UltimaCantidadValida = existente.Cantidad;
                existente.Subtotal = CarritoController.CalcularSubtotal(existente.Cantidad, existente.PrecioUnitario);

                Escribir($"Carrito: {cantidad} x \"{existente.Titulo}\" sumado a la línea existente " +
                    $"({existente.Cantidad} x {existente.PrecioUnitario:C2} = {existente.Subtotal:C2}).");
            }
            else
            {
                DetallePedido detalle = new DetallePedido
                {
                    IdLibro = libro.Id,
                    Titulo = libro.Titulo,
                    PrecioUnitario = libro.PrecioRentaDiaria,
                    Cantidad = cantidad,
                    UltimaCantidadValida = cantidad
                };
                detalle.Subtotal = CarritoController.CalcularSubtotal(detalle.Cantidad, detalle.PrecioUnitario);

                _vistaCarrito.Add(detalle);

                Escribir($"Carrito: añadido {cantidad} x \"{detalle.Titulo}\" " +
                    $"({detalle.PrecioUnitario:C2} c/u) = {detalle.Subtotal:C2}.");
            }

            ActualizarTotal();
            nudCantidad.Value = 1;
        }

        // Cada vez que cambia el carrito se recorre la lista y se proyecta el total en lblTotalVenta.
        private void ActualizarTotal()
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.7 (Carrito de Compras) - Cálculo del total
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            decimal total = CarritoController.CalcularTotal(_carrito);

            int unidades = 0;
            foreach (DetallePedido linea in _carrito)
                unidades += linea.Cantidad;

            lblTotalVenta.Text = total.ToString("C2");
            lblResumen.Text = $"Libros distintos: {_carrito.Count} | Unidades: {unidades}";
        }

        // La Cantidad de la cuadrícula del carrito es editable: se rechaza lo que no sea entero positivo.
        private void dgvCarrito_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != colCantidadCarrito.Index)
                return;

            string texto = Convert.ToString(e.FormattedValue)?.Trim() ?? string.Empty;

            if (!int.TryParse(texto, out int cantidad))
            {
                e.Cancel = true;
                MessageBox.Show("La cantidad debe ser un número entero.\nCorrija el valor o presione Esc para cancelar.",
                    "Cantidad inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cantidad <= 0)
            {
                e.Cancel = true;
                MessageBox.Show("La cantidad debe ser mayor a cero. No se aceptan negativos.\nCorrija el valor o presione Esc para cancelar.",
                    "Cantidad inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Si el valor llega a ser ilegible para la columna, no se muestra el cuadro de error por defecto.
        private void dgvCarrito_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        // Al terminar de editar la Cantidad: se valida el stock y se recalcula el subtotal y el total
        // automáticamente (no hace falta ningún botón "Recalcular").
        private void dgvCarrito_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.7 (Carrito de Compras) - Cálculo de subtotales
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            if (e.RowIndex < 0 || e.ColumnIndex != colCantidadCarrito.Index)
                return;

            if (dgvCarrito.Rows[e.RowIndex].DataBoundItem is not DetallePedido linea)
                return;

            CarritoController.ResultadoValidacion validacion = _controlador.ValidarCantidadYStock(
                linea.IdLibro, linea.Titulo, linea.Cantidad, 0, MotorActual);

            if (!validacion.Valido)
            {
                int rechazada = linea.Cantidad;
                linea.Cantidad = linea.UltimaCantidadValida;   // se restaura la última cantidad válida
                Escribir($"Cantidad {rechazada} rechazada para \"{linea.Titulo}\". " + validacion.Mensaje);
                MessageBox.Show(validacion.Mensaje, "Existencias insuficientes",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                linea.UltimaCantidadValida = linea.Cantidad;
                Escribir($"Cantidad de \"{linea.Titulo}\" cambiada a {linea.Cantidad}.");
            }

            linea.Subtotal = CarritoController.CalcularSubtotal(linea.Cantidad, linea.PrecioUnitario);
            ActualizarTotal();
        }

        private void btnQuitar_Click(object? sender, EventArgs e)
        {
            if (dgvCarrito.CurrentRow?.DataBoundItem is not DetallePedido linea)
            {
                Escribir("Quitar: seleccione primero una línea del carrito.");
                MessageBox.Show("Seleccione una línea del carrito para quitarla.",
                    "Quitar del carrito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _vistaCarrito.Remove(linea);
            ActualizarTotal();
            Escribir($"Carrito: se quitó \"{linea.Titulo}\".");
        }

        private void btnVaciar_Click(object? sender, EventArgs e)
        {
            if (_carrito.Count == 0)
            {
                Escribir("Vaciar: el carrito ya está vacío.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea quitar todos los libros del carrito?",
                "Vaciar carrito", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            _vistaCarrito.Clear();
            ActualizarTotal();
            Escribir("Carrito: vaciado.");
        }

        // ================== CONFIRMACIÓN DEL PEDIDO ==================

        // Validación final: carrito no vacío y datos consistentes antes de pasar al guardado transaccional.
        private void btnConfirmar_Click(object? sender, EventArgs e)
        {
            /*
             * ENCABEZADO DE AUTORÍA - Actividad 4.7 (Carrito de Compras) - Confirmación del pedido
             * Equipo N°: 3
             * 1. CONTRERAS Rodríguez Janis Isabel
             * 2. MACÍAS Cruz Meredith Miranda
             * 3. TORRES Barrera José Ángel
             */

            decimal total = CarritoController.CalcularTotal(_carrito);

            Cursor = Cursors.WaitCursor;
            CarritoController.ResultadoValidacion validacion;
            try
            {
                validacion = _controlador.ValidarPedido(_carrito, total, MotorActual);
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            if (!validacion.Valido)
            {
                Escribir("Confirmar pedido: no válido. " + validacion.Mensaje);
                MessageBox.Show(validacion.Mensaje, "No se puede confirmar el pedido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                $"¿Confirmar el pedido de {_carrito.Count} libro(s) por un total de {total:C2}?",
                "Confirmar pedido", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
            {
                Escribir("Confirmar pedido: el usuario respondió No.");
                return;
            }

            Escribir($"Pedido validado correctamente ({_carrito.Count} libro(s)):");
            foreach (DetallePedido linea in _carrito)
                Escribir($"   {linea.Cantidad} x {linea.Titulo} ({linea.PrecioUnitario:C2}) = {linea.Subtotal:C2}");
            Escribir($"   TOTAL: {total:C2}. Listo para la fase de guardado transaccional.");

            MessageBox.Show(
                $"Pedido validado por {total:C2}.\n\nQueda listo para la siguiente fase (guardado transaccional).",
                "Pedido confirmado", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _vistaCarrito.Clear();
            ActualizarTotal();
        }
    }
}
