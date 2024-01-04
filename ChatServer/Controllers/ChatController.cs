using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using ChatServer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRedisStack;
using NuGet.Protocol;
using StackExchange.Redis;

namespace ChatServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase {
        private readonly IDatabase _redis;

        private JsonSerializerOptions _serializerOptions => new JsonSerializerOptions() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        
        // GET: api/Chat
        public ChatController(IConnectionMultiplexer muxer) {
            _redis = muxer.GetDatabase();
        }

        [HttpGet]
        public IEnumerable<ChatMessage> GetMessages([FromQuery] Guid userId) {
            // Get userId from JWT
            
            // Read Cache
            var messages = _redis.StringGet(userId.ToString()).ToJson();
            // Return Messages
            return JsonSerializer.Deserialize<List<ChatMessage>>(messages, _serializerOptions);
        }

        // POST: api/Chat
        [HttpPost]
        public ActionResult SendMessage([FromBody] ChatMessage value) {
            // Get recipient userId
            
            // Store message
            var keyPair = new KeyValuePair<RedisKey, RedisValue>(value.Recipient, JsonSerializer.Serialize(value));
            _redis.StringSet(key: value.Recipient, value: JsonSerializer.Serialize(value), TimeSpan.FromDays(3));

            return Ok();
        }
    }
}
