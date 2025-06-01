using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IAmazonSimpleNotificationService _sns;

        public NotificationsController(IAmazonSimpleNotificationService sns)
        {
            _sns = sns;
        }

        [HttpPost]
        public async Task<IActionResult> PublishNotification(CreateOrderRequest request)
        {
            // create notification
            var notification = new OrderCreatedNotification(request.OrderId, request.CustomerId, request.ProductDetails);

            // create topic if needed
            var topicName = "OrderCreated";
            var topicArn = string.Empty;
            try
            {
                var topicExists = await _sns.FindTopicAsync(topicName);
                topicArn = topicExists.TopicArn;
            }
            catch (Exception)
            {
                var newTopic = await _sns.CreateTopicAsync(topicName);
                topicArn = newTopic.TopicArn;
            }

            // create publish request
            var publishRequest = new PublishRequest()
            {
                TopicArn = topicArn,
                Message = JsonSerializer.Serialize(notification),
                Subject = $"Order#{request.OrderId}"
            };

            publishRequest.MessageAttributes.Add("Scope", new MessageAttributeValue()
            {
                DataType = "String",
                StringValue = "Lambda"
            });

            await _sns.PublishAsync(publishRequest);
            return Ok();
        }
    }
}
