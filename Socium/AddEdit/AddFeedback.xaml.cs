using Database.Models;
using Database;

namespace Socium.AddEdit
{
    public partial class AddFeedback : ContentPage
    {
        private Event _event;

        public AddFeedback(Event eventItem)
        {
            InitializeComponent();
            _event = eventItem;
        }

        private async void LeaveFeedbackB(object sender, EventArgs e)
        {
            double rating = Ratings.Value;
            string likes = Like.Text;
            string improvements = Improvement.Text;

            var review = new Review
            {
                EventId = _event.Id,
                Content = $"������: {rating:F1}, �����������: {likes}, ���������: {improvements}",
                Date = DateTime.Now
            };

            using (var context = new ApplicationContext())
            {
                context.Reviews.Add(review);
                await context.SaveChangesAsync();
            }

            await DisplayAlert("�����", "����� ������� ��������", "OK");

            await Navigation.PopAsync();
        }
    }
}
