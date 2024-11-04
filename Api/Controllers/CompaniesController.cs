using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly Repository _repository;

        public CompaniesController(Repository repository)
        {
            _repository = repository;
        }

        // New method to get all contacts for a specific company
        [HttpGet("{companyId}/contacts")]
        public IActionResult GetContactsByCompany(int companyId)
        {
            var contacts = _repository.GetContactsByCompany(companyId);
            if (contacts == null || contacts.Count == 0)
            {
                return NotFound("No contacts found for this company.");
            }
            return Ok(contacts);
        }

        [HttpGet("{companyid}")]
        public IActionResult GetCompanyById(int companyid)
        {
            var company = _repository.GetCompanyById(companyid);
            if (company == null)
            {
                return NotFound("Company not found");
            }
            return Ok(company);
        }

        // POST: api/companies
        [HttpPost]
        public IActionResult AddCompany([FromBody] Company company)
        {
            // Validate the company object
            if (company == null || string.IsNullOrEmpty(company.CompanyName))
            {
                return BadRequest("Invalid company data.");
            }

            // Add the company using the repository
            _repository.AddCompany(company);

            // Return a response with the created company
            return CreatedAtAction(nameof(GetCompanyById), new { companyId = company.Id }, company);
        }

        private User GetCurrentUser()
        {
            // Logic to retrieve the currently logged-in user as a User object
            return new User { UserId = 1, Username = "testUser" }; // Replace with actual logic
        }

        [HttpDelete("{companyId}")]
        public IActionResult DeleteCompany(int companyId)
        {
            try
            {
                _repository.DeleteCompany(companyId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
