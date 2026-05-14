using Bogus;
using Erp.Seeder.Models;

namespace Erp.Seeder.Generators;

public class ArticleGenerator
{
    private readonly Faker _faker;
    private readonly HashSet<string> _usedCodes = new();

    // Reference data seeded with fixed GUIDs so they're stable across runs
    public static readonly List<ArticleCategorySeedRow> Categories =
    [
        new(Guid.Parse("11111111-0000-0000-0000-000000000001"), "Koolstofstaal",    0),
        new(Guid.Parse("11111111-0000-0000-0000-000000000002"), "RVS",              1),
        new(Guid.Parse("11111111-0000-0000-0000-000000000003"), "Non-Ferro",        2),
        new(Guid.Parse("11111111-0000-0000-0000-000000000004"), "Aluminium",        3),
        new(Guid.Parse("11111111-0000-0000-0000-000000000005"), "Kunststof",        4),
        new(Guid.Parse("11111111-0000-0000-0000-000000000006"), "Diversen",         5),
        new(Guid.Parse("11111111-0000-0000-0000-000000000007"), "Gereedschapstaal", 6),
        new(Guid.Parse("11111111-0000-0000-0000-000000000008"), "Gietstuk/deel",    7),
        new(Guid.Parse("11111111-0000-0000-0000-000000000009"), "Titaan",           8),
    ];

    public static readonly List<UnitOfMeasureSeedRow> UnitsOfMeasure =
    [
        new(Guid.Parse("22222222-0000-0000-0000-000000000001"), "Kilogram",        "kg"),
        new(Guid.Parse("22222222-0000-0000-0000-000000000002"), "Meter",           "m"),
        new(Guid.Parse("22222222-0000-0000-0000-000000000003"), "Stuk",            "st"),
        new(Guid.Parse("22222222-0000-0000-0000-000000000004"), "Uur",             "uur"),
        new(Guid.Parse("22222222-0000-0000-0000-000000000005"), "Vierkante meter", "m²"),
    ];

    private static readonly (string Code, string Name, string CategoryName, string Uom, decimal Price)[] Materials =
    [
        ("S235JR",    "Constructiestaal S235",          "Koolstofstaal",    "kg",  0.85m),
        ("S275JR",    "Constructiestaal S275",          "Koolstofstaal",    "kg",  0.92m),
        ("S355J2H",   "Constructiestaal S355",          "Koolstofstaal",    "kg",  1.05m),
        ("S355MC",    "Thermomechanisch staal S355",    "Koolstofstaal",    "kg",  1.15m),
        ("S420MC",    "Thermomechanisch staal S420",    "Koolstofstaal",    "kg",  1.25m),
        ("C45",       "Koolstofstaal C45",              "Koolstofstaal",    "kg",  1.45m),
        ("42CrMo4V",  "Gelegeerd staal 42CrMo4V",      "Koolstofstaal",    "kg",  2.80m),
        ("16MnCr5",   "Cementatiestaal 16MnCr5",       "Koolstofstaal",    "kg",  1.95m),
        ("304",       "RVS 304",                        "RVS",              "kg",  3.20m),
        ("304L",      "RVS 304L",                       "RVS",              "kg",  3.35m),
        ("316",       "RVS 316",                        "RVS",              "kg",  4.10m),
        ("316L",      "RVS 316L",                       "RVS",              "kg",  4.25m),
        ("321",       "RVS 321 Titaangestabiliseerd",   "RVS",              "kg",  4.80m),
        ("430",       "Ferritisch RVS 430",             "RVS",              "kg",  2.90m),
        ("2205",      "Duplex RVS 2205",                "RVS",              "kg",  6.50m),
        ("Cu-ETP",    "Elektrolytisch koper",           "Non-Ferro",        "kg",  9.20m),
        ("CuZn37",    "Messing 37",                     "Non-Ferro",        "kg",  7.80m),
        ("CuSn6",     "Brons 6",                        "Non-Ferro",        "kg",  8.40m),
        ("6060-T5",   "Aluminium 6060-T5",              "Aluminium",        "kg",  2.95m),
        ("6082-T6",   "Aluminium 6082-T6",              "Aluminium",        "kg",  3.40m),
        ("7075-T6",   "Aluminium 7075-T6",              "Aluminium",        "kg",  5.90m),
        ("5083-H111", "Aluminium 5083 Scheepsbouw",     "Aluminium",        "kg",  3.80m),
        ("K110",      "Gereedschapstaal K110 (D2)",     "Gereedschapstaal", "kg", 12.50m),
        ("K340",      "Gereedschapstaal K340",          "Gereedschapstaal", "kg", 14.20m),
        ("HSS",       "Sneldraaistaal HSS",             "Gereedschapstaal", "kg",  8.60m),
        ("Grade-2",   "Titaan Grade 2 Commercieel",     "Titaan",           "kg", 28.00m),
        ("Grade-5",   "Titaan Grade 5 (Ti6Al4V)",       "Titaan",           "kg", 45.00m),
        ("PA6",       "Polyamide PA6",                  "Kunststof",        "kg",  3.50m),
        ("PE1000",    "Polyethyleen PE1000",             "Kunststof",        "kg",  4.20m),
        ("PEEK",      "PEEK Engineering kunststof",     "Kunststof",        "kg", 85.00m),
        ("GG25",      "Gietijzer GG25",                 "Gietstuk/deel",    "kg",  0.95m),
        ("GGG50",     "Nodulair gietijzer GGG50",       "Gietstuk/deel",    "kg",  1.40m),
    ];

