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
        public void MakeMapImage([Values(ProblemReader.PART_1_INITIAL, ProblemReader.PART_2_TELEPORTS)]string pack)
        {
            var sb = new StringBuilder();
            var dir = Path.Combine(FileHelper.PatchDirectoryName("problems"), pack, "images");
            var problems = new ProblemReader(pack).ReadAll();
            foreach (var problemMeta in problems)
            {
                var map = problemMeta.Problem.ToState().Map;
                var bmp = new Image<Rgba32>(map.SizeX, map.SizeY);
                foreach (var cell in map.EnumerateCells())
                {
                    bmp[cell.Item1.X, cell.Item1.Y] = GetColor(cell.Item2);
                }

                foreach (var booster in problemMeta.Problem.Boosters)
                {
                    bmp[booster.Position.X, booster.Position.Y] = GetColor(booster.Type);

                }
                bmp.Mutate(x => x.Resize(map.SizeX * 2, map.SizeY * 2, new BoxResampler()));
                bmp.Save(Path.Combine(dir, problemMeta.ProblemId + ".png"));
                sb.Append($"<img style=\"margin:10px\" src=\"{problemMeta.ProblemId}.png\" alt=\"{problemMeta.ProblemId} title=\"{problemMeta.ProblemId}\"\">");
            }
            File.WriteAllText(Path.Combine(dir, "index.html"), sb.ToString());
        }

        private Rgba32 GetColor(BoosterType boosterType)
        {
            if (boosterType == BoosterType.Extension) return Rgba32.Blue;
            if (boosterType == BoosterType.FastWheels) return Rgba32.Brown;
            if (boosterType == BoosterType.Drill) return Rgba32.Green;
            if (boosterType == BoosterType.Teleport) return Rgba32.Violet;
            if (boosterType == BoosterType.MysteriousPoint) return Rgba32.Red;
            throw new Exception(boosterType.ToString());
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