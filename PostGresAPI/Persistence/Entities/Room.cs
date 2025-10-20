namespace PostGresAPI.Models;

public abstract class Room
{
    protected Room() { }                      
    protected Room(string name) { Name = name; }

    public int Id { get; private set; }
    public string Name { get; private set; } = "";
 public string Type { get; private set; } = ""; 


    public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();

    internal void SetName(string name) => Name = name; // internal makes only visible in the same Project
}
