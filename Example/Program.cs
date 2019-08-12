using Example.Services;
using Example.Services.Decorators;
using Microsoft.Extensions.DependencyInjection;
using AttributeDecorator;
using System;
using System.Collections.Generic;

namespace Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = ConfigureServices();

            var service = serviceProvider.GetService<IDummyService>();

            Console.WriteLine($" Value returned from service {service.Get()}");

            Console.ReadKey();
        }

        private static ServiceProvider ConfigureServices()
        {
            var serviceProvider = new ServiceCollection();

            // Config settings
            serviceProvider.AddSingleton(_ => new AppConfiguration(100));

            // Instead of
            //serviceProvider.AddTransient<IDummyService>(x => 
            //    new RetryDecorator(
            //        new AdditionDecorator(
            //            new DoubleResultDecorator(
            //                new DummyService())), x.GetService<AppConfiguration>()));

            // Use default to scan for decorators with naming convention
            serviceProvider.AddDecoratedService<DummyService>(ServiceLifetime.Transient);

            return serviceProvider.BuildServiceProvider();
        }
    }
}