using System;
using System.IO;
using System.Text;
using lib;
using lib.Models;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace tests
{
    [TestFixture]
    public class MapTools
    {
        [Test]
        [Explicit]
        public void MakeMapImage()
        {
            var sb = new StringBuilder();
            var dir = Path.Combine(FileHelper.PatchDirectoryName("problems"), "part-1-initial", "images");
            for (int problemNumber = 1; problemNumber <= 150; problemNumber++)
            {
                var problem = new ProblemReader(ProblemReader.PART_1_INITIAL).Read(problemNumber);
                var map = problem.ToState().Map;
                var bmp = new Image<Rgba32>(map.SizeX, map.SizeY);
                foreach (var cell in map.EnumerateCells())
                {
                    bmp[cell.Item1.X, cell.Item1.Y] = GetColor(cell.Item2);
                }
                bmp.Mutate(x => x.Resize(map.SizeX * 2, map.SizeY * 2, new BoxResampler()));
                bmp.Save(Path.Combine(dir, problemNumber + ".png"));
                sb.Append($"<img style=\"margin:10px\" src=\"{problemNumber}.png\" alt=\"{problemNumber}\">");
            }
            File.WriteAllText(Path.Combine(dir, "index.html"), sb.ToString());
        }

        private Rgba32 GetColor(CellState cellState)
        {
            if (cellState == CellState.Obstacle) return Rgba32.Black;
            else if (cellState == CellState.Void) return Rgba32.White;
            else if (cellState == CellState.Wrapped) return Rgba32.Yellow;
            else throw new Exception(cellState.ToString());
        }
    }
}