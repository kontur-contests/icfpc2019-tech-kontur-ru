using System.Threading.Tasks;
using FluentAssertions;
using lib.Models;
using NUnit.Framework;

namespace tests
{
    [TestFixture]
    public class PuzzleReaderTests
    {
        [Test]
        public async Task ReadCurrentFromApi()
        {
            var puzzle = await PuzzleReader.GetCurrentPuzzleFromApiAsync();
            puzzle.Should().NotBeNull();
        }
    }
}