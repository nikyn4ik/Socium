namespace Socium.Windows;
using Database;
using Microsoft.EntityFrameworkCore;

public partial class ViewFeedbacks : ContentPage
{
    public ViewFeedbacks()
    {
        InitializeComponent();
        LoadReviews();
    }

    private void LoadReviews()
    {
        using (var context = new ApplicationContext())
        {
            var reviews = context.Reviews
                                 .Include(r => r.User)
                                 .Include(r => r.Event)
                                 .ToList();

            var reviewViews = reviews.Select(r => new ReviewView
            {
                UserFullName = $"{r.User.LastName} {r.User.FirstName} {r.User.MiddleName}",
                EventName = r.Event.Title,
                Rating = r.Rating,
                Content = r.Content,
                Date = r.Date
            });

            ReviewsList.ItemsSource = reviewViews;
        }
    }
}
public class ReviewView
{
    public string UserFullName { get; set; }
    public string EventName { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
}
