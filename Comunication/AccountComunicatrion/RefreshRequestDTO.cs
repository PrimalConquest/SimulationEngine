using CommunicationShared;

namespace AccountComunication
{
    public class RefreshRequestDTO : DTO<RefreshRequestDTO>
    {
        public string RefreshToken { get; set; } = "";
    }
}
