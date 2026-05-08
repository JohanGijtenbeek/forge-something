namespace Erp.Seeder.Generators;

public static class CompanyNames
{
    private static readonly string[] Prefixes =
    [
        "Aero", "Agri", "Alpha", "Ambi", "Ampli", "Aqua", "Arco",
        "Arma", "Aster", "Atlas", "Atmo", "Auto", "Avant", "Avex",
        "Axel", "Axio", "Axon", "Axum",
        "Beta", "Bio", "Bolt",
        "Carbo", "Centro", "Chromo", "Civi", "Cobalt", "Cogni", "Core",
        "Corvo", "Cosmo", "Crono", "Crux", "Cyan",
        "Data", "Delta", "Dex", "Digi", "Dura", "Dyna",
        "Echo", "Edge", "Electro", "Endo", "Ergo", "Euro", "Exo",
        "Ferro", "Flex", "Flow", "Flux", "Force", "Forge", "Forma",
        "Gamma", "Geo", "Giga", "Gyro",
        "Helio", "Hexa", "Hydro",
        "Igni", "Inno", "Intra", "Iron",
        "Kine", "Klaro",
        "Levo", "Litho", "Logi", "Lumio",
        "Macro", "Magna", "Magno", "Manu", "Meca", "Mega", "Meta",
        "Micro", "Modo", "Moto", "Multi",
        "Nano", "Neo", "Nexo", "Nova",
        "Octo", "Omni", "Opti", "Orbi",
        "Para", "Peri", "Plasto", "Poly", "Power", "Pro", "Pyro",
        "Quad", "Quant",
        "Radi", "Rapid", "Retro", "Robo",
        "Servo", "Smart", "Solar", "Solid", "Sono", "Speed",
        "Steel", "Strato", "Syntho",
        "Techno", "Termo", "Terra", "Titan", "Torque", "Trans",
        "Ultra", "Uni",
        "Velo", "Verso", "Vibro", "Volt", "Vulca",
        "Xeno", "Zero", "Zeta",
    ];

    private static readonly string[] Roots =
    [
        "arc", "arm", "ax", "cast", "cor", "craft", "cut",
        "drive", "duct", "edge", "fab", "fix", "flex", "flow",
        "flux", "force", "form", "forge", "gate", "hub", "ion",
        "jet", "lab", "line", "link", "lux", "max", "mesh",
        "mill", "mix", "net", "nex", "norm", "orb", "path",
        "peak", "plex", "point", "port", "press", "prime",
        "rad", "ram", "range", "rate", "rex", "rig", "rise",
        "set", "shaft", "shape", "sharp", "shift", "slot",
        "span", "spark", "spec", "spin", "stem", "step",
        "strand", "stream", "strip", "tec", "tech", "tek",
        "track", "trak", "trim", "tron", "unit", "val", "vex",
        "via", "volt", "vox", "wave", "work", "zon",
    ];

    private static readonly string[] NameSuffixes =
    [
        "al", "an", "ar", "co", "el", "em", "en", "er", "ex",
        "ia", "ic", "id", "il", "in", "io", "is", "ix", "nix",
        "on", "or", "os", "tek", "um", "us", "x",
    ];

    private static readonly string[] StandaloneNames =
    [
        "Anvil", "Apex", "Arc", "Armet", "Armor",
        "Basalt", "Blade", "Blast", "Bolt", "Braze",
        "Cairn", "Cast", "Chisel", "Chrome", "Cinder", "Cobalt",
        "Coil", "Core", "Crest", "Crux",
        "Delta", "Drake", "Drive",
        "Edge", "Ember", "Epoch",
        "Facet", "Ferrum", "Fibre", "Flare", "Flint", "Flux",
        "Forge", "Frame", "Fuse",
        "Glyph", "Grain", "Grind", "Grit",
        "Hammer", "Helix", "Hinge",
        "Impact", "Ingot", "Joint",
        "Keel", "Kinetic",
        "Lance", "Lathe", "Lever",
        "Magma", "Mast", "Matrix", "Mesh",
        "Nexus", "Notch",
        "Onyx", "Orbit", "Origin",
        "Peak", "Pivot", "Plane", "Prism", "Probe",
        "Quartz", "Radius", "Ratchet", "Relay", "Ridge", "Rivet",
        "Seam", "Shard", "Shear", "Shell", "Signal",
        "Slag", "Span", "Spire", "Splice", "Spring", "Spur",
        "Stratum", "Strike", "Strut",
        "Temper", "Tensile", "Thrust", "Torque", "Trace", "Truss",
        "Turbo", "Umbra",
        "Vanadium", "Vault", "Vector", "Vertex", "Vortex",
        "Weld", "Wire", "Xenon", "Yoke", "Zenith", "Zircon",
        "Acumen", "Adept", "Advent", "Agile", "Axiom",
        "Beacon", "Cadence", "Catalyst", "Clarity", "Conduit",
        "Delphi", "Emblem", "Emerge", "Fathom", "Fenix",
        "Genesis", "Gradient", "Halcyon", "Harbinger",
        "Ignite", "Impetus", "Index", "Keystone", "Kinesis",
        "Lattice", "Leverage", "Locus",
        "Mandate", "Manifold", "Maverick", "Modular", "Momentum",
        "Nucleus", "Optima",
        "Parallax", "Paragon", "Pinnacle", "Proxima",
        "Quantum", "Rapture", "Ratio", "Reflex",
        "Sentinel", "Sequence", "Solstice", "Summit", "Synergy",
        "Tangent", "Theorem", "Valence", "Valiant", "Vanguard",
        "Veritas", "Warden", "Zephyr",
    ];

