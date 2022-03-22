namespace Morsley.UK.People.API.Contracts.Shared
{
    public abstract class Resource<T> where T : class
    {
        public T? Data { get; private set; }

        public IList<Link>? Links { get; private set; }

        //public IList<Resource<T>> Embedded { get; private set; }

        public virtual void AddData(T data)
        {
            Data = data;
        }

        public virtual void AddLinks(IList<Link> links)
        {
            Links = links;
        }

        //public virtual void AddEmbedded(Resource<T> embedded)
        //{
        //    if (embedded == null) return;
        //    if (Embedded == null) Embedded = new List<Resource<T>>();
        //    Embedded.Add(embedded);
        //}

        //public virtual void AddEmbedded(IList<Resource<T>> embedded)
        //{
        //    Embedded = embedded;
        //}
    }
}