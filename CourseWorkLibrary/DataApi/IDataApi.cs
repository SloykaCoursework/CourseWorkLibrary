using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWorkLibrary.DataApi
{
    public interface IDataApi
    {

        Task<string?> SendCommand(CommandCode command);

    }
}
