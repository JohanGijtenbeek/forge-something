# Legacy System Analysis — Old WinForms ERP (2016)

This document captures domain knowledge extracted from the old application's database schema and sample data. It exists so future development sessions can understand the business without re-sharing the raw data.

## What the company does

A **precision CNC job shop** (contract manufacturer) that produces custom metal parts to customer drawings and specifications. They do not sell their own products — they manufacture entirely to order for other companies. Core competencies are CNC turning and milling, with a supporting range of finishing and subcontracted operations.

## Machine park

### CNC Lathes (Draaien)

| Machine                    | Brand    | Control |
| -------------------------- | -------- | ------- |
| VICTOR 26/36/46 (multiple) | Victor   | Fanuc   |
| VTURN A26                  | Victor   | Fanuc   |
| NAKA SC200/1, SC200/2      | Nakamura | Fanuc   |
| NAKA WY250, AS200, WT150   | Nakamura | Fanuc   |
| MURATEC                    | Muratec  | Fanuc   |
| VICTOR/BUIS (tube turning) | Victor   | Fanuc   |

### CNC Milling Centers (Frezen)

| Machine                   | Brand      | Control    |
| ------------------------- | ---------- | ---------- |
| SABRE 750, ARROW 750      | Cincinnati | Cincinnati |
| HAAS 14, 15, 16           | Haas       | Haas       |
| VICTOR A-110, 102E, AX350 | Victor     | Fanuc      |
| MAZAK                     | Mazak      | Haas       |
| OKK-HM, OKK-HP            | OKK        | Fanuc      |
| CHALENGER, OLYMPIC        | —          | Fanuc      |

All machines communicate via serial or network (DNC — Direct Numerical Control) for program transfer from a central server.

## Materials handled

### Material types (Materiaalsoort)

| Code | Type                              |
| ---- | --------------------------------- |
| 0    | Carbon steel (Koolstofstaal)      |
| 1    | Stainless steel (RVS)             |
| 2    | Non-ferrous (Non-Ferro)           |
| 3    | Aluminium                         |
| 4    | Plastic (Kunststof)               |
| 5    | Miscellaneous (Diversen)          |
| 6    | Tool steel (Gereedschapstaal)     |
| 7    | Castings (Gietstuk/deel)          |
| 8    | Titanium                          |
| 9    | Invoice item (Faktuur)            |
| 10   | Finished product (Gereed product) |

### Stock shapes (Materiaalgeometrie)

Round bar (Rnd), Tube (Buis), Square (Vierkant), Hexagonal (Zesk), Plate (Plaat), Flat/rectangular (Plat), Cut/profile (Snij), Casting (Gietdeel), Assembly (Samenstelling), Existing part (Bestaand).

### Common material grades (examples from Materiaalcode)

Carbon steel: C45, S355J2G3, 42CrMo4, 16MnCr5, St52  
Stainless: 303, 304, 316L, 1.4462 (duplex), 1.4571, Inconel 718 (2.4668)  
Aluminium: 7075-T6  
Non-ferrous: CuAl10Ni, CuZn39Pb3  
Tool steel: 1.2312, 1.3343 (HSS)  
Cast iron: GGG50  
Plastic: POM black (Pom Zwart)  
Titanium: tracked as type 8

Material prices in the legacy system are per-code with a date, used for quoting. A `MateriaalMarge` factor (e.g., 1.15 = 15% markup) is applied on top.

## Operations (Bewerkingen)

### In-house operations

- CNC draaien (CNC turning)
- CNC frezen (CNC milling)
- Muratec / Mazak / OKK / Nakamura rechts (machine-specific operations)
- Zagen (sawing)
- Afbramen (deburring)
- Slijpen (grinding)
- Honen (honing)
- Brootsen / Steken-Brootsen (broaching / grooving)
- Stansen (punching/stamping)
- Lassen (welding)
- Trommelen (barrel/tumble finishing)
- Polijsten (polishing)
- Merken (marking/stamping)
- Materiaaluitgifte (material issue from stores)
- Meten / 3D Meten (measuring / 3D measuring)
- Tussencontrole (intermediate inspection)
- Eindcontrole (final inspection)
- Eindafwerking (final finishing)
- Montage (assembly)

### Subcontracted operations (Uitbesteding)

- Oppervlaktebehandeling (surface treatment — plating, anodising, etc.)
- Warmtebehandeling (heat treatment)
- Stelliteren (Stellite hard-facing)
- Carbide opspuiten (carbide spraying)
- Plasmaspuiten (plasma spraying)
- Meetrapport (certified measurement report)
- CAD/CAM (external programming)
- Engineering

## Business workflow

