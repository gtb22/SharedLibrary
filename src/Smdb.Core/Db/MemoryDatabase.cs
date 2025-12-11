using Smdb.Core.Movies;
using Smdb.Core.Users;

namespace Smdb.Core.Db;

public class MemoryDatabase
{
    private static MemoryDatabase? _instance;
    public static MemoryDatabase Instance => _instance ??= new MemoryDatabase();

    public List<Movie> Movies { get; private set; }
    public List<User> Users { get; private set; }

    private int _nextMovieId = 1;
    private int _nextUserId = 1;

    private MemoryDatabase()
    {
        Movies = new List<Movie>();
        Users = new List<User>();

        SeedMovies();
        SeedUsers();
    }

    public int NextMovieId() => ++_nextMovieId;
    public int NextUserId() => ++_nextUserId;

    private void SeedMovies()
    {
        Movies.AddRange(new Movie[]
        {
            new Movie(1, "The Shawshank Redemption", 1994, "A banker wrongly convicted of murder finds hope and friendship while serving time in prison."),
            new Movie(2, "Forrest Gump", 1994, "A man with a low IQ lives an extraordinary life, witnessing key moments in U.S. history while chasing love."),
            new Movie(3, "Gladiator", 2000, "A betrayed Roman general becomes a gladiator, seeking revenge against the emperor who destroyed his life."),
            new Movie(4, "Inception", 2010, "A skilled thief who steals secrets through dreams is tasked with planting an idea in someone's mind."),
            new Movie(5, "Parasite", 2019, "A poor family infiltrates the lives of a wealthy household, exposing deep social inequalities."),
            new Movie(6, "Dune: Part One", 2021, "Adaptation of Frank Herbert's novel where Paul Atreides faces destiny on the desert planet Arrakis."),
            new Movie(7, "Oppenheimer", 2023, "Biopic of J. Robert Oppenheimer, the physicist behind the atomic bomb, exploring moral and political dilemmas."),
            new Movie(8, "Anora", 2024, "A young dancer in New York falls into a whirlwind romance with a Russian oligarch's son, changing her life forever."),
            new Movie(9, "The Dark Knight", 2008, "Batman faces the Joker, who pushes Gotham into chaos."),
            new Movie(10, "Pulp Fiction", 1994, "Interlocking LA crime stories unfold with dark humor."),
            new Movie(11, "Schindler's List", 1993, "A businessman saves Jewish workers during the Holocaust."),
            new Movie(12, "The Lord of the Rings: The Return of the King", 2003, "The final push to destroy the One Ring decides Middle-earth's fate."),
            new Movie(13, "Fight Club", 1999, "An insomnia-plagued worker joins a charismatic anarchist's secret club."),
            new Movie(14, "The Matrix", 1999, "A hacker learns reality is a simulated prison for humanity."),
            new Movie(15, "Se7en", 1995, "Two detectives hunt a killer using the seven deadly sins."),
            new Movie(16, "Goodfellas", 1990, "Henry Hill's rise and fall inside the New York mob."),
            new Movie(17, "The Silence of the Lambs", 1991, "An FBI trainee consults Hannibal Lecter to catch a serial killer."),
            new Movie(18, "Star Wars: Episode IV – A New Hope", 1977, "A farm boy joins rebels to destroy the Empire's Death Star."),
            new Movie(19, "The Empire Strikes Back", 1980, "The Rebels scatter as Luke confronts Darth Vader."),
            new Movie(20, "Interstellar", 2014, "Astronauts travel through a wormhole to save a dying Earth."),
            new Movie(21, "Spirited Away", 2001, "A girl navigates a spirit bathhouse to free her parents."),
            new Movie(22, "Saving Private Ryan", 1998, "A squad risks everything to bring a paratrooper home."),
            new Movie(23, "The Green Mile", 1999, "Death-row guards encounter a prisoner with miraculous gifts."),
            new Movie(24, "The Lion King", 1994, "An exiled lion cub returns to claim his destiny."),
            new Movie(25, "Back to the Future", 1985, "A teen time-travels and risks erasing his own existence."),
            new Movie(26, "The Departed", 2006, "An infiltrator and a mole play cat-and-mouse in Boston."),
            new Movie(27, "Whiplash", 2014, "A jazz drummer endures a brutal mentor in pursuit of greatness."),
            new Movie(28, "The Prestige", 2006, "Rival magicians wage a dangerous war of one-upmanship."),
            new Movie(29, "The Pianist", 2002, "A Jewish pianist struggles to survive Warsaw's ghetto."),
            new Movie(30, "American Beauty", 1999, "A suburban man's midlife crisis upends his family."),
            new Movie(31, "The Social Network", 2010, "Facebook's founding sparks friendship and legal battles."),
            new Movie(32, "Get Out", 2017, "A Black man uncovers a disturbing secret at his girlfriend's estate."),
            new Movie(33, "Joker", 2019, "A marginalized comedian's breakdown sparks violent unrest."),
            new Movie(34, "La La Land", 2016, "A musician and an actress chase dreams in modern LA."),
            new Movie(35, "Mad Max: Fury Road", 2015, "A desert chase pits a warlord against a defiant road warrior."),
            new Movie(36, "The Avengers", 2012, "Earth's mightiest heroes unite to stop an alien invasion."),
            new Movie(37, "Black Panther", 2018, "A prince returns home to claim the throne of Wakanda."),
            new Movie(38, "Spider-Man: No Way Home", 2021, "Peter Parker faces villains from across the multiverse."),
            new Movie(39, "Top Gun: Maverick", 2022, "A legendary pilot trains a new generation of fighter pilots."),
            new Movie(40, "Everything Everywhere All at Once", 2022, "A woman discovers she must connect with parallel universe versions of herself."),
            new Movie(41, "Barbie", 2023, "Barbie leaves Barbieland to discover the real world."),
            new Movie(42, "The Super Mario Bros. Movie", 2023, "Mario and Luigi journey through the Mushroom Kingdom."),
            new Movie(43, "Guardians of the Galaxy", 2014, "A group of intergalactic misfits must save the universe."),
            new Movie(44, "Thor: Ragnarok", 2017, "Thor must escape an alien planet to save Asgard."),
            new Movie(45, "Doctor Strange", 2016, "A surgeon discovers the mystic arts after a career-ending accident."),
            new Movie(46, "Eternals", 2021, "Immortal beings emerge from hiding to protect Earth."),
            new Movie(47, "Shang-Chi and the Legend of the Ten Rings", 2021, "A martial artist confronts his past and a mysterious organization."),
            new Movie(48, "Ant-Man", 2015, "A thief becomes a shrinking superhero."),
            new Movie(49, "Captain Marvel", 2019, "A former pilot becomes one of the universe's most powerful heroes."),
            new Movie(50, "Deadpool", 2016, "A wisecracking mercenary seeks revenge after a rogue experiment.")
        });

        _nextMovieId = 50;
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
