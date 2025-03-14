using Amazon.SQS.Model;
using Amazon.SQS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Amazon;

namespace AmazonSQS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagingController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> SendMessageAsync([FromBody] string message)
        {
            var client = new AmazonSQSClient(RegionEndpoint.USEast2);
            var queueUrl = await client.GetQueueUrlAsync("MyTestQueue");

            var messageSQS = new SendMessageRequest
            {
                //QueueUrl = "https://sqs.us-east-2.amazonaws.com/140023366146/MyTestQueue",
                QueueUrl = queueUrl.QueueUrl,
                MessageBody = new { Text = message, Date = DateTime.Now }.ToString()
            };

            await client.SendMessageAsync(messageSQS);

            return Ok();
        }
    }
}