```
Customer inquiry (phone / email / fax / verbal)
  ↓
Quote (Offerte)
  - Header: customer, date, reference, delivery time, hourly rate (€72), markup factors
  - Line items (OfferteItem): part number, name, quantity options,
    material (code + geometry + size + length + source + price),
    number of operations + time per operation, subcontracting count + cost
  - Material source: Inclusief (supplied by shop) or Klant (customer-supplied)
  ↓
Quote accepted → Production Order (Order / Opdracht)
  - Ordernummer: internal sequential ID
  - Productienummer / Productienummer_verkort: part tracking number (e.g. 611-0001-000 / 611-0001)
  - Ordernummer_klant: customer's own PO reference
  - Klantnummer: customer ID
  - Aantal: ordered quantity
  - Leverweek: delivery week (YYYYWW format)
  - Leverwijze: delivery method (mostly Brengen = shop delivers)
  - Toeslag: surcharge
  - Eigendomklant: flag for customer-owned material/tooling stored at the shop
  ↓
Raw material ordered (Inkoop)
  - Inkoopnummer: purchase order to supplier
  - Links to Ordernummer (per order, not batched by default)
  - Tracks: code, geometry, size, length, quantity
  - Dates: Datumbesteld, Datumbevestigd, Datumgeleverd
  - Source masked as "*****" in queries (supplier name/stock location in real data)
  ↓
Materiaaluitgifte (material issued to shop floor)
  ↓
Geleidekaart (routing card) — paper document that travels with the part
  - Lists the ordered operation sequence
  - Each operation is booked off as it is completed
  ↓
Production (operations executed per machine in sequence)
  - Tussencontrole (intermediate inspection) at key stages
  - Subcontracting steps dispatched and tracked when needed
  ↓
Eindcontrole (final inspection)
  - Meetrapport (measurement report) if customer requires it
  - Certificaat (material certificate) tracked per order
  ↓
Pakbon (packing slip) generated → delivery to customer
  ↓
Factuur (invoice)
  - Faktuurnummer tracked on the order
  - Gefactureerd flag set
  - Credit notes possible (Crediteren, Crediteer_prijs, Crediteer_aantal, Gecrediteerd)
```

## Order state fields (on the Order record)

| Field                    | Meaning                                |
| ------------------------ | -------------------------------------- |
| Voorraadger              | Stock reserved for this order          |
| Halfvoorraadgereserveerd | Semi-finished stock reserved           |
| Gefactureerd             | Invoiced (bool)                        |
| Aantalgeleverd           | Quantity actually delivered            |
| Aantalnuteleveren        | Quantity still to deliver              |
| Geannuleerd              | Cancelled                              |
| Aantalextratemaken       | Extra pieces to make (scrap allowance) |
| Actueel                  | Active/current (shown in planning)     |
| OnHold                   | On hold                                |
| Orderbevestiging         | Order confirmation sent                |
| Geleidekaart             | Routing card printed                   |
| Inclusief                | Inclusive pricing flag                 |
| Chargenummer             | Batch/heat number (traceability)       |
| Certificaatbinnen        | Material certificate received          |

## Quoting logic

- **Hourly rate**: €72
- **Labor markup** (`Marge`): typically 1.1 (10%)
- **Material markup** (`MateriaalMarge`): typically 1.15 (15%)
- **Setup time** (`Insteltijd`): tracked per quote, but typically 1 hour in data seen
- Price per OfferteItem = (machining time × hourly rate × marge) + (material cost × materiaalMarge) + subcontracting costs
- Multiple quantity breaks per line item are common (e.g., 50/75/100 pieces with sliding unit price)

## Customers and market

- Batch sizes range from 1 (one-off fixtures, tools) to 2000+ (series production)
- Part examples: shafts, spacers, rings, pistons, housings, drill taps, bridges, balls, brackets, bearing components
- End markets: hydraulics, industrial machinery, precision instrumentation, possibly oil & gas (Inconel 718 use)
- Cross-border customers: German part names (Aussenring, Spalttopf) and German-language references indicate significant business with DACH-region customers
- Customer contact channel tracked per quote: telephone, email, fax, verbal (Mondeling)

## Inventory concepts

- **Materiaalvoorraad**: raw stock on hand (material code + geometry + size + quantity + location)
- **Voorraad**: finished/semi-finished part stock (part number + quantity + price)
- **Halfvoorraad**: semi-finished stock (partially machined blanks held for repeat orders)
- **Eigendomklant**: customer-owned material or tooling stored at the shop

## Key data relationships (for Orders domain design)

The legacy system only had one "Order" concept. The new ERP should separate two distinct concerns:

