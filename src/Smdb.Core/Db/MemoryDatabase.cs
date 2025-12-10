using Smdb.Core.Movies;
using Smdb.Core.Actors;
using Smdb.Core.ActorMovies;
using Smdb.Core.Users;

namespace Smdb.Core.Db;

public class MemoryDatabase
{
    private static MemoryDatabase? _instance;
    public static MemoryDatabase Instance => _instance ??= new MemoryDatabase();

    public List<Movie> Movies { get; private set; }
    public List<Actor> Actors { get; private set; }
    public List<ActorMovie> ActorMovies { get; private set; }
    public List<User> Users { get; private set; }

    private int _nextMovieId = 1;
    private int _nextActorId = 1;
    private int _nextActorMovieId = 1;
    private int _nextUserId = 1;

    private MemoryDatabase()
    {
        Movies = new List<Movie>();
        Actors = new List<Actor>();
        ActorMovies = new List<ActorMovie>();
        Users = new List<User>();

        SeedMovies();
        SeedActors();
        SeedUsers();
    }

    public int NextMovieId() => ++_nextMovieId;
    public int NextActorId() => ++_nextActorId;
    public int NextActorMovieId() => ++_nextActorMovieId;
    public int NextUserId() => ++_nextUserId;

    private void SeedMovies()
    {
        Movies.AddRange(new Movie[]
        {
            new Movie(1, "The Godfather", 1972, "A mafia patriarch hands the family empire to his reluctant son."),
            new Movie(2, "The Godfather Part II", 1974, "Michael consolidates power as flashbacks trace Vito Corleone's rise."),
            new Movie(3, "The Dark Knight", 2008, "Batman faces the Joker, who pushes Gotham into chaos."),
            new Movie(4, "The Shawshank Redemption", 1994, "An innocent banker forms a life-saving friendship in prison."),
            new Movie(5, "Pulp Fiction", 1994, "Interlocking LA crime stories unfold with dark humor."),
            new Movie(6, "Schindler's List", 1993, "A businessman saves Jewish workers during the Holocaust."),
            new Movie(7, "The Lord of the Rings: The Return of the King", 2003, "The final push to destroy the One Ring decides Middle-earth's fate."),
            new Movie(8, "Fight Club", 1999, "An insomnia-plagued worker joins a charismatic anarchist's secret club."),
            new Movie(9, "Forrest Gump", 1994, "A kind man unwittingly drifts through historic American moments."),
            new Movie(10, "Inception", 2010, "A thief enters dreams to plant an idea in a target's mind."),
            new Movie(11, "The Matrix", 1999, "A hacker learns reality is a simulated prison for humanity."),
            new Movie(12, "Se7en", 1995, "Two detectives hunt a killer using the seven deadly sins."),
            new Movie(13, "Goodfellas", 1990, "Henry Hill's rise and fall inside the New York mob."),
            new Movie(14, "The Silence of the Lambs", 1991, "An FBI trainee consults Hannibal Lecter to catch a serial killer."),
            new Movie(15, "Star Wars: Episode IV – A New Hope", 1977, "A farm boy joins rebels to destroy the Empire's Death Star."),
            new Movie(16, "The Empire Strikes Back", 1980, "The Rebels scatter as Luke confronts Darth Vader."),
            new Movie(17, "Interstellar", 2014, "Astronauts travel through a wormhole to save a dying Earth."),
            new Movie(18, "Parasite", 2019, "A poor family infiltrates a wealthy household with unforeseen fallout."),
            new Movie(19, "Spirited Away", 2001, "A girl navigates a spirit bathhouse to free her parents."),
            new Movie(20, "City of God", 2002, "Two boys take diverging paths amid Rio's gang wars."),
            new Movie(21, "Saving Private Ryan", 1998, "A squad risks everything to bring a paratrooper home."),
            new Movie(22, "The Green Mile", 1999, "Death-row guards encounter a prisoner with miraculous gifts."),
            new Movie(23, "Gladiator", 2000, "A betrayed general becomes Rome's fiercest arena fighter."),
            new Movie(24, "The Lion King", 1994, "An exiled lion cub returns to claim his destiny."),
            new Movie(25, "Back to the Future", 1985, "A teen time-travels and risks erasing his own existence."),
            new Movie(26, "The Departed", 2006, "An infiltrator and a mole play cat-and-mouse in Boston."),
            new Movie(27, "Whiplash", 2014, "A jazz drummer endures a brutal mentor in pursuit of greatness."),
            new Movie(28, "The Prestige", 2006, "Rival magicians wage a dangerous war of one-upmanship."),
            new Movie(29, "The Usual Suspects", 1995, "A survivors' tale hints at the legend of Keyser Söze."),
            new Movie(30, "Terminator 2: Judgment Day", 1991, "A reprogrammed cyborg protects the future leader of humanity."),
            new Movie(31, "Alien", 1979, "A crew is stalked by a lethal lifeform aboard a spaceship."),
            new Movie(32, "Aliens", 1986, "Ripley returns to face a hive of xenomorphs on LV-426."),
            new Movie(33, "Blade Runner", 1982, "A detective hunts rogue androids in a neon-soaked future."),
            new Movie(34, "Apocalypse Now", 1979, "A captain journeys upriver to terminate a renegade officer."),
            new Movie(35, "One Flew Over the Cuckoo's Nest", 1975, "A rebel patient challenges a tyrannical nurse in a psych ward."),
            new Movie(36, "Taxi Driver", 1976, "A disturbed NYC cabbie spirals toward violence."),
            new Movie(37, "Oldboy", 2003, "A man seeks answers after 15 years of inexplicable captivity."),
            new Movie(38, "Amélie", 2001, "A shy Parisian decides to secretly improve others' lives."),
            new Movie(39, "The Pianist", 2002, "A Jewish pianist struggles to survive Warsaw's ghetto."),
            new Movie(40, "American Beauty", 1999, "A suburban man's midlife crisis upends his family."),
            new Movie(41, "No Country for Old Men", 2007, "A stolen briefcase triggers relentless pursuit across Texas."),
            new Movie(42, "There Will Be Blood", 2007, "An oilman's ambition consumes everything around him."),
            new Movie(43, "Mad Max: Fury Road", 2015, "A desert chase pits a warlord against a defiant road warrior."),
            new Movie(44, "La La Land", 2016, "A musician and an actress chase dreams in modern LA."),
            new Movie(45, "Joker", 2019, "A marginalized comedian's breakdown sparks violent unrest."),
            new Movie(46, "Avengers: Infinity War", 2018, "Earth's heroes battle Thanos for the fate of half the universe."),
            new Movie(47, "Avengers: Endgame", 2019, "Survivors attempt a time-heist to undo cosmic devastation."),
            new Movie(48, "Toy Story", 1995, "Rivalry between a cowboy doll and a space ranger turns to friendship."),
            new Movie(49, "Inside Out", 2015, "A girl's emotions guide her through a difficult move."),
            new Movie(50, "The Social Network", 2010, "Facebook's founding sparks friendship and legal battles.")
        });
    }

