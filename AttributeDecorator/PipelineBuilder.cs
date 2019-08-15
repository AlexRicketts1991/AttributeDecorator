using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AttributeDecorator
{
    internal class PipelineBuilder
    {
        public static void BuildAttributeDecoratorPipeline(
            IServiceCollection services,
            IDictionary<Type, Type> attributeDecorators,
            KeyValuePair<Type, Type> attributeDecorator)
        {
            // Get all interfaces for the decorator
            var interfaceTypes = attributeDecorator.Value.GetInterfaces();

            // Get all types that implement the interface
            IEnumerable<Type> implementedTypes = GetImplementedTypes(interfaceTypes);

            foreach (var type in implementedTypes)
                BuildPipelineForImplementedType(services, attributeDecorators, type);
        }

        private static IEnumerable<Type> GetImplementedTypes(Type[] interfaceTypes)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                                     .SelectMany(s => s.GetTypes());

            // Get all types that implement the interfaces
            return interfaceTypes
                    .SelectMany(interfaceType =>
                        types
                        .Where(type => interfaceType.IsAssignableFrom(type))
                        .ToList());
        }

        private static void BuildPipelineForImplementedType(
            IServiceCollection services, 
            IDictionary<Type, Type> attributeDecorators, 
            Type type)
        {
            // Filter where type is using attribute that has decorated service.
            var attributes = type.GetCustomAttributes(false)
                                    .Select(attr => attr.GetType())
                                    .Intersect(attributeDecorators.Keys);

            foreach (var attribute in attributes)
                BuildPipeline(services, type, attributeDecorators, attributes);
        }

        private static void BuildPipeline(
            IServiceCollection services,
            Type type,
            IDictionary<Type, Type> attributeDecorators,
            IEnumerable<Type> attributes)
        {
            List<Type> pipeline = attributes
                                    .Select(x => ToDecorator(x, attributeDecorators))
                                    .Concat(new[] { type })
                                    .Reverse()
                                    .ToList();

            Type interfaceType = type.GetInterfaces().Single();

            Func<IServiceProvider, object> factory = BuildPipeline(pipeline, interfaceType);

            services.AddScoped(interfaceType, factory);
        }
                
        private static Func<IServiceProvider, object> BuildPipeline(List<Type> pipeline, Type interfaceType)
        {
            List<ConstructorInfo> ctors = pipeline
                .Select(x =>
                {
                    Type type = x.IsGenericType ? x.MakeGenericType(interfaceType.GenericTypeArguments) : x;
                    return type.GetConstructors().Single();
                })
                .ToList();

            Func<IServiceProvider, object> func = provider =>
            {
                object current = null;

                foreach (ConstructorInfo ctor in ctors)
                {
                    List<ParameterInfo> parameterInfos = ctor.GetParameters().ToList();

                    object[] parameters = GetParameters(parameterInfos, current, provider, interfaceType);

                    current = ctor.Invoke(parameters);
                }

                return current;
            };

            return func;
        }

        private static object[] GetParameters(List<ParameterInfo> parameterInfos, object current, IServiceProvider provider, Type interfaceType)
        {
            var result = new object[parameterInfos.Count];

            for (int i = 0; i < parameterInfos.Count; i++)
            {
                result[i] = GetParameter(parameterInfos[i], current, provider, interfaceType);
            }

            return result;
        }

        private static object GetParameter(ParameterInfo parameterInfo, object current, IServiceProvider provider, Type interfaceType)
        {
            Type parameterType = parameterInfo.ParameterType;

            if (parameterType == interfaceType)
                return current;

            object service = provider.GetService(parameterType);
            if (service != null)
                return service;

            throw new ArgumentException($"Type {parameterType} not found");
        }

        private static Type ToDecorator(Type attributeType, IDictionary<Type, Type> attributeDecorators)
        {
            foreach (var attributeDecorator in attributeDecorators)
                if (attributeDecorator.Key == attributeType)
                    return attributeDecorator.Value;

            throw new ArgumentException(attributeType.ToString());
        }
    }
}