- **Sales Order** — customer-facing. Created from a quote. Groups one or more Production Orders. Carries the customer PO reference, delivery address, invoice details, and total price. This is what the customer sees.
- **Production Order** — shop floor-facing. One per article/part. Carries the routing, material, machine assignments, and operation bookings. This is what the Loader sees on their machine queue.

A single Sales Order from a quote with three line items produces three Production Orders. The shop floor never sees the Sales Order — they work from Production Orders only.

```
Offerte (1) ──< OfferteItem (many)
  ↓ accepted
SalesOrder (1) ──< ProductionOrder (many, one per line item)
                       ├── OrderOperations[]  ← the pipeline (copied from article routing)
                       ├── Materiaal (1..n)   ← raw material requirements
                       ├── Inkoop (1..n)      ← purchase orders to suppliers
                       └── Deliveries[]       ← partial deliveries tracked here
```

A single Productienummer can span multiple Order records (sub-positions), identified by the suffix (e.g., 611-0001-000, 611-0001-001).

## Mental model — the Factorio analogy

Understanding this company is easiest through the lens of **Factorio**, but with one permanent constraint: **you can never fully automate it**, because every recipe is different.

- **Raw materials = ore patches** — steel bar stock, stainless rod, aluminium billet arrive from suppliers and sit in the warehouse. Wrong size ordered means you wait.
- **Machines = assemblers and furnaces** — a fixed number of lathes and mills, all expensive. When three jobs need the CNC lathe at the same time, you have a classic Factorio bottleneck.
- **The Geleidekaart = a crafting recipe that travels with the item** — each part has its own sequence (saw → turn → mill → grind → deburr → inspect). The routing card physically follows the part through the factory and gets stamped at each station. Steps cannot be skipped or reordered.
- **Subcontracting = technologies you haven't unlocked** — heat treatment, plasma coating, stellite hard-facing are steps you ship out and wait for. You can't build those machines in-house.
- **Multiple simultaneous orders = spaghetti mid-game** — dozens of parts in various stages, all competing for the same machines.

The key difference from Factorio: in Factorio you eventually reach a steady state where everything flows. Here, the moment you optimize for today's jobs, tomorrow's orders have completely different parts, materials, and routings. The factory never reaches a steady state. Every day is mid-game spaghetti.

## Mental model — the CI/CD pipeline analogy

For software developers, a CI/CD pipeline is the cleaner analogy:

- **The `.yml` pipeline definition = the article routing** — configured once, parameterized, reusable across any number of runs
- **A pipeline run = an order** — triggered with specific inputs (quantity, due date, customer PO), produces a specific artifact
- **Stages = operations** — sequential by default, some parallelizable, some conditional (`if: customer_requires_measurement_report`)
- **Runners = machines** — the job is assigned to whatever runner is available with the right capabilities; the pipeline definition doesn't care which one
- **An external action / webhook call = subcontracting** — you dispatch to a third party, wait for the callback, the pipeline resumes
- **The run log = the Geleidekaart** — every stage, its status, duration, who executed it

The one meaningful difference from CI/CD: a failed run in software is cheap — fix and re-trigger. A scrapped part means expensive material and machine hours are gone. That's why intermediate quality checks (Tussencontrole) exist — they are **deployment gates** before promoting to the next environment. You catch the defect after turning, before milling, not at final inspection.

The `OutForSubcontracting` status is a pipeline waiting on an external webhook that may take days.

## Design pattern — configurable pipeline

The Geleidekaart maps directly to the **Pipeline pattern**, not Chain of Responsibility. The distinction matters for implementation.

The routing maps to the **Pipeline pattern**: each stage receives the part, transforms it (removes material, applies coating, adds a measurement), and passes a new state downstream. Stages are ordered, and each produces output the next depends on. Think Unix pipes — `saw | turn | mill | grind`.

Nuances that shape the implementation:

- **Configurable per article** — the routing is data, not code. The operation list with sequence numbers (Volgordenummer in the Bewerking table) defines the pipeline at runtime.
- **Conditional stages** — measurement report only if the customer requires it, surface treatment only if specified. The pipeline needs optional/skippable steps.
- **Async stages** — subcontracting steps leave the pipeline, go somewhere external, and re-enter. The order needs an `OutForSubcontracting` state to handle this gap cleanly.
- **Shared resources** — multiple pipelines compete for the same machines. Scheduling (which machine, when) is a separate concern layered on top of the routing.
- **Strategy at the stage level** — the routing says "CNC turning"; the scheduler assigns it to a specific available lathe. The operation type and the machine are decoupled.
- **DAG for assemblies** — when a part is an assembly of sub-components, each sub-component is its own pipeline. The assembly step carries a `dependsOn` all sub-pipelines. The full structure is a Directed Acyclic Graph, not a linear sequence. Think Azure Data Factory: parallel pipelines running independently, converging at a join step. The routing designer must support this.

