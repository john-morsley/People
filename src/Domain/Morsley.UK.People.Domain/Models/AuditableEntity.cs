namespace Morsley.UK.People.Domain.Models
{
    public abstract class AuditableEntity<T> : Entity<T>
    {
        protected AuditableEntity(T id) : base(id) {}

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }
    }
}