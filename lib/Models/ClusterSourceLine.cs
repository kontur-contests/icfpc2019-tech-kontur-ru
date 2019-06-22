namespace lib.Models
{
    public class ClusterSourceLine
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int[] cluster_hierarchy { get; set; }
    }
}