The practical data shape for `OrderOperations`:

```csharp
// Each row is one stage in the pipeline for this order
OrderOperation {
    OrderId
    SequenceNumber       // defines pipeline order
    OperationId          // what type of work (CNC turning, grinding, etc.)
    MachineId            // nullable until scheduled
    EstimatedMinutes
    ActualMinutes
    Status               // Pending | InProgress | OutForSubcontracting | Done
    CompletedAt
    CompletedBy
}
```

The "chain" is an ordered list of steps with status. No clever routing pattern needed in the application code — the complexity lives in scheduling and the async subcontracting flow.

## Roles and responsibilities

### Process Planner (Werkvoorbereider)

The bridge between a customer order and the shop floor. Their job is to answer: _"how are we going to make this part?"_

- Interprets the customer drawing
- Defines the operation sequence — the article routing template (the pipeline)
- Selects which machine types are needed per operation
- Estimates machining times per operation
- Specifies tooling and clamping setups
- Creates the Geleidekaart

Primary author of the article routing template in the ERP. Every time a new part enters the system, it is their work that populates the pipeline stages.

### Engineer

Overlaps with the Process Planner in smaller shops, but more focused on:

- Interpreting and validating customer drawings and models
- CAD/CAM — translating 3D models into NC programs
- Resolving technical problems during production
- Managing technical documentation per article

Engineering and CAD/CAM both appear as billable operations in the legacy data (machine numbers 101 and 114), meaning they are tracked as steps in the pipeline.

#### NC programs
An NC program (or CNC program) is the code that runs on a CNC machine and tells it exactly how to move the cutting tool to produce a part. It is written in **G-code** — a standardized instruction language where each line is an instruction: move to this position, spin the spindle at this speed, feed at this rate, stop.

The Engineer loads the customer's 3D model into CAD/CAM software (e.g. Mastercam, Hypermill), defines the toolpaths visually, and the software generates the G-code automatically. The Engineer reviews and verifies it before it goes near a machine.

NC programs are stored on a central server and transferred to the machine at runtime via **DNC (Direct Numerical Control)** — the communication type listed on every machine in the legacy data (serial or network). The Loader selects the correct program for the job, it downloads to the machine's controller, and production starts. This is why DNC integration is a potential ERP feature: the system already knows which article is being produced and could automatically push the correct program to the assigned machine.

### Loader

The CNC machine operator/setter on the shop floor:

- Receives the job via the Geleidekaart
- Loads the NC program to the machine via DNC (every machine in the legacy data has a communication type for this reason)
- Sets up tooling and fixtures
- Feeds the raw material
- Runs the first part and checks it before producing the batch
- Books the operation as complete and logs actual time

The name comes from the physical act of loading a program onto the machine and loading material into it.

### Interface implications

| Role            | Interface                              | Primary actions                                          |
| --------------- | -------------------------------------- | -------------------------------------------------------- |
| Process Planner | Desktop — article & routing management | Define pipelines, estimate times, create Geleidekaarten  |
| Engineer        | Desktop — article & documentation      | NC programs, drawings, technical specs                   |
| Loader          | Shop floor screen (touch-friendly)     | Book operations start/done, log actual time, flag issues |

The Loader's interface needs to work on a dirty shop floor with gloves on — simplicity and large touch targets over information density.

## Workflow scenarios

Each scenario is split into two parts: what happens in the real world, and what the ERP needs to do about it.

### Repeat order for an existing part

**Situation:** A returning customer orders 50 pieces of a part they have ordered before. A sales/planning person takes the call and creates the order.

**System response:** The order is created from the **article master** — not from a previous order instance. Previous orders may have had deviations (broken machine, added step, manual adjustment). The article master is the clean canonical version. Copying from a prior order is how technical debt accumulates.

---

### New part, never made before

**Situation:** A new customer sends a drawing for a part that has never been produced. No article exists yet.

**System response:** The **Engineer** validates the drawing and produces the NC program. The **Process Planner** uses the process designer in the ERP to create the article routing — which operations, in which sequence, with which time estimates. Only once the routing is complete can a Production Order be placed against it. The process designer is the Process Planner's tool, not the Engineer's.

---

### Machine bottleneck mid-pipeline

**Situation:** A job is queued for milling but all milling machines are occupied by other jobs.

