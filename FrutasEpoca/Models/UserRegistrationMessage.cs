namespace FrutasEpoca.Models
{
    public class UserRegistrationMessage
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public DateTime RegistrationDateTime { get; set; }
    }
}
