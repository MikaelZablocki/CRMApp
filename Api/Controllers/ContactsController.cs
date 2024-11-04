using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly Repository _repository;

        public ContactsController(Repository repository)
        {
            _repository = repository;
        }

 

        // Optional: You might want to have a method to get all contacts
        [HttpGet]
        public IActionResult GetContacts()
        {
            var contacts = _repository.GetAllContacts();
            return Ok(contacts);
        }

    
    }
}
