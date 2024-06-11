using Database;
using Socium.AddEdit;
using Socium.Windows;
using System.Diagnostics;
using System.Globalization;

namespace Socium
{
    public partial class App : Application
    {
        private readonly ApplicationContext _context;

        public App()
        {
            InitializeComponent();

            var culture = new CultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            MainPage = new AppShell();
            AppDomain.CurrentDomain.ProcessExit += OnAppExit;
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

        private void OnAppExit(object sender, EventArgs e)
        {
            KillProcess("sqlceip");
            KillProcess("sqlservr");
        }

        private void KillProcess(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            foreach (var process in processes)
            {
                try
                {
                    if (process.MainModule.FileName.Contains("MSSQLLocalDB"))
                    {
                        process.Kill();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error killing process {processName}: {ex.Message}");
                }
            }
        }
    }
}
