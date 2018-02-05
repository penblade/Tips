using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tips.Tuples.Tests
{
    [TestClass]
    public class UserRepositoryUnnamedTuplesTest
    {
        [TestMethod]
        public void GetFirstNameAndLastNameListTest()
        {
            var repository = new UserRepositoryUnnamedTuples();
            var firstNameAndLastNameList = repository.GetFirstNameAndLastNameList();

            // Tuple items are named Item1 and Item2.
            var firstName1 = firstNameAndLastNameList[0].Item1;
            var lastName1 = firstNameAndLastNameList[0].Item2;

            Assert.AreEqual("John", firstName1);
            Assert.AreEqual("Doe", lastName1);

            var firstName2 = firstNameAndLastNameList[1].Item1;
            var lastName2 = firstNameAndLastNameList[1].Item2;

            Assert.AreEqual("Steve", firstName2);
            Assert.AreEqual("Smith", lastName2);
        }

        [TestMethod]
        public void GetFirstNameAndLastNameTest()
        {
            var repository = new UserRepositoryUnnamedTuples();
            var firstNameAndLastName = repository.GetFirstNameAndLastName();

            Assert.IsNotNull(firstNameAndLastName);

            // Tuple items are named Item1 and Item2.
            var firstName = firstNameAndLastName.Item1;
            var lastName = firstNameAndLastName.Item2;

            // Tuple items are named.
            Assert.AreEqual("John", firstName);
            Assert.AreEqual("Doe", lastName);
        }
    }
}
