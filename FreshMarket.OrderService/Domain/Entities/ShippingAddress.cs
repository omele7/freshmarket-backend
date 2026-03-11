namespace FreshMarket.OrderService.Domain.Entities;

/// <summary>
/// Objeto de valor que representa una dirección de envío
/// </summary>
public class ShippingAddress
{
    /// <summary>
    /// Identificador único (si se usa como tabla separada)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Calle y número
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Ciudad
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Estado o provincia
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Código postal
    /// </summary>
    public string ZipCode { get; set; } = string.Empty;

    /// <summary>
    /// País
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Constructor por defecto
    /// </summary>
    public ShippingAddress()
    {
    }

    /// <summary>
    /// Constructor con todos los parámetros
    /// </summary>
    public ShippingAddress(string street, string city, string state, string zipCode, string country)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state ?? throw new ArgumentNullException(nameof(state));
        ZipCode = zipCode ?? throw new ArgumentNullException(nameof(zipCode));
        Country = country ?? throw new ArgumentNullException(nameof(country));
    }

    /// <summary>
    /// Retorna la dirección completa como string
    /// </summary>
    public string GetFullAddress()
    {
        return $"{Street}, {City}, {State} {ZipCode}, {Country}";
    }
}

