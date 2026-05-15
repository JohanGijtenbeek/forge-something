namespace Erp.Domain.Quotes;

public static class QuoteStatus
{
    public const string Draft    = "draft";
    public const string Sent     = "sent";
    public const string Accepted = "accepted";
    public const string Rejected = "rejected";

    public static bool IsValid(string status) =>
        status is Draft or Sent or Accepted or Rejected;

    public static bool IsTerminal(string status) =>
        status is Accepted or Rejected;

    public static bool CanTransitionTo(string from, string to) => (from, to) switch
    {
        (Draft,    Sent)     => true,
        (Draft,    Rejected) => true,
        (Sent,     Accepted) => true,
        (Sent,     Rejected) => true,
        _ => false
    };
}
