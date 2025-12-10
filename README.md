# SimpleMDB - Simple Movie Database

A full-stack .NET 8.0 application for managing movies and actors with a custom HTTP server and vanilla JavaScript frontend.

## 🚀 Features

- **Movie Management**: View, add, update, and delete movies
- **Actor Management**: Manage actors in the database
- **Pagination**: Browse movies with configurable page sizes
- **In-Memory Database**: Fast, lightweight data persistence (seeded with 50 movies and 10 actors)
- **Custom HTTP Server**: Built from scratch with TcpListener (no ASP.NET Core)
- **Responsive Frontend**: Vanilla JavaScript with clean, modern UI
- **RESTful API**: Full CRUD endpoints for movies and actors

## 📁 Project Structure

```
SimpleMDB/
├── SharedLibrary/                 # Shared HTTP abstractions
│   └── src/Shared/
│       ├── Http/                  # Custom HTTP server & middleware
│       │   ├── HttpServer.cs      # TcpListener-based server
│       │   ├── HttpRouter.cs      # Routing logic
│       │   ├── HttpUtils.cs       # Middleware pipeline
│       │   ├── IHttpRequest.cs    # Request interface
│       │   ├── IHttpResponse.cs   # Response interface
│       │   └── Results/           # JSON response utilities
│       └── Results/               # Result<T> and PagedResult<T> types
│
├── src/
│   ├── Smdb.Core/                 # Domain models & business logic
│   │   ├── Movies/                # Movie service & repository
│   │   ├── Actors/                # Actor service & repository
│   │   ├── Users/                 # User authentication
│   │   ├── ActorMovies/           # Actor-Movie relationships
│   │   └── Db/                    # MemoryDatabase singleton
│   │
│   ├── Smdb.Api/                  # Backend API (Port 5000)
│   │   ├── Controllers/           # Movies & Actors endpoints
│   │   ├── Routers/               # Route configuration
│   │   ├── App.cs                 # Middleware setup
│   │   └── Program.cs             # Entry point
│   │
│   └── Smdb.Csr/                  # Frontend CSR (Port 5001)
│       ├── App.cs                 # Static file serving
│       └── Program.cs             # Entry point
│
└── wwwroot/                       # Static files
    ├── index.html                 # Movies page
    ├── add-movie.html             # Add movie form
    ├── actors.html                # Actors page
    ├── add-actor.html             # Add actor form
    ├── index.js                   # Movies logic
    ├── actors.js                  # Actors logic
    ├── add-movie.js               # Add movie logic
    ├── add-actor.js               # Add actor logic
    ├── common.js                  # Shared utilities
    └── main.css                   # Styling
```

## 🛠️ Technology Stack

- **Language**: C# .NET 8.0
- **Backend**: Custom HTTP Server (TcpListener)
- **Frontend**: Vanilla JavaScript (Client-Side Rendering)
- **Data**: In-Memory Database with LINQ queries
- **Architecture**: MVC + Repository Pattern

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Windows PowerShell 5.1 or later

### Building the Project

```powershell
cd C:\Users\andre\Downloads\SIMPLEMDB1
dotnet build SimpleMDB.sln
```

### Running the Servers

**Option 1: Run Both Servers Separately**

```powershell
# Terminal 1 - Backend API (Port 5000)
dotnet run --project src/Smdb.Api/Smdb.Api.csproj

# Terminal 2 - Frontend Server (Port 5001)
dotnet run --project src/Smdb.Csr/Smdb.Csr.csproj
```

**Option 2: Run Both Servers in One Command**

```powershell
$apiProcess = Start-Process -FilePath dotnet -ArgumentList 'run --project src/Smdb.Api/Smdb.Api.csproj' -NoNewWindow -PassThru
Start-Sleep -Milliseconds 1500
$csrProcess = Start-Process -FilePath dotnet -ArgumentList 'run --project src/Smdb.Csr/Smdb.Csr.csproj' -NoNewWindow -PassThru
```

### Access the Application

- **Frontend**: http://localhost:5001
- **API Documentation**: http://localhost:5000/api/v1

## 📚 API Endpoints

### Movies

```
GET    /api/v1/movies           # Get all movies (paginated)
GET    /api/v1/movies/:id       # Get single movie
POST   /api/v1/movies           # Create new movie
PUT    /api/v1/movies/:id       # Update movie
DELETE /api/v1/movies/:id       # Delete movie
```

