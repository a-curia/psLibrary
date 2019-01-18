using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using psLibrary_RestLevel3API.Helpers;
using psLibrary_RestLevel3API.Models;
using psLibrary_RestLevel3API.Services;
using AutoMapper;

namespace psLibrary_RestLevel3API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        public IActionResult GetAuthors()
        {

            //throw  new Exception("Random exception for testing purpose!");

            var authorsFromRepo = _libraryRepository.GetAuthors();
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
        public IActionResult GetAuthor(Guid id)
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


    }
}
