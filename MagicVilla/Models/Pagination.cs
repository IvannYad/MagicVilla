using Microsoft.AspNetCore.Identity;

namespace MagicVilla.Models
{
    public class Pagination
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
