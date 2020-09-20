namespace Tips.ApiMessage.Contracts
{
    public class Request
    {
    }

    public class Request<TItem>
    {
        public TItem Item { get; set; }
    }
}
