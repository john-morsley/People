using Morsley.UK.People.Common.Extensions;

namespace Morsley.UK.People.API.Example.MVC.Models;

public class Person : IValidatableObject
{
    [Required]
    [StringLength(250)]
    public string? FirstName { get; set; }

    [Required]
    [StringLength(250)]
    public string? LastName { get; set; }

    [StringLength(25)]
    public string? Sex { get; set; }

    [StringLength(25)]
    public string? Gender { get; set; }

    [Range(0, 31)]
    public string? Day { get; set; }

    [Range(0, 12)]
    public string? Month { get; set; }

    [Range(0, 9999)]
    public string? Year { get; set; }

    //[EmailAddress]
    //public string? Email { get; set; }

    //[Phone]
    //public string? Mobile { get; set; }

    private int? GetNumber(string? value)
    {
        if (value is null) return null;

        if (int.TryParse(value, out var number)) return number;

        return null;
    }

    private bool IsValidDate()
    {
        if (Day is null && Month is null && Year is null) return true;

        if (Day is null || Month is null || Year is null) return false;
        var day = GetNumber(Day);
        var month = GetNumber(Month);
        var year = GetNumber(Year);

        if (day is null || month is null || year is null) return false;

        var potentialDate = $"{year:000#}-{month:0#}-{day:0#}";
        var validFormat = "yyyy-MM-dd";

        return DateOnly.TryParseExact(potentialDate, validFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }

    public override string ToString()
    {
        var fn = FirstName.GetDisplayValue();
        var ln = LastName.GetDisplayValue();
        var s = Sex.GetDisplayValue();
        var g = Gender.GetDisplayValue();
        var d = Day.GetDisplayValue();
        var m = Month.GetDisplayValue();
        var y = Year.GetDisplayValue();

        var output = $"{nameof(FirstName)}:{fn}|" +
                     $"{nameof(LastName)}:{ln}|" +
                     $"{nameof(Sex)}:{s}|" +
                     $"{nameof(Gender)}:{g}|" +
                     $"{nameof(Day)}:{d}|" +
                     $"{nameof(Month)}:{m}|" +
                     $"{nameof(Year)}:{y}|";

        return output;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!IsValidDate())
        {
            yield return new ValidationResult("That date is not valid!");
        }
    }
}
