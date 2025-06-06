using System.Reflection;

namespace dobo.core.Extensions;

public static class AssemblyExtensions
{
    public static Type[] GetImplementationTypes<T>(this Assembly assembly)
    {
        return new[] {assembly}.GetImplementationTypes<T>();
    }

    public static Type[] GetImplementationTypes<T>(this Assembly[] assemblies)
    {
        var fullname = typeof(T).FullName;
        if (fullname.IsNullOrEmpty())
        {
            return [];
        }

        var handlers = assemblies.SelectMany(assembly => assembly.GetExportedTypes())
            .Where(type => type.IsClass && type is {IsAbstract: false, IsGenericType: false, IsNested: false} &&
                           IsDerivedFromAbstractBase(type, fullname))
            .ToArray();

        return handlers;
    }

    private static bool IsDerivedFromAbstractBase(Type type, string baseFullName)
    {
        return type.FullName == baseFullName || type.GetInterfaces().Any(t => t.FullName == baseFullName);
    }
}