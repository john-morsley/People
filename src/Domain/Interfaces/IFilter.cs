﻿namespace Users.Domain.Interfaces;

public interface IFilter
{
    string Key { get; }

    string? Value { get; }
}
