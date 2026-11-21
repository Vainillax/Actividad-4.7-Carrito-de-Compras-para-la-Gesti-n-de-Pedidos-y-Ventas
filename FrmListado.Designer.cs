namespace Biblioteca
{
    partial class FrmListado
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            lblAutoria = new Label();
            btnPreparar = new Button();
            btnListarMariaDB = new Button();
            btnListarMongoDB = new Button();
            btnLimpiar = new Button();
            btnEditar = new Button();
            btnVerDetalle = new Button();
            btnBorradoLogico = new Button();
            btnBorradoFisico = new Button();
            lblOrigen = new Label();
            dgvListado = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colTitulo = new DataGridViewTextBoxColumn();
            colCategoria = new DataGridViewTextBoxColumn();
            colPrecio = new DataGridViewTextBoxColumn();
            lblVistaPrevia = new Label();
            pbxPortada = new PictureBox();
            lblImagen = new Label();
            txtBitacora = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dgvListado).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbxPortada).BeginInit();
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
            // btnPreparar
            // 
            btnPreparar.Location = new Point(12, 70);
            btnPreparar.Name = "btnPreparar";
            btnPreparar.Size = new Size(220, 34);
            btnPreparar.TabIndex = 1;
            btnPreparar.Text = "Preparar datos de ejemplo";
            btnPreparar.UseVisualStyleBackColor = true;
            btnPreparar.Click += btnPreparar_Click;
            // 
            // btnListarMariaDB
            // 
            btnListarMariaDB.Location = new Point(240, 70);
            btnListarMariaDB.Name = "btnListarMariaDB";
            btnListarMariaDB.Size = new Size(220, 34);
            btnListarMariaDB.TabIndex = 2;
            btnListarMariaDB.Text = "Listar MariaDB (JOIN)";
            btnListarMariaDB.UseVisualStyleBackColor = true;
            btnListarMariaDB.Click += btnListarMariaDB_Click;
            // 
            // btnListarMongoDB
            // 
            btnListarMongoDB.Location = new Point(468, 70);
            btnListarMongoDB.Name = "btnListarMongoDB";
            btnListarMongoDB.Size = new Size(220, 34);
            btnListarMongoDB.TabIndex = 3;
            btnListarMongoDB.Text = "Listar MongoDB ($lookup)";
            btnListarMongoDB.UseVisualStyleBackColor = true;
            btnListarMongoDB.Click += btnListarMongoDB_Click;
            // 
            // btnLimpiar
            // 
            btnLimpiar.Location = new Point(696, 70);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(120, 34);
            btnLimpiar.TabIndex = 4;
            btnLimpiar.Text = "Limpiar";
            btnLimpiar.UseVisualStyleBackColor = true;
            btnLimpiar.Click += btnLimpiar_Click;
            // 
            // btnEditar  (Actividad 4.5: envía el libro seleccionado al formulario de edición)
            // 
            btnEditar.Location = new Point(824, 70);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(164, 34);
            btnEditar.TabIndex = 11;
            btnEditar.Text = "Editar seleccionado";
            btnEditar.UseVisualStyleBackColor = true;
            btnEditar.Click += btnEditar_Click;
            // 
            // btnVerDetalle  (Actividad 4.6: ventana modal de solo lectura)
            // 
            btnVerDetalle.Location = new Point(12, 108);
            btnVerDetalle.Name = "btnVerDetalle";
            btnVerDetalle.Size = new Size(160, 34);
            btnVerDetalle.TabIndex = 12;
            btnVerDetalle.Text = "Ver detalle";
            btnVerDetalle.UseVisualStyleBackColor = true;
            btnVerDetalle.Click += btnVerDetalle_Click;
            // 
            // btnBorradoLogico  (Actividad 4.6: soft delete, cambia el estado a inactivo)
            // 
            btnBorradoLogico.Location = new Point(180, 108);
            btnBorradoLogico.Name = "btnBorradoLogico";
            btnBorradoLogico.Size = new Size(190, 34);
            btnBorradoLogico.TabIndex = 13;
            btnBorradoLogico.Text = "Borrado lógico";
            btnBorradoLogico.UseVisualStyleBackColor = true;
            btnBorradoLogico.Click += btnBorradoLogico_Click;
            // 
            // btnBorradoFisico  (Actividad 4.6: hard delete en MariaDB, MongoDB y disco)
            // 
            btnBorradoFisico.Location = new Point(378, 108);
            btnBorradoFisico.Name = "btnBorradoFisico";
            btnBorradoFisico.Size = new Size(190, 34);
            btnBorradoFisico.TabIndex = 14;
            btnBorradoFisico.Text = "Borrado físico";
            btnBorradoFisico.UseVisualStyleBackColor = true;
            btnBorradoFisico.Click += btnBorradoFisico_Click;
            // 
            // lblOrigen
            // 
            lblOrigen.AutoSize = true;
            lblOrigen.Location = new Point(12, 152);
            lblOrigen.Name = "lblOrigen";
            lblOrigen.Size = new Size(230, 15);
            lblOrigen.TabIndex = 5;
            lblOrigen.Text = "Origen de datos: ninguno | Registros: 0";
            // 
            // dgvListado
            // 
            dgvListado.AllowUserToAddRows = false;
            dgvListado.AllowUserToDeleteRows = false;
            dgvListado.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            dgvListado.AutoGenerateColumns = false;
            dgvListado.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvListado.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvListado.Columns.AddRange(new DataGridViewColumn[] { colId, colTitulo, colCategoria, colPrecio });
            dgvListado.Location = new Point(12, 174);
            dgvListado.MultiSelect = false;
            dgvListado.Name = "dgvListado";
            dgvListado.ReadOnly = true;
            dgvListado.RowHeadersWidth = 51;
            dgvListado.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvListado.Size = new Size(640, 316);
            dgvListado.TabIndex = 6;
            dgvListado.SelectionChanged += dgvListado_SelectionChanged;
            dgvListado.CellDoubleClick += dgvListado_CellDoubleClick;
            // 
            // colId
            // 
            colId.DataPropertyName = "Id";
            colId.FillWeight = 15F;
            colId.HeaderText = "ID";
            colId.MinimumWidth = 6;
            colId.Name = "colId";
            colId.ReadOnly = true;
            // 
            // colTitulo
            // 
            colTitulo.DataPropertyName = "Titulo";
            colTitulo.FillWeight = 40F;
            colTitulo.HeaderText = "Título";
            colTitulo.MinimumWidth = 6;
            colTitulo.Name = "colTitulo";
            colTitulo.ReadOnly = true;
            // 
            // colCategoria
            // 
            colCategoria.DataPropertyName = "Categoria";
            colCategoria.FillWeight = 25F;
            colCategoria.HeaderText = "Categoría";
            colCategoria.MinimumWidth = 6;
            colCategoria.Name = "colCategoria";
            colCategoria.ReadOnly = true;
            // 
            // colPrecio
            // 
            dataGridViewCellStyle1.Format = "C2";
            colPrecio.DataPropertyName = "PrecioRentaDiaria";
            colPrecio.DefaultCellStyle = dataGridViewCellStyle1;
            colPrecio.FillWeight = 20F;
            colPrecio.HeaderText = "Renta por día";
            colPrecio.MinimumWidth = 6;
            colPrecio.Name = "colPrecio";
            colPrecio.ReadOnly = true;
            // 
            // lblVistaPrevia
            // 
            lblVistaPrevia.AutoSize = true;
            lblVistaPrevia.Location = new Point(672, 152);
            lblVistaPrevia.Name = "lblVistaPrevia";
            lblVistaPrevia.Size = new Size(110, 15);
            lblVistaPrevia.TabIndex = 7;
            lblVistaPrevia.Text = "Portada del libro:";
            // 
            // pbxPortada
            // 
            pbxPortada.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            pbxPortada.BorderStyle = BorderStyle.FixedSingle;
            pbxPortada.Location = new Point(672, 174);
            pbxPortada.Name = "pbxPortada";
            pbxPortada.Size = new Size(316, 316);
            pbxPortada.SizeMode = PictureBoxSizeMode.Zoom;
            pbxPortada.TabIndex = 8;
            pbxPortada.TabStop = false;
            // 
            // lblImagen
            // 
            lblImagen.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblImagen.Location = new Point(672, 496);
            lblImagen.Name = "lblImagen";
            lblImagen.Size = new Size(316, 34);
            lblImagen.TabIndex = 9;
            lblImagen.Text = "Selecciona un libro para ver su portada.";
            lblImagen.TextAlign = ContentAlignment.TopCenter;
            // 
            // txtBitacora
            // 
            txtBitacora.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtBitacora.Location = new Point(12, 540);
            txtBitacora.Multiline = true;
            txtBitacora.Name = "txtBitacora";
            txtBitacora.ReadOnly = true;
            txtBitacora.ScrollBars = ScrollBars.Vertical;
            txtBitacora.Size = new Size(976, 150);
            txtBitacora.TabIndex = 10;
            // 
            // FrmListado
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            AutoScrollMinSize = new Size(1000, 700);
            ClientSize = new Size(1000, 700);
            Controls.Add(txtBitacora);
            Controls.Add(lblImagen);
            Controls.Add(pbxPortada);
            Controls.Add(lblVistaPrevia);
            Controls.Add(dgvListado);
            Controls.Add(lblOrigen);
            Controls.Add(btnBorradoFisico);
            Controls.Add(btnBorradoLogico);
            Controls.Add(btnVerDetalle);
            Controls.Add(btnEditar);
            Controls.Add(btnLimpiar);
            Controls.Add(btnListarMongoDB);
            Controls.Add(btnListarMariaDB);
            Controls.Add(btnPreparar);
            Controls.Add(lblAutoria);
            Name = "FrmListado";
            Text = "Listado relacional (JOIN / $lookup)";
            ((System.ComponentModel.ISupportInitialize)dgvListado).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbxPortada).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblAutoria;
        private Button btnPreparar;
        private Button btnListarMariaDB;
        private Button btnListarMongoDB;
        private Button btnLimpiar;
        private Button btnEditar;
        private Button btnVerDetalle;
        private Button btnBorradoLogico;
        private Button btnBorradoFisico;
        private Label lblOrigen;
        private DataGridView dgvListado;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colTitulo;
        private DataGridViewTextBoxColumn colCategoria;
        private DataGridViewTextBoxColumn colPrecio;
        private Label lblVistaPrevia;
        private PictureBox pbxPortada;
        private Label lblImagen;
        private TextBox txtBitacora;
    }
}
