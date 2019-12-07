using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imagegram.API.Application.Repositories;
using Imagegram.API.Application.Validations;
using Imagegram.API.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Imagegram.API.Controllers
{
    [ApiExplorerSettings(GroupName = "imagegram")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRequestHeaders _requestHeaders;
        private readonly string UUID;

        public AccountController(IAccountRepository accountRepository, IRequestHeaders requestHeaders)
        {
            _accountRepository = accountRepository;
            _requestHeaders = requestHeaders;

            if (!string.IsNullOrWhiteSpace(_requestHeaders.UUID))
            {
                UUID = _requestHeaders.UUID;
            }
        }

        /// <summary>
        /// Create Account
        /// </summary>
        /// <remarks>
        /// Returns a UUID for a successful Account creation.
        /// </remarks>
        /// <returns>UUID</returns>
        /// <response code="201">
        /// {
        ///     "UUID": string
        /// }
        /// </response>
        /// <response code="201">Successful Account creation</response>
        /// <response code="400">List of validation errors</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(void))]
        [HttpPost(Name = "CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountQuery query)
        {
            var id = Guid.NewGuid().ToString();
            var result = await _accountRepository.CreateAccount(query.Name, id);
            if (string.IsNullOrEmpty(result))
                return BadRequest();
            return Created("/Account/" + result, result);
        }

        /// <summary>
        /// Delete Account
        /// </summary>
        /// <remarks>
        /// Returns a UUID for a successful Account deletion.
        /// </remarks>
        /// <returns>UUID</returns>
        /// <response code="200">Successful Account Deletion</response>
        /// <response code="204">No account exists</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{uuid}", Name = "DeleteAccount")]
        public async Task<IActionResult> DeleteAccount(string uuid)
        {
            var result = await _accountRepository.DeleteAccount(uuid);
            if (string.IsNullOrEmpty(result))
                return NoContent();
            return Ok(result);
        }
    }
}