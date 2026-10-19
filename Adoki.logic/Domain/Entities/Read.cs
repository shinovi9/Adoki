namespace Adoki.logic.Domain.Entities;
using Adoki.logic.Domain.ValueObjects;
using Adoki.logic.Domain.Enums;


public class Read
{
    public Read()
    {
        this.Date = DateTime.Now.Date;
        this.Odometer = new(0);
        this.Operation = Maintenance.NONE;
    }
    public Read(DateTime Date, Kilometer Km, Maintenance Op = Maintenance.NONE)
    {
        this.Date = Date.Date;
        this.Odometer = Km;
        this.Operation = Op;
    }

    public DateTime Date { get; set; }
    public Kilometer Odometer { get; set; }
    public Maintenance Operation { get; set; }

    public static (int Days, Kilometer Kilometer) operator -(Read a, Read b)
    {
        int diferenciaDias = (a.Date - b.Date).Days;
        Kilometer diferenciaKm = a.Odometer - b.Odometer;
        return (diferenciaDias, diferenciaKm);
    }
    public override string ToString() => $"Date {
        DateOnly.FromDateTime(Date)}\nOdometer {
            Odometer}\nOperations {Operation}";
}