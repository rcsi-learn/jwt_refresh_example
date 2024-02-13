using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Setting {
    public interface IParameter {
        string? GetParameter(string ParameterName);
    }
}
