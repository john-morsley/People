
namespace Users.Domain.Models
{
    public abstract class Entity<T> : IEntity<T>
    {
        //protected Entity() {}

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
                if (object.Equals(value, default(T))) return; //throw new DomainModelIdException("The Id cannot be set to its default value!");
                _id = value;
            }
        }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        protected bool Equals(Entity<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Id, other.Id) && Created.Equals(other.Created);
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
                return (EqualityComparer<T>.Default.GetHashCode(Id) * 397) ^ Created.GetHashCode();
            }
        }
    }
}