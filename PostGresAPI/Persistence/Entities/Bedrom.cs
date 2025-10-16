namespace PostGresAPI.Models;

public sealed class Bedroom : Room
{
    private Bedroom() { }

    public Bedroom(string name, int numberOfBeds) : base(name)
    {
        NumberOfBeds = numberOfBeds; 
    }

    public int NumberOfBeds { get; internal set; }
}
