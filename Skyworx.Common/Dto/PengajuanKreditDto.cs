namespace Skyworx.Common.Dto;

public class PengajuanKreditDto
{
    public Guid Id { get; set; }
    public decimal Plafon { get; set; }
    public decimal Bunga { get; set; }
    public int Tenor { get; set; }
    public decimal Angsuran { get; set; }
}