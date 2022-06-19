# **Sesion 7:**

## **Ejercicio 1**

- Agregar un proyecto de consola llamado AlmacenRabbitMQ e incluir el paquete Nuget cliente de RabbitMQ.

## **Ejercicio 2**

Desarrollar un servicio RabbitMQEmailProducer basado en una interface IEmailProducer.

- La interface expondrá un método  SendEmail que recibirá una clase DTOEmail. Desarrollar esta clase con las propiedades EmailAddress, Subject y Body, todas ellas de texto.

- Desarrollar el servicio configurando una conexión, un canal, un Exchange y una cola para enviar los emails al Consumer RabbitMQ.

- Incluir este servicio en el archivo de configuración para poder inyectarlo en otras clases.

## **Ejercicio 3**

- Crear un controller RabbitMQController con una petición post. Esta petición conectará con el servicio RabbitMQEmailProducer y le enviará los datos del email a enviar.

## **Ejercicio 4**

- Desarrollar en el proyecto de consola un Consumer que se suscriba a la cola de enviar correos y los envíe.

