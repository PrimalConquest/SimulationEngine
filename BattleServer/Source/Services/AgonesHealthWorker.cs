namespace BattleServer.Source.Services
{
    // Background service — marks the GameServer Ready on startup then sends
    // health pings every 2 seconds so Agones does not restart the pod.
    public class AgonesHealthWorker : BackgroundService
    {
        readonly AgonesService           _agones;
        readonly ILogger<AgonesHealthWorker> _log;

        public AgonesHealthWorker(AgonesService agones, ILogger<AgonesHealthWorker> log)
        {
            _agones = agones;
            _log    = log;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            await _agones.ReadyAsync();
            _log.LogInformation("Agones: GameServer marked Ready");

            while (!ct.IsCancellationRequested)
            {
                await _agones.HealthAsync();
                await Task.Delay(1_900, ct);
            }
        }
    }
}