    public static readonly string[] LastNames =
    [
        "Bakker", "Boer", "Visser", "De Vries", "Van den Berg", "Van Dijk",
        "Janssen", "Smit", "Meijer", "De Groot", "Van der Meer", "Willems",
        "Peters", "Hendriks", "Maas", "Mulder", "Dekker", "Brouwer",
        "Peeters", "Vermeulen", "Van Leeuwen", "Jacobs", "Linders",
        "Hoekstra", "Koster", "Schouten", "Bos", "Sanders", "Driessen",
        "Kuipers", "Lammers", "Hermans", "Vos", "Kok", "Van der Laan",
        "Wolters", "Martens", "Scholten", "Vink", "Huisman", "Van Dam",
        "Timmermans", "Claassen", "Baas", "Van der Heiden", "Prins",
        "Aarts", "Kuiper", "Verhoeven", "Kroese", "Florijn",
        "Zwart", "Roos", "Winter", "Zomer", "Molenaar", "Molendijk",
        "Brugman", "Vogels", "Ravensberg", "Duivenvoorde", "Appeldoorn",
        "Steenhoven", "Waterreus", "Langerak", "Havermans", "Haverkamp",
        "Groenenberg", "Roodbergen", "Lindeboom", "Hazelaar", "Oosterhout",
        "Westerink", "Noordergraaf", "Zuidervaart", "Kortrijk", "Bogaard",
        "Cools", "Witjes", "Zwartjes", "Geelkerken", "Zwaanswijk",
        "Blauwboer", "Eikenboom", "Steenbruggen", "Herfst",
        "Kooistra", "Dijkstra", "Wiersma", "Postma",
        "Bouma", "Hofstra", "Feenstra", "Veenstra", "Hiemstra",
        "Bijlsma", "Tolsma", "Nauta", "Stienstra",
        "Holwerda", "Hellinga", "Kingma", "Faber", "Bosma",
        "Van der Wal", "Van der Veen", "Van der Molen", "Van der Sluis",
        "Van der Werf", "Van der Linde", "Van der Steen", "Van der Hoek",
        "Van der Brink", "Van der Horst", "Van der Haar", "Van der Kamp",
        "Van den Brink", "Van den Bosch", "Van den Heuvel", "Van den Broek",
        "Van de Ven", "Van de Berg", "Van de Pol", "Van de Waal",
        "De Jong", "De Boer", "De Bruijn", "De Wit", "De Haan",
        "De Lange", "De Ruiter", "De Wolf", "De Leeuw", "De Graaf",
        "De Jonge", "De Bruin", "De Ridder",
        "Ten Hoeve", "Ten Berg", "Ten Brinke", "Ten Cate",
        "Ter Maat", "Ter Huurne", "Ter Beek", "Ter Haar",
        "Nieuwenhuis", "Nieuwkamp", "Nieuwland", "Nieuwenhuizen",
        "Rijsbergen", "Rijswijk", "Rijkeboer",
        "Zonneveld", "Zonnenberg", "Zondervan",
        "Oosterbeek", "Oosterbosch", "Oosterink",
        "Westerhof", "Westerbeek", "Westerman",
        "Noordhuis", "Noordijk", "Noordam",
        "Zuidema", "Zuidhof", "Zuidberg",
        "Groenewegen", "Roosenboom",
        "IJzerman", "IJssel", "IJzendoorn", "IJsselstein",
        "Müller", "Schröder", "Bäcker", "Köhler", "Möller",
        "Schäfer", "Günther", "Böhm", "Bühler", "Löffler", "Hübner",
        "Kröger", "Wölfle", "Büscher", "Lüdemann",
        "Schmidt", "Schneider", "Fischer", "Weber", "Wagner",
        "Becker", "Schulz", "Hoffmann", "Koch", "Bauer",
        "Richter", "Klein", "Wolf", "Zimmermann",
        "Braun", "Krüger", "Hofmann", "Hartmann", "Schwarz",
        "Krause", "Werner", "Schmitz", "Lehmann",
        "Schubert", "Roth", "Bergmann", "Friedrich", "Keller",
        "Berger", "Weiß", "Böttcher", "Brandt",
        "Löwe", "Stöhr", "Höfer", "Römer", "Götz",
        "De Smedt", "Van Acker", "De Cock", "Vermeersch", "Claes",
        "Nijs", "Maes", "Claeys", "Wouters", "Janssens",
        "Stevens", "Goossens", "Mertens",
        "Declercq", "Desmet", "Devos", "Dewaele",
        "Vandenberghe", "Vandermeersch", "Vandecasteele",
        "Van Hecke", "Van Hove", "Van Damme", "Van Dyck",
        "Dubois", "Lecomte", "Fontaine", "Beaumont",
        "Renard", "Lefèvre", "Léger", "Liénard",
        "Piérard", "Gérard",
        "Çelik", "Yılmaz", "Kılıç", "Şahin", "Güneş", "Özdemir",
        "Yıldız", "Arslan", "Doğan", "Kaya", "Demir", "Çetin",
        "Aydın", "Özkan", "Şimşek", "Bozkurt", "Çakır", "Öztürk",
        "Kowalski", "Nowak", "Wiśniewski", "Wójcik",
        "Kamiński", "Lewandowski", "Zieliński", "Szymański",
        "Andersen", "Eriksson", "Björk", "Ström", "Löfgren",
        "Lindström", "Björnsson", "Ångström", "Søndergaard",
        "García", "González", "Rodríguez", "López", "Martínez",
        "Sánchez", "Pérez", "Gómez", "Fernández",
        "Rossi", "Ferrari", "Esposito", "Bianchi", "Romano",
        "Colombo", "Ricci", "Marino", "Greco",
        "Martin", "Bernard", "Thomas", "Robert",
        "Richard", "Petit", "Durand", "Leroy", "Moreau",
    ];

