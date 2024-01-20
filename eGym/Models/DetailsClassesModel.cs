using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eGym.Models
{
    public class DetailsClassesModel
    {
        public ClassesModel Classes { get; set; }
        public IEnumerable<ClassesUserModel> ClassesUser { get; set; }
    }

    public class MyClassesClientModel
    {
        public IEnumerable<ClassesModel> Classes { get; set; }
        public IEnumerable<ClassesUserModel> ClassesUser { get; set; }
    }
}
