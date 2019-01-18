using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace psLibrary_RestLevel3API.Models
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        // do not include the Author so that we don't have circular reference, but include the AuthorId
        public Guid AuthorId { get; set; }

    }
}
