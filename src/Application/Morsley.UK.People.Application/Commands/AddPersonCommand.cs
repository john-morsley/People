﻿namespace Morsley.UK.People.Application.Commands;

public sealed class AddPersonCommand : IRequest<Person>
{
    public Guid? Id { get; set; }

    public string? FirstName { get; set;  }

    public string? LastName { get; set; }

    public Sex? Sex { get; set; }

    public Gender? Gender { get; set; }

    public string? DateOfBirth { get; set; }

    public override string ToString()
    {
        return $"Id:{Id.GetDisplayValue()}|" +
               $"FirstName:{FirstName.GetDisplayValue()}|" +
               $"LastName:{LastName.GetDisplayValue()}|" +
               $"Sex:{Sex.GetDisplayValue()}|" +
               $"Gender:{Gender.GetDisplayValue()}|" +
               $"DateOfBirth:{DateOfBirth.GetDisplayValue()}";
    }
}