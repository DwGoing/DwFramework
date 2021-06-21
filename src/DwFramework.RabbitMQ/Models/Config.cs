namespace DwFramework.RabbitMQ
{
    public sealed class Config
    {
        public string Host { get; init; } = "localhost";
        public int Port { get; init; } = 5672;
        public string UserName { get; init; }
        public string Password { get; init; }
        public string VirtualHost { get; init; } = "/";
    }
}