
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Net;

namespace AmazonSQS.API.Consumer;

public class MessagingConsumer : BackgroundService
{
    private readonly AmazonSQSClient _client;
    private readonly string _queueUrl;
    public MessagingConsumer()
    {
        _client = new AmazonSQSClient(RegionEndpoint.USEast2);
        _queueUrl = GetQueueUrlAsyncText(_client).Result;
    }

    private async Task<string> GetQueueUrlAsyncText(AmazonSQSClient client)
    {
        var queueResponse = await client.GetQueueUrlAsync("MyTestQueue");
        return queueResponse.QueueUrl;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var receiveMessageRequest = new ReceiveMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MaxNumberOfMessages = 10,
                    MessageAttributeNames = new List<string> { "All" }, //Retrieves all message attributes.
                    WaitTimeSeconds = 5
                };

                var messages = await _client.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);

                if (messages.HttpStatusCode == HttpStatusCode.OK)
                {
                    foreach (var message in messages.Messages)
                    {
                        Console.WriteLine($"Message: {message.Body}");
                        var deleteMessageRequest = new DeleteMessageRequest
                        {
                            QueueUrl = _queueUrl,
                            ReceiptHandle = message.ReceiptHandle
                        };
                        await _client.DeleteMessageAsync(deleteMessageRequest);
                    }
                }
                await Task.Delay(1000, stoppingToken);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    
    }
}
