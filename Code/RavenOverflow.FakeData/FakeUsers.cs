using System;
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
                .With(x => x.FullName = string.Format("{0} {1}", GetRandom.FirstName(), GetRandom.LastName()))
                .And(x => x.DisplayName = x.FullName.Replace(' ', '.'))
                .And(x => x.Id = string.Format("Users/{0}", x.DisplayName))
                .And(x => x.Email = GetRandom.Email())
                .And(x => x.CreatedOn = GetRandom.DateTime(DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow))
                .And(x => x.Score = GetRandom.PositiveInt(50000))
                .And(x => x.IsActive = GetRandom.Boolean())
                .Build();
        }

        public static ICollection<User> CreateFakeUsers()
        {
            return CreateFakeUsers(GetRandom.Int(20, 100));
        }

        public static ICollection<User> CreateFakeUsers(int numberOfFakeUsers)
        {
            if (_fakeUsers == null)
            {
                var fakeUsers = new List<User>();
                fakeUsers.AddRange(CreateFixedFakeUsers());
                for (int i = 0; i < numberOfFakeUsers; i++)
                {
                    fakeUsers.Add(CreateAFakeUser());
                }

                _fakeUsers = fakeUsers;
            }

            return _fakeUsers;
        }

        private static IEnumerable<User> CreateFixedFakeUsers()
        {
            return new List<User>
                       {
                           new User
                               {
                                   FullName = "Pure Krome",
                                   DisplayName = "Pure.Krome",
                                   Email = "pure.krome@pewpew.xxx",
                                   CreatedOn = new DateTime(2010, 5, 23, 13, 05, 00),
                                   Score = 69,
                                   IsActive = true,
                                   FavoriteTags = new List<string> {"ravendb", "c#", "asp.net-mvc3"}
                               }
                       };
        }

    }
}