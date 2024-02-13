using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers {
    public static class ApplicationExtension {
        public static async Task<IActionResult> ExecuteAndHandleErrorsAsync<T>(this T applicationMethod, Func<Task<object>> func) where T : class {
            try {
                var result = await func.Invoke();
                return new OkObjectResult(result);
            }
            catch (Exception ex) {
                return new ObjectResult($"Operation Error: {ex.Message}") {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
