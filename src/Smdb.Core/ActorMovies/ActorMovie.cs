namespace Smdb.Core.ActorMovies;

public class ActorMovie
{
    public int Id { get; set; }
    public int ActorId { get; set; }
    public int MovieId { get; set; }
    public string Role { get; set; } = "";
}
