﻿namespace Morsley.UK.People.Application.Events;

public class PersonUpdatedEvent : Event
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Sex? Sex { get; set; }

    public Gender? Gender { get; set; }

    public string? DateOfBirth { get; set; }
}