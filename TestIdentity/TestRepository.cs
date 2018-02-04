namespace TestIdentity
{
    public interface ITestRepository
    {
        string Get();
    }

    public class TestRepository : ITestRepository
    {
        public string Get()
        {
            return "Hello World!";
        }
    }
}