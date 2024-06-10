using Database.Models;
using Database;
using Socium.Windows;

namespace Socium.AddEdit;

public partial class CreateAdmin : ContentPage
{
    private readonly ApplicationContext _context;
    private readonly Role _adminRole;

    public CreateAdmin(ApplicationContext context)
    {
        InitializeComponent();
        _context = context;
        _adminRole = _context.Roles.FirstOrDefault(r => r.Name == "Admin") ?? new Role { Name = "Admin" };
    }

    private async void CreateAdminB(object sender, EventArgs e)
    {
        string lastName = LastNameEntry.Text;
        string firstName = FirstNameEntry.Text;
        string middleName = MiddleNameEntry.Text;
        string email = EmailEntry.Text;
        string phoneNumber = PhoneNumberEntry.Text;
        string login = LoginEntry.Text;
        string password = PasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;

        if (password != confirmPassword)
        {
            await DisplayAlert("Ошибка", "Пароли не совпадают.", "OK");
            return;
        }

        var user = new User
        {
            LastName = lastName,
            FirstName = firstName,
            MiddleName = middleName,
            UserName = login,
            Email = email,
            PhoneNumber = phoneNumber,
            Role = _adminRole
        };
        user.SetP(password);

        if (_adminRole.Id == 0)
        {
            _context.Roles.Add(_adminRole);
        }
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await DisplayAlert("Успех", "Администратор добавлен в систему!", "OK");
        Application.Current.MainPage = new NavigationPage(new Login(_context));
    }
}
