using System.ComponentModel.DataAnnotations;

namespace eGym.Models
{
    public class LogModel
    {
        public DateTime LogDate { get; set; }
        public string UserId { get; set; }
        public string Operation { get; set; }

        private readonly string file = "Database_logs.txt";

        public LogModel(string Operation1, string currentUserId)
        {
            LogDate = DateTime.Now;
            UserId = currentUserId;
            Operation=Operation1;
            string logEntry = $"{LogDate} - User Id: {UserId} - {Operation}";
            File.AppendAllText(file, logEntry + Environment.NewLine);
        }
    }
}