**System response:** The order enters a **Queued** state. The ERP surfaces this visibly so a planner can see where bottlenecks are building. Optimal machine assignment (see confirmed decisions) should prevent this in normal operation, but the queued state must exist as a fallback.

---

### Subcontracting step in the pipeline

**Situation:** A part needs heat treatment after milling, performed by an external company. The part leaves the building.

**System response:** The order enters an **OutForSubcontracting** state and the pipeline pauses. When the part is physically delivered back it is scanned or confirmed, the status updates, and the pipeline resumes from the next operation. Dispatch to external → wait for callback → continue.

---

### Customer-supplied material

**Situation:** The customer supplies their own raw material for the job. It arrives at the shop with the order.

**System response:** No purchase order (Inkoop) is created — the Inkoop flow is skipped entirely. The material is booked into customer-owned stock. The quote shows material source as "Klant" with €0 material cost. Traceability is more critical than usual: the shop is accountable for someone else's material. Scrap has significantly more serious consequences than with shop-supplied stock.

---

### Scrap and scrap allowance

**Situation:** A Loader discovers 3 defective pieces mid-batch of 50. The diameter is out of tolerance and they cannot be salvaged.

**System response:** A **scrap allowance** (`Aantalextratemaken`) is set when the Production Order is created — a 50-piece order plans for 53 pieces, anticipating a small scrap rate. The 3 extras absorb the loss and the order is still fulfilled. Scrapped pieces are logged against the order for traceability. If scrap exceeds the allowance, a rework order is raised. There is no general spare parts pool for custom machined parts.

---

### Partial delivery

**Situation:** A customer orders 100 pieces. 60 are ready on time; the remaining 40 need another week. The customer needs something now.

**System response:** The Production Order is not split or closed — it remains the single source of truth. Each physical delivery generates a packing slip and increments the delivered quantity. The order status moves to **Partially Delivered** and stays open until fully fulfilled or cancelled. Splitting orders creates administrative overhead and breaks traceability back to the original customer PO.

---

### Article revision

**Situation:** A customer sends a revised drawing — same part number, slightly different geometry, one extra operation now required. Two Production Orders for the old version are currently in progress on the shop floor.

**System response:** The article gets a new **revision** (Rev A → Rev B). In-progress orders were instantiated from Rev A and own their own copy of the routing — they complete as planned, untouched. New orders are placed against Rev B. This is why the routing is copied into the order at creation time: from that moment the order is fully decoupled from the article template.

---

### Urgent / rush order

**Situation:** A high-priority customer calls and needs 10 pieces by end of week. No quote, but they have ordered this part many times before.

**System response:** A Production Order is created directly from the existing article template, bypassing the quote flow. The scheduler replans all **queued** (not yet started) jobs to accommodate the new priority. Jobs currently **in progress** on a machine cannot be moved. The ERP must distinguish between these two states when recalculating the planning.

---

### Assembly — parallel pipelines with dependencies

**Situation:** A finished product consists of three individually machined sub-components that are bolted together at the end.

**System response:** Each sub-component is its own Production Order with its own routing pipeline, running in parallel. The assembly Production Order carries a `dependsOn` all three — it cannot start until all sub-components are complete. The full structure is a **DAG (Directed Acyclic Graph)**: parallel sub-pipelines converging at a join step. The routing designer must support this — small, independently configurable pipelines that compose into larger ones. The mental model is Azure Data Factory or Azure DevOps pipeline dependencies.

---

### Conditional pipeline step

**Situation:** Some customers require a certified measurement report at the end of production; others do not.

**System response:** The routing template must support optional/conditional steps — stages that are active only when a specific condition is set on the order or customer record. This is a routing designer detail to be worked out during the Orders domain deep-dive.

## Confirmed decisions

- **Geleidekaart** → digital shop floor document in the new ERP. Operators book operations as done on screen. The Orders domain will need a routing/operation-booking screen per order.
- **Optimal machine scheduling** → a core USP of the new ERP. The system should automatically assign jobs to the best available machine based on capability and availability, rather than relying on manual planning. This is a scheduling/optimisation problem to be designed in detail during the Orders domain phase.
- **Quote domain (bare-bones)** → implemented without a material catalog. Material fields on quote lines are free-text/denormalized for now. Evaluate introducing a proper material catalog (Soort → Werkstoffnummer → Geometrie) in a later iteration.
- **Quote line article linkage** → quote lines have an optional `article_id`. Conversion to production orders requires all accepted lines to have an article linked. Evaluate whether to enforce article selection earlier (at line creation) rather than at conversion time.
- **Copy-from-order** → repeat orders without a quote are deferred to a separate branch. Evaluate priority after the quote domain is in production.

## Open questions (to clarify before building Orders domain)

