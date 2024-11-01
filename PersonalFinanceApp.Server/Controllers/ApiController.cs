using Microsoft.AspNetCore.Mvc;
using Going.Plaid;
using Going.Plaid.Entity;
using Going.Plaid.Link;
using System.Globalization;
using System.Net;
using System.Security.Principal;
using System.Transactions;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.Text.Json.Nodes;
using System.Net.Mime;
using Going.Plaid.Item;

namespace PersonalFinanceApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger<ApiController> _logger;

        private readonly PlaidClient _client;

        public ApiController(ILogger<ApiController> logger,
            IConfiguration configuration, PlaidClient client)
        {
            _logger = logger;
            _configuration = configuration;
            _client = client;
        }

        [HttpGet("GetLinkToken")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK, contentType: "application/json")]
        [ProducesResponseType(typeof(PlaidError), StatusCodes.Status400BadRequest,contentType: "application/json")]
        public async Task<IActionResult> GetLinkToken()
        {
            try
            {
                var response = await _client.LinkTokenCreateAsync(new LinkTokenCreateRequest()
                {
                    User = new LinkTokenCreateRequestUser { ClientUserId = Guid.NewGuid().ToString(), },
                    ClientName = "Personal Finance App",
                    Products = [Products.Auth, Products.Identity, Products.Transactions],
                    Language = Language.English,
                    CountryCodes = [CountryCode.Gb],
                });

                if (response.Error != null)
                {
                    dynamic error = JsonNode.Parse(JsonSerializer.Serialize(response.Error))!;
                    string msg = (string)error["error_message"];

                    _logger.LogError(msg);
                    return StatusCode(StatusCodes.Status400BadRequest, response);
                }

                _logger.LogInformation("Successfully obtained link token: {token}", response.LinkToken);
                return Ok(response.LinkToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("GetAccessToken")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK, contentType: "application/json")]
        [ProducesResponseType(typeof(PlaidError), StatusCodes.Status400BadRequest, contentType: "application/json")]
        public async Task<IActionResult> GetAccessToken([FromBody]string publicToken)
        {
            try
            {
                var response = await _client.ItemPublicTokenExchangeAsync(new ItemPublicTokenExchangeRequest()
                {
                    PublicToken = publicToken
                });

                if (response.Error != null)
                {
                    dynamic error = JsonNode.Parse(JsonSerializer.Serialize(response.Error))!;
                    string msg = (string)error["error_message"];

                    _logger.LogError(msg);
                    return StatusCode(StatusCodes.Status400BadRequest, response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
