using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Socium.Windows
{
    public partial class Registration : ContentPage
    {
        public Registration()
        {
            InitializeComponent();
        }

        private async void RegisterB(object sender, EventArgs e)
        {
            using (var context = new ApplicationContext())
            {
                string lastName = LastNameEntry.Text;
                string firstName = FirstNameEntry.Text;
                string middleName = MiddleNameEntry.Text;
                string email = EmailEntry.Text;
                string phoneNumber = PhoneNumberEntry.Text;
                string login = LoginEntry.Text;
                string password = PasswordEntry.Text;
                string confirmPassword = ConfirmPasswordEntry.Text;

                if (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(firstName) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phoneNumber) ||
                    string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(confirmPassword))
                {
                    ErrorLabel.Text = "Все поля должны быть заполнены.";
                    ErrorLabel.IsVisible = true;
                    return;
                }
                if (password != confirmPassword)
                {
                    ErrorLabel.Text = "Пароли не совпадают.";
                    ErrorLabel.IsVisible = true;
                    return;
                }

                var existingUser = await context.Users
                    .FirstOrDefaultAsync(u => u.UserName == login || u.Email == email);

                if (existingUser != null)
                {
                    ErrorLabel.Text = "Пользователь с таким логином или email уже существует.";
                    ErrorLabel.IsVisible = true;
                    return;
                }

                var newUser = new User
                {
                    LastName = lastName,
                    FirstName = firstName,
                    MiddleName = middleName,
                    UserName = login,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    RoleId = 2
                };

                newUser.SetP(password); 

                context.Users.Add(newUser);
                await context.SaveChangesAsync();

                await DisplayAlert("Успех", "Регистрация прошла успешно", "OK");
                await Navigation.PopAsync();
            }
        }
    }
}
