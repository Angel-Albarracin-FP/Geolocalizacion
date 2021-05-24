using Mensajeria.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Geocodificador
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configuracion HTTP
            HttpClient httpClient = new HttpClient();
            var productValue = new ProductInfoHeaderValue("Geo", "1.0");
            httpClient.DefaultRequestHeaders.UserAgent.Add(productValue);

            // Configuracion Rabbit
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            factory.UserName = "guest";
            factory.Password = "guest";
            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();
            channel.QueueDeclare("geolocalizar", false, false, false, null);
            channel.QueueDeclare("geocodificado", false, false, false, null);

            // Evento receptor
            var geolocalizar = new EventingBasicConsumer(channel);
            geolocalizar.Received += (model, ea) =>
            {
                // Obtencion del dato original
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var direccion = JsonSerializer.Deserialize<Direccion>(message);

                // Llamado a OpenStreetMap
                var strAddress = HttpUtility.UrlEncode($"{direccion.Calle} {direccion.Numero},{direccion.Ciudad},{direccion.Provincia}");
                var result = httpClient.GetAsync($"https://nominatim.openstreetmap.org/search?q={strAddress}&format=json").Result;
                var json = result.Content.ReadAsStringAsync().Result;
                var data = JsonSerializer.Deserialize<List<SearchResult>>(json);
                
                // Actualizacion y envio del dato
                direccion.Latitud = float.Parse(data[0].lat);
                direccion.Longitud = float.Parse(data[0].lon);
                direccion.Estado = "TERMINADO";
                var bodyNew = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(direccion));
                channel.BasicPublish("", "geocodificado", null, bodyNew);
            };
            channel.BasicConsume("geolocalizar", true, geolocalizar);

            Console.ReadKey();
        }
    }
}