    private void SeedActors()
    {
        Actors.AddRange(new Actor[]
        {
            new Actor(1, "Tom Hanks", 1956),
            new Actor(2, "Leonardo DiCaprio", 1974),
            new Actor(3, "Christian Bale", 1974),
            new Actor(4, "Keanu Reeves", 1964),
            new Actor(5, "Samuel L. Jackson", 1948),
            new Actor(6, "Morgan Freeman", 1937),
            new Actor(7, "Al Pacino", 1940),
            new Actor(8, "Jack Nicholson", 1937),
            new Actor(9, "Harrison Ford", 1942),
            new Actor(10, "Denzel Washington", 1954),
            new Actor(11, "Meryl Streep", 1949),
            new Actor(12, "Robert De Niro", 1943),
            new Actor(13, "Marlon Brando", 1924),
            new Actor(14, "Brad Pitt", 1963),
            new Actor(15, "Johnny Depp", 1963),
            new Actor(16, "Tom Cruise", 1962),
            new Actor(17, "Will Smith", 1968),
            new Actor(18, "Matthew McConaughey", 1969),
            new Actor(19, "Ryan Gosling", 1984),
            new Actor(20, "Christian Nolan", 1970),
            new Actor(21, "Anthony Hopkins", 1937),
            new Actor(22, "Gary Oldman", 1958),
            new Actor(23, "Joaquin Phoenix", 1974),
            new Actor(24, "Timothée Chalamet", 1995),
            new Actor(25, "Oscar Isaac", 1979),
            new Actor(26, "Michael Fassbender", 1977),
            new Actor(27, "Jake Gyllenhaal", 1980),
            new Actor(28, "Chris Evans", 1981),
            new Actor(29, "Robert Downey Jr.", 1965),
            new Actor(30, "Mark Ruffalo", 1966),
            new Actor(31, "Chris Hemsworth", 1983),
            new Actor(32, "Chris Pratt", 1979),
            new Actor(33, "Tom Hiddleston", 1981),
            new Actor(34, "Benedict Cumberbatch", 1976),
            new Actor(35, "Michael B. Jordan", 1987),
            new Actor(36, "Chadwick Boseman", 1977),
            new Actor(37, "Idris Elba", 1972),
            new Actor(38, "Dwayne Johnson", 1972),
            new Actor(39, "Vin Diesel", 1978),
            new Actor(40, "Jason Statham", 1967),
            new Actor(41, "Arnold Schwarzenegger", 1947),
            new Actor(42, "Sylvester Stallone", 1946),
            new Actor(43, "Bruce Willis", 1955),
            new Actor(44, "Jean-Claude Van Damme", 1960),
            new Actor(45, "Jackie Chan", 1954),
            new Actor(46, "Jet Li", 1963),
            new Actor(47, "Pierce Brosnan", 1953),
            new Actor(48, "Daniel Craig", 1968),
            new Actor(49, "Sean Connery", 1930),
            new Actor(50, "Roger Moore", 1927)
        });
    }

    private void SeedUsers()
    {
        Users.Add(new User
        {
            Id = NextUserId(),
            Username = "admin",
            PasswordHash = SimpleHash("admin123"),
            Role = UserRole.Admin
        });

        Users.Add(new User
        {
            Id = NextUserId(),
            Username = "user",
            PasswordHash = SimpleHash("user123"),
            Role = UserRole.Regular
        });
    }

    private static string SimpleHash(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }
}
