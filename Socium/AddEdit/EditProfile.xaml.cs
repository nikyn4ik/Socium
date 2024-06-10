using Database;
using Database.Models;

namespace Socium.Windows
{
    public partial class EditProfile : ContentPage
    {
        private readonly User _user;
        public event EventHandler<User> ProfileUpdated;
        public EditProfile(User user)
        {
            InitializeComponent();
            _user = user ?? throw new ArgumentNullException(nameof(user));

            InitializeEditProfile();
        }

        private void InitializeEditProfile()
        {
            if (!string.IsNullOrWhiteSpace(_user.FullName))
            {
                var fullNameParts = _user.FullName.Split(' ');
                if (fullNameParts.Length == 3)
                {
                    LastNameEntry.Text = fullNameParts[0];
                    FirstNameEntry.Text = fullNameParts[1];
                    MiddleNameEntry.Text = fullNameParts[2];
                }
                else
                {
                    LastNameEntry.Text = _user.LastName;
                    FirstNameEntry.Text = _user.FirstName;
                    MiddleNameEntry.Text = _user.MiddleName;
                }
            }

            if (_user.ImageData != null && _user.ImageData.Length > 0)
            {
                UserPhoto.Source = ImageSource.FromStream(() => new MemoryStream(_user.ImageData));
            }
            else
            {
                UserPhoto.IsVisible = false;
            }

            UserEmail.Text = _user.Email ?? string.Empty;
            UserPhone.Text = _user.PhoneNumber ?? string.Empty;
            UserLogin.Text = _user.UserName ?? string.Empty;
            UserPassword.Text = string.Empty;
            UserDescription.Text = _user.Description ?? string.Empty;
        }

        private async void SelectImageB(object sender, EventArgs e)
        {
            var file = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Выберите изображение"
            });

            if (file != null)
            {
                var stream = await file.OpenReadAsync();

                using (MemoryStream ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    _user.ImageData = ms.ToArray();
                }

                UserPhoto.Source = ImageSource.FromStream(() => new MemoryStream(_user.ImageData));
                UserPhoto.IsVisible = true;
            }
        }

        private async void OnSaveB(object sender, EventArgs e)
        {
            _user.LastName = LastNameEntry.Text;
            _user.FirstName = FirstNameEntry.Text;
            _user.MiddleName = MiddleNameEntry.Text;
            _user.Email = UserEmail.Text;
            _user.PhoneNumber = UserPhone.Text;
            _user.UserName = UserLogin.Text;

            if (!string.IsNullOrWhiteSpace(UserPassword.Text))
            {
                _user.SetP(UserPassword.Text);
            }

            _user.Description = UserDescription.Text;

            using (var context = new ApplicationContext())
            {
                context.Users.Update(_user);
                await context.SaveChangesAsync();
            }
            await Navigation.PopAsync();
            ProfileUpdated?.Invoke(this, _user);
        }
    }
}
