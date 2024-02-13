using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application {
    public static class ApplicationExtension {
        public static async Task<T?> ExecuteAndHandleErrorsAsync<T>(this T applicationMethod, Func<Task<object>> func) where T : class {
            try {
                var result = await func.Invoke();
                return (T)result;
            }
            catch (Exception) {
                //write ex.message in log
                return null;
            }
        }
    }
}
