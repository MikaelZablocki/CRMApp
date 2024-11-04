using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly Repository _repository;

        public MeetingsController(Repository repository)
        {
            _repository = repository;
        }

        private IActionResult HandleNotFound(int meetingId)
        {
            var meeting = _repository.GetMeetingById(meetingId);
            if (meeting == null)
            {
                return NotFound($"Meeting with ID {meetingId} not found.");
            }
            return Ok(meeting);
        }

        // GET: api/meetings/{meetingId}
        [HttpGet("{meetingId}")]
        public IActionResult GetMeetingById(int meetingId)
        {
            var meeting = _repository.GetMeetingById(meetingId); // Implement this method in the repository
            if (meeting == null)
            {
                return NotFound($"Meeting with ID {meetingId} not found.");
            }
            return Ok(meeting);
        }
        // DELETE: api/meetings/{meetingId}
        [HttpDelete("{meetingId}")]
        public IActionResult DeleteMeeting(int meetingId)
        {
            try
            {
                _repository.DeleteMeeting(meetingId);
                return NoContent(); // Return 204 No Content if successful
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Handle any exceptions and return an error response
            }
        }


        [HttpGet("user/{userId}/date/{dateString}")]
        public IActionResult GetMeetingsByDate(int userId, string dateString)
        {
            try
            {
                var meetings = _repository.GetMeetingsByDate(userId, dateString);
                if (meetings == null || meetings.Count == 0)
                {
                    return NotFound("No meetings found for the specified date.");
                }
                return Ok(meetings);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if the date format is invalid
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Handle other exceptions
            }
        }


        // GET: api/meetings/user/{userId}
        [HttpGet("user/{userId}")]
        public IActionResult GetAllMeetings(int userId)
        {
            var meetings = _repository.GetAllMeetingsByUserId(userId);
            return Ok(meetings);
        }

        // POST: api/meetings
        [HttpPost]
        public IActionResult AddMeeting([FromBody] Meeting meeting)
        {
            if (meeting == null || string.IsNullOrEmpty(meeting.MeetingName) || meeting.MeetingTime == default)
            {
                return BadRequest("Invalid meeting data.");
            }

            _repository.AddMeeting(meeting);
            return CreatedAtAction(nameof(GetMeetingById), new { meetingId = meeting.MeetingId }, meeting);
        }

        private User GetCurrentUser()
        {
            // Logic to retrieve the currently logged-in user as a User object
            return new User { UserId = 1, Username = "testUser" }; // Replace with actual logic
        }
    }
}
