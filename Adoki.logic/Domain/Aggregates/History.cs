using Adoki.logic.Domain.Entities;
using Adoki.logic.Domain.ValueObjects;
using Adoki.logic.Domain.Enums;

namespace Adoki.logic.Domain.Aggregates;

public class History
{
    public List<Read> Items { get; init; }

    public int Count => Items.Count;
    public (int Days, Kilometer Km) FinalStretch => Items.Count > 1 ?
                            Items.Last() - Items[^2] :
                                throw new InvalidOperationException("Insuficiente cantidad de lecturas.");
    public (int Days, Kilometer Km) LastOperationDifference => Items.Count > 1 ?
                             Items.Last() - Items.Last(rd => rd.Operation != Maintenance.NONE) :
                                throw new StackOverflowException("No hay mas de una lectura.");

    public History() => Items = [];
    public History(List<Read> reads) => Items = reads;

    public void Append(Read read)
    {
        bool fechaExistente = Items.Any(r => r.Date == read.Date);

        if (fechaExistente)
            throw new ExisteReadException("Lectura ya añadida anteriormente, solo actualizable.");

        int index = Items.FindLastIndex(rd => rd.Date < read.Date);
        if (index == Items.Count - 1 ) Items.Add(read);
        Items.Insert(index-1, read);
    }

    public (int Days, Kilometer Km) DifferenceSince(DateTime date)
    {
        if (Items.Count <= 1)
            throw new InvalidOperationException("Insuficiente cantidad de lecturas.");

        Read lastRead = Items.Last();
        Read? ReadInData = Items.FirstOrDefault(rd => rd.Date == date);

        if (ReadInData != null)
            return lastRead - ReadInData;

        Read? readAtDate = Items.FirstOrDefault(rd => rd.Date > date);
        if (readAtDate != null)
            return lastRead - readAtDate;
        throw new NullReferenceException();
    }

    public Read? FindLastReadWithOperation(Maintenance operation) =>
             Items.FindLast(rd => rd.Operation.HasFlag(operation));

    public List<Read> FindReadWithOperation(Maintenance operation, int maxSize = 2)
    {
        List<Read> listRead = Items.FindAll(rd => rd.Operation.HasFlag(operation));
        if (listRead.Count == 0)
            return listRead;

        return listRead.Count >= maxSize ? listRead[^maxSize..^1] : listRead[^listRead.Count..^1];
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
    int lastRead;
    if (Items.Count >= 1)
    {
        lastRead = Items.Count - 1;
        Items[lastRead].Operation |= operation;
    }
}
public void UpdateRead(Kilometer kilometer)
{
    int lastRead;
    if (Items.Count >= 1)
    {
        lastRead = Items.Count() - 1;
        Items[lastRead].Odometer += kilometer;
    }
}
public void UpdateRead(Maintenance operation, Kilometer kilometer)
{
    UpdateRead(kilometer);
    UpdateRead(operation);
}
*/