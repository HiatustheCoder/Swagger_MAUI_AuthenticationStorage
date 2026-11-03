
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AuthenticationStorageMobile.Models;
using AuthenticationStorageMobile.Services;

namespace AuthenticationStorageMobile.ViewModels;

public class ItemViewModel : INotifyPropertyChanged
{
    private readonly ApiService _api = new();

    private string _itemID = "";
    private string _itemName = "";
    private string _itemDescription = "";
    private string _message = "";
    private bool _isBusy;

    public string ItemID
    {
        get => _itemID;
        set { _itemID = value; Notify(); }
    }

    public string ItemName
    {
        get => _itemName;
        set { _itemName = value; Notify(); }
    }

    public string ItemDescription
    {
        get => _itemDescription;
        set { _itemDescription = value; Notify(); }
    }

    public string Message
    {
        get => _message;
        set { _message = value; Notify(); }
    }

    public ObservableCollection<Item> Items { get; } = new();

    public ICommand SaveCommand { get; }
    public ICommand RefreshCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ItemViewModel()
    {
        SaveCommand = new Command(async () => await SaveAsync());
        RefreshCommand = new Command(async () => await LoadItemsAsync());
    }

    public async Task LoadItemsAsync()
    {
        if (_isBusy) return;

        _isBusy = true;

        try
        {
            var items = await _api.GetItemsAsync();

            Items.Clear();

            foreach (var item in items)
                Items.Add(item);
        }
        catch (Exception)
        {
            Message = "Unable to retrieve items from the API.";
        }
        finally
        {
            _isBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        if (_isBusy) return;

        if (string.IsNullOrWhiteSpace(ItemID) ||
            string.IsNullOrWhiteSpace(ItemName) ||
            string.IsNullOrWhiteSpace(ItemDescription))
        {
            Message = "All fields are required.";
            return;
        }

        _isBusy = true;

        try
        {
            var item = new Item
            {
                ItemID = ItemID.Trim(),
                ItemName = ItemName.Trim(),
                ItemDescription = ItemDescription.Trim()
            };

            bool saved = await _api.SaveItemAsync(item);

            if (!saved)
            {
                Message = "Failed to save item.";
                return;
            }

            ItemID = "";
            ItemName = "";
            ItemDescription = "";

            var items = await _api.GetItemsAsync();

            Items.Clear();

            foreach (var savedItem in items)
                Items.Add(savedItem);

            Message = "Item saved successfully!";
        }
        catch (Exception)
        {
            Message = "Unable to communicate with the API.";
        }
        finally
        {
            _isBusy = false;
        }
    }

    private void Notify([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(
            this, new PropertyChangedEventArgs(name));
    }
}
