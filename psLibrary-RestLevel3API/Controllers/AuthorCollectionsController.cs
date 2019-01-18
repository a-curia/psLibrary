using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using psLibrary_RestLevel3API.Models;
using psLibrary_RestLevel3API.Services;

namespace psLibrary_RestLevel3API.Controllers
{
    [Route("api/authorcollections")]
    public class AuthorCollectionsController: Controller
    {
        private readonly ILibraryRepository _libraryRepository;

        public AuthorCollectionsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpPost]
        public IActionResult CreateAuthorCollection([FromBody] IEnumerable<AuthorBooksForCreationDto> authorCollection)
        {
            if (authorCollection == null)
            {
                return BadRequest();
            }

            var authorEntities = AutoMapper.Mapper.Map<IEnumerable<Entities.Author>>(authorCollection);

            foreach (var author in authorEntities)
            {
                _libraryRepository.AddAuthor(author);
            }

            if (!_libraryRepository.Save())
            {
                throw new Exception("Creating an author collection failed on save.");
            }

            return Ok(); // what to display
        }




    }
}
