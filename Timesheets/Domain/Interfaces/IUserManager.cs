﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheets.Models;
using Timesheets.Models.Dto;

namespace Timesheets.Domain.Interfaces
{
    public interface IUserManager
    {
        Task<User> GetItem(Guid id);
        Task<IEnumerable<User>> GetItems();
        Task<Guid> Create(UserRequest user);
        Task Update(Guid id, UserRequest user);
        
        /// <summary> Возвращает пользователя по логину и паролю </summary>
        Task<User> GetUser(LoginRequest request);

        /// <summary> Создает нового пользователя </summary>
        Task<Guid> CreateUser(CreateUserRequest request);
    }
}