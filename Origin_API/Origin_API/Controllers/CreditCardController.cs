using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Origin_API.Models;
using Origin_API.Models.DTOs;
using Origin_API.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Origin_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreditCardController : ControllerBase
    {
        private readonly ICreditCardRepository _ATMRepository;
        public IConfiguration _configuration;

        public CreditCardController(ICreditCardRepository creditCardRepository, IConfiguration config)
        {
            _ATMRepository = creditCardRepository;
            _configuration = config;
        }

        [HttpGet("GetCards", Name = "GetCards")]
        public Task<IEnumerable<CreditCard>> GetCreditCards()
        {
            var cards = _ATMRepository.GetCreditCards();

            return cards;
        }

        [HttpPost("CardExists")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CardExists([FromBody] string CardNumber)
        {
            if (!await _ATMRepository.CreditCardExists(CardNumber))
            {
                return BadRequest(new { message = "Credit card not found." });
            }

            return Ok(CardNumber);
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var creditCard = await _ATMRepository.GetByNumber(loginDTO.CreditCardNumber);

                if (creditCard == null)
                {
                    return BadRequest(new { message = "Credit card not found." });
                }

                if (!creditCard.Enabled)
                {
                    return BadRequest(new { message = "The credit card was blocked." });
                }

                if (creditCard.Pin != loginDTO.Pin)
                {

                    bool isBlocked = await _ATMRepository.AddFailedLoginAttempt(creditCard.Id);

                    if (isBlocked)
                    {
                        return BadRequest(new { message = "Incorrect pin. Card blocked." });
                    }

                    return BadRequest(new { message = "Incorrect pin." });
                }

                // JWT Implementation
                var claims = new Claim[]
                {
                    new Claim("CreditCardId", creditCard.Id.ToString())
                };

                var singingCredentials = new SigningCredentials(
                                            new SymmetricSecurityKey(
                                                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                                            SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                                _configuration["Jwt:Issuer"],
                                _configuration["Jwt:Audience"],
                                claims,
                                null,
                                DateTime.UtcNow.AddMinutes(10),
                                singingCredentials);

                string tokenValue = new JwtSecurityTokenHandler()
                                        .WriteToken(token);

                LoginResponse response = new LoginResponse();
                response.Message = tokenValue;

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("GetBalance")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreditCardDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBalance([FromBody] string SecurityToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                handler.ValidateToken(SecurityToken, validationParameters, out _);
                var jwt = handler.ReadJwtToken(SecurityToken);

                var creditCard = await _ATMRepository.GetById(Int32.Parse(jwt.Claims.FirstOrDefault(c => c.Type == "CreditCardId")?.Value));

                if (creditCard != null)
                {
                    CreditCardDTO creditCardDTO = (CreditCardDTO)creditCard;

                    if (await _ATMRepository.RegisterBalance(creditCard.Id, "TEST CODE"))
                    {
                        return Ok(creditCardDTO);
                    }
                }

                return BadRequest(new { message = "Credit card not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Withdraw")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawDTO withdrawDTO)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                handler.ValidateToken(withdrawDTO.SecurityToken, validationParameters, out _);
                var jwt = handler.ReadJwtToken(withdrawDTO.SecurityToken);
                int cardId = Int32.Parse(jwt.Claims.FirstOrDefault(c => c.Type == "CreditCardId")?.Value);

                WithdrawResult result = await _ATMRepository.Withdraw(cardId, withdrawDTO.Amount);

                if (result == WithdrawResult.Non_sufficient_funds)
                {
                    return BadRequest(new { message = "Insufficient funds." });
                }

                int withdrawOperationId = await _ATMRepository.RegisterWithdraw(cardId, "TEST CODE", withdrawDTO.Amount);
                if (withdrawOperationId > -1)
                {
                    return Ok(withdrawOperationId);
                }

                return StatusCode(500, "An unhandeled error ocurred.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("WithdrawReport")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WithdrawOperationDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> WithdrawReport([FromBody] WithdrawOperationRequestDTO requestDTO)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                handler.ValidateToken(requestDTO.securityToken, validationParameters, out _);
                var jwt = handler.ReadJwtToken(requestDTO.securityToken);

                return Ok(await _ATMRepository.GetWithdrawOperation(requestDTO.withdrawOperationId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
