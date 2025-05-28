using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLogs.Domain.Entities;
using TransactionLogs.Domain.Interfaces;
using TransactionLogs.Infrastructure.Data;

namespace TransactionLogs.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TransactionContext context;

        public UserRepository(TransactionContext context)
        {
            this.context = context;
        }
        public async Task AddUserAsync(User user)
        {
            // Validate the input user object  
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            // Add the user to the database  
            await context.Users.AddAsync(user);

            // Save changes to the database  
            await context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            // Retrieve the user with the given ID from the database  
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

            // If the user does not exist, throw an exception  
            if (user == null)
            {
                throw new ArgumentException($"User with ID {id} does not exist.");
            }

            // Remove the user from the database  
            context.Users.Remove(user);

            // Save changes to the database  
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            // Retrieve all users from the database  
            var users = await context.Users.ToListAsync();

            // Return the list of users  
            return users;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            // Retrieve the user with the given ID from the database  
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

            // Return the user (null if not found)  
            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            // Check if the user exists in the database  
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (existingUser == null)
            {
                throw new ArgumentException($"User with ID {user.Id} does not exist.");
            }

            // Update the user properties  
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;

            context.Users.Update(existingUser); // Update the existing user in the context

            // Save changes to the database  
            await context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            // Check if a user with the given ID exists in the database  
            return await context.Users.AnyAsync(u => u.Id == id);
        }
    }
}
