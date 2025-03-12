namespace App.Models
{
    public class Course
    {
        public string courseShortName { get; set; }
        public string courseFullName { get; set; }
        public string type { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is Course other && obj is not null)
            {
                return other.courseShortName == courseShortName && other.courseFullName == courseFullName && other.type == type;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(courseShortName, courseFullName, type);
        }
    }
}