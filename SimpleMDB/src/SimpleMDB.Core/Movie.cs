namespace SimpleMDB.Core;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public double Rating { get; set; } // 0.0 to 10.0
    public string Description { get; set; } = string.Empty;
}
