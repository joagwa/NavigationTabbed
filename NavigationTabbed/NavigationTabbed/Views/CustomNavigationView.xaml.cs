using NavigationTabbed.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NavigationTabbed.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomNavigationView : NavigationPage, ICustomNavigationView
    {
        public CustomNavigationView() : base()
        {
            InitializeComponent();
        }

        public CustomNavigationView(Page root) : base(root)
        {
            InitializeComponent();
        }
    }
}