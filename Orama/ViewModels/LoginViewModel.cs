using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orama.Interfaces;
using Orama.Views;

namespace Orama.ViewModels
{
    public partial class LoginViewModel: ObservableObject
    {
        private readonly INavigationService _navigationService;

        // Automatically implements INotifyPropertyChanged
        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private  string _userEmail;

        [ObservableProperty]
        private  string _password;

        [ObservableProperty]
        private string _errorMessage;

        public LoginViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            Application.Current.MainPage.DisplayAlert("Alert","Login clicked","Ok");
        }

        [RelayCommand]
        private async Task ShowForgotPasswordAsync()
        {
            await _navigationService.NavigateToAsync("ForgotPassword");
        }

        [RelayCommand]
        private async Task ShowSignupAsync()
        {
            await _navigationService.NavigateToAsync("Signup");
        }

    }
}
