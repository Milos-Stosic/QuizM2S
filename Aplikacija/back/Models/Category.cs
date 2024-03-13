using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Models;
public class Category{

    [Key]
    public int ID { get; set; }
    public string Name { get; set; }=null!;
    [JsonIgnore]
    public List<Quiz>? Quizzes { get; set; }
}