- [ ] What industries / customer sectors are most important? (hydraulics, automotive, energy, defence?) — unknown, to be determined from customer data
- [ ] Is Halfvoorraad stock managed actively, or is it incidental?
- [ ] Does Eigendomklant material/tooling need its own management screen?
- [ ] Is DNC (machine program transfer) in scope for the new ERP, or out of scope?
- [ ] What replaces the Chargenummer / Certificaatbinnen traceability in the new system?
- [ ] Are credit notes (Crediteren) a separate document type or an attribute on the invoice?
- [ ] What is the 4th navigation tab (after Planning)? Label unclear from screenshot — possibly Uitbestedingen (subcontracting)?
- [ ] What does "Vorderingsblad" represent? Progress report or cost statement?
- [ ] What does "Royali" mean in the operations grid — royalties, or a different term?
- [ ] What does "Geacc." (accepted) on a quote line item actually trigger? — believed to enable quote→order conversion when all lines are accepted; bare-bones "Convert to Order" will be implemented first
- [ ] Quote statuses — exact states not confirmed; Draft / Sent / Accepted / Rejected assumed as starting point
- [ ] What is "Vorderingsblad" on the order detail screen — a progress report, a cost statement, or something else?
- [ ] Evaluate mandatory article linkage on quote lines — should article selection be required at line creation rather than only enforced at conversion?
- [ ] Copy-from-order (repeat order without a quote) — deferred to separate branch; design and prioritise separately.
- [ ] Material catalog — introduce proper Soort → Werkstoffnummer (Code/Code2/Code3) → Geometrie reference tables to replace free-text material fields on quote lines.

---

## UI Screens

This section documents screen-level observations from the legacy WinForms application, derived from screenshots. Complements the database-level analysis above.

### Module navigation

The top tab bar reveals the full module set:

| Tab | Purpose |
| --- | ------- |
| Orders | Production order management — the primary daily-use screen |
| Offerte | Quote creation and management |
| Planning | Scheduling and capacity planning |
| Uitleveren | Delivery / dispatch management |
| Facturatie | Invoicing |
| Rapporten | Reports |
| Instellingen | Settings and configuration |
| Zoeken | Search |
| Tekenning | Technical drawings and documentation |

### Orders — Production Order detail screen

The main daily screen. Three vertical zones.

**Header zone**

| Field | Dutch label | Notes |
| ----- | ----------- | ----- |
| Customer name | Klant naam | Dropdown, linked to customer master |
| Internal order number | Order nummer | Sequential, e.g. 015026 |
| Sub-position | Positie | Identifies a sub-part within a multi-line order |
| Customer PO reference | Bestelnummer klant | The customer's own order number |
| Production number | Productienummer | Part tracking number, e.g. CSS/17/001 |
| Order date | Order datum | |
| Reference | Referentienummer | |
| Delivery done | Bezorgen gedaan | Checkbox |

Status flags (checkboxes on the header): Maatrapport, Garanties, Creditnota, Certificaat, Tracking, On-Hold, Annuleer, Orderbest., Leverwijze

Prominent status badge: **Niet gefactureerd** (Not invoiced) — flips to Gefactureerd once invoiced. This confirms invoicing state is a first-class visual concept, not just a DB flag.

**Material zone**

| Field | Notes |
| ----- | ----- |
| Soort | Material type |
| Geo | Geometry (round, tube, plate, etc.) |
| Bron | Source — Inclusief (shop supplies) or Klant (customer supplies) |
| Mat.art.nr | Material article number |
| Datum geleverd | Date material was delivered to shop |
| Raming | Estimated/budgeted material cost |

**Operations grid (Bewerkingen)**

Each row is one pipeline step for this order. Columns:

| Column | Dutch | Meaning |
| ------ | ----- | ------- |
| VLG | Volgorde | Sequence number — defines pipeline order |
| NR | Nummer | Operation ID / number |
| Bewerking | — | Operation name |
| Mach. | Machine | Machine number |
| Duur | — | Duration (time estimate) |
| Week | — | Planned production week |
| Instel. | Insteltijd | Setup time estimate |
| Cyclus | — | Cycle time estimate (per piece) |
| Prijs | — | Price for this operation |
| Opmerking | — | Comment |
| Gedaan | — | Done — checkbox booked by operator when operation is complete |
| N Inst | Aantal instellingen | Number of actual setups performed |
| N Cycl | Aantal cycli | Number of actual cycles run |
| Werkn. | Werknemer | Employee / operator who performed the operation |
| Uitb. nr | Uitbesteding nummer | Subcontracting order number |
| Uitbest. | Uitbesteding | Outsourced flag |
| Besteld | — | Subcontracting or material ordered flag |

