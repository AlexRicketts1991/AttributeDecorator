using Example.Services.Decorators;
using System;

namespace Example.Services
{
    [RetryDecorator]
    [AdditionDecorator]
    [DoubleResultDecorator]
    public class DummyService : IDummyService
    {
        public int Get()
        {
            if (ConnectionSucceeded())
                return 10;

            throw new ConnectionException();
        }

        /// <summary>
        ///     Simulates a poor connection.
        /// </summary>
        /// <returns></returns>
        private static bool ConnectionSucceeded()
        {
            var random = new Random();

            return random.Next(100) < 40;
        }
    }
}
