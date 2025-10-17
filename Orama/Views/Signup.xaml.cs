using Orama.Interfaces;
using Orama.ViewModels;

namespace Orama.Views;

public partial class Signup : ContentPage
{
    private readonly INavigationService _navigationService;
    public Signup(INavigationService navigationService)
	{
		InitializeComponent();
        _navigationService = navigationService;
        BindingContext = new SignupViewModel(_navigationService);
    }
}