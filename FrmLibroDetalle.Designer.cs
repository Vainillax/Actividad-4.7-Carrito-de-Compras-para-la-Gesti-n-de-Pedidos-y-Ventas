namespace Biblioteca
{
    partial class FrmLibroDetalle
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
            lblAviso = new Label();
            lblId = new Label();
            txtId = new TextBox();
            lblTituloLibro = new Label();
            txtTitulo = new TextBox();
            lblCategoria = new Label();
            txtCategoria = new TextBox();
            lblPrecio = new Label();
            txtPrecio = new TextBox();
            lblEstado = new Label();
            txtEstado = new TextBox();
            lblNovedad = new Label();
            txtNovedad = new TextBox();
            lblCopias = new Label();
            txtCopias = new TextBox();
            lblRuta = new Label();
            txtRuta = new TextBox();
            pbxPortada = new PictureBox();
            lblImagen = new Label();
            btnContinuar = new Button();
            btnCancelar = new Button();
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
            // lblAviso
            // 
            lblAviso.Location = new Point(12, 62);
            lblAviso.Name = "lblAviso";
            lblAviso.Size = new Size(616, 40);
            lblAviso.TabIndex = 1;
            lblAviso.Text = "Vista de solo lectura.";
            // 
            // lblId
            // 
            lblId.AutoSize = true;
            lblId.Location = new Point(20, 118);
            lblId.Name = "lblId";
            lblId.Size = new Size(18, 15);
            lblId.TabIndex = 2;
            lblId.Text = "ID";
            // 
            // txtId
            // 
            txtId.Location = new Point(130, 115);
            txtId.Name = "txtId";
            txtId.ReadOnly = true;
            txtId.Size = new Size(250, 23);
            txtId.TabIndex = 3;
            txtId.TabStop = false;
            // 
            // lblTituloLibro
            // 
            lblTituloLibro.AutoSize = true;
            lblTituloLibro.Location = new Point(20, 154);
            lblTituloLibro.Name = "lblTituloLibro";
            lblTituloLibro.Size = new Size(38, 15);
            lblTituloLibro.TabIndex = 4;
            lblTituloLibro.Text = "Título";
            // 
            // txtTitulo
            // 
            txtTitulo.Location = new Point(130, 151);
            txtTitulo.Name = "txtTitulo";
            txtTitulo.ReadOnly = true;
            txtTitulo.Size = new Size(250, 23);
            txtTitulo.TabIndex = 5;
            txtTitulo.TabStop = false;
            // 
            // lblCategoria
            // 
            lblCategoria.AutoSize = true;
            lblCategoria.Location = new Point(20, 190);
            lblCategoria.Name = "lblCategoria";
            lblCategoria.Size = new Size(58, 15);
            lblCategoria.TabIndex = 6;
            lblCategoria.Text = "Categoría";
            // 
            // txtCategoria
            // 
            txtCategoria.Location = new Point(130, 187);
            txtCategoria.Name = "txtCategoria";
            txtCategoria.ReadOnly = true;
            txtCategoria.Size = new Size(250, 23);
            txtCategoria.TabIndex = 7;
            txtCategoria.TabStop = false;
            // 
            // lblPrecio
            // 
            lblPrecio.AutoSize = true;
            lblPrecio.Location = new Point(20, 226);
            lblPrecio.Name = "lblPrecio";
            lblPrecio.Size = new Size(79, 15);
            lblPrecio.TabIndex = 8;
            lblPrecio.Text = "Renta por día";
            // 
            // txtPrecio
            // 
            txtPrecio.Location = new Point(130, 223);
            txtPrecio.Name = "txtPrecio";
            txtPrecio.ReadOnly = true;
            txtPrecio.Size = new Size(250, 23);
            txtPrecio.TabIndex = 9;
            txtPrecio.TabStop = false;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.Location = new Point(20, 262);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(42, 15);
            lblEstado.TabIndex = 10;
            lblEstado.Text = "Estado";
            // 
            // txtEstado
            // 
            txtEstado.Location = new Point(130, 259);
            txtEstado.Name = "txtEstado";
            txtEstado.ReadOnly = true;
            txtEstado.Size = new Size(250, 23);
            txtEstado.TabIndex = 11;
            txtEstado.TabStop = false;
            // 
            // lblNovedad
            // 
            lblNovedad.AutoSize = true;
            lblNovedad.Location = new Point(20, 298);
            lblNovedad.Name = "lblNovedad";
            lblNovedad.Size = new Size(54, 15);
            lblNovedad.TabIndex = 12;
            lblNovedad.Text = "Novedad";
            // 
            // txtNovedad
            // 
            txtNovedad.Location = new Point(130, 295);
            txtNovedad.Name = "txtNovedad";
            txtNovedad.ReadOnly = true;
            txtNovedad.Size = new Size(250, 23);
            txtNovedad.TabIndex = 13;
            txtNovedad.TabStop = false;
            // 
            // lblCopias
            // 
            lblCopias.AutoSize = true;
            lblCopias.Location = new Point(20, 334);
            lblCopias.Name = "lblCopias";
            lblCopias.Size = new Size(43, 15);
            lblCopias.TabIndex = 14;
            lblCopias.Text = "Copias";
            // 
            // txtCopias
            // 
            txtCopias.Location = new Point(130, 331);
            txtCopias.Name = "txtCopias";
            txtCopias.ReadOnly = true;
            txtCopias.Size = new Size(250, 23);
            txtCopias.TabIndex = 15;
            txtCopias.TabStop = false;
            // 
            // lblRuta
            // 
            lblRuta.AutoSize = true;
            lblRuta.Location = new Point(20, 370);
            lblRuta.Name = "lblRuta";
            lblRuta.Size = new Size(66, 15);
            lblRuta.TabIndex = 16;
            lblRuta.Text = "Ruta imagen";
            // 
            // txtRuta
            // 
            txtRuta.Location = new Point(130, 367);
            txtRuta.Name = "txtRuta";
            txtRuta.ReadOnly = true;
            txtRuta.Size = new Size(250, 23);
            txtRuta.TabIndex = 17;
            txtRuta.TabStop = false;
            // 
            // pbxPortada
            // 
            pbxPortada.BorderStyle = BorderStyle.FixedSingle;
            pbxPortada.Location = new Point(410, 115);
            pbxPortada.Name = "pbxPortada";
            pbxPortada.Size = new Size(210, 275);
            pbxPortada.SizeMode = PictureBoxSizeMode.Zoom;
            pbxPortada.TabIndex = 18;
            pbxPortada.TabStop = false;
            // 
            // lblImagen
            // 
            lblImagen.Location = new Point(410, 395);
            lblImagen.Name = "lblImagen";
            lblImagen.Size = new Size(210, 34);
            lblImagen.TabIndex = 19;
            lblImagen.Text = "Portada del libro";
            lblImagen.TextAlign = ContentAlignment.TopCenter;
            // 
            // btnContinuar
            // 
            btnContinuar.DialogResult = DialogResult.OK;
            btnContinuar.Location = new Point(320, 445);
            btnContinuar.Name = "btnContinuar";
            btnContinuar.Size = new Size(170, 34);
            btnContinuar.TabIndex = 20;
            btnContinuar.Text = "Continuar con el borrado";
            btnContinuar.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            btnCancelar.DialogResult = DialogResult.Cancel;
            btnCancelar.Location = new Point(500, 445);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(120, 34);
            btnCancelar.TabIndex = 21;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            // 
            // FrmLibroDetalle
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancelar;
            ClientSize = new Size(640, 495);
            Controls.Add(btnCancelar);
            Controls.Add(btnContinuar);
            Controls.Add(lblImagen);
            Controls.Add(pbxPortada);
            Controls.Add(txtRuta);
            Controls.Add(lblRuta);
            Controls.Add(txtCopias);
            Controls.Add(lblCopias);
            Controls.Add(txtNovedad);
            Controls.Add(lblNovedad);
            Controls.Add(txtEstado);
            Controls.Add(lblEstado);
            Controls.Add(txtPrecio);
            Controls.Add(lblPrecio);
            Controls.Add(txtCategoria);
            Controls.Add(lblCategoria);
            Controls.Add(txtTitulo);
            Controls.Add(lblTituloLibro);
            Controls.Add(txtId);
            Controls.Add(lblId);
            Controls.Add(lblAviso);
            Controls.Add(lblAutoria);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmLibroDetalle";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Detalle del libro (solo lectura)";
            ((System.ComponentModel.ISupportInitialize)pbxPortada).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblAutoria;
        private Label lblAviso;
        private Label lblId;
        private TextBox txtId;
        private Label lblTituloLibro;
        private TextBox txtTitulo;
        private Label lblCategoria;
        private TextBox txtCategoria;
        private Label lblPrecio;
        private TextBox txtPrecio;
        private Label lblEstado;
        private TextBox txtEstado;
        private Label lblNovedad;
        private TextBox txtNovedad;
        private Label lblCopias;
        private TextBox txtCopias;
        private Label lblRuta;
        private TextBox txtRuta;
        private PictureBox pbxPortada;
        private Label lblImagen;
        private Button btnContinuar;
        private Button btnCancelar;
    }
}
