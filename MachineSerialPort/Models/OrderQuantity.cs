using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MachineSerialPort.Models
{
    public class OrderQuantity : INotifyPropertyChanged
    {
        private decimal _total;
        private decimal _delivered;
        private decimal _remaining;

        public decimal Total
        {
            get => _total;
            set
            {
                if (_total != value)
                {
                    _total = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Delivered
        {
            get => _delivered;
            set
            {
                if (_delivered != value)
                {
                    _delivered = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Remaining
        {
            get => _remaining;
            set
            {
                if (_remaining != value)
                {
                    _remaining = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
