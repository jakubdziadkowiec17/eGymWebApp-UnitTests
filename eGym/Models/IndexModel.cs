using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eGym.Models
{
    public class IndexModel
    {
        public IEnumerable<GymModel> Gym { get; set; }
        public IEnumerable<AdModel> Ad { get; set; }
        public IEnumerable<NewsModel> NewsItem { get; set; }
    }
}