Example rows visible in screenshot: Materiaaluitgifte → Afbramen → Eindcontrole.

Bottom actions: Opslaan, Annuleren, Wijzigen, Nieuwe order, Nieuwe positie, Verplaatsen + navigation `<< < > >>` Zoeken.

**Implication for new ERP:** Setup time and cycle time are tracked as **separate fields**, not a single duration. Actual execution is tracked via N Inst and N Cycl counts, not a single "actual minutes" field. The `OrderOperation` data model should reflect this split:

```
OrderOperation {
    SequenceNumber      // VLG
    OperationId         // Bewerking
    MachineId           // Mach.
    PlannedWeek         // Week
    EstimatedSetupTime  // Instel.
    EstimatedCycleTime  // Cyclus
    Price               // Prijs
    ActualSetups        // N Inst — booked by operator
    ActualCycles        // N Cycl — booked by operator
    CompletedBy         // Werkn. (Werknemer)
    IsOutsourced        // Uitbest.
    OutsourcingOrderNr  // Uitb. nr
    IsOrdered           // Besteld
    IsDone              // Gedaan
}
```

**"Nieuwe positie"** confirms that one Order in the legacy system can have multiple positions (sub-parts). This maps to the Sales Order → multiple Production Orders relationship described in the Key data relationships section.

### Relaties — Klanten (Customer master) screen

Covered by the Parties domain. Fields map as follows:

| Legacy field | Maps to |
| ------------ | ------- |
| Nummer | `customer_roles.customer_number` |
| Actief | `parties.is_active` |
| Debiteurnummer | `customer_roles` seq_debtor_number |
| Aflever adres | `party_addresses` type 2 (Delivery) |
| Post adres | `party_addresses` type 1 (Postal) |
| Telefoon / Fax | `party_contact_methods` |
| Contact + E-mail grid | `party_contact_methods` (partially) |

**Gaps not currently in the new ERP:**

- **Zoek code** — a short search/abbreviation code separate from the name, used for fast keyboard lookup. The new ERP relies on Meilisearch full-text search instead, which likely covers this need.
- **Korting (%)** — a default discount percentage per customer, applied when quoting. Not in the Parties domain; belongs on the customer role or the quote header.
- **Btw** (BTW = VAT) — a per-customer VAT flag indicating whether VAT applies to invoices. Not currently stored.
- **Contacts grid** — the legacy screen shows a list of contact persons (name + email) per customer, distinct from the party's own contact methods. The new ERP's `party_contact_methods` tracks communication channels (phone/email/mobile) on the party itself, not named individual contacts at that company.
- **"Post adres = afleveradres?"** checkbox — a convenience flag that copies the delivery address to postal. UI convenience, no schema impact.
- **Outlook Contact** button — Outlook sync integration. Out of scope for new ERP.

### Offerte (Quote) dialog

**Header**

| Section | Fields |
| ------- | ------ |
| Klant | Klant naam (dropdown), Klant nummer, Adres, Postcode, Plaats, Contact (with Dhr./Mevr. prefix), Referentie, Levertijd (dropdown) |
| Order | Offerte nummer (sequential, e.g. 15766), Datum |
| Variabelen | Uur tarief, Materiaal marge, Standaard marge, Insteltijd |

Margin fields are entered as **whole-number percentages** in the UI (`115` = 115% = 1.15× multiplier, `11` = 11% markup). The new ERP form should match this convention so users are not confused.

A **Klantenbeheer** (Customer Management) button opens the customer master from within the quote dialog — inline navigation without leaving context.

**Line items grid**

| Column | Dutch | Notes |
| ------ | ----- | ----- |
| Part name | Onderdeelnaam | |
| Part number | Onderdeelnummer | |
| Quantity | Aantal | |
| Material | Materiaal | Opens the Materiaal dialog |
| Material price | Mat. prijs | Calculated from Code + Geo + Maat |
| Source | Bron | Inclusief or Klant |
| Number of operations | Aantal bew. | |
| Operation time | Bewerkingstijd | Hours per piece |
| Subcontracting count | Buitenbew. | Number of subcontracted steps |
| Subcontracting price | Bui. prijs | |
| Total price per unit | Totale prijs p/st € | Calculated field |
| Manual price | Handprijs | Checkbox — enables a fixed override price instead of the calculated result |
| Accepted | Geacc. | Checkbox per line — when all lines accepted, triggers quote→order conversion |
| Comment | Opmerking | |

Bottom actions: Wijzigen, Opslaan, Annuleren, Nieuw, Verwijderen, Update prijzen + export Word / E-mail / PDF / Print + navigation `<< < > >>` Zoeken.

