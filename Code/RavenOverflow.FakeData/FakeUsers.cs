using System.Collections.Generic;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using RavenOverflow.Core.Entities;

namespace RavenOverflow.FakeData
{
    public static class FakeUsers
    {
        private static IList<User> _fakeUsers;

        public static User CreateAFakeUser()
        {
            return Builder<User>
                .CreateNew()
                .With(x => x.DisplayName = string.Format("{0}.{1}", GetRandom.FirstName(), GetRandom.LastName()))
                .And(x => x.Id = string.Format("Users/{0}", x.DisplayName))
                .Build();
        }

        public static ICollection<User> CreateFakeUsers()
        {
            if (_fakeUsers == null)
            {
                var fakeUsers = new List<User>();
                for (int i = 0; i < GetRandom.Int(20, 100); i++)
                {
                    fakeUsers.Add(CreateAFakeUser());
                }

                _fakeUsers = fakeUsers;
            }

            return _fakeUsers;
        }
    }
}