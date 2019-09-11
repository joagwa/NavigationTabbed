using NavigationTabbed.Interfaces;
using NavigationTabbed.ViewModels;
using NavigationTabbed.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NavigationTabbed.Services
{
    public class NavigationService : INavigationService
    {
        public BaseViewModel CurrentViewModel { get; set; }
        public Page CurrentPage { get; set; }
        public NavigationPage CurrentNavigationPage { get; set; }
        public BaseViewModel PreviousPageViewModel
        {
            get
            {
                var CurrentNavigationPage = Application.Current.MainPage as CustomNavigationView;
                var viewModel = CurrentNavigationPage.Navigation.NavigationStack[CurrentNavigationPage.Navigation.NavigationStack.Count - 2].BindingContext;
                return viewModel as BaseViewModel;
            }
        }

        public void InitCurrentNavigationPage (BaseViewModel baseViewModel)
        {
            if (baseViewModel != null)
            {
                Type currentPageType = GetPageTypeForViewModel(baseViewModel.GetType());

                foreach (var page in ((TabbedPage)Application.Current.MainPage).Children)
                {
                    if (page is NavigationPage)
                    {
                        foreach (var child in page.Navigation.NavigationStack)
                        {
                            if (child.GetType() == currentPageType)
                            {
                                CurrentNavigationPage = page as NavigationPage;
                                CurrentPage = child;
                                return;
                            }
                        }
                    }
                }
            }
            CurrentNavigationPage = Application.Current.MainPage as CustomNavigationView;
        }

        public Task InitializeAsync()
        {
            return NavigateToAsync<MainViewModel>();
        }

        public Task NavigateToAsync<TViewModel>(BaseViewModel currentVm = null) where TViewModel : BaseViewModel
        {
            InitCurrentNavigationPage(currentVm);

            CurrentViewModel = currentVm;
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(object parameter, BaseViewModel currentVm = null) where TViewModel : BaseViewModel
        {
            InitCurrentNavigationPage(currentVm);
            
            CurrentViewModel = currentVm;
         
            return InternalNavigateToAsync(typeof(TViewModel), parameter);
        }

        public Task RemoveLastFromBackStackAsync()
        {

            if (CurrentNavigationPage != null)
            {
                CurrentNavigationPage.Navigation.RemovePage(
                    CurrentNavigationPage.Navigation.NavigationStack[CurrentNavigationPage.Navigation.NavigationStack.Count - 2]);
            }

            return Task.FromResult(true);
        }

        public Task RemoveBackStackAsync()
        {

            if (CurrentNavigationPage != null)
            {
                for (int i = 0; i < CurrentNavigationPage.Navigation.NavigationStack.Count - 1; i++)
                {
                    var page = CurrentNavigationPage.Navigation.NavigationStack[i];
                    CurrentNavigationPage.Navigation.RemovePage(page);
                }
            }

            return Task.FromResult(true);
        }

        private async Task InternalNavigateToAsync(Type viewModelType, object parameter)
        {
            if (CurrentNavigationPage == null)
            {
                CurrentNavigationPage = Application.Current.MainPage as CustomNavigationView;
            }
            Page page = CreatePage(viewModelType, parameter);

            if (CurrentNavigationPage != null)
            {
                await CurrentNavigationPage.PushAsync(page);
            }
            else
            {
                Application.Current.MainPage = new CustomNavigationView(page);
            }

            await (page.BindingContext as BaseViewModel).InitializeAsync(parameter);
        }

        private Type GetPageTypeForViewModel(Type viewModelType)
        {
            var viewName = viewModelType.FullName.Replace("ViewModel", "View");
            var viewModelAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;
            var viewAssemblyName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
            var viewType = Type.GetType(viewAssemblyName);
            return viewType;
        }

        private Page CreatePage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);
            if (pageType == null)
            {
                throw new Exception($"Cannot locate page type for {viewModelType}");
            }

            Page page = Activator.CreateInstance(pageType) as Page;
            return page;
        }

        public async Task GoBackAsync()
        {
            await CurrentNavigationPage.PopAsync();
        }
    }
}
