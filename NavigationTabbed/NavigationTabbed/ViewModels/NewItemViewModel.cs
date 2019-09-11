using NavigationTabbed.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace NavigationTabbed.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        public Item Item { get; set; }

        public Command SaveCommand { get; set; }
        public Command CancelCommand { get; set; }
        public NewItemViewModel()
        {
            SaveCommand = new Command(item => { MessagingCenter.Send(this, "AddItem", item as Item);
                NavigationService.GoBackAsync();
            });
            CancelCommand = new Command(() => NavigationService.GoBackAsync());
        }
    }
}