The navigation buttons confirm this dialog handles **both** creating new quotes and browsing/editing the full quote history.

### Offerte — filled line item

A complete line item for "Sigaret" (titanium, 20 pieces) shows:

| Column | Value | Notes |
| ------ | ----- | ----- |
| Onderdeelnaam | Sigaret | Part name |
| Onderdeelnummer | 356-001 | Part number |
| Aantal | 20 | Quantity |
| Materiaal | Titaan 20 Stuks | Displayed as: type + quantity + geometry |
| Mat. prijs | 26 | Material price (per piece or per kg — TBD) |
| Bron | Inclusief | Shop supplies the material |
| Aantal bew. | 2 | Number of operations |
| Bewerkingstijd | 30 | Operation time (unit TBD — minutes or hours) |
| Buitenbew. | 5 | Number of subcontracted steps |
| Bui. prijs | 0,00 | Subcontracting price |
| Totale prijs p/st € | 33.365,20 | Calculated total price per unit |
| Handprijs | ☐ | Not overridden — price is calculated |
| Geacc. | Ja | Displays "Ja" as text when accepted; empty rows show a checkbox |

**Geacc. behavior clarified:** The field shows "Ja" as text on saved/accepted rows and a checkbox on new empty rows. Not a simple boolean display — likely set explicitly by the user and stored as a flag.

**Pricing calculation:** Bewerkingstijd is in **minutes**. With €72/hr rate, 115% material margin, 11% standard margin, and 1 hr setup — the full formula is not yet reverse-engineered from this single example, but minutes is the confirmed unit.

**Opmerking (remarks) field:** The white empty area below the line items grid is a free-text remarks field for the entire quote — not a totals summary.

**Quantity breaks:** There is no sub-dialog for multiple quantity options on a single line. Quantity breaks are expressed as **separate rows** — e.g. 50 pcs of part X on row 1, 75 pcs of the same part on row 2, 100 pcs on row 3. Each row is fully independent with its own material, pricing, and operations. This means a quote for one part with three quantity options has three line items.

### Offerte — Materiaal selection dialog

Opened by clicking the Materiaal cell on a quote line item. **Three-panel linked lookup** — not free text.

**Panel 1 — Soort (material type)**

| Option | Notes |
| ------ | ----- |
| Gereed product | Finished/bought-out item |
| Koolstofstaal | Carbon steel |
| RVS | Stainless steel |
| Non-Ferro | Non-ferrous metals |
| Aluminium | |
| Kunststof | Plastic |
| Diversen | Miscellaneous |
| *(scroll)* | Gereedschapstaal, Gietstuk/deel, Titanium, Faktuur |

**Panel 2 — Code (material grade, filtered by Soort)**

Selecting a Soort populates a code list with up to three columns:

| Column | Example | Meaning |
| ------ | ------- | ------- |
| Code | 1.0425 | EN Werkstoffnummer |
| Code2 | C-STAAL | Common/trade name |
| Code3 | — | Third standard designation (e.g. AISI/ASTM) |

Examples for Koolstofstaal: 1.0425 (C-STAAL), 1.0566 (TStE.355), 1.0715 (11SMn30+C), 1.0718, 1.2162 (21MnCr5), 1.5752 (ECN35).

Selecting a code auto-fills the Code field in the Materiaal section below.

**Panel 3 — Geometrie (shape, filtered by Soort)**

| Visible option | Code | Notes |
| -------------- | ---- | ----- |
| Bestaand | Bestaand | Existing part — no new raw material |
| Gietdeel/gietstuk | Gietdeel/stu | Casting |
| Samenstelling | Sam | Assembly |
| Faktuur | — | Invoice item |
| Diversen | — | Miscellaneous |
| Rond | Rnd | Round bar |
| *(scroll)* | — | Buis, Vierkant, Zesk, Plaat, Plat, Snij from DB analysis |

Selecting a geometry auto-fills the Geo field in the Materiaal section below.

**Materiaal section (manual entry after selections)**

| Field | Dutch | Notes |
| ----- | ----- | ----- |
| Grade / code | Code | Auto-filled from panel 2, editable |
| Geometry | Geo | Auto-filled from panel 3, editable |
| Cross-section | Maat (mm) | Diameter or width — manual entry |
| Length | Lengte (mm) | Bar or piece length — manual entry |
| Quantity | Aantal | Number of material pieces needed — manual entry |

**Implication for new ERP:** Material selection requires a **material catalog** (Soort → Code/grade lookup → Geometry options). This is a reference data domain, not a free-text field. The catalog needs Werkstoffnummer + at least two alternative name columns.

