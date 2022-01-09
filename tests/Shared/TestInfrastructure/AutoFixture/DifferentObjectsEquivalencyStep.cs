namespace TestInfrastructure.AutoFixture;

public class UserEquivalencyStep : DifferentObjectsEquivalencyStep<Users.API.Models.Response.v1.UserResponse, Users.Domain.Models.User>
{

}

public class DifferentObjectsEquivalencyStep<T1, T2> : IEquivalencyStep
{
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context, IEquivalencyValidator nestedValidator)
    {
        throw new NotImplementedException();
    }
}
