using System.Collections.Generic;

namespace Users.API.Models
{
    public class PagedList<T> : List<T>, Users.Domain.Interfaces.IPagedList<T>
    {
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public bool HasPrevious => CurrentPage > 1;

        public bool HasNext => CurrentPage < TotalPages;
    }
}