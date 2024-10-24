namespace Api
{
    public class Meeting
    {
       public int MeetingsId { get; set; }
         public DateTime MeetingDate { get; set; }
        public string MeetingName { get; set; }
        public string MeetingSpot { get; set; }        
        public string MeetingDescription { get; set; }
        public string MeetingSubject { get; set; }

        public int UserId { get; set; }
        public int ContactId { get; set; }

    }
}
