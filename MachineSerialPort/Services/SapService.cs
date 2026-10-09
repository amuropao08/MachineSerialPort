using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort.Services
{
    internal class SapService
    {
        private readonly DataService _dataService;

        public interface ISapService
        {
            ObservableCollection<ComboItem> GetPackagings();
        }

        public SapService(DataService dataService)
        {
            _dataService = dataService;
        }

        //public ObservableCollection<ComboItem> GetPackagings()
        //{
        //    var orders = _dataService.GetSapOrders();
        //    var comboItems = new ObservableCollection<ComboItem>();
        //    foreach (var order in orders)
        //    {
        //        comboItems.Add(new ComboItem { Id = order.Id, Name = order.Name });
        //    }
        //    return comboItems;
        //}
    }
}