    private static readonly (string Code, string Name, string CategoryName, string Uom)[] ManufacturedParts =
    [
        ("ASM-FLENS-DN50",   "Flensdeel DN50",                "Koolstofstaal",    "st"),
        ("ASM-FLENS-DN100",  "Flensdeel DN100",               "Koolstofstaal",    "st"),
        ("ASM-FLENS-DN150",  "Flensdeel DN150",               "RVS",              "st"),
        ("ASM-STEUN-A",      "Steunkonstruktie type A",       "Koolstofstaal",    "st"),
        ("ASM-STEUN-B",      "Steunkonstruktie type B",       "Koolstofstaal",    "st"),
        ("ASM-BEUGEL-M12",   "Beugel M12",                    "Koolstofstaal",    "st"),
        ("ASM-BEUGEL-M16",   "Beugel M16",                    "Koolstofstaal",    "st"),
        ("ASM-FRAME-001",    "Lasframe standaard",            "Koolstofstaal",    "st"),
        ("ASM-FRAME-002",    "Lasframe zwaar",                "Koolstofstaal",    "st"),
        ("ASM-AS-30",        "Gedraaide as Ø30mm",            "Koolstofstaal",    "st"),
        ("ASM-AS-50",        "Gedraaide as Ø50mm",            "Koolstofstaal",    "st"),
        ("ASM-BUSH-SS",      "RVS bus Ø40/30",                "RVS",              "st"),
        ("ASM-RING-CNC",     "CNC-gefreesd ringdeel",         "Aluminium",        "st"),
        ("ASM-PLAAT-LASER",  "Lasergestanst plaatdeel",       "Koolstofstaal",    "st"),
        ("ASM-DEKSEL-RVS",   "RVS deksel 200×200",            "RVS",              "st"),
        ("ASM-KOKER-SQ80",   "Gelaste kokerkonstruktie 80²",  "Koolstofstaal",    "st"),
        ("ASM-HOUDER-ALU",   "Aluminium klemhouder",          "Aluminium",        "st"),
        ("ASM-STEEK-TITAN",  "Titaan steekkoppeling",         "Titaan",           "st"),
        ("ASM-PROFIEL-CNC",  "CNC-gefreesd profieldeel",      "Aluminium",        "st"),
        ("ASM-CILINDER-PEN", "Cilindrische stelpen Ø20",      "Koolstofstaal",    "st"),
    ];

    public ArticleGenerator(int seed)
    {
        _faker = new Faker("nl") { Random = new Randomizer(seed + 100) };
    }

    public List<ArticleSeedRow> Generate(int count)
    {
        var categoryMap = Categories.ToDictionary(c => c.Name, c => c.Id);
        var uomMap = UnitsOfMeasure.ToDictionary(u => u.Abbreviation, u => u.Id);
        var articles = new List<ArticleSeedRow>();

        // ~40% manufactured, rest raw_material — ensures order generation always has candidates
        var manufacturedTarget = Math.Max(1, (int)Math.Ceiling(count * 0.4));

        foreach (var (code, name, catName, uom) in ManufacturedParts)
        {
            if (articles.Count >= manufacturedTarget) break;
            if (_usedCodes.Contains(code)) continue;

            _usedCodes.Add(code);
            categoryMap.TryGetValue(catName, out var categoryId);
            uomMap.TryGetValue(uom, out var uomId);

            articles.Add(new ArticleSeedRow(
                Id: Guid.NewGuid(),
                Code: code,
                Name: name,
                ArticleType: "manufactured",
                Description: null,
                CategoryId: categoryId == Guid.Empty ? null : categoryId,
                UnitOfMeasureId: uomId == Guid.Empty ? null : uomId,
                PurchasePrice: null,
                IsActive: true
            ));
        }

        // Fill remaining count with raw materials from the fixed list
        foreach (var (code, name, catName, uomAbbr, price) in Materials)
        {
            if (articles.Count >= count) break;
            if (_usedCodes.Contains(code)) continue;

            _usedCodes.Add(code);
            categoryMap.TryGetValue(catName, out var categoryId);
            uomMap.TryGetValue(uomAbbr, out var uomId);

            articles.Add(new ArticleSeedRow(
                Id: Guid.NewGuid(),
                Code: code,
                Name: name,
                ArticleType: "raw_material",
                Description: null,
                CategoryId: categoryId == Guid.Empty ? null : categoryId,
                UnitOfMeasureId: uomId == Guid.Empty ? null : uomId,
                PurchasePrice: price,
                IsActive: true
            ));
        }

        // Fabricate any remaining articles needed beyond the fixed lists
        while (articles.Count < count)
        {
            var suffix = _faker.Random.Int(100, 9999);
            var prefix = _faker.PickRandom("X", "Z", "M", "HV", "DP");
            var code = $"{prefix}{suffix}";
            if (_usedCodes.Contains(code)) continue;
            _usedCodes.Add(code);

            var cat = _faker.PickRandom(Categories);
            uomMap.TryGetValue("kg", out var kgId);

            articles.Add(new ArticleSeedRow(
                Id: Guid.NewGuid(),
                Code: code,
                Name: $"Materiaal {code}",
                ArticleType: "raw_material",
                Description: null,
                CategoryId: cat.Id,
                UnitOfMeasureId: kgId == Guid.Empty ? null : kgId,
                PurchasePrice: Math.Round((decimal)_faker.Random.Double(1.0, 50.0), 2),
                IsActive: _faker.Random.Bool(0.9f)
            ));
        }

        return articles;
    }
}
