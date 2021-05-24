using GEO.Data;
using GEO.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GEO.Services
{
    public class ReceiverMessage : BackgroundService
    {
        private IServiceProvider _sp;
        private ConnectionFactory _factory;
        private IConnection _conn;
        private IModel _channel;

        public ReceiverMessage(IServiceProvider sp)
        {
            _sp = sp;
            _factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            _factory.UserName = "guest";
            _factory.Password = "guest";
            _conn = _factory.CreateConnection();
            _channel = _conn.CreateModel();
            _channel.QueueDeclare("geocodificado", false, false, false, null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                _channel.Dispose();
                _conn.Dispose();
                return Task.CompletedTask;
            }

            var geocodificado = new EventingBasicConsumer(_channel);
            geocodificado.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var direccion = JsonSerializer.Deserialize<Direccion>(message);
                Task.Run(() =>
                {
                    using (var scope = _sp.CreateScope())
                    {
                        try
                        {
                            var db = scope.ServiceProvider.GetRequiredService<GeoContext>();
                            db.Direcciones.Update(direccion);
                            db.SaveChangesAsync().Wait();
                        }
                        catch (Exception e)
                        {
                            throw new NullReferenceException(e.Message);
                        }
                    }
                });

            };
            _channel.BasicConsume("geocodificado", true, geocodificado);

            return Task.CompletedTask;
        }
    }
}
