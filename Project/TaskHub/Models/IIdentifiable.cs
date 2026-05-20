namespace TaskHub.Models;

// Interface used by the generic repository so it can work with any item that has an Id.
public interface IIdentifiable
{
    int Id { get; set; }
}
