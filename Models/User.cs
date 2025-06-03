public record User
{
    public int Id { get; set; }
    public required string Name { get; init;}
    public int Age { get; set; }
}