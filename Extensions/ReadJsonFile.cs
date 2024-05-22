using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WorkerServiceTimesheet.Extensions
{
    public static class ReadJsonFile
    {
        public static async Task<T> OpenAndParseJson<T>(string PathFile)
        {

            var json = await File.ReadAllTextAsync(PathFile);


            T dataObject = JsonConvert.DeserializeObject<T>(json);

            return dataObject;

        }

        public static DateTime? ParseUnixTimestamps(long? timestamps)
        {
            if (timestamps == null)
            {
                return null;
            }
            else
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestamps ?? 0).ToLocalTime();

            }
        }


    }
}
