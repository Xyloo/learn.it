﻿using learn.it.Models;

namespace learn.it.Repos.Interfaces
{
    public interface IUsersRepository
    {
        Task<User> CreateUser(User user);
        Task DeleteUser(int userId);
        Task<User?> GetUserById(int userId);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByUsername(string username);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> UpdateUser(User user);
    }
}
