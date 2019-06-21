namespace lib.Models
{
    public class UseExtension : ActionBase
    {
        public UseExtension(V relative)
        {
            Relative = relative;
        }

        public V Relative { get; }
        
        public override string ToString()
        {
            return $"B{Relative}";
        }
    }
}