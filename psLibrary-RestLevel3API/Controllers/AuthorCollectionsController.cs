using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using psLibrary_RestLevel3API.Helpers;
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

        // array keys and composite keys
        // array keys : 1,2,3
        // composite keys: key1=neme, key2=sdgsd
        
        // (key1, key2)
        [HttpGet("({ids})", Name = "GetAuthorCollectionRoute")]
        public IActionResult GetAuthorCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var authorEntities = _libraryRepository.GetAuthors(ids);
            if (ids.Count() != authorEntities.Count())
            {
                return NotFound();
            }

            var authorsToReturn = AutoMapper.Mapper.Map<IEnumerable<AuthorDto>>(authorEntities);

            return Ok(authorsToReturn);
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

            //return Ok(); // what to display

            var authorCollectionToReturn = AutoMapper.Mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            var idsAsString = string.Join(",", authorCollectionToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetAuthorCollectionRoute", new { ids = idsAsString }, authorCollectionToReturn);
        }




    }
}
