using System.Text.Json;
using Azure.Storage.Queues;

namespace queue_storages
{
    public class WeatherDataService : BackgroundService
    {
        private readonly QueueClient _queueClient;
        private readonly ILogger<WeatherDataService> _logger;

        public WeatherDataService(ILogger<WeatherDataService> logger, QueueClient queueClient)
        {
            _queueClient = queueClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                var queueMessage = await _queueClient.ReceiveMessageAsync();
                if (queueMessage.Value != null)
                {
                    var weatherdata = JsonSerializer.Deserialize<WeatherForecast>(queueMessage.Value.MessageText);
                    _logger.LogWarning("Information: {weatherdata}", weatherdata);
                    await _queueClient.DeleteMessageAsync(queueMessage.Value.MessageId, queueMessage.Value.PopReceipt);
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}