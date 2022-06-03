using Morsley.UK.People.API.Contracts.Responses;

namespace Morsley.UK.People.Test.Fixture.AutoFixture;

public class PersonEquivalencyStep : DifferentObjectsEquivalencyStep<PersonResponse, Person> {}

public class DifferentObjectsEquivalencyStep<T1, T2> : IEquivalencyStep
{
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context, IEquivalencyValidator nestedValidator)
    {
        throw new NotImplementedException();
    }
}