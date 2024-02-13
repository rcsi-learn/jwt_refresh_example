using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.User
{
    public class Seed
    {
        public static async Task TestData(Context context)
        {
            if (context.Users is null) return;
            if (context.Users.Any()) return;
            await context.Users.AddRangeAsync(new List<Domain.User>{
                new() {Id = Guid.NewGuid(), Account = "rc", Name = "rcsi", Password = "psw"}
            });
            await context.SaveChangesAsync();
        }
    }
}
