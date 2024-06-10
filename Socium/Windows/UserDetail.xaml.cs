namespace Socium.Windows;
using Database.Models;

public partial class UserDetail : ContentPage
{
	public UserDetail(User user)
    {
        InitializeComponent();
        BindingContext = user;

        if (user.ImageData != null)
        {
            var image = new Image
            {
                Source = ImageSource.FromStream(() => new System.IO.MemoryStream(user.ImageData)),
                WidthRequest = 200,
                HeightRequest = 200,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            stackLayout.Children.Add(image);
        }
    }
}