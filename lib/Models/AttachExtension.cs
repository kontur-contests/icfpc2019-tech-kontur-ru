namespace lib.Models
{
    public class AttachExtension : ActionBase
    {
        public AttachExtension(V relative)
        {
            Relative = relative;
        }

        public V Relative { get; }
    }
}