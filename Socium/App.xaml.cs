using Database;
using Socium.AddEdit;
using Socium.Windows;

namespace Socium
{
    public partial class App : Application
    {
        private readonly ApplicationContext _context;

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            await _context.InitializeRolesAsync();

            if (!await _context.IsAdminUserExists())
            {
                MainPage = new CreateAdmin(_context);
            }
            else
            {
                MainPage = new Login(_context);
            }
        }
    }
}
