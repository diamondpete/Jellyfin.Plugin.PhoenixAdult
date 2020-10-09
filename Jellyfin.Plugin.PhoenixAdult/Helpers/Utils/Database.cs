using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PhoenixAdult.Helpers.Utils
{
    internal static class Database
    {
        private static readonly string DatabasePath = Path.Combine(Plugin.Instance.DataFolderPath, "data");

        public static SiteListStructure SiteList { get; set; }

        public static ActorsStructure Actors { get; set; }

        public static GenresStructure Genres { get; set; }

        public static async Task<bool> Download(string url, string fileName, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(DatabasePath))
            {
                Logger.Info($"Creating database directory \"{DatabasePath}\"");
                Directory.CreateDirectory(DatabasePath);
            }

            var encoding = new UTF8Encoding(false);
            var http = await HTTP.Request(url, cancellationToken).ConfigureAwait(false);
            if (http.IsOK)
            {
                Logger.Info($"Database file \"{fileName}\" downloaded successfully");
                File.WriteAllText(Path.Combine(DatabasePath, fileName), http.Content, encoding);

                return true;
            }

            return false;
        }

        public static void Update(string fileName)
        {
            var encoding = new UTF8Encoding(false);

            var filePath = Path.Combine(DatabasePath, fileName);
            if (File.Exists(filePath))
            {
                var data = File.ReadAllText(filePath, encoding);
                switch (fileName)
                {
                    case "SiteList.json":
                        SiteList = JsonConvert.DeserializeObject<SiteListStructure>(data);
                        break;

                    case "Actors.json":
                        Actors = JsonConvert.DeserializeObject<ActorsStructure>(data);
                        break;

                    case "Genres.json":
                        Genres = JsonConvert.DeserializeObject<GenresStructure>(data);
                        break;

                    default:
                        break;
                }
            }
        }

        public static bool IsExist(string fileName)
        {
            return File.Exists(Path.Combine(Database.DatabasePath, fileName));
        }

        public struct SiteListStructure
        {
            public Dictionary<int, Dictionary<int, string[]>> Sites { get; set; }

            public Dictionary<int, string> SiteIDList { get; set; }

            public Dictionary<string, string> Abbrieviations { get; set; }
        }

        public struct ActorsStructure
        {
            public Dictionary<string, string[]> ActorsReplace { get; set; }

            public Dictionary<int, string[]> ActorsStudioIndexes { get; set; }

            public Dictionary<int, Dictionary<string, string[]>> ActorsReplaceStudios { get; set; }
        }

        public struct GenresStructure
        {
            public Dictionary<string, string[]> GenresReplace { get; set; }

            public Dictionary<string, string[]> GenresPartialReplace { get; set; }

            public List<string> GenresSkip { get; set; }

            public List<string> GenresPartialSkip { get; set; }
        }
    }
}
