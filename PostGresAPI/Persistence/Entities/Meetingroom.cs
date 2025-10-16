namespace PostGresAPI.Models;

public sealed class Meetingroom : Room // no other class can inherit
{
    private Meetingroom() { }

    public Meetingroom(string name, int numberOfChairs) : base(name)
    {
        NumberOfChairs = numberOfChairs; 
    }

    public int NumberOfChairs { get; internal set; }
}
