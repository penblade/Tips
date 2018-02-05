using System;
using System.Collections.Generic;

namespace Tips.Tuples
{
    // Named Tuples are available as of C# 7
    public class UserRepositoryNamedTuples
    {
        public List<(string FirstName, string LastName)> GetFirstNameAndLastNameList()
        {
            var users = new List<(string FirstName, string LastName)>
            {
                ("John", "Doe"),
                ("Steve", "Smith")
            };

            return users;
        }

        public (string FirstName, string LastName) GetFirstNameAndLastName()
        {
            // You can use labels to assign the values in the Tuple.
            return (FirstName: "John", LastName: "Doe");
        }
    }
}
