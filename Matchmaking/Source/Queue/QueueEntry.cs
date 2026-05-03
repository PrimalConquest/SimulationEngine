namespace Matchmaking.Source.Queue
{
    public class QueueEntry
    {
        public string   UserId       { get; set; } = "";
        public string   UserName     { get; set; } = "";
        public string   ConnectionId { get; set; } = "";
        public int      RankPoints   { get; set; }
        public DateTime QueuedAt     { get; set; }
    }
}
