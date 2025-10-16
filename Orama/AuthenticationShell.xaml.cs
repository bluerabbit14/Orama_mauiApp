using Orama.Views;
namespace Orama;

public partial class AuthenticationShell : Shell
{
	public AuthenticationShell()
	{
		InitializeComponent();

        // Register a route (Explicit: Useful for pages not directly in the Shell hierarchy) 
        Routing.RegisterRoute(nameof(Login), typeof(Login));
        Routing.RegisterRoute(nameof(Signup), typeof(Signup));
        Routing.RegisterRoute(nameof(ForgotPassword), typeof(ForgotPassword));
    }
}