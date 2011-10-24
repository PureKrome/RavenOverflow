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