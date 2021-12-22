namespace API.Models.Shared.Tests;

public class DateOfBirthTests
{
    [Test]
    [Category("Happy")]
    [TestCase(2000, 01, 01, 2000, 01, 01, 0)]
    [TestCase(2021, 12, 31, 2022, 12, 31, 1)]
    [TestCase(1967, 11, 11, 2021, 11, 09, 53)]
    [TestCase(1967, 11, 11, 2021, 11, 10, 53)]
    [TestCase(1967, 11, 11, 2021, 11, 11, 54)]
    [TestCase(1967, 11, 11, 2021, 11, 12, 54)]
    [TestCase(2021, 01, 01, 2021, 12, 31, 0)]
    [TestCase(2021, 01, 01, 2022, 01, 01, 1)]
    [TestCase(2024, 2, 29, 2028, 2, 29, 4)] // Leap year
    [TestCase(1, 01, 01, 9999, 12, 31, 9998)]
    [TestCase(9999, 12, 31, 9999, 12, 31, 0)]
    public void Given_Valid_Parameters___When_Instantiated___Then_Should_Be_Correct(int year, int month, int day, int todayYear, int todayMonth, int todayDay, int age)
    {
        // Act...
        var sut = new DateOfBirth(year, month, day);

        // Assert...
        sut.Year.Should().Be(year);
        sut.Month.Should().Be(month);
        sut.Day.Should().Be(day);
        var today = new DateOnly(todayYear, todayMonth, todayDay);
        sut.Age(today).Should().Be(age);       
    }

    [Test]
    [Category("Unhappy")]
    [TestCase(0, 01, 01)] // Year too low
    [TestCase(10000, 01, 01)] // Year too high
    [TestCase(2000, 0, 01)] // Month too low
    [TestCase(2000, 13, 01)] // Month too high
    [TestCase(2000, 01, 0)] // Day too low
    [TestCase(2000, 01, 32)] // Day too high
    [TestCase(2021, 2, 29)] // Not a Leap year
    [TestCase(2021, 2, 30)] // Not 30 days in February
    [TestCase(2021, 4, 31)] // Not 31 days in April
    public void Given_Invalid_Parameters___When_Trying_To_Instantiate___Then_An_Error_Should_Be_Thrown(int year, int month, int day)
    {
        // Act...
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new DateOfBirth(year, month, day));

        // Assert...
        //sut.Year.Should().Be(year);
        //sut.Month.Should().Be(month);
        //sut.Day.Should().Be(day);
    }
}
