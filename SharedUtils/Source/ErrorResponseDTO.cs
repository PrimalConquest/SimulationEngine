using CommunicationShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedUtils
{
    public class ErrorResponseDTO : DTO<ErrorResponseDTO>
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; } = "";
    }
}
