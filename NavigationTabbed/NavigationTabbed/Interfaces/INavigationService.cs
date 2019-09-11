using NavigationTabbed.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NavigationTabbed.Interfaces
{
    public interface INavigationService
    {
        BaseViewModel PreviousPageViewModel { get; }
        Task InitializeAsync();
        Task NavigateToAsync<TViewModel>(BaseViewModel currentVm) where TViewModel : BaseViewModel;
        Task NavigateToAsync<TViewModel>(object parameter, BaseViewModel currentVm) where TViewModel : BaseViewModel;
        Task RemoveLastFromBackStackAsync();
        Task RemoveBackStackAsync();
        Task GoBackAsync();
    }
}
