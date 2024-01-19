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
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using StackExchange.Redis;

namespace ChatServer.Controllers {
    [ApiController]
    public class ChatController : ControllerBase {
        private readonly RedisClient _redis;

        private JsonSerializerOptions _serializerOptions => new JsonSerializerOptions() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        
        public ChatController(RedisClient redis) {
            _redis = redis;
        }
        
        [HttpGet]
        [Route("chat/{username}")]
        public IEnumerable<ChatMessage> GetMessages(string username) {
            var redisTypedClient = _redis.As<ChatMessage>();
            // Read Cache
            var messageList = redisTypedClient.Lists["message:" + username];
            var messages = messageList.GetAll();
            messageList.RemoveAll();
            // Return Messages
            return messages;
        }
        
        [HttpPost]
        [Route("chat")]
        public ActionResult SendMessage([FromBody] ChatMessage message) {
            var redisTypedClient = _redis.As<ChatMessage>();
            var redisMessageList = redisTypedClient.Lists["message:" + message.Recipient];
            // Store message
            redisMessageList.Add(message);
            _redis.ExpireEntryIn("message:" + message.Recipient, TimeSpan.FromDays(3));
            return Ok();
        }
        
        [HttpPost]
        [Route("user")]
        public ActionResult Login([FromBody] SignUp signUp) {
            _redis.Set("publicKey:" + signUp.Username, signUp.PublicKey);
            return Ok();
        }
        
        [HttpGet("user/{username}")]
        public string GetPublicKey(string username) {
            return _redis.Get<string>("publicKey:" + username);
        }
    }
}