    private static readonly string[] CompanyTypes =
    [
        "Metaal", "Staal", "Techniek", "Industrie", "Constructie",
        "Machinefabriek", "Handel", "Plaatbewerking", "Lasbedrijf",
        "Fabricage", "Groep", "Holding", "Engineering", "Services",
        "Solutions", "Systems", "Products", "Manufacturing", "Works",
        "International", "Europe", "Nederland", "Precision",
        "Components", "Assembly", "Welding", "Cutting", "Forming",
        "Processing", "Finishing", "Coating", "Casting", "Stamping",
    ];

    private static readonly string[] CompanySuffixes =
    [
        "B.V.", "B.V.", "B.V.", "N.V.", "V.O.F.", "B.V. & Co.",
    ];

    // ============================================================
    // Shuffled pools — elk item wordt gebruikt voor het herhaald wordt
    // ============================================================
    private static readonly Random _rng = new(42);

    private static Queue<string> _prefixPool = new();
    private static Queue<string> _rootPool = new();
    private static Queue<string> _suffixPool = new();
    private static Queue<string> _standalonePool = new();
    private static Queue<string> _lastNamePool = new();
    private static Queue<string> _typePool = new();

    private static string PickFromPool(Queue<string> pool, string[] source)
    {
        if (pool.Count == 0)
            foreach (var item in source.OrderBy(_ => _rng.Next()))
                pool.Enqueue(item);
        return pool.Dequeue();
    }

    public static string GenerateUniqueName(HashSet<string> usedNames)
    {
        string name;
        var attempts = 0;
        do
        {
            name = _rng.Next(10) switch
            {
                < 6 => GenerateConstructedName(),
                < 7 => GenerateStandaloneName(),
                _ => GenerateLastNameBased()
            };

            if (attempts > 100)
                name = $"{GenerateConstructedName()} {attempts}";

            attempts++;
        } while (!usedNames.Add(name));

        return name;
    }

    private static string GenerateConstructedName()
    {
        var prefix = PickFromPool(_prefixPool, Prefixes);
        var root = PickFromPool(_rootPool, Roots);
        var suffix = PickFromPool(_suffixPool, CompanySuffixes);
        var type = _rng.Next(2) == 0 ? $" {PickFromPool(_typePool, CompanyTypes)}" : "";

        return _rng.Next(3) switch
        {
            0 => $"{prefix}{root}{type} {suffix}",
            1 => $"{prefix}{root}{PickFromPool(new Queue<string>(), NameSuffixes)}{type} {suffix}",
            _ => $"{prefix}{PickFromPool(new Queue<string>(), NameSuffixes)}{type} {suffix}",
        };
    }

    private static string GenerateStandaloneName()
    {
        var baseName = PickFromPool(_standalonePool, StandaloneNames);
        var suffix = PickFromPool(_suffixPool, CompanySuffixes);
        var type = _rng.Next(2) == 0 ? $" {PickFromPool(_typePool, CompanyTypes)}" : "";
        return $"{baseName}{type} {suffix}";
    }

    private static string GenerateLastNameBased()
    {
        var lastName = PickFromPool(_lastNamePool, LastNames);
        var type = PickFromPool(_typePool, CompanyTypes);
        var suffix = PickFromPool(_suffixPool, CompanySuffixes);

        return _rng.Next(4) == 0
            ? $"{lastName} & {PickFromPool(_lastNamePool, LastNames)} {type} {suffix}"
            : $"{lastName} {type} {suffix}";
    }
}
