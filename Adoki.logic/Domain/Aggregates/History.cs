using Adoki.logic.Domain.Entities;
using Adoki.logic.Domain.ValueObjects;
using Adoki.logic.Domain.Enums;

namespace Adoki.logic.Domain.Aggregates;

public class History
{
    public List<Read> Items { get; init; }

    public int Count => Items.Count;
    public (int Days, Kilometer Km) LastDifference => Items.Count > 1 ?
                            Items.Last() - Items[^2] :
                                throw new StackOverflowException("No Hay mas de una lectura.");
    public (int Days, Kilometer Km) LastOperationDifference => Items.Count > 1 ?
                             Items.Last() - Items.Last(rd => rd.Operation != Maintenance.NONE) :
                                throw new StackOverflowException("No hay mas de una lectura.");

    public History() => Items = [];

    public void Append(Read read)
    {
        bool fechaExistente = Items.Any(r => r.Date == read.Date);

        if (!fechaExistente)
            Items.Add(read);
        else
            throw new ExisteReadException("Lectura ya añadida anteriormente, solo actualizable.");
    }

    public (int Days, Kilometer Km) DifferenceSince(DateTime date)
    {
        if (Items.Count > 1)
            throw new StackOverflowException("No Hay mas de una lectura.");

        Read last = Items.Last();
        Read? read = Items.FirstOrDefault(rd => rd.Date == date);

        if (read != null)
            return last - read;

        Read? readAtDate = Items.Where(rd => rd.Date > date).OrderBy(rd => rd.Date).FirstOrDefault();
        if (readAtDate != null)
            return last - readAtDate;
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