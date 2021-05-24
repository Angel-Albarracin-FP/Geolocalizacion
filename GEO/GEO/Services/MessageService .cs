using System;
using System.Text;
using System.Text.Json;
using GEO.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GEO.Services
{
    public interface IMessageService
    {
        bool SendGeocodificar(Direccion direccion);
    }

    public class MessageService : IMessageService
    {
        ConnectionFactory _factory;
        IConnection _conn;
        IModel _channel;
        IAddressService _addressService;
        public MessageService(IAddressService addressService)
        {
            _addressService = addressService;
            _factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            _factory.UserName = "guest";
            _factory.Password = "guest";
            _conn = _factory.CreateConnection();
            _channel = _conn.CreateModel();
            _channel.QueueDeclare("geolocalizar", false, false, false, null);
            _channel.QueueDeclare("geocodificado", false, false, false, null);
            /*
            var geocodificado = new EventingBasicConsumer(_channel);
            geocodificado.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var direccion = JsonSerializer.Deserialize<Direccion>(message);
                _addressService.PutAddress(direccion.Id, direccion);
                
            };
            _channel.BasicConsume("geocodificado", true, geocodificado);
            */
        }

        public void UpdateAddress(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            var direccion = JsonSerializer.Deserialize<Direccion>(message);
            _addressService.PutAddress(direccion.Id, direccion);
        }

        public bool SendGeocodificar(Direccion direccion)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(direccion));
            _channel.BasicPublish(exchange: "",
                                routingKey: "geolocalizar",
                                basicProperties: null,
                                body: body);
            return true;
        }
    }
}