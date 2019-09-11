using NavigationTabbed.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace NavigationTabbed.ViewModels
{
    public static class ViewModelLocator
    {

        public static readonly BindableProperty AutoWireViewModelProperty =
            BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutoWireViewModelChanged);

        public static bool GetAutoWireViewModel(BindableObject bindable)
        {
            return (bool)bindable.GetValue(ViewModelLocator.AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(BindableObject bindable, bool value)
        {
            bindable.SetValue(ViewModelLocator.AutoWireViewModelProperty, value);
        }

        public static bool UseMockService { get; set; }

        static ViewModelLocator()
        {

            DependencyService.Register<MainViewModel>();
           

            DependencyService.Register<NavigationService>();
           
        }

        public static void UpdateDependencies(bool useMockServices)
        {
          
        }

        public static void RegisterSingleton<TInterface, T>() where TInterface : class where T : class, TInterface
        {
            DependencyService.Register<TInterface, T>();
        }

        public static T Resolve<T>() where T : class
        {
            return DependencyService.Resolve<T>();
        }

        private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as Element;
            if (view == null)
            {
                return;
            }

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName);

            Type viewModelType = Type.GetType(viewModelName);
            if (viewModelType == null)
            {
                return;
            }
            var utilType = typeof(DependencyService);
            var baseMethod = utilType.GetMethod("Resolve", new Type[] { viewModelType });
            var typedViewModel = baseMethod.MakeGenericMethod(viewModelType.GetType());
            var viewModel = typedViewModel.Invoke(null, new[] { viewModelType });
            view.BindingContext = viewModel;
        }
    }
}
