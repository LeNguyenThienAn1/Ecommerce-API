using Application.DTOs;
using Application.EntityHandler.Queries;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => /api/users
    public class UsersController : ControllerBase
    {
        private readonly IUserQueries _userQueries;
        private readonly IUserService _userService;

        public UsersController(IUserQueries userQueries, IUserService userService)
        {
            _userQueries = userQueries;
            _userService = userService;
        }

        // 🟦 GET: /api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userQueries.GetAllUsersAsync();
            return Ok(users);
        }

        // 🟦 GET: /api/users/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userQueries.GetUserByIdAsync(id);
            if (user == null)
                return NotFound($"Không tìm thấy người dùng với ID: {id}");
            return Ok(user);
        }

        // 🟦 GET: /api/users/search?keyword=abc
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var users = await _userQueries.SearchUsersAsync(keyword);
            return Ok(users);
        }

        // 🟦 GET: /api/users/phone/{phoneNumber}
        [HttpGet("phone/{phoneNumber}")]
        public async Task<IActionResult> GetByPhone(string phoneNumber)
        {
            var user = await _userQueries.GetUserByPhoneNumberAsync(phoneNumber);
            if (user == null)
                return NotFound("Không tìm thấy người dùng với số điện thoại này.");
            return Ok(user);
        }

        // 🟩 POST: /api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto userDto)
        {
            var result = await _userService.CreateUserAsync(userDto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // 🟨 PUT: /api/users/{id}
        [HttpPost(":update")]
        public async Task<IActionResult> Update([FromBody] UserDto userDto)
        {
            var updated = await _userService.UpdateUserAsync(userDto);
            if (updated == null)
                return NotFound($"Không tìm thấy người dùng với ID: {userDto.Id}");
            return Ok(updated);
        }

        // 🟥 DELETE: /api/users/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
                return NotFound($"Không tìm thấy người dùng với ID: {id}");
            return NoContent();
        }

        // 🟧 PATCH: /api/users/{id}/status?isActive=true
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] bool isActive)
        {
            var success = await _userService.ChangeUserStatusAsync(id, isActive);
            if (!success)
                return NotFound($"Không tìm thấy người dùng với ID: {id}");
            return Ok(new { message = $"Đã {(isActive ? "kích hoạt" : "vô hiệu hóa")} người dùng." });
        }
    }
}

