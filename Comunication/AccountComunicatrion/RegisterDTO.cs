using CommunicationShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountComunication
{
    public class RegisterDTO : DTO<RegisterDTO>
    {
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
