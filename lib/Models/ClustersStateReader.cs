using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace lib.Models
{
    public static class ClustersStateReader
    {
        public static string GetClustersPath(int problem)
        {
            return Path.Combine(FileHelper.PatchDirectoryName("clusters.v2"), $"prob-{problem}.clusters");
        }
        
        public static ClusterSourceLine[] Read(int problem)
        {
            var fileName = GetClustersPath(problem);
            return File.ReadAllLines(fileName).Select(JsonConvert.DeserializeObject<ClusterSourceLine>).ToArray();
        }
    }
}