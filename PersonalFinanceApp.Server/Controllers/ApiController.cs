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
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<string> GetLinkToken()
        {
            var response = await _client.LinkTokenCreateAsync(new LinkTokenCreateRequest()
            {
                User = new LinkTokenCreateRequestUser { ClientUserId = Guid.NewGuid().ToString(), },
                ClientName = "Personal Finance App",
                Products = [Products.Auth, Products.Identity, Products.Transactions],
                Language = Language.English,
                CountryCodes = [CountryCode.Gb],
            });

            if(response.StatusCode == HttpStatusCode.OK)
            {
                _logger.LogInformation("Successfully obtained link token: {token}", response.LinkToken);
            }
            else if(response.Error != null)
            {
                dynamic error = JsonNode.Parse(JsonSerializer.Serialize(response.Error));
                string msg = (string)error["error_message"];

                _logger.LogError(msg);
            }

            return response.LinkToken;
        }
    }
}
