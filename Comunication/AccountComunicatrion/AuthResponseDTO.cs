using CommunicationShared;

namespace AccountComunication
{
    public class AuthResponseDTO : DTO<AuthResponseDTO>
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
    }
}
