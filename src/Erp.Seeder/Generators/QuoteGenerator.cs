using Bogus;
using Erp.Seeder.Models;
using System.Net.Http.Json;

namespace Erp.Seeder.Generators;

public class QuoteGenerator
{
    private readonly Faker _faker;
    private readonly HttpClient _client;

    private static readonly string[] PartNames =
    [
        "Behuizing", "Deksel", "Flens", "As", "Tandwiel", "Steunplaat", "Borgring",
        "Lagerschild", "Koppelstuk", "Montageplaat", "Cilinderkop", "Afdekking",
        "Verbindingsstuk", "Basisplaat", "Eindstuk", "Tussenring", "Draagarm",
        "Spindel", "Scharnierpunt", "Bevestigingsblok"
    ];

    private static readonly string[] MaterialTypes = ["Staal", "Aluminium", "RVS", "Koper", "Messing"];
    private static readonly string[] MaterialGeometries = ["Rnd", "Plaat", "Vierkant", "Buis", "Zeshoek"];

    private static readonly (string Code, string Code2)[] Materials =
    [
        ("1.0503", "C45"), ("1.0037", "S235JR"), ("1.0570", "S355J2"),
        ("1.4301", "AISI 304"), ("1.4404", "AISI 316L"), ("1.4571", "AISI 316Ti"),
        ("3.3206", "AlMgSi0.5"), ("3.3547", "AlMg4.5Mn"), ("3.1325", "AlCuMgPb"),
        ("2.0060", "Cu-ETP"), ("2.0401", "CuZn37"), ("2.0375", "CuZn39Pb3")
    ];

    private static readonly string[] DeliveryTimes = ["2 weken", "3 weken", "4 weken", "6 weken", "8 weken", "nader te bepalen"];

    public QuoteGenerator(int seed, string apiUrl)
    {
        _faker = new Faker("nl") { Random = new Randomizer(seed + 500) };
        _client = new HttpClient { BaseAddress = new Uri(apiUrl) };
    }

    public async Task<List<QuoteSeedRow>> GenerateAsync(int count, CancellationToken ct = default)
    {
        var customersResponse = await _client.GetFromJsonAsync<List<CustomerItem>>(
            "/api/parties/customers", ct) ?? [];

        var quotes = new List<QuoteSeedRow>();

        for (var i = 0; i < count; i++)
        {
            CustomerItem? customer = customersResponse.Count > 0 && _faker.Random.Bool(0.85f)
                ? _faker.PickRandom(customersResponse)
                : null;

            var daysOffset = _faker.Random.Int(-60, 30);
            var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysOffset));

            var lineCount = _faker.Random.Int(1, 5);
            var lines = new List<QuoteLineSeedRow>();

            for (var j = 0; j < lineCount; j++)
            {
                lines.Add(GenerateLine(j + 1));
            }

            // Draft ~45%, sent ~25%, accepted ~20%, rejected ~10%
            var targetStatus = _faker.Random.WeightedRandom(
                ["draft", "sent", "accepted", "rejected"],
                [0.45f, 0.25f, 0.20f, 0.10f]);

            // Mark some lines as accepted when quote will be accepted
            if (targetStatus == "accepted")
            {
                lines = lines
                    .Select(l => l with { ShouldAccept = _faker.Random.Bool(0.75f) })
                    .ToList();
            }

            quotes.Add(new QuoteSeedRow(
                CustomerId:     customer?.Id,
                CustomerName:   customer?.Name,
                Date:           date,
                Reference:      _faker.Random.Bool(0.5f) ? $"REF-{_faker.Random.AlphaNumeric(6).ToUpper()}" : null,
                ContactPerson:  _faker.Random.Bool(0.6f) ? $"Dhr. {_faker.Name.LastName()}" : null,
                DeliveryTime:   _faker.Random.Bool(0.7f) ? _faker.PickRandom(DeliveryTimes) : null,
                HourlyRate:     _faker.Random.Decimal(60, 95),
                MaterialMargin: _faker.Random.Decimal(110, 125),
                StandardMargin: _faker.Random.Decimal(8, 15),
                SetupTime:      Math.Round(_faker.Random.Decimal(0.5m, 2m), 2),
                TargetStatus:   targetStatus,
                Lines:          lines
            ));
        }

        return quotes;
    }

    private QuoteLineSeedRow GenerateLine(int sortOrder)
    {
        var partBaseName = _faker.PickRandom(PartNames);
        var suffix = _faker.Random.AlphaNumeric(3).ToUpper();
        var partName = $"{partBaseName} {suffix}";
        var partNumber = $"{_faker.Random.AlphaNumeric(3).ToUpper()}-{_faker.Random.Int(100, 999)}";
        var quantity = Math.Round(_faker.Random.Decimal(1, 200), 0);

        var hasMaterial = _faker.Random.Bool(0.75f);
        var (materialCode, materialCode2) = hasMaterial ? _faker.PickRandom(Materials) : (null, null);

        var matSizeMm = hasMaterial ? Math.Round(_faker.Random.Decimal(10, 200), 1) : (decimal?)null;
        var matLengthMm = hasMaterial ? Math.Round(_faker.Random.Decimal(50, 3000), 0) : (decimal?)null;
        var matQty = hasMaterial ? Math.Round(quantity * _faker.Random.Decimal(1, 1.2m), 4) : (decimal?)null;
        var matPrice = hasMaterial ? Math.Round(_faker.Random.Decimal(1.5m, 25m), 4) : (decimal?)null;

        var opCount = _faker.Random.Int(1, 4);
        var opMinutes = Math.Round(_faker.Random.Decimal(5, 120), 2);

        var hasSubcontract = _faker.Random.Bool(0.2f);
        var subCount = hasSubcontract ? _faker.Random.Int(1, 2) : 0;
        var subPrice = hasSubcontract ? Math.Round(_faker.Random.Decimal(50, 500), 2) : 0m;

        var isManual = _faker.Random.Bool(0.15f);
        var manualPrice = isManual ? Math.Round(_faker.Random.Decimal(10, 800), 2) : (decimal?)null;

        return new QuoteLineSeedRow(
            SortOrder:             sortOrder * 10,
            PartName:              partName,
            PartNumber:            partNumber,
            Quantity:              quantity,
            MaterialType:          hasMaterial ? _faker.PickRandom(MaterialTypes) : null,
            MaterialCode:          materialCode,
            MaterialCode2:         materialCode2,
            MaterialGeometry:      hasMaterial ? _faker.PickRandom(MaterialGeometries) : null,
            MaterialSizeMm:        matSizeMm,
            MaterialLengthMm:      matLengthMm,
            MaterialQuantity:      matQty,
            MaterialPrice:         matPrice,
            MaterialSource:        _faker.Random.Bool(0.9f) ? "inclusive" : "customer",
            OperationCount:        opCount,
            OperationTimeMinutes:  opMinutes,
            SubcontractingCount:   subCount,
            SubcontractingPrice:   subPrice,
            IsManualPrice:         isManual,
            ManualPrice:           manualPrice,
            ShouldAccept:          false
        );
    }

    private record CustomerItem(Guid Id, string Name);
}
