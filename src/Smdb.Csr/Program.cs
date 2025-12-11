using Smdb.Csr;

try
{
    Console.WriteLine("[CSR] Starting");
    var app = new App();
    Console.WriteLine("[CSR] Calling StartAsync");
    await app.StartAsync("http://localhost", 5001);
    Console.WriteLine("[CSR] StartAsync returned (should never happen)");
}
catch (Exception ex)
{
    Console.WriteLine($"[CSR FATAL] {ex.GetType().Name}: {ex.Message}");
    Console.WriteLine($"[CSR FATAL] Stack: {ex.StackTrace}");
    throw;
}
