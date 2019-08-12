namespace Example.Services.Decorators
{
    public class DoubleResultDecorator : IDummyService
    {
        private readonly IDummyService _decoratedService;

        public DoubleResultDecorator(IDummyService dummyService)
        {
            _decoratedService = dummyService;
        }

        public int Get()
        {
            return _decoratedService.Get() * 2;
        }
    }
}
