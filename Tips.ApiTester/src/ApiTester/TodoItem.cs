namespace ApiTester
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public bool IsComplete { get; set; }
    }
}
