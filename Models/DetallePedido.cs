/*
 * ENCABEZADO DE AUTORÍA
 * Equipo N°: 3
 * Integrantes (orden alfabético por primer apellido):
 * 1. CONTRERAS Rodríguez Janis Isabel
 * 2. MACÍAS Cruz Meredith Miranda
 * 3. TORRES Barrera José Ángel
 */
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Biblioteca.Models
{
    // Actividad 4.7: una línea del carrito de compras (objeto temporal de la capa de Vista).
    // Vive solo en memoria dentro de una List<DetallePedido>; no se guarda en la base de datos
    // hasta la fase de guardado transaccional.
    // Implementa INotifyPropertyChanged para que la cuadrícula del carrito se actualice sola
    // cuando cambian la Cantidad o el Subtotal.
    public class DetallePedido : INotifyPropertyChanged
    {
        private int _cantidad;
        private decimal _subtotal;

        public int IdLibro { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }

        // Última cantidad que pasó la validación de stock; si el usuario escribe una cantidad
        // inválida en la cuadrícula, se vuelve a este valor.
        public int UltimaCantidadValida { get; set; }

        public int Cantidad
        {
            get => _cantidad;
            set
            {
                if (_cantidad == value)
                    return;
                _cantidad = value;
                Notificar();
            }
        }

        public decimal Subtotal
        {
            get => _subtotal;
            set
            {
                if (_subtotal == value)
                    return;
                _subtotal = value;
                Notificar();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Notificar([CallerMemberName] string? propiedad = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
        }
    }
}
