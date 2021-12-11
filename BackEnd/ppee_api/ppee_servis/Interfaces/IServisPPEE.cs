using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ppee_servis.Interfaces
{
    public interface IServisPPEE
    {
        Task<bool> ReadFile(Stream stream);

    }
}
