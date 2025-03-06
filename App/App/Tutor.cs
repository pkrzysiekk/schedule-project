using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    public class Tutor
    {
        public string Name {  get; set; }
        public Course Course { get; set; }

        public bool IsLead { get; set; }

        public static bool operator ==(Tutor a, Tutor b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            if (a.Name == b.Name && a.Course == b.Course && a.IsLead == b.IsLead && a.Course.type == b.Course.type)
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(Tutor a, Tutor b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is Tutor other)
            {
                return other.Name == Name && other.Course == Course && other.IsLead == IsLead && other.Course.type==Course.type;
            }
            return false;
        }
    }
}
