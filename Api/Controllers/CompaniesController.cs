using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly Repository _repository;

        
        public CompaniesController()
        {
            _repository = new Repository();
        }
        // In your CompaniesController

        [HttpGet("WithCompany")]
        public IActionResult GetCompaniesWithContacts()
        {
            var companiesWithContacts = _repository.GetCompaniesWithContacts();
            return Ok(companiesWithContacts);
        }



        // GET: api/notes/{id}
        [HttpGet("{companyid}")]
        public IActionResult GetCompanyById(int companyid)
        {
            var company = _repository.GetCompanyById(companyid);
            if (company == null)
            {
                return NotFound("Note not found");
            }
            return Ok(company);
        }


        // POST: api/Companies
        [HttpPost]
        public IActionResult AddCompany([FromBody] Company company)
        {
            // Assuming you have a method to get the current user's ID
            var userId = GetCurrentUserId(); // This method should return the logged-in user's ID

            if (company == null || string.IsNullOrEmpty(company.CompanyName))
            {
                return BadRequest("Invalid company data.");
            }

            // Set the UserId to the logged-in user's ID
            company.UserId = userId;

            _repository.AddCompany(company);
            return CreatedAtAction(nameof(GetCompanyById), new { companyid = company.CompanyId }, company);
        }

        // Dummy method to simulate getting current user's ID
        private int GetCurrentUserId()
        {
            // Logic to retrieve the currently logged-in user's ID
            return 1; // Replace this with actual logic
        }


        [HttpDelete("{companyId}")]
        public IActionResult DeleteCompany(int companyId)
        {
            try
            {
                _repository.DeleteCompany(companyId); // Call the repository method to delete the company and its contacts
                return NoContent(); // Return 204 No Content if successful
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Handle any exceptions and return an error response
            }
        }


    }
}
