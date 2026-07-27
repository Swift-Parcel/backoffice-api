namespace SwiftParcel.Domain.Entities;

public class CaseParcel
{
    public int CaseId  { get; set; }
    public int ParcelId  { get; set; }
    
    public Case? Case { get; set; }
    public Parcel? Parcel { get; set; }
}