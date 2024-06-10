using Microsoft.EntityFrameworkCore;
using Database;
using Database.Models;
namespace Socium.Windows;

public partial class ViewParticipant : ContentPage
{
    private Event _event;
    public ViewParticipant(Event eventItem)
    {
        InitializeComponent();
        _event = eventItem;
        LoadParticipants(eventItem.Id);
    }
    private void LoadParticipants(int eventId)
    {
        using (var context = new ApplicationContext())
        {
            var participants = context.UserEvents
                .Where(ue => ue.EventId == eventId)
                .Include(ue => ue.User)
                .Select(ue => ue.User)
                .ToList();

            ParticipantsList.ItemsSource = participants;
        }
    }
    private async void OnRemoveP(object sender, EventArgs e)
    {
        var button = sender as Button;
        var user = button?.BindingContext as User;
        if (user != null)
        {
            var confirm = await DisplayAlert("Удаление участника", $"Вы уверены, что хотите удалить участника {user.FullName} с мероприятия?", "Да", "Отмена");
            if (confirm)
            {
                using (var context = new ApplicationContext())
                {
                    var userEvent = await context.UserEvents.FirstOrDefaultAsync(ue => ue.EventId == _event.Id && ue.UserId == user.Id);
                    if (userEvent != null)
                    {
                        context.UserEvents.Remove(userEvent);
                        await context.SaveChangesAsync();
                        await DisplayAlert("Успех", $"Участник {user.FullName} успешно удален с мероприятия", "OK");
                        LoadParticipants(_event.Id);
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", $"Участник {user.FullName} не найден на мероприятии", "OK");
                    }
                }
            }
        }
    }
    private async void OnUserTapped(object sender, EventArgs e)
    {
        var user = (sender as StackLayout)?.BindingContext as User;
        if (user != null)
        {
            await Navigation.PushAsync(new UserDetail(user));
        }
    }
}