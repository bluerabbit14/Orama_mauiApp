using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orama.Interfaces;

namespace Orama.ViewModels
{
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private string _successMessage = string.Empty;

        partial void OnEmailChanged(string value)
        {
            // Clear error message when user starts typing
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = string.Empty;
            }
        }

        public ForgotPasswordViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task SendResetLinkAsync()
        {
            if (!ValidateEmail())
                return;

            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            try
            {
                // Simulate API call
                await Task.Delay(2000);
                
                SuccessMessage = "Password reset link has been sent to your email address.";
                
                // Optional: Navigate back to login after a delay
                await Task.Delay(3000);
                await _navigationService.NavigateToAsync("Login");
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while sending the reset link. Please try again.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToLoginAsync()
        {
            await _navigationService.NavigateToAsync("Login");
        }

        [RelayCommand]
        private async Task NavigateToSignupAsync()
        {
            await _navigationService.NavigateToAsync("Signup");
        }

        private bool ValidateEmail()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email address is required.";
                return false;
            }

            if (!IsValidEmail(Email))
            {
                ErrorMessage = "Please enter a valid email address.";
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
