using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using System.Text.Json;
using Going.Plaid.Entity;
using PersonalFinanceApp.Server;
using Going.Plaid;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PersonalFinanceApp.Server
{
    public static class Utils
    {
        //public static ObjectResult Error(ResponseBase response, ILogger logger)
        //{
        //    dynamic errorResult = JsonNode.Parse(JsonSerializer.Serialize(response.Error))!;
        //    string msg = (string)errorResult["error_message"];

        //    logger.LogError(msg);
        //    return Results.BadRequest(msg);
        //}
    }
}
