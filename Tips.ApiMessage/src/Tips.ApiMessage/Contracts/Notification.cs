namespace Tips.ApiMessage.Contracts
{
    public class Notification
    {
        public string Id { get; set; }
        public Severity Severity { get; set; }
        public string Detail { get; set; }
    }
}
