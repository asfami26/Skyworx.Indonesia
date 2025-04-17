namespace Skyworx.Common.Command;

public class CalculateAngsuranCommand : IKreditRequest
{
    public decimal Plafon { get; set; }
    public decimal Bunga { get; set; }
    public int Tenor { get; set; }
}