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

        partial void OnUserEmailChanged(string value)
        {
            // Clear error message when user starts typing
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = string.Empty;
            }
        }
        partial void OnPasswordChanged(string value)
        {
            // Clear error message when user starts typing
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = string.Empty;
            }
        }

        public LoginViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(UserEmail))
            {
                ErrorMessage = "Email cannot be empty";
                return;
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Password cannot be empty";
                return;
            }

            // Clear any previous error messages
            ErrorMessage = string.Empty;
            IsLoading = true;

            try
            {
                // Simulate API call for login
                await Task.Delay(2000);
                
                // For demo purposes, simple validation
                if (UserEmail.ToLower() == "14asifcr7@gmail.com" && Password == "Admin@123")
                {
                    // Successful login
                    await Application.Current.MainPage.DisplayAlert("Success", "Login successful!", "OK");
                    
                }
                else
                {
                    ErrorMessage = "Invalid email or password. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during login. Please try again.";
            }
            finally
            {
                IsLoading = false;
            }
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
