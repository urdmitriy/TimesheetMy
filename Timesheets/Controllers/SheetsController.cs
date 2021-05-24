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
    public class SheetsController: ControllerBase
    {
        private readonly ISheetManager _sheetManager;
        private readonly IContractManager _contractManager;
        private readonly ILoginManager _loginManager;

        public SheetsController(ISheetManager sheetManager, IContractManager contractManager, ILoginManager loginManager)
        {
            _sheetManager = sheetManager;
            _contractManager = contractManager;
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
            var result = _sheetManager.GetItem(id);
            
            return Ok(result);
        }
        
        [Authorize(Roles = "user")]
        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var result = await _sheetManager.GetItems();
            return Ok(result);
        }

        /// <summary> Возвращает запись табеля </summary>
        [Authorize(Roles = "user")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SheetRequest sheet)
        {
            var isAllowedToCreate = await _contractManager.CheckContractIsActive(sheet.ContractId);

            if (isAllowedToCreate !=null && !(bool)isAllowedToCreate)
            {
                return BadRequest($"Contract {sheet.ContractId} is not active or not found.");
            }
            
            var id = await _sheetManager.Create(sheet);
            return Ok(id);
        }

        /// <summary> Обновляет запись табеля </summary>
        [Authorize(Roles = "user")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] SheetRequest sheet)
        {
            var isAllowedToCreate = await _contractManager.CheckContractIsActive(sheet.ContractId);

            if (isAllowedToCreate !=null && !(bool)isAllowedToCreate)
            {
                return BadRequest($"Contract {sheet.ContractId} is not active or not found.");
            }

            await _sheetManager.Update(id, sheet);

            return Ok();
        }
    }
}