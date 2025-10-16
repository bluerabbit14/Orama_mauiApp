namespace Orama
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            if(IsUserLoggedIn())
            {
                return new Window(new AppShell());
            }
            else 
            {
                return new Window(new AuthenticationShell());
            }    
        }
        private bool IsUserLoggedIn()
        {
            return false;
        }
    }
}