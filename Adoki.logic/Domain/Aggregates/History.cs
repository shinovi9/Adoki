using Adoki.logic.Domain.Entities;
using Adoki.logic.Domain.ValueObjects;
using Adoki.logic.Domain.Enums;

namespace Adoki.logic.Domain.Aggregates;

public class History
{
    public List<Read> Items { get; init; }
    public (int Days, Kilometer Km) FinalStretch => Items.Count > 1 ?
                            Items.Last() - Items[^2] :
                                throw new InvalidOperationException("Insuficiente cantidad de lecturas.");
    //No comprendo del todo el propósito de LastOperationDifference?
    public (int Days, Kilometer Km) LastOperationDifference => Items.Count > 1 ?
                             Items.Last() - Items.Last(rd => rd.Operation != Maintenance.NONE) :
                                throw new InvalidOperationException("Insuficiente cantidad de lecturas.");

    public History() => Items = [];

    public History(List<Read> Readings) => Items = Readings;

    public Read? this[DateTime date] {
        get => Items.Find(rd => rd.Date >= date);
    }

    public void Append(Read read)
    {
        var fechaExistente = Items.Any(r => r.Date == read.Date);

        if (!fechaExistente)
            Items.Add(read);
        else
            throw new InvalidOperationException("Sólo se acepta una lectura por día.");
    }

    public (int Days, Kilometer Km) DifferenceSince(DateTime date)
    {
        if (Items.Count < 1)
            throw new InvalidOperationException("No hay suficientes lecturas, para esta operación.");

        var last = Items.Last();
        //Read? read = Items.FirstOrDefault(rd => rd.Date == date);

        //if (read != null)
        //    return last - read;

        var readAtDate = Items.Where(rd => rd.Date >= date).OrderBy(rd => rd.Date).FirstOrDefault();
        if (readAtDate != null)
            return last - readAtDate;
        throw new NullReferenceException();
    }

    public Read? FindLastReadWithOperation(Maintenance operation) =>
             Items.FindLast(rd => rd.Operation.HasFlag(operation));

    public History FilterByOperation(Maintenance operation, int maxSize = 2)
    {
        var listRead = Items.FindAll(rd => rd.Operation.HasFlag(operation));
        if (listRead.Count <= maxSize) //Si la lista tiene igual o menos cantidad
            return new History(listRead);

        //return listRead.Count > maxSize ? listRead[^maxSize..^1] : listRead[^listRead.Count..^1];
        //Si el código llegó hasta este punto es porque la cantidad excede el límite, entonces:
        return new History(listRead[^maxSize..]);
    }
}

public class ExisteReadException : Exception
{
    // Constructor por defecto (hereda del padre)
    public ExisteReadException() : base()
    {
    }
    // Constructor con mensaje personalizado
    public ExisteReadException(string message) : base(message)
    {
    }
    // Constructor con mensaje y pila de llamadas (stack trace)
    public ExisteReadException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}


/*
    public History(List<Read> Items)
    {
        this.Items = Items.Count > 0
        ? (List<Read>)(
            from item in Items
            orderby item.Date
            select item
            ) : Items;
    }*/
/*
public void UpdateRead(Maintenance operation)
{
    int last;
    if (Items.Count >= 1)
    {
        last = Items.Count - 1;
        Items[last].Operation |= operation;
    }
}
public void UpdateRead(Kilometer kilometer)
{
    int last;
    if (Items.Count >= 1)
    {
        last = Items.Count() - 1;
        Items[last].Odometer += kilometer;
    }
}
public void UpdateRead(Maintenance operation, Kilometer kilometer)
{
    UpdateRead(kilometer);
    UpdateRead(operation);
}
*/