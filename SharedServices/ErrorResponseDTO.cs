using System;
using System.Collections.Generic;
using System.Text;

namespace SharedServices
{
    internal class ErrorResponseDTO
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; } = "";
    }
}
