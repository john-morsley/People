using Morsley.UK.People.Domain.Exceptions;
using Morsley.UK.People.Domain.Interfaces;

namespace Morsley.UK.People.Domain.Models
{
    public abstract class Entity<T> : IEntity<T>
    {
        public Entity(T id)
        {
            if (object.Equals(id, default(T))) throw new DomainModelIdException("The Id cannot be set to its default value!");
            _id = id;
        }

        protected T _id;

        public T Id
        {
            get { return _id; }
            private set {
                if (object.Equals(value, default(T))) return;
                _id = value;
            }
        }

        protected bool Equals(Entity<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Id, other.Id);
        }

        public bool Equals(T other)
        {
            // ToDo
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Entity<T>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(Id) * 397);
            }
        }
    }
}
