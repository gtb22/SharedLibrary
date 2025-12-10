namespace Smdb.Core.Actors;

public class Actor
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int BirthYear { get; set; }
    public string Bio { get; set; } = "";

    public Actor() { }

    public Actor(int id, string name, int birthYear)
    {
        Id = id;
        Name = name;
        BirthYear = birthYear;
    }
}
