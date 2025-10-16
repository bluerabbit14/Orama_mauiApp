using Orama.Interfaces;
using Orama.ViewModels;

namespace Orama.Views;

public partial class Login : ContentPage
{
    private readonly INavigationService _navigationService;
    public Login(INavigationService navigationService)
	{
		InitializeComponent();
		_navigationService = navigationService;
		BindingContext = new LoginViewModel(_navigationService);
	}
}