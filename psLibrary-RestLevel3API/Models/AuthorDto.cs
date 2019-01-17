using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace psLibrary_RestLevel3API.Models
{
    public class AuthorDto // used for returning data to consumer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Genre { get; set; }

    }
}
