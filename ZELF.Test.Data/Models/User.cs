using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ZELF.Test.Data.Models
{
    public class User
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Имя, состоит из пробелов и букв, содержит макс 64 символа
        /// </summary>
        [System.ComponentModel.DataAnnotations.MaxLength(64)]
        [Required]
        public string Name { get; set; }
        
        /// <summary>
        /// Подписчики
        /// </summary>
        public List<SubscribedUser> Subscribers { get; set; }
        
        // TODO Подумать как сделать поле вычисляемым на уровне БД
        // TODO Подумать стоит ли делать индекс по полю
        /// <summary>
        /// Кол-во подписчиков
        /// Используем переменную для упрощения поиска топа
        /// </summary>
        public uint SubscribersCount { get; set; }
    }
}