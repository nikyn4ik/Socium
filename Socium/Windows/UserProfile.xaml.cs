using Database.Models;

namespace Socium.Windows
{
    public partial class UserProfile : ContentPage
    {
        private User _user;
        public event EventHandler<User> ProfileUpdated;
        public UserProfile(User user)
        {
            InitializeComponent();
            _user = user ?? throw new ArgumentNullException(nameof(user));

            InitializeUserProfile();
        }

        private void InitializeUserProfile()
        {
            if (!string.IsNullOrWhiteSpace(_user.FullName))
            {
                UserFullName.Text = _user.FullName;
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
            UserDescription.Text = _user.Description ?? string.Empty;

            if (_user.Role.Name == "Admin")
            {
                ProfileTitle.Text = "Профиль администратора";
            }
            else
            {
                ProfileTitle.Text = "Профиль пользователя";
            }
        }

        private async void EditProfileB(object sender, EventArgs e)
        {
            var editProfilePage = new EditProfile(_user);
            editProfilePage.ProfileUpdated += OnProfileUpdated;
            await Navigation.PushAsync(editProfilePage);
        }
        private void OnProfileUpdated(object sender, User updatedUser)
        {
            _user = updatedUser;
            InitializeUserProfile();
            ProfileUpdated?.Invoke(this, updatedUser);
        }
    }
}
