using Smdb.Api;

try
{
    Console.WriteLine("[Main] Starting application");
    var app = new App();
    Console.WriteLine("[Main] Starting async");
    await app.StartAsync("127.0.0.1", 5000);
    Console.WriteLine("[Main] Completed");
}
catch (Exception ex)
{
    Console.WriteLine($"[FATAL] {ex.GetType().Name}: {ex.Message}");
    Console.WriteLine($"[FATAL] Stack: {ex.StackTrace}");
    Console.WriteLine($"[FATAL] InnerException: {ex.InnerException?.Message}");
    if (ex.InnerException != null)
        Console.WriteLine($"[FATAL] InnerStack: {ex.InnerException.StackTrace}");
    await System.IO.File.WriteAllTextAsync("fatal.log", ex.ToString());
    throw;
}
