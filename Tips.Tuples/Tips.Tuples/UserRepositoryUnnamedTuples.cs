using System;
using System.Collections.Generic;

namespace Tips.Tuples
{
    public class UserRepositoryUnnamedTuples
    {
        public List<(string, string)> GetFirstNameAndLastNameList()
        {
            var users = new List<(string, string)>
            {
                ("John", "Doe"),
                ("Steve", "Smith")
            };

            return users;
        }

        public (string, string) GetFirstNameAndLastName()
        {
            return ("John", "Doe");
        }
    }
}
