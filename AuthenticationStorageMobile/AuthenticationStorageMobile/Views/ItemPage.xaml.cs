
using AuthenticationStorageMobile.ViewModels;

namespace AuthenticationStorageMobile.Views;

public partial class ItemPage : ContentPage
{
    private readonly ItemViewModel _viewModel;

    public ItemPage()
    {
        InitializeComponent();

        _viewModel = new ItemViewModel();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadItemsAsync();
    }
}
