using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionLogs.Domain.Entities;
using TransactionLogs.Domain.Interfaces;

namespace TransactionLogs.web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync(); // Assuming GetAllUsersAsync exists in IUserRepository  
            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id); // Assuming GetUserByIdAsync exists in IUserRepository  
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(user);
        }
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User cannot be null.");
            }
            await _userRepository.AddUserAsync(user); // Assuming AddUserAsync exists in IUserRepository  
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (user == null || user.Id != id)
            {
                return BadRequest("User data is invalid.");
            }
            var existingUser = await _userRepository.GetUserByIdAsync(id); // Assuming GetUserByIdAsync exists in IUserRepository  
            if (existingUser == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            await _userRepository.UpdateUserAsync(user); // Assuming UpdateUserAsync exists in IUserRepository  
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id); // Assuming GetUserByIdAsync exists in IUserRepository  
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            await _userRepository.DeleteUserAsync(id); // Assuming DeleteUserAsync exists in IUserRepository  
            return NoContent();
        }
        


    }
}
