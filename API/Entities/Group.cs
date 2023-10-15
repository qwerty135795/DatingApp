using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Group
    {
        public Group()
        {
        }

        [Key]
        public string Name { get; set; }

        public Group(string name)
        {
            Name = name;
        }

        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
}