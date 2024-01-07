using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using ChatServer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRedisStack;
using StackExchange.Redis;

namespace ChatServer.Controllers {
    [ApiController]
    public class ChatController : ControllerBase {
        private readonly IDatabase _redis;
        private readonly HttpClient _httpClient;

        private JsonSerializerOptions _serializerOptions => new JsonSerializerOptions() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        
        public ChatController(IConnectionMultiplexer muxer) {
            _redis = muxer.GetDatabase();
            _httpClient = new HttpClient() {
                BaseAddress = new Uri($"{Program.Config.KeycloakUrl}/")
            };
        }

        [Authorize]
        [HttpGet]
        [Route("Chat")]
        public IEnumerable<ChatMessage> GetMessages() {
            // Get Username
            var username = HttpContext.Items["username"];
            // Read Cache
            var messages = _redis.StringGet(username.ToString());
            // Return Messages
            return JsonSerializer.Deserialize<List<ChatMessage>>(messages, _serializerOptions);
        }
        
        [Authorize]
        [HttpPost]
        [Route("chat")]
        public ActionResult SendMessage([FromBody] ChatMessage message) {
            // Store message
            _redis.StringSet(key: message.Recipient, value: JsonSerializer.Serialize(message), TimeSpan.FromDays(3));

            return Ok();
        }
        
        [HttpPost]
        [Route("user")]
        public string Login([FromBody] Login login) {
            // Get JWT from keycloak
            
            // Return JWT to user
            return "value";
        }
        
        [Authorize]
        [HttpGet("user/{username}")]
        public string GetPublicKey(string username) {
            return _redis.StringGet(username).ToString();
        }
    }
}
