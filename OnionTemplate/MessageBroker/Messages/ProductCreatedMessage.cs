namespace OnionTemplate.MessageBroker.Messages
{
    public class ProductCreatedMessage : BasicMessage
    {
        public string Brand { get; set; }
        public string Name { get; set; }

        public double Price { get; set; }
    }
}
