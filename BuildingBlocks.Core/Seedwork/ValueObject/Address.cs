namespace BuildingBlocks.Core.Seedwork.ValueObject;

public sealed class Address
{
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
 
    private Address() { }
 
    public Address(
        string street,
        string city,
        string state,
        string postalCode,
        string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }
 
    public override string ToString() =>
        $"{Street}, {City}, {State} {PostalCode}, {Country}";
}