namespace Smdb.Core.Movies;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int Year { get; set; }
    public string Description { get; set; } = "";
    public string Genre { get; set; } = "";
    public double Rating { get; set; }

    public Movie() { }

    public Movie(int id, string title, int year, string description)
    {
        Id = id;
        Title = title;
        Year = year;
        Description = description;
    }
}
