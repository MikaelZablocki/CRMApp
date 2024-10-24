﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Repository _repository;

        // Inject IConfiguration into the controller and pass it to the repository
        public UsersController()
        {
            _repository = new Repository();
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _repository.GetUserById(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }

        // POST: api/user
        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Invalid user data.");
            }

            _repository.AddUser(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }

        [HttpGet("login")]
        public IActionResult GetUserByCredentials(string username, string password)
        {
            var user = _repository.GetUserByCredentials(username, password);
            if (user == null)
            {
                return NotFound("User not found or invalid credentials.");
            }
            return Ok(user);
        }

    }
}