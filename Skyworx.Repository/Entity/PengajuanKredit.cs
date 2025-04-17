using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skyworx.Repository.Entity;
[Table("pengajuan_kredit")]
public class PengajuanKredit
{
    [Key, Column("id")]
    public Guid Id { get; set; }
    [Column("plafon")]
    public decimal Plafon { get; set; }
    [Column("bunga")]
    public decimal Bunga { get; set; }
    [Column("tenor")]
    public int Tenor { get; set; }
    [Column("angsuran")]
    public decimal Angsuran { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}