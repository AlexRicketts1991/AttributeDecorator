namespace Example.Services.Decorators
{
    public class AppConfiguration
    {
        public AppConfiguration(int numberOfRetries)
        {
            NumberOfRetries = numberOfRetries;
        }

        public int NumberOfRetries { get; }
    }
}