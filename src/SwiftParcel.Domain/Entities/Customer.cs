using System.ComponentModel.DataAnnotations.Schema;

namespace SwiftParcel.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    [Column(TypeName = "citext")]
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public Address? Address { get; set; }
    
    public DateTime RegisteredDate { get; set; }
    public bool Vip { get; set; }
    public string? Notes { get; set; }

    public ICollection<Parcel> Parcels { get; set; } = new List<Parcel>();
    public ICollection<Case> Cases { get; set; } = new List<Case>();
}