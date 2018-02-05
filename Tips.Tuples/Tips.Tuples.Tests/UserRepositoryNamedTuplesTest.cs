using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tips.Tuples.Tests
{
    [TestClass]
    public class UserRepositoryNamedTuplesTest
    {
        [TestMethod]
        public void GetFirstNameAndLastNameListTest()
        {
            var repository = new UserRepositoryNamedTuples();
            var firstNameAndLastNameList = repository.GetFirstNameAndLastNameList();

            Assert.IsNotNull(firstNameAndLastNameList);
            Assert.AreEqual(2, firstNameAndLastNameList.Count);

            // Tuple items are named.
            var firstName1 = firstNameAndLastNameList[0].FirstName;
            var lastName1 = firstNameAndLastNameList[0].LastName;

            Assert.AreEqual("John", firstName1);
            Assert.AreEqual("Doe", lastName1);

            var firstName2 = firstNameAndLastNameList[1].FirstName;
            var lastName2 = firstNameAndLastNameList[1].LastName;

            Assert.AreEqual("Steve", firstName2);
            Assert.AreEqual("Smith", lastName2);
        }

        [TestMethod]
        public void GetFirstNameAndLastNameTest()
        {
            var repository = new UserRepositoryNamedTuples();
            var firstNameAndLastName = repository.GetFirstNameAndLastName();

            Assert.IsNotNull(firstNameAndLastName);

            // Tuple items are named.
            var firstName = firstNameAndLastName.FirstName;
            var lastName = firstNameAndLastName.LastName;

            Assert.AreEqual("John", firstName);
            Assert.AreEqual("Doe", lastName);
        }
    }
}
