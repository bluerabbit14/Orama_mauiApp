using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Orama.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Orama.ViewModels
{
    public partial class SignupViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        [ObservableProperty]
        private bool _acceptTerms;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private string _passwordStrengthMessage = string.Empty;

        [ObservableProperty]
        private Color _passwordStrengthColor = Colors.Gray;

        partial void OnFullNameChanged(string value)
        {
            // Clear error message when user starts typing
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = string.Empty;
            }
        }
        partial void OnEmailChanged(string value)
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
            UpdatePasswordStrength();
        }
        partial void OnConfirmPasswordChanged(string value)
        {
            // Clear error message when user starts typing
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = string.Empty;
            }
        }
        partial void OnPhoneNumberChanged(string value)
        {
            // Clear error message when user starts typing
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = string.Empty;
            }
        }
        partial void OnAcceptTermsChanged(bool value)
        {
            // Clear error message when user starts typing
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = string.Empty;
            }
        }

        public SignupViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task SignupAsync()
        {
            if (!ValidateForm())
                return;

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                // Simulate API call
                await Task.Delay(2000);
                
                // Navigate to dashboard or show success message
                await Application.Current.MainPage.DisplayAlert("Success", "Account created successfully!", "OK");
                //await _navigationService.NavigateToAsync("Dashboard");
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during signup. Please try again.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ShowTermsAsync()
        {
            await Application.Current.MainPage.DisplayAlert("Terms & Conditions", "Terms & Conditions content would be displayed here.", "OK");
        }

        [RelayCommand]
        private async Task ShowPrivacyAsync()
        {
            await Application.Current.MainPage.DisplayAlert("Privacy Policy", "Privacy Policy content would be displayed here.", "OK");
        }
        private void UpdatePasswordStrength()
        {
            if (string.IsNullOrEmpty(Password))
            {
                PasswordStrengthMessage = string.Empty;
                PasswordStrengthColor = Colors.Gray;
                return;
            }

            int score = CalculatePasswordStrength(Password);
            
            switch (score)
            {
                case 0:
                case 1:
                    PasswordStrengthMessage = "Weak";
                    PasswordStrengthColor = Colors.Red;
                    break;
                case 2:
                case 3:
                    PasswordStrengthMessage = "Medium";
                    PasswordStrengthColor = Colors.Orange;
                    break;
                case 4:
                case 5:
                    PasswordStrengthMessage = "Strong";
                    PasswordStrengthColor = Colors.Green;
                    break;
            }
        }

        private int CalculatePasswordStrength(string password)
        {
            int score = 0;
            
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;
            if (Regex.IsMatch(password, @"[a-z]")) score++;
            if (Regex.IsMatch(password, @"[A-Z]")) score++;
            if (Regex.IsMatch(password, @"[0-9]")) score++;
            if (Regex.IsMatch(password, @"[^a-zA-Z0-9]")) score++;
            
            return score;
        }

        private bool ValidateForm()
        {
            ErrorMessage = string.Empty;

            // Required field validation
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Full name is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Password is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "Please confirm your password.";
                return false;
            }

            // Email format validation
            if (!IsValidEmail(Email))
            {
                ErrorMessage = "Please enter a valid email address.";
                return false;
            }

            // Password match validation
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return false;
            }

            // Password strength validation
            if (CalculatePasswordStrength(Password) < 2)
            {
                ErrorMessage = "Password is too weak. Please use at least 8 characters with a mix of letters, numbers, and symbols.";
                return false;
            }

            // Terms acceptance validation
            if (!AcceptTerms)
            {
                ErrorMessage = "Please accept the Terms & Conditions to continue.";
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
