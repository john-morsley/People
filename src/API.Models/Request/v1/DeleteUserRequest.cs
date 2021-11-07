using System;

namespace Users.API.Models.Request.v1
{
    public class DeleteUserRequest
    {
        public DeleteUserRequest(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}