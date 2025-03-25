namespace App.Models;

public class Course
{
    public string CourseShortName { get; set; }
    public string CourseFullName { get; set; }
    public string ScheduleType { get; set; }
    public string Type { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is Course other && obj is not null)
        {
            return other.CourseShortName == CourseShortName
                && other.CourseFullName == CourseFullName
                && other.Type == Type
                && other.ScheduleType == ScheduleType;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CourseShortName, CourseFullName, Type, ScheduleType);
    }
}