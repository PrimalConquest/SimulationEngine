using CommunicationShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountComunication
{
    public class LoginDTO : DTO<LoginDTO>
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
