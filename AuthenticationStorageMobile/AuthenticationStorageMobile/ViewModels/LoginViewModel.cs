
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AuthenticationStorageMobile.Services;

namespace AuthenticationStorageMobile.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly ApiService _api = new();

    private string _username = "";
    private string _password = "";
    private string _message = "";
    private bool _isBusy;

    public string Username
    {
        get => _username;
        set { _username = value; Notify(); }
    }

    public string Password
    {
        get => _password;
        set { _password = value; Notify(); }
    }

    public string Message
    {
        get => _message;
        set { _message = value; Notify(); }
    }

    public ICommand LoginCommand { get; }
    public ICommand CancelCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event Action? LoginSucceeded;

    public LoginViewModel()
    {
        LoginCommand = new Command(async () => await LoginAsync());

        CancelCommand = new Command(() =>
        {
            Username = "";
            Password = "";
            Message = "";
        });
    }

    private async Task LoginAsync()
    {
        if (_isBusy) return;

        if (string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Password))
        {
            Message = "Please enter your username and password.";
            return;
        }

        _isBusy = true;

        try
        {
            bool success = await _api.LoginAsync(Username, Password);

            if (success)
            {
                Message = "Login successful!";
                LoginSucceeded?.Invoke();
            }
            else
            {
                Message = "Login failed. Invalid credentials.";
            }
        }
        catch (Exception)
        {
            Message = "Unable to connect to the API.";
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
