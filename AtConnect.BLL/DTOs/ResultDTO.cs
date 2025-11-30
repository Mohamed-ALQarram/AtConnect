using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtConnect.BLL.DTOs
{
    public class ResultDTO<T>
    {

        public bool Success { get;  }
        public T? Data { get;  } = default;
        public string? ErrorMessage { get; }

        public ResultDTO(bool success =false, string error=null!, T? data = default)
        {
            Success = success;
            ErrorMessage = error;
            Data = data;
        }
    }
}
