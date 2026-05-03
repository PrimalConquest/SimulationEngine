namespace BattleServer.Source.Services
{
    // Wraps the Agones SDK sidecar REST API (localhost:9358).
    // Fails silently when not running inside a k8s GameServer pod (local dev).
    public class AgonesService
    {
        const string SidecarUrl = "http://localhost:9358";

        readonly HttpClient           _http = new() { BaseAddress = new Uri(SidecarUrl) };
        readonly ILogger<AgonesService> _log;

        public AgonesService(ILogger<AgonesService> log) => _log = log;

        public Task ReadyAsync()    => PostAsync("ready");
        public Task HealthAsync()   => PostAsync("health");
        public Task ShutdownAsync() => PostAsync("shutdown");

        public async Task<string?> GetGameServerJsonAsync()
        {
            try
            {
                var resp = await _http.GetAsync("gameserver");
                return await resp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _log.LogWarning("Agones SDK /gameserver unreachable: {Message}", ex.Message);
                return null;
            }
        }

        async Task PostAsync(string path)
        {
            try
            {
                await _http.PostAsync(path, null);
            }
            catch (Exception ex)
            {
                _log.LogWarning("Agones SDK /{Path} unreachable (not in k8s?): {Message}", path, ex.Message);
            }
        }
    }
}
