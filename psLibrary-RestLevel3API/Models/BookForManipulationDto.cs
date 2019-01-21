using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace psLibrary_RestLevel3API.Models
{

    // try to DRY for BookForCreationDto and BookForUpdateDto
    public abstract class BookForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out title.")]
        [MaxLength(100, ErrorMessage = "The title shouldn't have more than 100 character.")]
        public string Title { get; set; }

        [MaxLength(500, ErrorMessage = "This description should be less than 500 characters.")]
        public virtual string Description { get; set; } // we have an implementation in base class but we want to allow overwriting
    }
}
