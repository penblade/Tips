using System;
using System.Collections.Generic;

namespace Tips.Tuples
{
    // Named Tuples are available as of C# 7
    public class UserRepositoryNestedTuples
    {
        public List<(string FirstName, string LastName, (string City, string State) Address)> GetFirstNameAndLastNameList()
        {
            var users = new List<(string FirstName, string LastName, (string City, string State) Address)>
            {
                ("John", "Doe", ("Columbus", "OH")),
                ("Steve", "Smith", ("Lansing", "MI"))
            };

            return users;
        }

        public (string FirstName, string LastName, (string City, string State) Address) GetFirstNameAndLastName()
        {
            // You can use labels to assign the values in the Tuple.
            return (FirstName: "John", LastName: "Doe", Address: ("Columbus", "OH"));
        }
    }
}
