namespace Morsley.UK.People.Tests.Common;

public static class GenderExtensions
{
    public static Gender? GenerateDifferentGender(this Gender? initialGender)
    {
        var af = new Fixture();
        Gender? different;
        do
        {
            different = af.Create<Gender?>();
        }
        while (different == initialGender);

        return different;
    }
}
