using Database;
using Database.Models;

namespace Socium.AddEdit;

public partial class LeaveFeedback : ContentPage
{
    private Event _event;
    private int _userId;

    public LeaveFeedback(Event eventItem, int userId)
    {
        InitializeComponent();
        _event = eventItem;
        _userId = userId;
    }
    private async void LeaveFeedbackB(object sender, EventArgs e)
    {
        using (var context = new ApplicationContext())
        {
            var eventEntity = await context.Events.FindAsync(_event.Id);
            if (eventEntity != null)
            {
                var review = new Review
                {
                    EventId = eventEntity.Id,
                    UserId = _userId,
                    Content = $"{LikesEditor.Text}\n{DislikesEditor.Text}\n{ImprovementsEditor.Text}",
                    Rating = (int)RatingStepper.Value,
                    Date = DateTime.Now
                };

                context.Reviews.Add(review);
                await context.SaveChangesAsync();

                await DisplayAlert("Успех", "Ваш отзыв успешно отправлен", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Ошибка", "Мероприятие не найдено", "OK");
                await Navigation.PopAsync();
            }
        }
    }
}
