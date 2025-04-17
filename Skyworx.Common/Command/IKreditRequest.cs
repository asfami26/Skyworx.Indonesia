namespace Skyworx.Common.Command;

public interface IKreditRequest
{
    decimal Plafon { get; set; }
    decimal Bunga { get; set; }
    int Tenor { get; set; }
}