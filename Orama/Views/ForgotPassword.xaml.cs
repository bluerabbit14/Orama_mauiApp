using Orama.Interfaces;
using Orama.ViewModels;

namespace Orama.Views;

public partial class ForgotPassword : ContentPage
{
    private readonly INavigationService _navigationService;
    public ForgotPassword(INavigationService navigationService)
	{
		InitializeComponent();
        _navigationService = navigationService;
        BindingContext = new ForgotPasswordViewModel(_navigationService);
    }
}