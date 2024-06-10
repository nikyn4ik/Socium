using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
    public class EventView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string ImagePath { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsNotAdmin => !IsAdmin;

        public EventView(Event e, bool isAdmin)
        {
            Id = e.Id;
            Title = e.Title;
            Description = e.Description;
            Date = e.Date;
            Location = e.Location;
            ImagePath = e.ImagePath;
            IsAdmin = isAdmin;
        }
    }
}