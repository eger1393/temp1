﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ZELF.Test.Data.Models;

namespace ZELF.Test.Data.Repositories
{
    public interface IUserRepository
    {
        public User AddUser(string name);
        public User GetUser(Guid id);
        public User Subscribe(Guid userId, Guid toUserId);
        public User UnSubscribe(Guid userId, Guid toUserId);
        public IEnumerable<User> GetTop(int count);
    }
    
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Добавляет нового пользователя
        /// </summary>
        /// <param name="name">Имя пользователя</param>
        /// <returns>Пользователь</returns>
        public User AddUser(string name)
        {
            var newUser = new User()
            {
                Name = name,
                Subscribers = new List<User>(),
                SubscribersCount = 0
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return newUser;
        }

        /// <summary>
        /// Возвращает данные о пользователе, включая его подписчиков
        /// </summary>
        /// <param name="id">Ид пользователя</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Когда пользователь с переданным Ид не найден</exception>
        public User GetUser(Guid id)
        {
            var user = _context.Users.Include(x => x.Subscribers).FirstOrDefault(x => x.Id == id);
            if (user is null)
                throw new ArgumentException($"User with id: {id} not found");
            return user;
        }

        /// <summary>
        /// Подписывает на пользователя
        /// </summary>
        /// <param name="userId">Пользователь которого подписываем</param>
        /// <param name="toUserId">На кого подписываем</param>
        /// <returns>Обновленные данные о пользхователе</returns>
        /// <exception cref="ArgumentException">Если пользователь с переданным Ид не найден</exception>
        public User Subscribe(Guid userId, Guid toUserId)
        {
            var subscribingUser = GetUserWithOutSubscribers(userId);
            var user = GetUser(toUserId);
            if (user.Subscribers.Any(x => x.Id == userId)) return user;
            user.Subscribers.Add(subscribingUser);
            user.SubscribersCount++;
            _context.SaveChanges();
            return user;
        }

        /// <summary>
        /// Одписать пользователя
        /// </summary>
        /// <param name="userId">Пользователь которого отписываем</param>
        /// <param name="toUserId">От кого отписываем</param>
        /// <returns>Обновленные данные</returns>
        /// <exception cref="ArgumentException">Если пользователь с переданным Ид не найден</exception>
        /// <exception cref="InvalidOperationException">Если пытаемся отписать не подписанного пользователя</exception>
        public User UnSubscribe(Guid userId, Guid toUserId)
        {
            var user = GetUser(toUserId);
            if (!user.Subscribers.Remove(user.Subscribers.FirstOrDefault(x => x.Id == userId)))
                throw new InvalidOperationException($"User with id: {userId} not found with {toUserId} subscribers");
            user.SubscribersCount--;
            _context.SaveChanges();
            return user;
        }

        /// <summary>
        /// Возвращает пользователей с наибольшим чисслом подписок
        /// </summary>
        /// <param name="count">кол-во пользователей которое надо вернуть</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Если переданно не положительное кол-во</exception>
        public IEnumerable<User> GetTop(int count)
        {
            if (count <= 0)
                throw new ArgumentException($"count must be greater than 0");
            return _context.Users.OrderBy(x => x.SubscribersCount).Take(count);
        }

        /// <summary>
        /// Возвращает данные о пользователе, не вытягиваяя инфу о подписках
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private User GetUserWithOutSubscribers(Guid id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user is null)
                throw new ArgumentException($"User with id: {id} not found");
            return user;
        }
    }
}