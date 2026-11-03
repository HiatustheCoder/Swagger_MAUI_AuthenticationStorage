
using AuthenticationStorageMobile.ViewModels;
using AuthenticationStorageMobile.Views;

namespace AuthenticationStorageMobile;

public partial class MainPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public MainPage()
    {
        InitializeComponent();

        _viewModel = new LoginViewModel();
        BindingContext = _viewModel;

        _viewModel.LoginSucceeded += OnLoginSucceeded;
    }

    private async void OnLoginSucceeded()
    {
        await Navigation.PushAsync(new ItemPage());
    }
}
