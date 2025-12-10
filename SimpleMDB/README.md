# SimpleMDB

A simple movie database application with a C# .NET API backend and a Vanilla JavaScript frontend.

## Architecture

- **Backend**: C# .NET 9.0 API with layered architecture (Core, API)
- **Frontend**: Client-Side Rendered Vanilla JavaScript application
- **Database**: In-memory storage

## Project Structure

```
SimpleMDB/
├── src/
│   ├── SimpleMDB.Core/        // Domain models, services, repositories
│   └── SimpleMDB.Api/         // API controllers, middleware, startup
├── WWWRoot/                   // Static frontend files
├── .github/workflows/         // CI/CD pipeline
├── Dockerfile                 // Containerization
└── README.md
```

## API Endpoints

- `GET /api/v1/movies?page={int}&size={int}` - Get paginated movie list
- `GET /api/v1/movies?id={int}` - Get specific movie
- `POST /api/v1/movies` - Create new movie
- `PUT /api/v1/movies?id={int}` - Update movie
- `DELETE /api/v1/movies?id={int}` - Delete movie

## Running the Application

### Prerequisites

- .NET 9.0 SDK
- Docker (for containerized deployment)

### Local Development

1. Navigate to the project root
2. Run `dotnet run --project src/SimpleMDB.Api`
3. Open `http://localhost:5000` in your browser

### Docker

1. Build the image: `docker build -t simplemdb .`
2. Run the container: `docker run -p 8080:80 simplemdb`

## Frontend Pages

- `index.html` - Movie list with pagination
- `add.html` - Add new movie form
- `edit.html` - Edit existing movie form
- `view.html` - View movie details

## CI/CD

The project includes a GitHub Actions workflow for automated building, testing, and deployment to Azure App Service.
