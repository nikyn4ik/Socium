using Database;
using Database.Models;

namespace Socium.AddEdit
{
    public partial class EditEvent : ContentPage
    {
        private Event _event;

        public event EventHandler<Event> EventUpdated;

        public EditEvent(Event eventEntity)
        {
            InitializeComponent();
            _event = eventEntity;
            LoadEventData();
        }

        private void LoadEventData()
        {
            NameEntry.Text = _event.Title;
            DescriptionEditor.Text = _event.Description;
            DatePicker.Date = _event.Date;
            LocationEntry.Text = _event.Location;
            EventImage.Source = _event.ImagePath;
        }

        private async void SelectImageB(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Выберите изображение для события"
            });

            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                var imagePath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);

                using (var fileStream = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
                {
                    await stream.CopyToAsync(fileStream);
                }

                EventImage.Source = imagePath;
                _event.ImagePath = imagePath;
            }
        }

        private async void SaveEventB(object sender, EventArgs e)
        {
            using (var context = new ApplicationContext())
            {
                var eventToUpdate = await context.Events.FindAsync(_event.Id);
                if (eventToUpdate != null)
                {
                    eventToUpdate.Title = NameEntry.Text;
                    eventToUpdate.Description = DescriptionEditor.Text;
                    eventToUpdate.Date = DatePicker.Date;
                    eventToUpdate.Location = LocationEntry.Text;
                    eventToUpdate.ImagePath = _event.ImagePath;

                    context.Events.Update(eventToUpdate);
                    await context.SaveChangesAsync();
                    EventUpdated?.Invoke(this, _event);
                    await DisplayAlert("Успех", "Изменения сохранены", "OK");
                    await Navigation.PopAsync();
                }
            }
        }
    }
}
