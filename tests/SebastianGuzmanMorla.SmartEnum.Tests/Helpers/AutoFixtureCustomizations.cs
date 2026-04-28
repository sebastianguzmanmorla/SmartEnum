using AutoFixture.Kernel;

namespace SebastianGuzmanMorla.SmartEnum.Tests.Helpers;

public class SmartEnumCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        // Customize AutoFixture to work with SmartEnum
        fixture.Customizations.Add(new SmartEnumSpecimenBuilder());
    }
}

public class SmartEnumSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(SmartEnum<,>))
        {
            // For SmartEnum types, return a random instance
            var enumType = type;
            var keysProperty = enumType.GetProperty("Keys", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            if (keysProperty != null)
            {
                var keys = (IEnumerable<string>)keysProperty.GetValue(null)!;
                var randomKey = keys.OrderBy(x => Guid.NewGuid()).First();

                var parseMethod = enumType.GetMethod("Parse", new[] { typeof(string) });
                if (parseMethod != null)
                {
                    return parseMethod.Invoke(null, new object[] { randomKey })!;
                }
            }
        }

        return new NoSpecimen();
    }
}