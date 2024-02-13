using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Setting {
    public class Parameter : IParameter {
        private readonly Infrastructure.Enviroment.Context _context;
        private static string? Conexion;
        public static Dictionary<string, string>? Dictionary;
        public Parameter(Infrastructure.Enviroment.Context dbContext) {
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public static void InitSettings(string FilePath) {
            try {
                string[] result = File.ReadAllLines(FilePath);
                //File.Delete(Path); Uncomment if the services is always runing and have a singleton instance for this class.
                Conexion = result.FirstOrDefault(x => string.IsNullOrWhiteSpace(x));
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
        public string? GetParameter(string ParameterName) {
            if (Dictionary == null) {
                Conexion = string.Empty;
            }
            return "important_value";
        }
    }
}
