namespace Api
{
    public class Meeting
    {
       public int MeetingId { get; set; }
         public DateTime MeetingTime { get; set; }
        public string MeetingName { get; set; }              
        public string MeetingDescription { get; set; }
       

        public int UserId { get; set; }
        public int ContactId { get; set; }

    }
}
