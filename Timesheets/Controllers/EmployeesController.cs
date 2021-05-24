using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheets.Domain.Implementation;
using Timesheets.Domain.Interfaces;
using Timesheets.Models;
using Timesheets.Models.Dto;

namespace Timesheets.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeManager _employeeManager;
        private readonly ILoginManager _loginManager;

        public EmployeesController(IEmployeeManager employeeManager, ILoginManager loginManager)
        {
            _employeeManager = employeeManager;
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
        
        [Authorize(Roles = "user")]
        [HttpGet("{id}")]
        public IActionResult Get([FromQuery] Guid id)
        {
            var result = _employeeManager.GetItem(id);

            return Ok(result);
        }

        [Authorize(Roles = "user")]
        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var result = await _employeeManager.GetItems();
            return Ok(result);
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeRequest employee)
        {
            var id = await _employeeManager.Create(employee);

            return Ok(id);
        }

        [Authorize(Roles = "user")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] EmployeeRequest employee)
        {
            await _employeeManager.Update(id, employee);
            return Ok();
        }
    }
}