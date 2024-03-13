using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models;
public class QuizUser
{
    public int QuizzesID { get; set; }
    
    [JsonIgnore]
    public Quiz? Quiz { get; set; }

    public int UsersID { get; set; }

    [JsonIgnore]
    public Users? User { get; set; }
}