namespace App.Models;

public class Course
{
    public string courseShortName { get; set; }
    public string courseFullName { get; set; }
    public bool isFullTime { get; set; }
    public string type { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is Course other && obj is not null)
        {
            return other.courseShortName == courseShortName
                && other.courseFullName == courseFullName
                && other.type == type
                && other.isFullTime == isFullTime;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(courseShortName, courseFullName, type, isFullTime);
    }
}