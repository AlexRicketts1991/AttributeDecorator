namespace Example.Services.Decorators
{
    public class AdditionDecorator : IDummyService
    {
        private readonly IDummyService _decoratedService;

        public AdditionDecorator(IDummyService decoratedService)
        {
            _decoratedService = decoratedService;
        }
        public int Get()
        {
            return _decoratedService.Get() + 10;
        }
    }
}
