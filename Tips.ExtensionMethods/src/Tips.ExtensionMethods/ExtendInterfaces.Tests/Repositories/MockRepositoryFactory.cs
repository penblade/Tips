using System.Collections.Generic;
using Moq;
using Tips.ExtensionMethods.ExtendInterfaces.Models;
using Tips.ExtensionMethods.ExtendInterfaces.Repositories;

namespace Tips.ExtensionMethods.ExtendInterfaces.Tests.Repositories
{
    internal class MockRepositoryFactory
    {
        private readonly TestItemFactory _testItemFactory;

        public MockRepositoryFactory(TestItemFactory testItemFactory)
        {
            _testItemFactory = testItemFactory;
        }

        // Assume this repository gets items via a database.
        public IRepository CreateMockDatabaseRepository() => CreateMockDatabaseRepository(_testItemFactory.CreateDatabaseItems());

        // Assume this repository gets items via a file.
        public IRepository CreateMockFileRepository() => CreateMockDatabaseRepository(_testItemFactory.CreateFileItems());

        // Assume this repository gets items via a stream.
        public IRepository CreateMockStreamRepository() => CreateMockDatabaseRepository(_testItemFactory.CreateStreamItems());

        private static IRepository CreateMockDatabaseRepository(IEnumerable<Item> list)
        {
            var mockRepository = new Mock<IRepository>();
            mockRepository.Setup(x => x.GetItems()).Returns(list);
            return mockRepository.Object;
        }
    }
}
