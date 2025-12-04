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
        public string? Message { get; }
        public T? Data { get;  } = default;

        public ResultDTO(bool success =false, string? error=null, T? data = default)
        {
            Success = success;
            Message = error;
            Data = data;
        }
    }
}
