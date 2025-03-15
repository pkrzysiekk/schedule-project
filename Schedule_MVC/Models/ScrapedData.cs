using System.ComponentModel.DataAnnotations;

namespace Schedule_MVC.Models;

public class ScrapedData
{
    [Key]
    public int ID { get; set; }
    public string Name { get; set; }
    public string CourseFullName { get; set; }
    public string CourseType { get; set; }
    public string IsLead { get; set; }
}
