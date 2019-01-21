using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace psLibrary_RestLevel3API.Models
{
    public class BookForCreationDto
    {
        [Required(ErrorMessage = "You should fill out title.")]
        [MaxLength(100, ErrorMessage = "The title shouldn't have more than 100 character.")]
        public string Title { get; set; }

        [MaxLength(500, ErrorMessage = "This description should be less than 500 characters.")]
        public string Description { get; set; }
    }
}
