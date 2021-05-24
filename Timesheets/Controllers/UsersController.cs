using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheets.Domain.Interfaces;
using Timesheets.Models;
using Timesheets.Models.Dto;

namespace Timesheets.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly ILoginManager _loginManager;

        public UsersController(IUserManager userManager, ILoginManager loginManager)
        {
            _userManager = userManager;
            _loginManager = loginManager;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromQuery] User user)
        {
            var token = await _loginManager.Authenticate(user);
            if (string.IsNullOrWhiteSpace(token.AccessToken))
            {
                return BadRequest(new {message = "Username or password is incorrect"});
            }
            
            return Ok(token.AccessToken);
        }
        
        [Authorize(Roles = "admin")]
        [HttpGet("id")]
        public IActionResult Get([FromQuery] Guid id)
        {
            var result = _userManager.GetItem(id);

            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var result = await _userManager.GetItems();
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserRequest user)
        {
            var id = await _userManager.Create(user);

            return Ok(id);
        }


        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UserRequest user)
        {
            await _userManager.Update(id, user);
            return Ok();
        }
    }
}