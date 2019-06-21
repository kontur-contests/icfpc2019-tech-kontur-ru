namespace lib.Models
{
    public class Map
    {
        private readonly bool[,] cells;

        public Map(bool[,] cells)
        {
            this.cells = cells;
        }

        public bool this[Point p]
        {
            get { return cells[p.Y, p.X]; }
            set { cells[p.Y, p.X] = value; }
        }
    }
}