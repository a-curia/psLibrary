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

            return new JsonResult(authors);
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthor(Guid id)
        {
            var authorFromRepo = _libraryRepository.GetAuthor(id);

            var author = Mapper.Map<AuthorDto>(authorFromRepo);

            return new JsonResult(author);
        }


    }
}
