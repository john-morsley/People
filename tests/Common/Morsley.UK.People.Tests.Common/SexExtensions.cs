namespace Morsley.UK.People.Tests.Common;

public static class SexExtensions
{
    public static Sex? GenerateDifferentSex(this Sex? sex)
    {
        var af = new Fixture();
        Sex? differentSex;
        do
        {
            differentSex = af.Create<Sex?>();
        }
        while (differentSex == sex);

        return differentSex;
    }
}