using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models;
public class Answer{
    
    [Key]
    public int ID { get; set; }
    public string Text { get; set; }=null!;
    [JsonIgnore]
    public Question? Question { get; set; }        
}