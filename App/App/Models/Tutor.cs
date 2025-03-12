namespace App.Models
{
    public class Tutor
    {
        public string Name { get; set; }
        public Course Course { get; set; }

        public bool IsLead { get; set; }

        public static bool operator ==(Tutor a, Tutor b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;

            return a.Equals(b);
        }

        public static bool operator !=(Tutor a, Tutor b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Tutor other && obj is not null)
            {
                return other.Name == Name && other.Course.Equals(Course) && other.IsLead == IsLead;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Course, IsLead);
        }
    }
}