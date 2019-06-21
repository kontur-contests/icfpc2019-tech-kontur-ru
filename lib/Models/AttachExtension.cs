namespace lib.Models
{
    public class AttachExtension : ActionBase
    {
        public AttachExtension(Point relative)
        {
            Relative = relative;
        }

        public Point Relative { get; }
    }
}