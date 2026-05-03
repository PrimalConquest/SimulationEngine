using LoadoutComunication;

namespace BattleServer.Source.Services
{
    // Typed HttpClient — base address and X-Internal-Key header set at registration.
    public class DbWrapperClient
    {
        readonly HttpClient              _client;
        readonly ILogger<DbWrapperClient> _log;

        public DbWrapperClient(HttpClient client, ILogger<DbWrapperClient> log)
        {
            _client = client;
            _log    = log;
        }
        public async Task<string?> PostMatchResultAsync(string winnerId, string loserId)
        {
            try
            {
                var dto  = new MatchResultDTO { WinnerId = winnerId, LoserId = loserId };
                var resp = await _client.PostAsJsonAsync("/stats/match-result", dto);
                if (resp.IsSuccessStatusCode) return null;

                var body = await resp.Content.ReadAsStringAsync();
                return $"match-result failed ({(int)resp.StatusCode}): {body}";
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "PostMatchResult threw for winner={W} loser={L}", winnerId, loserId);
                return ex.Message;
            }
        }
    }
}
