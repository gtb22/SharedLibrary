namespace SimpleMDB.Core;

public interface IMovieService
{
    PagedResult<Movie> ReadMovies(int page, int size);
    Result<Movie> ReadMovie(int id);
    Result<Movie> CreateMovie(Movie movie);
    Result<Movie> UpdateMovie(Movie movie);
    Result<bool> DeleteMovie(int id);
}
