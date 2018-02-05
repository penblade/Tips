using System;
using System.Collections.Generic;

namespace Tips.Tuples
{
    public class UserRepositoryTupleClass
    {
        public List<Tuple<string, string>> GetFirstNameAndLastNameList()
        {
            var users = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("John", "Doe"),
                new Tuple<string, string>("Steve", "Smith")
            };

            return users;
        }

        public Tuple<string, string> GetFirstNameAndLastName()
        {
            return new Tuple<string, string>("John", "Doe");
        }
    }
}
