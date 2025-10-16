using Orama.Views;

namespace Orama
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register a route (Explicit: Useful for pages not directly in the Shell hierarchy) 
            Routing.RegisterRoute(nameof(Dashboard), typeof(Dashboard));
            Routing.RegisterRoute(nameof(Setting), typeof(Setting));
            Routing.RegisterRoute(nameof(Profile), typeof(Profile));

        }
    }
}
