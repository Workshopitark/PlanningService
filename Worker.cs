using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlanningService;
using System.Xml.Linq;
using System.Text;
using System.Threading.Channels;
using System.Text.Json;

namespace PlanningService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _MQHostName = configuration["MQHostName"] ?? "rabbitmq";
        _pathCSV = configuration["pathCSV"] ?? string.Empty;

        _logger.LogInformation(_pathCSV + _MQHostName);
    }

    private readonly string _MQHostName;
    private readonly string _pathCSV;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        
        var factory = new ConnectionFactory { HostName = _MQHostName };
        using var connection = factory.CreateConnection();
        using var _channel = connection.CreateModel();

        _logger.LogInformation("connection lavet");
        

        _channel.QueueDeclare(queue: "planqueue",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        //

        Console.WriteLine(" [*] Waiting for messages.");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();

            var message = Encoding.UTF8.GetString(body);

            TaxaBooking? taxaBooking = JsonSerializer.Deserialize<TaxaBooking>(message);

            _logger.LogInformation("received kunde" + taxaBooking!.Kundenavn);

            File.AppendAllText(_pathCSV, $"{taxaBooking.Kundenavn},{taxaBooking.Starttidspunkt},{taxaBooking.Startsted},{taxaBooking.Endested}"+Environment.NewLine);
        };
        _channel.BasicConsume(queue: "planqueue",
                             autoAck: true,
                             consumer: consumer);



        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(20000, stoppingToken);
        }
    }
}
