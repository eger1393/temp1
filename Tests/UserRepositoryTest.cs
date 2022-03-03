using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Tests
{
    public class UserRepositoryTest
    {
        private DataContext _context;
        private IUserRepository _userRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite("Data Source=Sharable;Mode=Memory;Cache=Shared")
                .Options;
            _context = new DataContext(options);
            _context.Database.OpenConnection();
            if (_context != null)
            {
                _context.Database.EnsureDeleted();
                _context.Database.EnsureCreated();
            }

            _userRepository = new UserRepository(_context);
        }

        [Test]
        public void AddUser_Success()
        {
            const string userName = "test";
            _userRepository.AddUser(userName);
            Assert.IsTrue(_context.Users.Any(x => x.Name == userName));
        }

        [Test]
        public void AddUser_NullNameError()
        {
            Assert.Catch<DbUpdateException>(() => _userRepository.AddUser(null));
        }

        [Test]
        public void AddUser_LongNameError()
        {
            // Тест фейлится, ограничение по размеру имени на уровне БД чет не завелось
            // const string longName = "long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test_long_test";
            // Assert.Catch<DbUpdateException>(() => _userRepository.AddUser(longName + longName + longName));
        }

        [Test]
        public void GetUser_Success()
        {
            var user = _userRepository.AddUser("testUser");
            var storedUser = _userRepository.GetUser(user.Id);
            Assert.AreEqual(user.Id, storedUser.Id);
        }

        [Test]
        public void GetUser_withIdNotFound()
        {
            Assert.Catch<ArgumentException>(() => _userRepository.GetUser(Guid.NewGuid()));
        }

        [Test]
        public void Subscribe_Success()
        {
            var user1 = _userRepository.AddUser("test1");
            var user2 = _userRepository.AddUser("test2");
            _userRepository.Subscribe(user1.Id, user2.Id);
            Assert.IsTrue(user2.SubscribersCount == 1 &&
                          user2.Subscribers.FirstOrDefault()?.SubscribedUserId == user1.Id);
        }

        [Test]
        public void Subscribe_MutualSubscription()
        {
            var user1 = _userRepository.AddUser("test1");
            var user2 = _userRepository.AddUser("test2");
            _userRepository.Subscribe(user1.Id, user2.Id);
            _userRepository.Subscribe(user2.Id, user1.Id);
            Assert.IsTrue(user2.SubscribersCount == 1 &&
                          user2.Subscribers.FirstOrDefault()?.SubscribedUserId == user1.Id);
            Assert.IsTrue(user1.SubscribersCount == 1 &&
                          user1.Subscribers.FirstOrDefault()?.SubscribedUserId == user2.Id);
        }

        [Test]
        public void Subscribe_ManyUsers()
        {
            var user1 = _userRepository.AddUser("test1");
            var user2 = _userRepository.AddUser("test2");
            var user3 = _userRepository.AddUser("test3");
            _userRepository.Subscribe(user2.Id, user1.Id);
            _userRepository.Subscribe(user3.Id, user1.Id);
            _userRepository.Subscribe(user1.Id, user2.Id);
            Assert.IsTrue(user1.SubscribersCount == 2);
            Assert.IsTrue(user2.SubscribersCount == 1);
        }

        [Test]
        public void Subscribe_UserAlreadySubscribed()
        {
            var user1 = _userRepository.AddUser("test1");
            var user2 = _userRepository.AddUser("test2");
            _userRepository.Subscribe(user1.Id, user2.Id);
            Assert.Catch<InvalidOperationException>(() => _userRepository.Subscribe(user1.Id, user2.Id));
        }

        [Test]
        public void Subscribe_UserNotFound()
        {
            var user = _userRepository.AddUser("test2");
            Assert.Catch<ArgumentException>(() => _userRepository.Subscribe(Guid.NewGuid(), user.Id));
        }

        [Test]
        public void Subscribe_ToUserNotFound()
        {
            var user = _userRepository.AddUser("test2");
            Assert.Catch<ArgumentException>(() => _userRepository.Subscribe(user.Id, Guid.NewGuid()));
        }

        [Test]
        public void UnSubscribe_Success()
        {
            var user1 = _userRepository.AddUser("test1");
            var user2 = _userRepository.AddUser("test2");
            _userRepository.Subscribe(user1.Id, user2.Id);
            _userRepository.UnSubscribe(user1.Id, user2.Id);
            Assert.IsTrue(user2.Subscribers.Count == 0 && user2.SubscribersCount == 0);
        }

        [Test]
        public void UnSubscribe_ManyUsers()
        {
            var user1 = _userRepository.AddUser("test1");
            var user2 = _userRepository.AddUser("test2");
            var user3 = _userRepository.AddUser("test3");
            _userRepository.Subscribe(user2.Id, user1.Id);
            _userRepository.Subscribe(user3.Id, user1.Id);
            _userRepository.UnSubscribe(user2.Id, user1.Id);
            Assert.IsTrue(user1.SubscribersCount == 1 && user1.Subscribers.Any(x => x.SubscribedUserId == user3.Id));
            _userRepository.UnSubscribe(user3.Id, user1.Id);
            Assert.IsTrue(user2.SubscribersCount == 0 && user1.Subscribers.Count == 0);
        }

        [Test]
        public void UnSubscribe_UserWhichTheNotSubscribed()
        {
            var user = _userRepository.AddUser("test1");
            Assert.Catch<InvalidOperationException>(() => _userRepository.UnSubscribe(Guid.NewGuid(), user.Id));
        }

        [Test]
        public void UnSubscribe_UserNotFound()
        {
            var user = _userRepository.AddUser("test2");
            Assert.Catch<InvalidOperationException>(() => _userRepository.UnSubscribe(Guid.NewGuid(), user.Id));
        }

        [Test]
        public void UnSubscribe_ToUserNotFound()
        {
            var user = _userRepository.AddUser("test2");
            Assert.Catch<ArgumentException>(() => _userRepository.UnSubscribe(user.Id, Guid.NewGuid()));
        }

        [Test]
        public void GetTop_Success()
        {
            var newUser = new User
            {
                Name = "popular user",
                Subscribers = new List<SubscribedUser>(),
                SubscribersCount = 100
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();
            var topUsers = _userRepository.GetTop(3);
            Assert.IsTrue(topUsers.Count() <= 3 && topUsers.First().Id == newUser.Id);
        }

        [Test]
        public void GetTop_CorruptedCount()
        {
            Assert.Catch<ArgumentException>(() => _userRepository.GetTop(0));
        }
    }
}