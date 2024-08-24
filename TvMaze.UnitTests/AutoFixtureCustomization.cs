using AutoFixture.Kernel;

namespace TvMaze.UnitTests
{
    public class AutoFixtureCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                 .ToList()
                 .ForEach(b =>
                 {
                     fixture.Behaviors.Remove(b);
                 });

            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            fixture.Customizations.Add(new DateOnlySpecimenBuilder());
        }


        public class DateOnlySpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (request is Type type && type == typeof(DateOnly))
                {
                    return  new DateOnly(2000, 1, 1);
                }

                return new NoSpecimen();
            }
        }
    }
}
