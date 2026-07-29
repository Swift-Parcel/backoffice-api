using System.ComponentModel.DataAnnotations.Schema;

namespace SwiftParcel.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string Email { get; set; } = string.Empty;
    public string Phone  { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime RegisteredDate { get; set; }
    public bool Vip { get; set; }
    public string Notes { get; set; } = string.Empty;
    public ICollection<Parcel> Parcels = new List<Parcel>();
    public ICollection<Case> Cases = new List<Case>();
}