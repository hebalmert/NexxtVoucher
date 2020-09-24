
namespace NexxtVoucher.Models.API
{
    public class UserResponse
    {
        public int UserId { get; set; }

        public int CompanyId { get; set; }

        public string Compania { get; set; }

        public string Logo { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Photo { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsUser { get; set; }

        public bool IsCobros { get; set; }

        public bool IsCobrosMulti { get; set; }

    }
}