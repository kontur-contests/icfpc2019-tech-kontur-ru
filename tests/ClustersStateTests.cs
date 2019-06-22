using FluentAssertions;
using lib.Models;
using NUnit.Framework;

namespace tests
{
    [TestFixture]
    public class ClustersStateTests
    {
        [Test]
        public void ReadFromFile([Range(1, 300)] int problem)
        {
            var state = ProblemReader.Read(problem).ToState();
            var clusterSourceLines = ClustersStateReader.Read(problem);
            var clustersState = new ClustersState(clusterSourceLines, state);
        }

        [Test]
        public void METHOD()
        {
            var state = ProblemReader.Read(2).ToState();
            var clusterSourceLines = ClustersStateReader.Read(2);
            var clustersState = new ClustersState(clusterSourceLines, state);

            clustersState.RootLevel.Should().Be(3);
            clustersState.RootIds.Should().Equal(0, 1);
            
            
        }
    }
}