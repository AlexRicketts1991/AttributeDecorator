# AttributeDecorator

This package allows you to define decorators as attributes on handler classes in order to wire up your decorated implementations of an interface.

Instead of manually declaring how each service is ordered

        serviceProvider.AddTransient<IDummyService>(x => 
            new RetryDecorator(
                new AdditionDecorator(
                    new DoubleResultDecorator(
                        new DummyService())), x.GetService<AppConfiguration>()));
Use attributes on the handler to declare the order in which the services are executed.

[RetryDecorator]
[AdditionDecorator]
[DoubleResultDecorator]
public class DummyService : IDummyService
and add in the ConfigureService method

serviceProvider.AddDecoratedService<DummyService>(ServiceLifetime.Transient);

To get started create a decorated service in the normal way then create an attribute with same name as the decorator with "Attribute" appended to the end.

public class RetryDecoratorAttribute : Attribute
{
}

public class RetryDecorator : IDummyService
Then add the attribute to the handler class

[RetryDecorator]
public class DummyService : IDummyService
and add the handler to the service provider

        serviceProvider.AddDecoratedService<DummyService>(ServiceLifetime.Transient);
See the example project or feel free to message me for more details.

https://dev.azure.com/alexricketts10/_git/AttributeDecorator

Inspired by Vladimir Khorikov course CQRS in Practice.
