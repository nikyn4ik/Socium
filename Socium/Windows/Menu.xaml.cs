using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Database;
using Database.Models;
using Socium.AddEdit;

namespace Socium.Windows
{
    public partial class Menu : ContentPage, INotifyPropertyChanged
    {
        private User _currentUser;
        public ObservableCollection<EventView> Events { get; set; }
        public bool IsAdmin { get; set; }
        private string _fullName;
        public int UserId { get; set; }
        public string FullName
        {
            get => _fullName;
            set
            {
                if (_fullName != value)
                {
                    _fullName = value;
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }

        public Menu(User user)
        {
            InitializeComponent();
            UserId = user.Id;
            _currentUser = user;
            IsAdmin = user.Role.Name == "Admin";
            FullName = user.FullName;
            UserFullNameLabel.Text = user.FullName;
            if (_currentUser.ImageData != null && _currentUser.ImageData.Length > 0)
            {
                UserPhoto.Source = ImageSource.FromStream(() => new MemoryStream(_currentUser.ImageData));
            }

            LoadEvents();

            this.BindingContext = this;
            var editProfilePage = new EditProfile(_currentUser);
        }
        private void OnProfileUpdated(object sender, User updatedUser)
        {
            _currentUser = updatedUser;
            FullName = updatedUser.FullName;
            UserFullNameLabel.Text = updatedUser.FullName;
            if (_currentUser.ImageData != null && _currentUser.ImageData.Length > 0)
            {
                UserPhoto.Source = ImageSource.FromStream(() => new MemoryStream(_currentUser.ImageData));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadEvents()
        {
            using (var context = new ApplicationContext())
            {
                var events = GetEvents(context);

                Events = new ObservableCollection<EventView>(
                    events.Select(e => new EventView(e, IsAdmin))
                );

                EventList.ItemsSource = Events;
            }
        }

        private List<Event> GetEvents(ApplicationContext context)
        {
            return context.Events.Include(e => e.UserEvents).ThenInclude(ue => ue.User).ToList();
        }

        private async void RegisterB(object sender, EventArgs e)
        {
            var button = sender as Button;
            var eventItem = button?.BindingContext as EventView;
            if (eventItem != null)
            {
                using (var context = new ApplicationContext())
                {
                    var existingUserEvent = await context.UserEvents.FirstOrDefaultAsync(ue => ue.UserId == _currentUser.Id && ue.EventId == eventItem.Id);

                    if (existingUserEvent != null)
                    {
                        await DisplayAlert("Ошибка", "Вы уже зарегистрированы на это мероприятие", "OK");
                        return;
                    }

                    var userEvent = new UserEvent
                    {
                        UserId = _currentUser.Id,
                        EventId = eventItem.Id
                    };

                    context.UserEvents.Add(userEvent);
                    await context.SaveChangesAsync();
                    await DisplayAlert("Успех", "Вы успешно откликнулись на мероприятие", "OK");
                }
            }
        }

        private async void EditEventB(object sender, EventArgs e)
        {
            var button = sender as Button;
            var eventItem = button?.BindingContext as EventView;
            if (eventItem != null)
            {
                using (var context = new ApplicationContext())
                {
                    var eventEntity = await context.Events.FindAsync(eventItem.Id);
                    if (eventEntity != null)
                    {
                        await Navigation.PushAsync(new EditEvent(eventEntity));
                    }
                }
            }
        }

        private async void ViewParticipantsB(object sender, EventArgs e)
        {
            var button = sender as Button;
            var eventItem = button?.BindingContext as EventView;
            if (eventItem != null)
            {
                using (var context = new ApplicationContext())
                {
                    var eventEntity = await context.Events.FindAsync(eventItem.Id);
                    if (eventEntity != null)
                    {
                        await Navigation.PushAsync(new ViewParticipant(eventEntity));
                    }
                }
            }
        }

        private async void ViewFeedbacksB(object sender, EventArgs e)
        {
            var button = sender as Button;
            var eventItem = button?.BindingContext as EventView;
            if (eventItem != null)
            {
                using (var context = new ApplicationContext())
                {
                    var eventEntity = await context.Events.FindAsync(eventItem.Id);
                    if (eventEntity != null)
                    {
                        await Navigation.PushAsync(new ViewFeedbacks());
                    }
                }
            }
        }

        private async void LeaveFeedbackB(object sender, EventArgs e)
        {
            var button = sender as Button;
            var eventItem = button?.BindingContext as EventView;
            if (eventItem != null)
            {
                using (var context = new ApplicationContext())
                {
                    var eventEntity = await context.Events.FindAsync(eventItem.Id);
                    if (eventEntity != null)
                    {
                        var existingReview = await context.Reviews
                                                          .FirstOrDefaultAsync(r => r.EventId == eventEntity.Id && r.UserId == _currentUser.Id);

                        if (existingReview != null)
                        {
                            await DisplayAlert("Ошибка", "Вы уже оставили отзыв на это мероприятие", "OK");
                            return;
                        }

                        await Navigation.PushAsync(new LeaveFeedback(eventEntity, _currentUser.Id));
                    }
                }
            }
        }

        private async void DeleteEventB(object sender, EventArgs e)
        {
            var button = sender as Button;
            var eventItem = button?.BindingContext as EventView;
            if (eventItem != null)
            {
                bool confirmed = await DisplayAlert("Подтверждение", "Вы уверены, что хотите удалить это мероприятие?", "Да", "Нет");
                if (confirmed)
                {
                    using (var context = new ApplicationContext())
                    {
                        var eventEntity = await context.Events.FindAsync(eventItem.Id);
                        if (eventEntity != null)
                        {
                            context.Events.Remove(eventEntity);
                            await context.SaveChangesAsync();
                            Events.Remove(eventItem);
                            await DisplayAlert("Успех", "Мероприятие успешно удалено", "OK");
                        }
                    }
                }
            }
        }

        private void OnEventAdded(object sender, Event newEvent)
        {
            Events.Add(new EventView(newEvent, IsAdmin));
        }

        private async void OnAddEventClicked(object sender, EventArgs e)
        {
            var AddEvent = new AddEvent();
            AddEvent.EventAdded += OnEventAdded;
            await Navigation.PushAsync(AddEvent);
        }

        private async void OnProfileB(object sender, EventArgs e)
        {
            var userProfilePage = new UserProfile(_currentUser);
            userProfilePage.ProfileUpdated += UserProfileUpdated;
            await Navigation.PushAsync(userProfilePage);
        }
        private void UserProfileUpdated(object sender, User updatedUser)
        {
            _currentUser = updatedUser;
            FullName = updatedUser.FullName;
            UserFullNameLabel.Text = updatedUser.FullName;
            if (_currentUser.ImageData != null && _currentUser.ImageData.Length > 0)
            {
                UserPhoto.Source = ImageSource.FromStream(() => new MemoryStream(_currentUser.ImageData));
            }
        }
        private void SearchEvents(object sender, TextChangedEventArgs e)
        {
            FilterEvents(e.NewTextValue);
        }

        private void FilterEvents(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                EventList.ItemsSource = Events;
            }
            else
            {
                var filteredEvents = Events.Where(ev =>
                    ev.Title.ToLower().Contains(searchText.ToLower()) ||
                    ev.Description.ToLower().Contains(searchText.ToLower()) ||
                    ev.Location.ToLower().Contains(searchText.ToLower())
                ).ToList();

                EventList.ItemsSource = new ObservableCollection<EventView>(filteredEvents);
            }
        }
    }
}