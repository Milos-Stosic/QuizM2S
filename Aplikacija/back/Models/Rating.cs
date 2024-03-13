using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Models;

public class Rating{
    [Key]
    public int ID { get; set; }
    public int Rate { get; set; }
    [JsonIgnore]
    public Quiz? Quiz { get; set; }
}