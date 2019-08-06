using System;
using System.Collections.Generic;
using System.Linq;
using GetDerivedTypes.Derives;

namespace GetDerivedTypes
{
    class Program
    {
        static void Main(string[] args)
        {
            GetDerivedTypes();
        }

        static void GetDerivedTypes()
        {
            var assembly = typeof(IHoge).Assembly;
            var types = assembly.GetExportedTypes();

            Console.WriteLine("  -- Check each type definition status");
            foreach (var type in types)
            {
                var isSubclass = type.IsSubclassOf(typeof(Base<>));
                var isSubclassOfBaseType = type.BaseType?.Name == "Base`1"; // true
                var isImplementationOfInterface = type.GetInterface(typeof(IHoge).FullName); // not null
                var isAssignableFrom = typeof(Base<>).IsAssignableFrom(type); // false になる....

                Console.WriteLine($"{type.FullName}: {isSubclass} ({isSubclassOfBaseType} / {isImplementationOfInterface?.FullName ?? "null"}) & ({isAssignableFrom})");
            }

            Console.WriteLine("  -- Get Derived Class from BaseType");
            var derivedClasses = types.Where(x => !x.IsAbstract && !x.IsInterface)
                .Where(x => x.BaseType != null)
                .Where(x => x.BaseType.IsGenericType)
                .Where(x => x.BaseType.GetGenericTypeDefinition() == typeof(Base<>));
            foreach (var type in derivedClasses)
            {
                var x = Activator.CreateInstance(type); // object
                var returnType = (IHoge)x;
                Console.WriteLine(returnType);
            }
        }
    }

    public interface IHoge { }
    public abstract class Base<T> : IHoge
    {
        public abstract T Create();
    }
}

namespace GetDerivedTypes.Derives
{
    public class Foo : Base<Hoge>
    {
        public override Hoge Create()
        {
            return new Hoge();
        }
    }
    public class Bar : Base<Moge>
    {
        public override Moge Create()
        {
            return new Moge();
        }
    }

    public class Hoge
    {
        public int Id { get; set; }
    }

    public class Moge
    {
        public string Name { get; set; }
    }
}
