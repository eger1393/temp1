using System;
using ApiGateway.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateUser model)
        {
            var user = _userRepository.AddUser(model.Name);
            return Ok(user);
        }

        [HttpPatch("{userId:guid}/subscribe/{toUserId:guid}")]
        public IActionResult Subscribe(Guid userId, Guid toUserId)
        {
            var user = _userRepository.Subscribe(userId, toUserId);
            return Ok(user);
        }
        
        [HttpPatch("{userId:guid}/unsubscribe/{toUserId:guid}")]
        public IActionResult UnSubscribe(Guid userId, Guid toUserId)
        {
            var user = _userRepository.UnSubscribe(userId, toUserId);
            return Ok(user);
        }

        [HttpGet("top/{count:int}")]
        public IActionResult GetTop(int count)
        {
            var users = _userRepository.GetTop(count);
            return Ok(users);
        }
    }
}