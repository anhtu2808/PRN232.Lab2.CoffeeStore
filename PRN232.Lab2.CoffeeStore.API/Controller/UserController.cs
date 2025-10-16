using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.Models.Enums;
using PRN232.Lab2.CoffeeStore.Models.Request.User;
using PRN232.Lab2.CoffeeStore.Services.IService;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;

namespace PRN232.Lab2.CoffeeStore.API.Controller
{
    [ApiController]
    [Route("users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserFilterRequest filter)
        {
            var pagedUsers = await _userService.GetUsers(filter);
            var response = new ApiResponse<object>
            {
                StatusCode = 200,
                Message = "Users retrieved successfully.",
                Data = pagedUsers
            };
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserById(id);

            var response = new ApiResponse<object>
            {
                StatusCode = 200,
                Message = "User retrieved successfully.",
                Data = user
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserRequest request)
        {
            var newUser = await _userService.CreateUser(request, Role.Customer);
            var response = new ApiResponse<object>
            {
                StatusCode = 201,
                Message = "User created successfully.",
                Data = newUser
            };
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserId }, response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserRequest request)
        {
            var updatedUser = await _userService.UpdateUser(id, request);
            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Message = "User updated successfully.",
                Data = updatedUser
            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUser(id);
            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Message = "User deleted successfully.",
                Data = null
            });
        }
    }
}
