using Database;
using Microsoft.EntityFrameworkCore;

namespace Socium.Windows
{
    public partial class Login : ContentPage
    {
        private readonly ApplicationContext _context;
        public Login()
        {
            InitializeComponent();
            _context = new ApplicationContext();
            PasswordEntry.Completed += EnterPressed;
        }

        public Login(ApplicationContext context)
            : this() 
        {
            _context = context;
        }

        private async void LoginB(object sender, EventArgs e)
        {
            await PerformLogin();
        }

        private async void EnterPressed(object sender, EventArgs e)
        {
            await PerformLogin();
        }

        private async Task PerformLogin()
        {
            using (var context = new ApplicationContext())
            {
                string login = LoginEntry.Text;
                string password = PasswordEntry.Text;

                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    ErrorLabel.Text = "Логин и пароль должны быть заполнены.";
                    ErrorLabel.IsVisible = true;
                    return;
                }

                var user = await context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserName == login);

                if (user == null || !user.VerifyP(password))
                {
                    ErrorLabel.Text = "Неверный логин или пароль.";
                    ErrorLabel.IsVisible = true;
                    return;
                }
                await Navigation.PushAsync(new Menu(user));
            }
        }

        private async void RegisterB(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Registration());
        }
    }
}
