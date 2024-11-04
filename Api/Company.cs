namespace Api
{
    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; } 
        public string Industry { get; set; }
        public User User { get; set; }
    }
}
