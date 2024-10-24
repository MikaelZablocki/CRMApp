using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly Repository _repository;

        public ContactsController()
        {
            _repository = new Repository();
        }

        // Helper method to check if contact exists
        private IActionResult HandleNotFound(int contactId)
        {
            var contact = _repository.GetContactById(contactId);
            if (contact == null)
            {
                return NotFound($"Contact with ID {contactId} not found.");
            }
            return Ok(contact);
        }

        // Helper method to validate Contact object in POST requests
        private IActionResult ValidateContact(Contact contact)
        {
            if (contact == null || string.IsNullOrEmpty(contact.ContactName))
            {
                return BadRequest("Invalid contact data.");
            }
            return null;
        }

        [HttpGet]
        public IActionResult GetContacts()
        {
            var contacts = _repository.GetAllContacts();
            return Ok(contacts);
        }

        [HttpGet("{contactid}")]
        public IActionResult GetContactById(int contactid)
        {
            return HandleNotFound(contactid);
        }

        [HttpPost]
        public IActionResult AddContact([FromBody] Contact contact)
        {
            var validationResult = ValidateContact(contact);
            if (validationResult != null)
            {
                return validationResult;
            }

            _repository.AddContact(contact);
            return CreatedAtAction(nameof(GetContactById), new { contactid = contact.ContactId }, contact);
        }

        [HttpDelete("{contactid}")]
        public IActionResult DeleteContact(int contactid)
        {
            var contactExists = HandleNotFound(contactid);
            if (contactExists is NotFoundObjectResult)
            {
                return contactExists;
            }

            _repository.DeleteContact(contactid);
            return NoContent();
        }



    }


}
