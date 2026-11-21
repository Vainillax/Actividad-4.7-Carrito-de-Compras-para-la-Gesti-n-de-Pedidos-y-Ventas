namespace Biblioteca
{
    partial class FrmRespaldo
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
            lblAutoria = new Label();
            lblEntidad = new Label();
            cmbEntidad = new ComboBox();
            btnExportarJson = new Button();
            btnExportarXml = new Button();
            btnImportarJson = new Button();
            btnImportarXml = new Button();
            btnDatosEjemplo = new Button();
            btnAbrirCarpeta = new Button();
            lblTotal = new Label();
            dgvDatos = new DataGridView();
            txtBitacora = new TextBox();
            ofdImportar = new OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)dgvDatos).BeginInit();
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
            // lblEntidad
            // 
            lblEntidad.AutoSize = true;
            lblEntidad.Location = new Point(12, 71);
            lblEntidad.Name = "lblEntidad";
            lblEntidad.Size = new Size(64, 15);
            lblEntidad.TabIndex = 1;
            lblEntidad.Text = "Colección:";
            // 
            // cmbEntidad
            // 
            cmbEntidad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEntidad.FormattingEnabled = true;
            cmbEntidad.Items.AddRange(new object[] { "Autores", "Géneros" });
            cmbEntidad.Location = new Point(90, 68);
            cmbEntidad.Name = "cmbEntidad";
            cmbEntidad.Size = new Size(180, 23);
            cmbEntidad.TabIndex = 2;
            cmbEntidad.SelectedIndexChanged += cmbEntidad_SelectedIndexChanged;
            // 
            // btnExportarJson
            // 
            btnExportarJson.Location = new Point(12, 102);
            btnExportarJson.Name = "btnExportarJson";
            btnExportarJson.Size = new Size(150, 34);
            btnExportarJson.TabIndex = 3;
            btnExportarJson.Text = "Exportar a JSON";
            btnExportarJson.UseVisualStyleBackColor = true;
            btnExportarJson.Click += btnExportarJson_Click;
            // 
            // btnExportarXml
            // 
            btnExportarXml.Location = new Point(170, 102);
            btnExportarXml.Name = "btnExportarXml";
            btnExportarXml.Size = new Size(150, 34);
            btnExportarXml.TabIndex = 4;
            btnExportarXml.Text = "Exportar a XML";
            btnExportarXml.UseVisualStyleBackColor = true;
            btnExportarXml.Click += btnExportarXml_Click;
            // 
            // btnImportarJson
            // 
            btnImportarJson.Location = new Point(328, 102);
            btnImportarJson.Name = "btnImportarJson";
            btnImportarJson.Size = new Size(150, 34);
            btnImportarJson.TabIndex = 5;
            btnImportarJson.Text = "Importar desde JSON";
            btnImportarJson.UseVisualStyleBackColor = true;
            btnImportarJson.Click += btnImportarJson_Click;
            // 
            // btnImportarXml
            // 
            btnImportarXml.Location = new Point(486, 102);
            btnImportarXml.Name = "btnImportarXml";
            btnImportarXml.Size = new Size(150, 34);
            btnImportarXml.TabIndex = 6;
            btnImportarXml.Text = "Importar desde XML";
            btnImportarXml.UseVisualStyleBackColor = true;
            btnImportarXml.Click += btnImportarXml_Click;
            // 
            // btnDatosEjemplo
            // 
            btnDatosEjemplo.Location = new Point(644, 102);
            btnDatosEjemplo.Name = "btnDatosEjemplo";
            btnDatosEjemplo.Size = new Size(170, 34);
            btnDatosEjemplo.TabIndex = 7;
            btnDatosEjemplo.Text = "Cargar datos de ejemplo";
            btnDatosEjemplo.UseVisualStyleBackColor = true;
            btnDatosEjemplo.Click += btnDatosEjemplo_Click;
            // 
            // btnAbrirCarpeta
            // 
            btnAbrirCarpeta.Location = new Point(822, 102);
            btnAbrirCarpeta.Name = "btnAbrirCarpeta";
            btnAbrirCarpeta.Size = new Size(106, 34);
            btnAbrirCarpeta.TabIndex = 8;
            btnAbrirCarpeta.Text = "Abrir carpeta";
            btnAbrirCarpeta.UseVisualStyleBackColor = true;
            btnAbrirCarpeta.Click += btnAbrirCarpeta_Click;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(12, 148);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(138, 15);
            lblTotal.TabIndex = 9;
            lblTotal.Text = "Registros en memoria: 0";
            // 
            // dgvDatos
            // 
            dgvDatos.AllowUserToAddRows = false;
            dgvDatos.AllowUserToDeleteRows = false;
            dgvDatos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDatos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDatos.Location = new Point(12, 170);
            dgvDatos.MultiSelect = false;
            dgvDatos.Name = "dgvDatos";
            dgvDatos.ReadOnly = true;
            dgvDatos.RowHeadersWidth = 51;
            dgvDatos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDatos.Size = new Size(916, 230);
            dgvDatos.TabIndex = 10;
            dgvDatos.DataBindingComplete += dgvDatos_DataBindingComplete;
            // 
            // txtBitacora
            // 
            txtBitacora.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtBitacora.Location = new Point(12, 410);
            txtBitacora.Multiline = true;
            txtBitacora.Name = "txtBitacora";
            txtBitacora.ReadOnly = true;
            txtBitacora.ScrollBars = ScrollBars.Vertical;
            txtBitacora.Size = new Size(916, 140);
            txtBitacora.TabIndex = 11;
            // 
            // ofdImportar
            // 
            ofdImportar.Title = "Importar respaldo";
            // 
            // FrmRespaldo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(940, 560);
            Controls.Add(txtBitacora);
            Controls.Add(dgvDatos);
            Controls.Add(lblTotal);
            Controls.Add(btnAbrirCarpeta);
            Controls.Add(btnDatosEjemplo);
            Controls.Add(btnImportarXml);
            Controls.Add(btnImportarJson);
            Controls.Add(btnExportarXml);
            Controls.Add(btnExportarJson);
            Controls.Add(cmbEntidad);
            Controls.Add(lblEntidad);
            Controls.Add(lblAutoria);
            Name = "FrmRespaldo";
            Text = "Respaldo en JSON y XML";
            ((System.ComponentModel.ISupportInitialize)dgvDatos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblAutoria;
        private Label lblEntidad;
        private ComboBox cmbEntidad;
        private Button btnExportarJson;
        private Button btnExportarXml;
        private Button btnImportarJson;
        private Button btnImportarXml;
        private Button btnDatosEjemplo;
        private Button btnAbrirCarpeta;
        private Label lblTotal;
        private DataGridView dgvDatos;
        private TextBox txtBitacora;
        private OpenFileDialog ofdImportar;
    }
}
