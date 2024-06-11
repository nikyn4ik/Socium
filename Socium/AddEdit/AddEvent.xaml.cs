using Database;
using Database.Models;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Socium.AddEdit
{
    public partial class AddEvent : ContentPage
    {
        public event EventHandler<Event> EventAdded;
        private Event newEvent = new Event();
        private string _selectedImagePath;
        private string ImageFolderPath => Path.Combine(AppContext.BaseDirectory, "Images");

        public AddEvent()
        {
            InitializeComponent();
            LoadImages();
        }

        private void LoadImages()
        {
            var imageFolderPath = ImageFolderPath;

            if (Directory.Exists(imageFolderPath))
            {
                var imageFiles = Directory.GetFiles(imageFolderPath);
                foreach (var imagePath in imageFiles)
                {
                    var image = new Image { Source = ImageSource.FromFile(imagePath) };
                    image.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => ImageSelected(imagePath)) });
                    ImageStackLayout.Children.Add(image);
                }
            }
        }

        private async void SelectImageB(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync();
                if (result != null)
                {
                    _selectedImagePath = result.FullPath;
                    EventImage.Source = ImageSource.FromFile(_selectedImagePath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось выбрать изображение: {ex.Message}", "OK");
            }
        }

        private async void ImageSelected(string selectedImagePath)
        {
            try
            {
                _selectedImagePath = selectedImagePath;
                await DisplayAlert("Успех", "Изображение выбрано", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось выбрать изображение: {ex.Message}", "OK");
            }
        }

        private async void OnAddEventB(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text) || string.IsNullOrWhiteSpace(DescriptionEditor.Text) ||
                DatePicker.Date == null || TimePicker.Time == null || string.IsNullOrWhiteSpace(_selectedImagePath))
            {
                await DisplayAlert("Ошибка", "Заполните все обязательные поля", "OK");
                return;
            }

            using (var context = new ApplicationContext())
            {
                var newEvent = new Event
                {
                    Title = NameEntry.Text,
                    Description = DescriptionEditor.Text,
                    Date = DatePicker.Date,
                    Time = TimePicker.Time,
                    Location = LocationEntry.Text,
                    ImagePath = _selectedImagePath
                };

                context.Events.Add(newEvent);
                await context.SaveChangesAsync();
                EventAdded?.Invoke(this, newEvent);
                await DisplayAlert("Успех", "Мероприятие добавлено", "OK");
                await Navigation.PopAsync();
            }
        }
    }
}
