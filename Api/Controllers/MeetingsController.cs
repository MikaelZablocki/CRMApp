using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly Repository _repository;
        public MeetingsController()
        {
            _repository = new Repository();
        }

        // Helper method to check if contact exists
        private IActionResult HandleNotFound(int meetingId)
        {
            var meeting = _repository.GetMeetingById(meetingId);
            if (meeting == null)
            {
                return NotFound($"Meeting with ID {meetingId} not found.");
            }
            return Ok(meeting);
        }

        [HttpGet("{meetingid}")]
        public IActionResult GetMeetingById(int meetingid)
        {
            return HandleNotFound(meetingid);
        }


        // Helper method to validate Contact object in POST requests
        private IActionResult ValidateMeeting(Meeting meeting)
        {
            if (meeting == null || string.IsNullOrEmpty(meeting.MeetingName))
            {
                return BadRequest("Invalid contact data.");
            }
            return null;
        }

        [HttpPost]
        public IActionResult AddMeeting([FromBody] Meeting meeting)
        {
            var validationResult = ValidateMeeting(meeting);
            if (validationResult != null)
            {
                return validationResult;
            }

            _repository.AddMeeting(meeting);
            return CreatedAtAction(nameof(GetMeetingById), new { meetingid = meeting.MeetingId }, meeting);
        }

    }
}
