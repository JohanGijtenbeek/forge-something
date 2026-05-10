using Erp.Seeder.Generators;
using Erp.Seeder.Models;
using Erp.Seeder.Writers;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var profile = args.FirstOrDefault() ?? "low";
var connectionString = config.GetConnectionString(profile)
                       ?? config.GetConnectionString("low")!;
var meilisearchUrl = config["Meilisearch:Url"] ?? "http://localhost:7700";
var meilisearchKey = config["Meilisearch:ApiKey"];
var apiUrl = config["Api:Url"] ?? "http://localhost:5272";
var seed = config.GetValue<int>("Seeder:Seed", 42);

var orgCount = config.GetValue<int>($"Seeder:Profiles:{profile}:Organizations", 50);
var personCount = config.GetValue<int>($"Seeder:Profiles:{profile}:Persons", 50);
var articleCount = config.GetValue<int>($"Seeder:Profiles:{profile}:Articles", 10);

Console.WriteLine($"""
                   ════════════════════════════════════════
                     ERP Seeder
                     Profiel:       {profile}
                     Organisaties:  {orgCount}
                     Personen:      {personCount}
                     Artikelen:     {articleCount}
                     Seed:          {seed}
                     API:           {apiUrl}
                   ════════════════════════════════════════
                   """);

var generator = new PartyGenerator(seed);
var articleGenerator = new ArticleGenerator(seed);
var dbWriter = new DatabaseWriter(connectionString);
var msWriter = new MeilisearchWriter(meilisearchUrl, meilisearchKey);
var apiWriter = new ApiWriter(apiUrl, generator);

var stopwatch = System.Diagnostics.Stopwatch.StartNew();

// Genereer data
Console.WriteLine("Genereren...");

var organizations = new List<Erp.Seeder.Models.GeneratedParty>();
for (var i = 0; i < orgCount; i++)
{
    organizations.Add(generator.GenerateOrganization());
    Console.Write($"\r  {i + 1}/{orgCount} organisaties gegenereerd...");
}
Console.WriteLine();

var persons = new List<Erp.Seeder.Models.GeneratedParty>();
for (var i = 0; i < personCount; i++)
{
    persons.Add(generator.GeneratePerson());
    Console.Write($"\r  {i + 1}/{personCount} personen gegenereerd...");
}
Console.WriteLine();

Console.WriteLine($"  ✓ {organizations.Count + persons.Count} parties gegenereerd in {stopwatch.ElapsedMilliseconds}ms");

var articles = articleGenerator.Generate(articleCount);

Console.WriteLine($"  ✓ {organizations.Count + persons.Count} parties + {articles.Count} artikelen gegenereerd in {stopwatch.ElapsedMilliseconds}ms");

// Leegmaken
Console.WriteLine("\nLeegmaken...");
await dbWriter.ClearAsync();
await msWriter.ClearAsync();

// Schrijven parties via API-pipeline (publishes events → EventConsumer → Meilisearch)
Console.WriteLine("\nAPI...");
var (parties, relationships, errors) = await apiWriter.WriteAsync(organizations, persons);

// Schrijven artikelen direct naar database + Meilisearch
Console.WriteLine("\nArtikelen...");
await dbWriter.WriteArticlesAsync(articles);
var categoryNames = ArticleGenerator.Categories.ToDictionary(c => c.Id, c => c.Name);
await msWriter.WriteArticlesAsync(articles, categoryNames);

stopwatch.Stop();

var customers = organizations.Count(o => o.CustomerRole != null);
var suppliers = organizations.Count(o => o.SupplierRole != null);
var both = organizations.Count(o => o.CustomerRole != null && o.SupplierRole != null);

Console.WriteLine($"""

                   ════════════════════════════════════════
                     Klaar in {stopwatch.ElapsedMilliseconds}ms
                     Totaal parties:   {parties}
                     Klanten:          {customers}
                     Leveranciers:     {suppliers}
                     Beide:            {both}
                     Relaties:         {relationships}
                     Artikelen:        {articles.Count}
                     Fouten:           {errors}
                   ════════════════════════════════════════
                   """);
