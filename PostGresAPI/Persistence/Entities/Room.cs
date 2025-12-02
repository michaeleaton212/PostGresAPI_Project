namespace PostGresAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class Room
{
    protected Room() { }
    protected Room(string name) { Name = name; }

    public int Id { get; private set; }
    public string Name { get; private set; } = "";

    public string? ImagePath { get; private set; }// image path

    public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();

    internal void SetName(string name) => Name = name;

    
    public void SetImagePath(string? path)// setter image path
    {
        ImagePath = path;
    }
}
