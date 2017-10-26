using System.Collections.Generic;
using System.Linq;
using CopyBud.Persistence;
using FluentAssertions;
using NUnit.Framework;

namespace CopyBud.Tests
{
    public class Tests
    {
        private DataContextMock<DataContext> _dbContexMock;
        private HistoryRepository _historyRepository;
        [SetUp]
        public void Setup()
        {
            _dbContexMock = new DataContextMock<DataContext>();
            _dbContexMock.WithDbSetFrom(x => x.Histories, new List<History>());
        _historyRepository = new HistoryRepository( _dbContexMock.Object );
        }
        [Test]
        public void Test_Add_History()
        {
            _historyRepository.AddHistory("Test History");
            _dbContexMock.Object.Histories.Count().Should().NotBe(0);
        }


        [TestCase(true)]
        [TestCase(false)]
        public void Test_Does_History_Exist( bool exists)
        {
            var hist = "Test Clipboard";
            if(exists) _historyRepository.AddHistory( hist );
            _historyRepository.DoesHistoryExist(hist).Should().Be(exists);
        }
    }
}
