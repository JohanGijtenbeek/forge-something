namespace Erp.Domain.Articles;

public static class ArticleType
{
    public const string RawMaterial = "raw_material";
    public const string Manufactured = "manufactured";
    public const string BoughtOut    = "bought_out";
    public const string Service      = "service";

    public static bool IsValid(string value) =>
        value is RawMaterial or Manufactured or BoughtOut or Service;
}
