namespace Biblioteca
{
    partial class FrmVentas
    {
        private System.ComponentModel.IContainer? components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DataGridViewCellStyle estiloMoneda1 = new DataGridViewCellStyle();
            DataGridViewCellStyle estiloMoneda2 = new DataGridViewCellStyle();
            DataGridViewCellStyle estiloMoneda3 = new DataGridViewCellStyle();
            DataGridViewCellStyle estiloCantidad = new DataGridViewCellStyle();
            lblAutoria = new Label();
            lblBuscarId = new Label();
            txtBuscarId = new TextBox();
            btnBuscarId = new Button();
            lblBuscarNombre = new Label();
            txtBuscarNombre = new TextBox();
            btnBuscarNombre = new Button();
            lblMotor = new Label();
            rdoMariaDB = new RadioButton();
            rdoMongoDB = new RadioButton();
            btnPreparar = new Button();
            btnLimpiarBusqueda = new Button();
            lblResultados = new Label();
            dgvResultados = new DataGridView();
            colResId = new DataGridViewTextBoxColumn();
            colResTitulo = new DataGridViewTextBoxColumn();
            colResCategoria = new DataGridViewTextBoxColumn();
            colResPrecio = new DataGridViewTextBoxColumn();
            colResStock = new DataGridViewTextBoxColumn();
            lblCantidad = new Label();
            nudCantidad = new NumericUpDown();
            btnAnadir = new Button();
            lblCarrito = new Label();
            dgvCarrito = new DataGridView();
            colCarId = new DataGridViewTextBoxColumn();
            colCarTitulo = new DataGridViewTextBoxColumn();
            colCantidadCarrito = new DataGridViewTextBoxColumn();
            colCarPrecio = new DataGridViewTextBoxColumn();
            colCarSubtotal = new DataGridViewTextBoxColumn();
            btnQuitar = new Button();
            btnVaciar = new Button();
            lblTotalTitulo = new Label();
            lblTotalVenta = new Label();
            lblResumen = new Label();
            btnConfirmar = new Button();
            lblBitacora = new Label();
            txtBitacora = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dgvResultados).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudCantidad).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).BeginInit();
            SuspendLayout();
            // 
            // lblAutoria
            // 
            lblAutoria.AutoSize = true;
            lblAutoria.Location = new Point(12, 9);
            lblAutoria.Name = "lblAutoria";
            lblAutoria.Size = new Size(500, 45);
            lblAutoria.TabIndex = 0;
            lblAutoria.Text = "Equipo N°: 3\r\nCONTRERAS Rodríguez Janis Isabel, MACÍAS Cruz Meredith Miranda,\r\nTORRES Barrera José Ángel";
            // 
            // lblBuscarId
            // 
            lblBuscarId.AutoSize = true;
            lblBuscarId.Location = new Point(12, 77);
            lblBuscarId.Name = "lblBuscarId";
            lblBuscarId.Size = new Size(85, 15);
            lblBuscarId.TabIndex = 1;
            lblBuscarId.Text = "Buscar por ID:";
            // 
            // txtBuscarId
            // 
            txtBuscarId.Location = new Point(110, 73);
            txtBuscarId.MaxLength = 9;
            txtBuscarId.Name = "txtBuscarId";
            txtBuscarId.Size = new Size(80, 23);
            txtBuscarId.TabIndex = 2;
            txtBuscarId.KeyDown += txtBuscarId_KeyDown;
            // 
            // btnBuscarId
            // 
            btnBuscarId.Location = new Point(198, 70);
            btnBuscarId.Name = "btnBuscarId";
            btnBuscarId.Size = new Size(100, 30);
            btnBuscarId.TabIndex = 3;
            btnBuscarId.Text = "Buscar ID";
            btnBuscarId.UseVisualStyleBackColor = true;
            btnBuscarId.Click += btnBuscarId_Click;
            // 
            // lblBuscarNombre
            // 
            lblBuscarNombre.AutoSize = true;
            lblBuscarNombre.Location = new Point(324, 77);
            lblBuscarNombre.Name = "lblBuscarNombre";
            lblBuscarNombre.Size = new Size(121, 15);
            lblBuscarNombre.TabIndex = 4;
            lblBuscarNombre.Text = "Buscar por nombre:";
            // 
            // txtBuscarNombre
            // 
            txtBuscarNombre.Location = new Point(452, 73);
            txtBuscarNombre.Name = "txtBuscarNombre";
            txtBuscarNombre.Size = new Size(190, 23);
            txtBuscarNombre.TabIndex = 5;
            txtBuscarNombre.KeyDown += txtBuscarNombre_KeyDown;
            // 
            // btnBuscarNombre
            // 
            btnBuscarNombre.Location = new Point(650, 70);
            btnBuscarNombre.Name = "btnBuscarNombre";
            btnBuscarNombre.Size = new Size(130, 30);
            btnBuscarNombre.TabIndex = 6;
            btnBuscarNombre.Text = "Buscar nombre";
            btnBuscarNombre.UseVisualStyleBackColor = true;
            btnBuscarNombre.Click += btnBuscarNombre_Click;
            // 
            // lblMotor
            // 
            lblMotor.AutoSize = true;
            lblMotor.Location = new Point(812, 77);
            lblMotor.Name = "lblMotor";
            lblMotor.Size = new Size(44, 15);
            lblMotor.TabIndex = 7;
            lblMotor.Text = "Motor:";
            // 
            // rdoMariaDB
            // 
            rdoMariaDB.AutoSize = true;
            rdoMariaDB.Checked = true;
            rdoMariaDB.Location = new Point(862, 75);
            rdoMariaDB.Name = "rdoMariaDB";
            rdoMariaDB.Size = new Size(73, 19);
            rdoMariaDB.TabIndex = 8;
            rdoMariaDB.TabStop = true;
            rdoMariaDB.Text = "MariaDB";
            rdoMariaDB.UseVisualStyleBackColor = true;
            // 
            // rdoMongoDB
            // 
            rdoMongoDB.AutoSize = true;
            rdoMongoDB.Location = new Point(945, 75);
            rdoMongoDB.Name = "rdoMongoDB";
            rdoMongoDB.Size = new Size(78, 19);
            rdoMongoDB.TabIndex = 9;
            rdoMongoDB.Text = "MongoDB";
            rdoMongoDB.UseVisualStyleBackColor = true;
            // 
            // btnPreparar
            // 
            btnPreparar.Location = new Point(12, 110);
            btnPreparar.Name = "btnPreparar";
            btnPreparar.Size = new Size(200, 30);
            btnPreparar.TabIndex = 10;
            btnPreparar.Text = "Preparar datos de ejemplo";
            btnPreparar.UseVisualStyleBackColor = true;
            btnPreparar.Click += btnPreparar_Click;
            // 
            // btnLimpiarBusqueda
            // 
            btnLimpiarBusqueda.Location = new Point(220, 110);
            btnLimpiarBusqueda.Name = "btnLimpiarBusqueda";
            btnLimpiarBusqueda.Size = new Size(130, 30);
            btnLimpiarBusqueda.TabIndex = 11;
            btnLimpiarBusqueda.Text = "Limpiar búsqueda";
            btnLimpiarBusqueda.UseVisualStyleBackColor = true;
            btnLimpiarBusqueda.Click += btnLimpiarBusqueda_Click;
            // 
            // lblResultados
            // 
            lblResultados.AutoSize = true;
            lblResultados.Location = new Point(12, 152);
            lblResultados.Name = "lblResultados";
            lblResultados.Size = new Size(147, 15);
            lblResultados.TabIndex = 12;
            lblResultados.Text = "Resultados de la búsqueda:";
            // 
            // dgvResultados
            // 
            dgvResultados.AllowUserToAddRows = false;
            dgvResultados.AllowUserToDeleteRows = false;
            dgvResultados.AutoGenerateColumns = false;
            dgvResultados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvResultados.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvResultados.Columns.AddRange(new DataGridViewColumn[] { colResId, colResTitulo, colResCategoria, colResPrecio, colResStock });
            dgvResultados.Location = new Point(12, 174);
            dgvResultados.MultiSelect = false;
            dgvResultados.Name = "dgvResultados";
            dgvResultados.ReadOnly = true;
            dgvResultados.RowHeadersWidth = 51;
            dgvResultados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResultados.Size = new Size(520, 170);
            dgvResultados.TabIndex = 13;
            dgvResultados.CellDoubleClick += dgvResultados_CellDoubleClick;
            // 
            // colResId
            // 
            colResId.DataPropertyName = "Id";
            colResId.FillWeight = 12F;
            colResId.HeaderText = "ID";
            colResId.MinimumWidth = 6;
            colResId.Name = "colResId";
            colResId.ReadOnly = true;
            // 
            // colResTitulo
            // 
            colResTitulo.DataPropertyName = "Titulo";
            colResTitulo.FillWeight = 36F;
            colResTitulo.HeaderText = "Título";
            colResTitulo.MinimumWidth = 6;
            colResTitulo.Name = "colResTitulo";
            colResTitulo.ReadOnly = true;
            // 
            // colResCategoria
            // 
            colResCategoria.DataPropertyName = "Categoria";
            colResCategoria.FillWeight = 24F;
            colResCategoria.HeaderText = "Categoría";
            colResCategoria.MinimumWidth = 6;
            colResCategoria.Name = "colResCategoria";
            colResCategoria.ReadOnly = true;
            // 
            // colResPrecio
            // 
            estiloMoneda1.Format = "C2";
            colResPrecio.DataPropertyName = "PrecioRentaDiaria";
            colResPrecio.DefaultCellStyle = estiloMoneda1;
            colResPrecio.FillWeight = 18F;
            colResPrecio.HeaderText = "Precio";
            colResPrecio.MinimumWidth = 6;
            colResPrecio.Name = "colResPrecio";
            colResPrecio.ReadOnly = true;
            // 
            // colResStock
            // 
            colResStock.DataPropertyName = "CantidadCopias";
            colResStock.FillWeight = 14F;
            colResStock.HeaderText = "Stock";
            colResStock.MinimumWidth = 6;
            colResStock.Name = "colResStock";
            colResStock.ReadOnly = true;
            // 
            // lblCantidad
            // 
            lblCantidad.AutoSize = true;
            lblCantidad.Location = new Point(12, 361);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new Size(58, 15);
            lblCantidad.TabIndex = 14;
            lblCantidad.Text = "Cantidad:";
            // 
            // nudCantidad
            // 
            nudCantidad.Location = new Point(78, 357);
            nudCantidad.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudCantidad.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudCantidad.Name = "nudCantidad";
            nudCantidad.Size = new Size(80, 23);
            nudCantidad.TabIndex = 15;
            nudCantidad.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // btnAnadir
            // 
            btnAnadir.Location = new Point(170, 352);
            btnAnadir.Name = "btnAnadir";
            btnAnadir.Size = new Size(170, 34);
            btnAnadir.TabIndex = 16;
            btnAnadir.Text = "Añadir al Carrito";
            btnAnadir.UseVisualStyleBackColor = true;
            btnAnadir.Click += btnAnadir_Click;
            // 
            // lblCarrito
            // 
            lblCarrito.AutoSize = true;
            lblCarrito.Location = new Point(552, 152);
            lblCarrito.Name = "lblCarrito";
            lblCarrito.Size = new Size(327, 15);
            lblCarrito.TabIndex = 17;
            lblCarrito.Text = "Carrito de compras (lista temporal; la Cantidad se puede editar):";
            // 
            // dgvCarrito
            // 
            dgvCarrito.AllowUserToAddRows = false;
            dgvCarrito.AllowUserToDeleteRows = false;
            dgvCarrito.AutoGenerateColumns = false;
            dgvCarrito.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCarrito.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCarrito.Columns.AddRange(new DataGridViewColumn[] { colCarId, colCarTitulo, colCantidadCarrito, colCarPrecio, colCarSubtotal });
            dgvCarrito.EditMode = DataGridViewEditMode.EditOnEnter;
            dgvCarrito.Location = new Point(552, 174);
            dgvCarrito.MultiSelect = false;
            dgvCarrito.Name = "dgvCarrito";
            dgvCarrito.RowHeadersWidth = 51;
            dgvCarrito.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCarrito.Size = new Size(536, 170);
            dgvCarrito.TabIndex = 18;
            dgvCarrito.CellEndEdit += dgvCarrito_CellEndEdit;
            dgvCarrito.CellValidating += dgvCarrito_CellValidating;
            dgvCarrito.DataError += dgvCarrito_DataError;
            // 
            // colCarId
            // 
            colCarId.DataPropertyName = "IdLibro";
            colCarId.FillWeight = 10F;
            colCarId.HeaderText = "ID";
            colCarId.MinimumWidth = 6;
            colCarId.Name = "colCarId";
            colCarId.ReadOnly = true;
            // 
            // colCarTitulo
            // 
            colCarTitulo.DataPropertyName = "Titulo";
            colCarTitulo.FillWeight = 34F;
            colCarTitulo.HeaderText = "Título";
            colCarTitulo.MinimumWidth = 6;
            colCarTitulo.Name = "colCarTitulo";
            colCarTitulo.ReadOnly = true;
            // 
            // colCantidadCarrito
            // 
            estiloCantidad.BackColor = Color.LemonChiffon;
            colCantidadCarrito.DataPropertyName = "Cantidad";
            colCantidadCarrito.DefaultCellStyle = estiloCantidad;
            colCantidadCarrito.FillWeight = 16F;
            colCantidadCarrito.HeaderText = "Cantidad";
            colCantidadCarrito.MinimumWidth = 6;
            colCantidadCarrito.Name = "colCantidadCarrito";
            // 
            // colCarPrecio
            // 
            estiloMoneda2.Format = "C2";
            colCarPrecio.DataPropertyName = "PrecioUnitario";
            colCarPrecio.DefaultCellStyle = estiloMoneda2;
            colCarPrecio.FillWeight = 20F;
            colCarPrecio.HeaderText = "Precio unitario";
            colCarPrecio.MinimumWidth = 6;
            colCarPrecio.Name = "colCarPrecio";
            colCarPrecio.ReadOnly = true;
            // 
            // colCarSubtotal
            // 
            estiloMoneda3.Format = "C2";
            colCarSubtotal.DataPropertyName = "Subtotal";
            colCarSubtotal.DefaultCellStyle = estiloMoneda3;
            colCarSubtotal.FillWeight = 20F;
            colCarSubtotal.HeaderText = "Subtotal";
            colCarSubtotal.MinimumWidth = 6;
            colCarSubtotal.Name = "colCarSubtotal";
            colCarSubtotal.ReadOnly = true;
            // 
            // btnQuitar
            // 
            btnQuitar.Location = new Point(552, 352);
            btnQuitar.Name = "btnQuitar";
            btnQuitar.Size = new Size(150, 34);
            btnQuitar.TabIndex = 19;
            btnQuitar.Text = "Quitar del carrito";
            btnQuitar.UseVisualStyleBackColor = true;
            btnQuitar.Click += btnQuitar_Click;
            // 
            // btnVaciar
            // 
            btnVaciar.Location = new Point(710, 352);
            btnVaciar.Name = "btnVaciar";
            btnVaciar.Size = new Size(140, 34);
            btnVaciar.TabIndex = 20;
            btnVaciar.Text = "Vaciar carrito";
            btnVaciar.UseVisualStyleBackColor = true;
            btnVaciar.Click += btnVaciar_Click;
            // 
            // lblTotalTitulo
            // 
            lblTotalTitulo.AutoSize = true;
            lblTotalTitulo.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalTitulo.Location = new Point(552, 402);
            lblTotalTitulo.Name = "lblTotalTitulo";
            lblTotalTitulo.Size = new Size(167, 20);
            lblTotalTitulo.TabIndex = 21;
            lblTotalTitulo.Text = "TOTAL DE LA VENTA:";
            // 
            // lblTotalVenta
            // 
            lblTotalVenta.AutoSize = true;
            lblTotalVenta.Font = new Font("Segoe UI", 22F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalVenta.ForeColor = Color.DarkGreen;
            lblTotalVenta.Location = new Point(726, 388);
            lblTotalVenta.Name = "lblTotalVenta";
            lblTotalVenta.Size = new Size(100, 41);
            lblTotalVenta.TabIndex = 22;
            lblTotalVenta.Text = "$0.00";
            // 
            // lblResumen
            // 
            lblResumen.AutoSize = true;
            lblResumen.Location = new Point(552, 436);
            lblResumen.Name = "lblResumen";
            lblResumen.Size = new Size(192, 15);
            lblResumen.TabIndex = 23;
            lblResumen.Text = "Libros distintos: 0 | Unidades: 0";
            // 
            // btnConfirmar
            // 
            btnConfirmar.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnConfirmar.Location = new Point(930, 388);
            btnConfirmar.Name = "btnConfirmar";
            btnConfirmar.Size = new Size(158, 50);
            btnConfirmar.TabIndex = 24;
            btnConfirmar.Text = "Confirmar pedido";
            btnConfirmar.UseVisualStyleBackColor = true;
            btnConfirmar.Click += btnConfirmar_Click;
            // 
            // lblBitacora
            // 
            lblBitacora.AutoSize = true;
            lblBitacora.Location = new Point(12, 464);
            lblBitacora.Name = "lblBitacora";
            lblBitacora.Size = new Size(123, 15);
            lblBitacora.TabIndex = 25;
            lblBitacora.Text = "Bitácora de la sesión:";
            // 
            // txtBitacora
            // 
            txtBitacora.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txtBitacora.Location = new Point(12, 486);
            txtBitacora.Multiline = true;
            txtBitacora.Name = "txtBitacora";
            txtBitacora.ReadOnly = true;
            txtBitacora.ScrollBars = ScrollBars.Vertical;
            txtBitacora.Size = new Size(1076, 110);
            txtBitacora.TabIndex = 26;
            // 
            // FrmVentas
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            AutoScrollMinSize = new Size(1100, 612);
            AutoValidate = AutoValidate.EnableAllowFocusChange;
            ClientSize = new Size(1100, 612);
            Controls.Add(txtBitacora);
            Controls.Add(lblBitacora);
            Controls.Add(btnConfirmar);
            Controls.Add(lblResumen);
            Controls.Add(lblTotalVenta);
            Controls.Add(lblTotalTitulo);
            Controls.Add(btnVaciar);
            Controls.Add(btnQuitar);
            Controls.Add(dgvCarrito);
            Controls.Add(lblCarrito);
            Controls.Add(btnAnadir);
            Controls.Add(nudCantidad);
            Controls.Add(lblCantidad);
            Controls.Add(dgvResultados);
            Controls.Add(lblResultados);
            Controls.Add(btnLimpiarBusqueda);
            Controls.Add(btnPreparar);
            Controls.Add(rdoMongoDB);
            Controls.Add(rdoMariaDB);
            Controls.Add(lblMotor);
            Controls.Add(btnBuscarNombre);
            Controls.Add(txtBuscarNombre);
            Controls.Add(lblBuscarNombre);
            Controls.Add(btnBuscarId);
            Controls.Add(txtBuscarId);
            Controls.Add(lblBuscarId);
            Controls.Add(lblAutoria);
            Name = "FrmVentas";
            Text = "Carrito de compras";
            ((System.ComponentModel.ISupportInitialize)dgvResultados).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudCantidad).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblAutoria;
        private Label lblBuscarId;
        private TextBox txtBuscarId;
        private Button btnBuscarId;
        private Label lblBuscarNombre;
        private TextBox txtBuscarNombre;
        private Button btnBuscarNombre;
        private Label lblMotor;
        private RadioButton rdoMariaDB;
        private RadioButton rdoMongoDB;
        private Button btnPreparar;
        private Button btnLimpiarBusqueda;
        private Label lblResultados;
        private DataGridView dgvResultados;
        private DataGridViewTextBoxColumn colResId;
        private DataGridViewTextBoxColumn colResTitulo;
        private DataGridViewTextBoxColumn colResCategoria;
        private DataGridViewTextBoxColumn colResPrecio;
        private DataGridViewTextBoxColumn colResStock;
        private Label lblCantidad;
        private NumericUpDown nudCantidad;
        private Button btnAnadir;
        private Label lblCarrito;
        private DataGridView dgvCarrito;
        private DataGridViewTextBoxColumn colCarId;
        private DataGridViewTextBoxColumn colCarTitulo;
        private DataGridViewTextBoxColumn colCantidadCarrito;
        private DataGridViewTextBoxColumn colCarPrecio;
        private DataGridViewTextBoxColumn colCarSubtotal;
        private Button btnQuitar;
        private Button btnVaciar;
        private Label lblTotalTitulo;
        private Label lblTotalVenta;
        private Label lblResumen;
        private Button btnConfirmar;
        private Label lblBitacora;
        private TextBox txtBitacora;
    }
}
