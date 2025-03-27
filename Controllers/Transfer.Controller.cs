using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrimeraWebAPI.Models;
using MongoDB.Driver;

namespace PrimeraWebAPI.Controllers
{
    [ApiController]
    [Route("api/v0/[controller]")]
    [Authorize]
    public class TransferController : ControllerBase {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoClient _client;

        public TransferController() {
            _usersCollection = UsersService.GetUsersDb(); // Use shared DB connection
            _client = _usersCollection.Database.Client;
        }

        public class TransferModel {
            public string Username { get; set; }
            public int Amount { get; set; }
        }

        [HttpPost]
        public IActionResult Transfer([FromBody] TransferModel transfer) {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username)) {
                return Unauthorized("Invalid user session.");
            }

            if (transfer.Amount <= 0) {
                return BadRequest("Amount must be greater than zero.");
            }

            using (var session = _client.StartSession()) {
                session.StartTransaction();
                try {
                    var senderFilter = Builders<User>.Filter.Eq(u => u.Username, username);
                    var receiverFilter = Builders<User>.Filter.Eq(u => u.Username, transfer.Username);

                    var sender = _usersCollection.Find(session, senderFilter).FirstOrDefault();
                    var receiver = _usersCollection.Find(session, receiverFilter).FirstOrDefault();

                    if (sender == null) return NotFound("Sender not found.");
                    if (receiver == null) return NotFound("Receiver not found.");
                    if (sender.Username == receiver.Username) return BadRequest("Cannot transfer funds to yourself.");
                    if (sender.Balance < transfer.Amount) return BadRequest("Insufficient funds.");

                    var senderUpdate = Builders<User>.Update.Inc(u => u.Balance, -transfer.Amount);
                    var receiverUpdate = Builders<User>.Update.Inc(u => u.Balance, transfer.Amount);

                    var senderResult = _usersCollection.UpdateOne(session, senderFilter & Builders<User>.Filter.Gte(u => u.Balance, transfer.Amount), senderUpdate);
                    if (senderResult.ModifiedCount == 0) {
                        session.AbortTransaction();
                        return BadRequest("Insufficient funds or concurrent transaction conflict.");
                    }

                    _usersCollection.UpdateOne(session, receiverFilter, receiverUpdate);

                    session.CommitTransaction();
                    return Ok( new { Message = $"Transferred ${transfer.Amount} to {receiver.Username}. New balance: ${sender.Balance - transfer.Amount}" });
                } catch {
                    session.AbortTransaction();
                    return StatusCode(500, "Transaction failed");
                }
            }
        }
    }

}