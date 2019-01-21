using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using psLibrary_RestLevel3API.Helpers;
using psLibrary_RestLevel3API.Models;
using psLibrary_RestLevel3API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace psLibrary_RestLevel3API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;

        //const int maxAuthorPageSize = 20;

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        //public IActionResult GetAuthors([FromQuery(Name = "page")] int pageNumber = 1, 
        //    [FromQuery(Name = "size")] int pageSize = 5)
        public IActionResult GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            //pageSize = (pageSize > maxAuthorPageSize) ? maxAuthorPageSize : pageSize;

            //throw  new Exception("Random exception for testing purpose!");

            var authorsFromRepo = _libraryRepository.GetAuthors(authorsResourceParameters);
            //var authors = new List<AuthorDto>();

            //foreach (var author in authorsFromRepo)
            //{
            //    authors.Add(new AuthorDto()
            //    {
            //        Id = author.Id,
            //        Name = $"{author.FirstName} {author.LastName}",
            //        Genre = author.Genre,
            //        Age = author.DateOfBirth.GetCurrentAge()
            //    });
            //}
            var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);

            return Ok(authors);
        }

        [HttpGet("{id}", Name = "GetAuthorRoute")]
        public IActionResult GetAuthor([FromRoute] Guid id) // [FromRoute] is not necesary in here
        {
            //if (!_libraryRepository.AuthorExists(id)) // this makes another call check IO which is not good
            //{
            //    return NotFound();
            //}

            var authorFromRepo = _libraryRepository.GetAuthor(id);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            var author = Mapper.Map<AuthorDto>(authorFromRepo);

            return Ok(author);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorForCreationDto author)
        {
            if (author == null)
            {
                return BadRequest();
            }

            var authorEntity = Mapper.Map<Entities.Author>(author);

            _libraryRepository.AddAuthor(authorEntity);
            if (!_libraryRepository.Save())
            {
                throw new Exception("creating an author failed on save.");
            }

            var authorToReturn = Mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthorRoute", new {id = authorToReturn.Id}, authorToReturn);

        }

        [HttpPost("onego")] // create author and it's books in one step
        public IActionResult CreateAuthorWithBooks([FromBody] AuthorBooksForCreationDto author)
        {
            if (author == null)
            {
                return BadRequest();
            }

            var authorEntity = Mapper.Map<Entities.Author>(author);

            _libraryRepository.AddAuthor(authorEntity);
            if (!_libraryRepository.Save())
            {
                throw new Exception("creating an author failed on save.");
            }

            var authorToReturn = Mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthorRoute", new { id = authorToReturn.Id }, authorToReturn);

        }

        [HttpPost("{id}")] // we use for check not for real POST - this is not allowed
        public IActionResult BlockAuthorCreation(Guid id)
        {
            if (_libraryRepository.AuthorExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(Guid id)
        {
            var authorFromRepo = _libraryRepository.GetAuthor(id);
            if (authorFromRepo == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteAuthor(authorFromRepo);

            if (_libraryRepository.Save())
            {
                throw new Exception($"Deleting author {id} failed on save!");
            }

            return NoContent();
        }

    }
}
