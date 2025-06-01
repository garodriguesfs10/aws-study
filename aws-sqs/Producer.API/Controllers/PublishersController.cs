using Amazon.SQS;
using Amazon.SQS.Model;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Producer.API.Command;
using System.Text.Json;

namespace Producer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly ILogger<PublishersController> _logger;
        private readonly IAmazonSQS _sqs;

        public PublishersController(ILogger<PublishersController> logger, IAmazonSQS sqs)
        {
            _logger = logger;
            _sqs = sqs;
        }

        [HttpPost("register")]
        public async Task<IActionResult> PublishMessage(UserRegistrationCommand command)
        {
            // validate the incoming request object
            // register the user into the database
            var userId = Guid.NewGuid();

            // create event to notify external services
            var userRegisteredEvent = new UserRegisteredEvent(userId, command.UserName, command.Email);

            // assume that the name of the registered queue is 'user-registered'
            var queueName = "user-registered";

            // get queue url
            // or create a new queue if it doesnt already exist
            var queueUrl = string.Empty;
            try
            {
                var response = await _sqs.GetQueueUrlAsync(queueName);
                queueUrl = response.QueueUrl;
            }
            catch (QueueDoesNotExistException)
            {
                _logger.LogInformation("Queue {queueName} doesn't exist. Creating...", queueName);
                var response = await _sqs.CreateQueueAsync(queueName);
                queueUrl = response.QueueUrl;
            }

            // create sqs request
            var sendMessageRequest = new SendMessageRequest()
            {
                QueueUrl = queueUrl,
                MessageBody = JsonSerializer.Serialize(userRegisteredEvent)
            };
            _logger.LogInformation("Publishing message to Queue {queueName} with body : \n {request}", queueName, sendMessageRequest.MessageBody);

            // send sqs request
            var result = await _sqs.SendMessageAsync(sendMessageRequest);

            return Ok();
        }
    }
}
