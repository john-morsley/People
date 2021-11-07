using System;
using MediatR;

namespace Users.Application.Queries
{
    public sealed class GetUserQuery : IRequest<Users.Domain.Models.User>
    {
        public Guid Id { get; set; }
    }
}