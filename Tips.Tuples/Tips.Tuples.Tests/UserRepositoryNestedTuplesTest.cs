using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tips.Tuples.Tests
{
    [TestClass]
    public class UserRepositoryNestedTuplesTest
    {
        [TestMethod]
        public void GetFirstNameAndLastNameListTest()
        {
            var repository = new UserRepositoryNestedTuples();
            var firstNameAndLastNameList = repository.GetFirstNameAndLastNameList();

            Assert.IsNotNull(firstNameAndLastNameList);
            Assert.AreEqual(2, firstNameAndLastNameList.Count);

            // Tuple items are named.
            Assert.IsNotNull(firstNameAndLastNameList[0].Address);

            var firstName1 = firstNameAndLastNameList[0].FirstName;
            var lastName1 = firstNameAndLastNameList[0].LastName;
            var city1 = firstNameAndLastNameList[0].Address.City;
            var state1 = firstNameAndLastNameList[0].Address.State;

            Assert.AreEqual("John", firstName1);
            Assert.AreEqual("Doe", lastName1);
            Assert.AreEqual("Columbus", city1);
            Assert.AreEqual("OH", state1);

            Assert.IsNotNull(firstNameAndLastNameList[1].Address);

            var firstName2 = firstNameAndLastNameList[1].FirstName;
            var lastName2 = firstNameAndLastNameList[1].LastName;
            var city2 = firstNameAndLastNameList[1].Address.City;
            var state2 = firstNameAndLastNameList[1].Address.State;

            Assert.AreEqual("Steve", firstName2);
            Assert.AreEqual("Smith", lastName2);
            Assert.AreEqual("Lansing", city2);
            Assert.AreEqual("MI", state2);
        }

        [TestMethod]
        public void GetFirstNameAndLastNameTest()
        {
            var repository = new UserRepositoryNestedTuples();
            var firstNameAndLastName = repository.GetFirstNameAndLastName();

            Assert.IsNotNull(firstNameAndLastName);

            // Tuple items are named.
            var firstName = firstNameAndLastName.FirstName;
            var lastName = firstNameAndLastName.LastName;
            var city = firstNameAndLastName.Address.City;
            var state = firstNameAndLastName.Address.State;

            Assert.AreEqual("John", firstName);
            Assert.AreEqual("Doe", lastName);
            Assert.AreEqual("Columbus", city);
            Assert.AreEqual("OH", state);
        }
    }
}
