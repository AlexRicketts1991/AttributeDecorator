using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AttributeDecorator
{
    public static class ServiceDecorator
    {
        public static void AddDecoratedService<T>(this IServiceCollection services, ServiceLifetime serviceLifetime)
        {
            var attributes = typeof(T).GetCustomAttributes();

            var attributeDecorators = GetAttributeDecorators(attributes);

            foreach (var attributeDecorator in attributeDecorators)
                PipelineBuilder.BuildAttributeDecoratorPipeline(services, attributeDecorators, attributeDecorator, serviceLifetime);
        }

        private static Dictionary<Type, Type> GetAttributeDecorators(IEnumerable<Attribute> attributes)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(s => s.GetTypes())
                            .ToList();

            var attributeDecorators = new Dictionary<Type, Type>();
            foreach (var attribute in attributes)
                foreach (var type in types)
                    if (attribute.GetType().Name == type.Name + "Attribute")
                    {
                        attributeDecorators.Add(attribute.GetType(), type);
                        break;
                    }

            return attributeDecorators;
        }
    }
}