**Get Movies Example:**
```bash
curl http://localhost:5000/api/v1/movies?page=1&size=10
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "title": "The Godfather",
      "year": 1972,
      "description": "A mafia patriarch hands the family empire to his reluctant son.",
      "genre": "",
      "rating": 0
    }
  ],
  "page": 1,
  "size": 10,
  "total": 50,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

### Actors

```
GET    /api/v1/actors           # Get all actors
GET    /api/v1/actors/:id       # Get single actor
POST   /api/v1/actors           # Create new actor
PUT    /api/v1/actors/:id       # Update actor
DELETE /api/v1/actors/:id       # Delete actor
```

## 🎯 Key Components

### Custom HTTP Server

Located in `SharedLibrary/src/Shared/Http/`:

- **HttpServer.cs**: TcpListener-based server listening on configurable ports
- **HttpRouter.cs**: Route matching with middleware pipeline execution
- **HttpUtils.cs**: Middleware implementations (logging, CORS, error handling, static files)

### Middleware Pipeline

1. **StructuredLogging** - Log all requests with unique request IDs
2. **CentralizedErrorHandling** - Catch and format exceptions
3. **CORS** - Enable cross-origin requests
4. **ParseRequest** - Parse HTTP request line and headers
5. **ReadRequestBody** - Read and buffer request body
6. **ServeStaticFiles** - Serve CSS, JS, HTML files
7. **DefaultResponse** - Return 404 if no route matched

### Data Layer

- **MemoryDatabase.cs**: Singleton with thread-safe dictionary storage
- **MemoryRepository**: Generic LINQ-based repository pattern
- **Services**: Business logic layer (MovieService, ActorService)

## 🔧 Configuration

### API Server (src/Smdb.Api/appsettings.cfg)

```ini
[server]
host=localhost
port=5000
```

### CSR Server (src/Smdb.Csr/appsettings.cfg)

```ini
[server]
host=localhost
port=5001

[app]
root.dir=./wwwroot
```

## 📊 Data Models

### Movie

```csharp
public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public string Description { get; set; }
    public string Genre { get; set; }
    public double Rating { get; set; }
}
```

### Actor

```csharp
public class Actor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Nationality { get; set; }
}
```

## 🧪 Testing

The application comes pre-seeded with:
- **50 Movies**: Classic films from The Godfather to recent releases
- **10 Actors**: Popular actors with biographical data

No additional setup needed - just run and browse!

## 🎨 Frontend Pages

### Movies Page (/)
- Display all movies in a paginated list
- Search and filter options
- Add, edit, and delete buttons
- Responsive grid layout

### Add Movie Page
- Form to create new movies
- Validation
- Auto-redirect to movies page on success

### Actors Page
- List all actors
- View actor details
- Add and remove actors
- Manage actor-movie relationships

### Add Actor Page
- Form to create new actors
- Date picker for birth date
- Nationality field

## 🔒 Architecture Highlights

### Result Pattern
All API responses follow a standard result pattern:

```csharp
public class Result<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
}
```

### Dependency Injection
Services are instantiated with required repositories:
- MovieService → MemoryMovieRepository
- ActorService → MemoryActorRepository
- AuthService → MemoryUserRepository

### Error Handling
- Centralized exception handling middleware
- Structured error responses
- Request ID tracking for debugging

## 🚀 Performance Features

- **In-Memory Database**: Ultra-fast data access
- **Pagination**: Handle large datasets efficiently
- **Static File Caching**: Browser-friendly headers
- **Minimal Dependencies**: Custom HTTP server reduces overhead

## 📝 Git History

The project is version controlled with meaningful commit messages:

```
commit 75fbdca - Fix CSR static file serving - resolve wwwroot path
commit <earlier> - Initial project setup and implementation
```

View full history:
```powershell
git log --oneline
```

## 🐛 Troubleshooting

### Port Already in Use
```powershell
# Find process using port 5000 or 5001
netstat -ano | findstr ":5000"
# Kill the process
taskkill /PID <PID> /F
```

### Static Files Not Serving
The middleware now correctly resolves the wwwroot directory from the solution root. If issues persist:
- Verify wwwroot exists in solution root
- Check file permissions
- Clear bin/obj folders and rebuild

### Build Failures
```powershell
# Clean and rebuild
dotnet clean SimpleMDB.sln
dotnet build SimpleMDB.sln -v detailed
```

## 📄 License

This is a demonstration project for educational purposes.

## 🤝 Future Enhancements

- [ ] Persistent SQL database
- [ ] User authentication and authorization
- [ ] Advanced search filters
- [ ] Movie ratings and reviews
- [ ] Actor filmography tracking
- [ ] API documentation (Swagger)
- [ ] Unit and integration tests

---

**Happy movie browsing! 🎬**
