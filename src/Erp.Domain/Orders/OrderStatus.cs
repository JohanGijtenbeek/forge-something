namespace Erp.Domain.Orders;

public static class OrderStatus
{
    public const string Draft = "draft";
    public const string Released = "released";
    public const string InProgress = "inprogress";
    public const string Done = "done";
    public const string Cancelled = "cancelled";

    private static readonly HashSet<string> Terminal = [Done, Cancelled];

    public static bool IsTerminal(string status) => Terminal.Contains(status);

    public static bool CanTransitionTo(string from, string to) => (from, to) switch
    {
        (Draft, Released)     => true,
        (Draft, Cancelled)    => true,
        (Released, InProgress) => true,
        (Released, Cancelled)  => true,
        (InProgress, Done)    => true,
        (InProgress, Cancelled) => true,
        _ => false
    };

    public static bool IsValid(string status) =>
        status is Draft or Released or InProgress or Done or Cancelled;
}
