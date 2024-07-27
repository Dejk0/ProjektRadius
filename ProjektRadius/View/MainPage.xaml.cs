using ProjektRadius.ViewModel;

namespace ProjektRadius
{
    public partial class MainPage : ContentPage
    {


        public MainPage(ViewModel.ViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            
        }

    }
    }


