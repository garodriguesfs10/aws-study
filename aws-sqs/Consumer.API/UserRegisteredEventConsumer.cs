using Amazon.SQS.Model;
using Amazon.SQS;

namespace Consumer.API
{
    public class UserRegisteredEventConsumer(IAmazonSQS sqs, ILogger<UserRegisteredEventConsumer> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            // assume that the name of the registered queue is 'user-registered'
            var queueName = "user-registered";

            logger.LogInformation("Polling Queue {queueName}", queueName);

            // get queue url
            // or create a new queue if it doesnt already exist
            var queueUrl = string.Empty;
            try
            {
                var response = await sqs.GetQueueUrlAsync(queueName);
                queueUrl = response.QueueUrl;
            }
            catch (QueueDoesNotExistException)
            {
                logger.LogInformation("Queue {queueName} doesn't exist. Creating...", queueName);
                var response = await sqs.CreateQueueAsync(queueName);
                queueUrl = response.QueueUrl;
            }

            var receiveRequest = new ReceiveMessageRequest()
            {
                QueueUrl = queueUrl
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await sqs.ReceiveMessageAsync(receiveRequest);
                if (response.Messages?.Count > 0)
                {
                    foreach (var message in response.Messages)
                    {
                        logger.LogInformation("Received Message from Queue {queueName} with body as : \n {body}", queueName, message.Body);
                        //perform some processing.
                        //mock 2 seconds delay for processing
                        Task.Delay(2000).Wait();
                        await sqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
                    }
                }
            }
        }
    }
}
