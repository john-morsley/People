namespace Users.Domain.Enumerations
{
    public enum Gender
    {
        PreferNotToSay = 0,
        Cisgender = 1, // A person whose gender identity and biological sex assigned at birth are the same. For example they were born biologically as a male, and express their gender as male. 
        Transgender = 2, // A person who lives as a member of a gender other than that expected based on sex assigned at birth. 
        GenderFluid = 3, // A person who identifies as genderfluid has a gender identity and presentation that shifts between, or shifts outside of, society’s expectations of gender.
        NonBinary = 4, // A person who identifies as nonbinary does not experience gender within the gender binary. People who are nonbinary may also experience overlap with different gender expressions, such as being gender non-conforming.
        Agender = 5, // Not having a gender or identifying with a gender. They may describe themselves as being gender neutral or genderless. 
        Bigender = 6, // A person who identifies as bigender has two genders. People who are bigender often display cultural masculine and feminine roles.
    }
}