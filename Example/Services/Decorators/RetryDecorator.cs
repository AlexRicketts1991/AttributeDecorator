namespace Example.Services.Decorators
{
    public class RetryDecorator : IDummyService
    {
        private readonly IDummyService _decoratedService;
        private readonly AppConfiguration _appConfiguration;

        public RetryDecorator(
            IDummyService decoratedService,
            AppConfiguration appConfiguration)
        {
            _decoratedService = decoratedService;
            _appConfiguration = appConfiguration;
        }

        public int Get()
        {
            for (int i = 0; ; i++)
            {
                try
                {
                    return _decoratedService.Get();
                }
                catch (ConnectionException)
                {
                    if (i >= _appConfiguration.NumberOfRetries)
                        throw;
                }
            }
        }
    }
}
