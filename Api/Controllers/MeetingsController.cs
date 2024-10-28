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

        // Helper method to check if Meetings exists
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


        // Fetch meetings for a specific date
        [HttpGet("{userId}/date")] // Use a clearer route for specific date requests
        public IActionResult GetMeetings(int userId, [FromQuery] string meetingDate) // Get date from query
        {
            // Parse meetingDate from DD-MM-YYYY to DateTime
            if (!DateTime.TryParseExact(meetingDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                return BadRequest("Invalid date format. Please use DD-MM-YYYY.");
            }

            // Retrieve meetings for the parsed date from the repository
            var meetings = _repository.GetMeetingsByDate(userId, parsedDate);

            return Ok(meetings);
        }

        // Fetch all meetings for a user
        [HttpGet("{userId}/All")] // This route remains the same
        public IActionResult GetAllMeetings(int userId)
        {
            var meetings = _repository.GetAllMeetingsByUserId(userId);
            return Ok(meetings);
        }

        // Add a new meeting
        [HttpPost]
        public IActionResult AddMeeting([FromBody] Meeting meeting)
        {
            if (meeting == null || string.IsNullOrEmpty(meeting.MeetingName) || meeting.MeetingTime == default)
            {
                return BadRequest("Invalid meeting data.");
            }
          
            _repository.AddMeeting(meeting);
            return CreatedAtAction(nameof(GetMeetings), new { userId = meeting.UserId, meetingDate = meeting.MeetingTime.Date }, meeting);
        }
    }

}

