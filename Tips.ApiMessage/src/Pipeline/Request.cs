namespace Tips.Pipeline
{
    public class Request<TItem> : Request
    {
        public TItem Item { get; set; }
    }

    public class Request
    {
    }
}
