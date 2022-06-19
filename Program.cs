using RabbitMQ.Client;
using System.Text;

IConnection connection;
IModel channel;

// Creamos la conexión
ConnectionFactory factory = new ConnectionFactory();
factory.HostName = "localhost";
factory.VirtualHost = "/"; // RabbitMQ permite, desde el panel, crear host virtuales. Un host virtual sería como un servidor propio e independiente donde administrar mensajes. Esto sirve para tener más de un servidor RabbitMQ en la misma máquina. Por defecto, crea uno (/)
factory.Port = 5672;
factory.UserName = "guest";
factory.Password = "guest";

connection = factory.CreateConnection();

// Creamos el canal
channel = connection.CreateModel();

//-------------------------------Fanout---------------------------------------
// Declaramos el Exchange
channel.ExchangeDeclare("ex.fanout", "fanout", true, false, null);

// Declaramos las Queue
channel.QueueDeclare("my.queue1", true, false, false, null);
channel.QueueDeclare("my.queue2", true, false, false, null);

// Vinculamos el Exchange con las Queue
channel.QueueBind("my.queue1", "ex.fanout", "");
channel.QueueBind("my.queue2", "ex.fanout", "");

// Publicamos
channel.BasicPublish("ex.fanout", "", null, Encoding.UTF8.GetBytes("Mensaje 1"));
channel.BasicPublish("ex.fanout", "", null, Encoding.UTF8.GetBytes("Mensaje 2"));

//------------------------Direct----------------------------
// Declaramos un Exchange
channel.ExchangeDeclare("ex.direct", "direct", true, false, null);

// Declaramos las Queue
channel.QueueDeclare("my.infos", true, false, false, null);
channel.QueueDeclare("my.warnings", true, false, false, null);
channel.QueueDeclare("my.errors", true, false, false, null);

// Vinculamos el Exchange con las Queue
channel.QueueBind("my.infos", "ex.direct", "info");
channel.QueueBind("my.warnings", "ex.direct", "warning");
channel.QueueBind("my.errors", "ex.direct", "error");

// Publicamos
channel.BasicPublish("ex.direct", "info", null, Encoding.UTF8.GetBytes("Mensaje con routing key info."));
channel.BasicPublish("ex.direct", "warning", null, Encoding.UTF8.GetBytes("Mensaje con routing key warning."));
channel.BasicPublish("ex.direct", "error", null, Encoding.UTF8.GetBytes("Mensaje con routing key error."));

//------------------------Topic----------------------------
// Declaramos un Exchange
channel.ExchangeDeclare("ex.topic", "topic", true, false, null);

// Declaramos las Queue
channel.QueueDeclare("my.queue3", true, false, false, null);
channel.QueueDeclare("my.queue4", true, false, false, null);
channel.QueueDeclare("my.queue5", true, false, false, null);

// Vinculamos el Exchange con las Queue
channel.QueueBind("my.queue3", "ex.topic", "*.image.*");
channel.QueueBind("my.queue4", "ex.topic", "#.image");
channel.QueueBind("my.queue5", "ex.topic", "image.#");

// Publicamos
channel.BasicPublish("ex.topic", "convert.image.bmp", null, Encoding.UTF8.GetBytes("Routing key es convert.image.bmp"));
channel.BasicPublish("ex.topic", "convert.bitmap.image", null, Encoding.UTF8.GetBytes("Routing key es convert.bitmap.image"));
channel.BasicPublish("ex.topic", "image.bitmap.32bit", null, Encoding.UTF8.GetBytes("Routing key es image.bitmap.32bit"));

//------------------------Headers-------------------------
// Declaramos un Exchange
channel.ExchangeDeclare("ex.headers", "headers", true, false, null);

// Declaramos las Queue
channel.QueueDeclare("my.queue6", true, false, false, null);
channel.QueueDeclare("my.queue7", true, false, false, null);

// Vinculamos el Exchange con las Queue añadiendo las cabeceras
channel.QueueBind("my.queue6", "ex.headers", "",
    new Dictionary<string, object>()
                {
                    {"x-match","all" },
                    {"job","convert" },
                    {"format","jpeg" }
                });

channel.QueueBind("my.queue7", "ex.headers", "",
    new Dictionary<string, object>()
    {
                    {"x-match","any" },
                    {"job","convert" },
                    {"format","jpeg" }
    });

// Configuramos las propiedades del mensaje con la cabecera y publicamos
IBasicProperties props = channel.CreateBasicProperties();
props.Headers = new Dictionary<string, object>();
props.Headers.Add("job", "convert");
props.Headers.Add("format", "jpeg");

channel.BasicPublish("ex.headers", "", props, Encoding.UTF8.GetBytes("Mensaje con header job/convert y format/jpeg"));

props = channel.CreateBasicProperties();
props.Headers = new Dictionary<string, object>();
props.Headers.Add("job", "convert");
props.Headers.Add("format", "bmp");

channel.BasicPublish("ex.headers", "", props, Encoding.UTF8.GetBytes("Mensaje con header job/convert y format/bmp"));

Console.WriteLine("Mensajes enviados!!!");
Console.ReadKey();

// Eliminamos el Exchange y las Queue
channel.QueueDelete("my.queue1");
channel.QueueDelete("my.queue2");
channel.QueueDelete("my.infos");
channel.QueueDelete("my.warnings");
channel.QueueDelete("my.errors");
channel.QueueDelete("my.queue3");
channel.QueueDelete("my.queue4");
channel.QueueDelete("my.queue5");
channel.QueueDelete("my.queue6");
channel.QueueDelete("my.queue7");
channel.ExchangeDelete("ex.fanout");
channel.ExchangeDelete("ex.direct");
channel.ExchangeDelete("ex.topic");
channel.ExchangeDelete("ex.headers");

channel.Close();
connection.Close();