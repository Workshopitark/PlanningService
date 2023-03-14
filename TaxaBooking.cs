namespace PlanningService;

public class TaxaBooking
{
    public string? Kundenavn { get; set; }
    public DateTime Starttidspunkt { get; set; }
    public string? Startsted { get; set; }
    public string? Endested { get; set; }

    public TaxaBooking(string? kundenavn, DateTime starttidspunkt, string? startsted, string? endested)
    {
        this.Kundenavn = kundenavn;
        this.Starttidspunkt = starttidspunkt;
        this.Startsted = startsted;
        this.Endested = endested;
    }

    public TaxaBooking()
    {

    }
}