using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleProducer
{
	class Program
	{
		static void Main(string[] args)
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			{
				List<string> queues = new List<string>();
				queues.Add("Order");
				queues.Add("Product");

				queues.ForEach(queue =>
				{
					var channel = CreateChannel(connection);

					BuildPublisher(channel, queue, queue);
				});

				Console.ReadLine();
			}
		}

		private static IModel CreateChannel(IConnection connection)
		{
			return connection.CreateModel();
		}
		private static void BuildPublisher(IModel channel, string queue, string producerName)
		{
			Task.Run(() =>
			{
				channel.QueueDeclare(queue: queue,
									 durable: false,
									 exclusive: false,
									 autoDelete: false,
									 arguments: null);

				int count = 0;
				while (true)
				{
					string message = $"{queue}: {count++}";
					var body = Encoding.UTF8.GetBytes(message);

					channel.BasicPublish(exchange: "",
								routingKey: queue,
								basicProperties: null,
								body: body);

					Console.WriteLine($" [x] Sent {message}");
					System.Threading.Thread.Sleep(2000);
				}
			});
		}
	}
}
