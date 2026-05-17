# Gundam Test Case — RX-78-2 Manufacturing Plan

## Purpose

This test case models the complete manufacturing plan for an RX-78-2 Gundam as a deterministic seed dataset. It serves two goals:

1. **Functional coverage** — exercises multilevel BOM explosion, recursive assembly trees, subcontracted operations, shared components (left/right mirroring), and the full quote-to-production-order conversion flow.
2. **Performance boundary testing** — a single finished product with 4 levels of nesting, ~40 articles, ~150 operations, and stock movement chains that span the entire BOM tree.

The "Factorio recipe" mental model applies throughout: each article has a recipe (BOM) and a routing (operations). Producing the top-level Gundam requires producing all sub-assemblies first, each of which consumes components from stock.

---

## Article Tree Overview

```
Level 0 — Finished product
  GND-0001  RX-78-2 Gundam

Level 1 — Major assemblies (10)
  GND-A001  Head Assembly
  GND-A002  Torso Assembly
  GND-A003  Right Arm Assembly
  GND-A004  Left Arm Assembly          ← mirrors right arm BOM
  GND-A005  Right Leg Assembly
  GND-A006  Left Leg Assembly          ← mirrors right leg BOM
  GND-A007  Backpack Unit
  GND-A008  Shield
  GND-A009  Beam Rifle
  GND-A010  Beam Saber

Level 2 — Sub-components (31)
  Head:      GND-C001 … GND-C005
  Torso:     GND-C006 … GND-C009
  Arms:      GND-C010 … GND-C015      ← shared by left and right arm
  Legs:      GND-C016 … GND-C020      ← shared by left and right leg
  Backpack:  GND-C021 … GND-C024
  Weapons:   GND-C025 … GND-C031

Level 3 — Raw materials (10)
  GND-RM-001 … GND-RM-010
```

**Key design choices:**

- Left and right arm assemblies reference the _same_ sub-component articles in their BOMs (e.g. `GND-C014 Elbow Joint` appears as a child of both `GND-A003` and `GND-A004`). This tests that the `UNIQUE(parent_article_id, child_article_id)` constraint works per parent — not globally.
- Same principle applies to legs.
- The backpack has `Vernier Thruster ×6` and `Fuel Tank ×2`, testing non-unit quantities in BOM.
- The top-level Gundam includes `Beam Saber ×2`, also testing qty > 1 at the top level.

---

## Raw Materials

| Code       | Name                                     | Category      | Unit | Purchase Price |
| ---------- | ---------------------------------------- | ------------- | ---- | -------------: |
| GND-RM-001 | Luna Titanium Alloy Bar ø50×500mm        | Titaan        | st   |        €485.00 |
| GND-RM-002 | Luna Titanium Alloy Sheet 4×1000×2000mm  | Titaan        | st   |      €1,240.00 |
| GND-RM-003 | Gundarium-G Composite Plate 6×500×1000mm | Koolstofstaal | st   |      €2,750.00 |
| GND-RM-004 | Aluminium 7075-T6 Bar ø80×500mm          | Aluminium     | st   |         €95.00 |
| GND-RM-005 | Aluminium 7075-T6 Sheet 3×1000×2000mm    | Aluminium     | st   |        €320.00 |
| GND-RM-006 | Inconel 718 Bar ø60×300mm                | Titaan        | st   |        €890.00 |
| GND-RM-007 | Bearing Steel 100Cr6 Bar ø40×200mm       | Koolstofstaal | st   |         €48.00 |
| GND-RM-008 | Optical Polycarbonate Sheet 10×300×400mm | Kunststof     | st   |        €125.00 |
| GND-RM-009 | S355J2 Steel Plate 8×500×1000mm          | Koolstofstaal | st   |         €85.00 |
| GND-RM-010 | Stainless Steel 316L Bar ø30×300mm       | RVS           | st   |         €62.00 |

---

## Sub-Components — Head

### GND-C001 — Helmet Outer Shell

5-axis milled titanium shell with compound curvature. One of the most complex milled parts in the build.

**BOM:**
| Child | Description | Qty |
|-------------|--------------------------------|----:|
| GND-RM-002 | Luna Titanium Alloy Sheet | 1 |
| GND-RM-003 | Gundarium-G Composite Plate | 0.5 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------|-----:|----------------------------------|
| 10 | Sawing | 10 | Cut Ti sheet to blank size |
| 20 | CNC Milling | 90 | Rough profile — 5-axis |
| 30 | CNC Milling | 120 | Finish contours and interfaces |
| 40 | Deburring | 15 | |
| 50 | Surface treatment | — | Anodize + primer coat (sub) |

---

### GND-C002 — Visor Unit

Optically clear polycarbonate panel in a ground titanium frame.

**BOM:**
| Child | Description | Qty |
|-------------|--------------------------------|----:|
| GND-RM-008 | Optical Polycarbonate Sheet | 1 |
| GND-RM-010 | Stainless Steel 316L Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|----------------|----:|--------------------------------|
| 10 | CNC Milling | 60 | Mill polycarbonate to profile |
| 20 | CNC Milling | 45 | Mill titanium frame |
| 30 | Grinding | 20 | Grind frame seating surfaces |
| 40 | Final finishing | 20 | Polish visor, assemble frame |

---

### GND-C003 — Vulcan Gun Barrel

Precision-bored barrel; honed bore for ballistic accuracy. Used ×2 in Head Assembly.

**BOM:**
| Child | Description | Qty |
|-------------|---------------------------|----:|
| GND-RM-007 | Bearing Steel 100Cr6 Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------|----:|------------------------------|
| 10 | Sawing | 5 | |
| 20 | CNC Turning | 40 | Rough OD and bore |
| 30 | CNC Turning | 35 | Finish bore and profile |
| 40 | Honing | 25 | Hone bore to Ra 0.4 |
| 50 | Marking | 5 | Engrave part number |

---

### GND-C004 — Neck Joint

Multi-axis pivot joint; ground to h5 tolerance on pivot journals.

**BOM:**
| Child | Description | Qty |
|-------------|---------------------------|----:|
| GND-RM-001 | Luna Titanium Alloy Bar | 1 |
| GND-RM-007 | Bearing Steel 100Cr6 Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|--------------------------|----:|------------------------------------|
| 10 | Sawing | 8 | |
| 20 | CNC Turning | 55 | Rough all diameters |
| 30 | CNC Turning | 45 | Finish pivot diameters to h6 |
| 40 | Grinding | 35 | Cylindrical grind journals to h5 |
| 50 | Intermediate inspection | 15 | CMM pivot runout < 0.005mm |

---

### GND-C005 — Sensor Array Housing

Precision housing for mono-eye sensor system. Includes subcontracted measurement certification.

**BOM:**
| Child | Description | Qty |
|-------------|------------------------------|----:|
| GND-RM-010 | Stainless Steel 316L Bar | 1 |
| GND-RM-008 | Optical Polycarbonate Sheet | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|--------------------|----:|--------------------------------|
| 10 | CNC Turning | 35 | Turn housing OD and bore |
| 20 | CNC Milling | 40 | Mill lens mounting slots |
| 30 | Assembly | 20 | Press-fit polycarbonate cover |
| 40 | Measurement report | — | Dimensional certification (sub)|

---

## Sub-Components — Torso

### GND-C006 — Core Fighter Frame

Welded structural backbone. Stress-relieved and datum-milled after welding — tests heat treatment subcontract in the middle of a routing.

**BOM:**
| Child | Description | Qty |
|-------------|------------------------------|----:|
| GND-RM-009 | S355J2 Steel Plate | 2 |
| GND-RM-003 | Gundarium-G Composite Plate | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|------------------|----:|------------------------------------|
| 10 | Sawing | 15 | Cut S355 plate sections |
| 20 | CNC Milling | 90 | Mill weld prep and pockets |
| 30 | Welding | 80 | MIG weld structural assembly |
| 40 | Heat treatment | — | Stress relief 600°C (sub) |
| 50 | CNC Milling | 60 | Datum mill after stress relief |
| 60 | Final inspection | 20 | Dimensional check all interfaces |

---

### GND-C007 — Chest Armor Panel

Gundarium composite armor panel. Used ×4 in Torso Assembly — tests BOM quantity > 1 at level 2.

**BOM:**
| Child | Description | Qty |
|-------------|------------------------------|----:|
| GND-RM-003 | Gundarium-G Composite Plate | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------|----:|-------------------------|
| 10 | CNC Milling | 75 | Profile mill plate |
| 20 | CNC Milling | 45 | Finish mounting boss |
| 30 | Deburring | 10 | |
| 40 | Surface treatment | — | Primer + topcoat (sub) |

---

### GND-C008 — Cockpit Unit

Pilot capsule with integrated polycarbonate hatch.

**BOM:**
| Child | Description | Qty |
|-------------|------------------------------|----:|
| GND-RM-004 | Aluminium 7075-T6 Bar | 2 |
| GND-RM-008 | Optical Polycarbonate Sheet | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------------|----:|------------------------------|
| 10 | CNC Milling | 70 | Mill main capsule body |
| 20 | CNC Milling | 35 | Mill hatch recess and seal |
| 30 | Assembly | 40 | Fit polycarbonate hatch |
| 40 | Intermediate inspection | 15 | |

---

### GND-C009 — Power Reactor Housing

Inconel/SS housing for minovsky fusion reactor. CMM verification after grinding.

**BOM:**
| Child | Description | Qty |
|-------------|----------------------------|----:|
| GND-RM-006 | Inconel 718 Bar | 1 |
| GND-RM-010 | Stainless Steel 316L Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|--------------|----:|---------------------------------|
| 10 | Sawing | 10 | |
| 20 | CNC Turning | 60 | Rough bore Inconel shell |
| 30 | CNC Milling | 55 | Mill flange faces and bolt patt |
| 40 | Grinding | 30 | Grind flange sealing faces |
| 50 | 3D Measuring | 25 | CMM all critical dimensions |

---

## Sub-Components — Arms (shared left/right)

### GND-C010 — Upper Arm Segment

**BOM:**
| Child | Description | Qty |
|-------------|------------------------------|----:|
| GND-RM-005 | Aluminium 7075-T6 Sheet | 1 |
| GND-RM-003 | Gundarium-G Composite Plate | 0.5 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------|----:|------------------------------------|
| 10 | CNC Milling | 80 | Mill main structural shell |
| 20 | CNC Milling | 60 | Mill Gundarium overlay pockets |
| 30 | Deburring | 12 | |
| 40 | Surface treatment | — | Anodize (sub) |

---

### GND-C011 — Forearm Segment

**BOM:**
| Child | Description | Qty |
|-------------|--------------------------|----:|
| GND-RM-005 | Aluminium 7075-T6 Sheet | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|--------------|----:|-----------------------------------|
| 10 | CNC Milling | 65 | Mill shell profile |
| 20 | CNC Milling | 50 | Mill cable routing channels |
| 30 | Deburring | 10 | |

---

### GND-C012 — Hand Unit

Five-digit manipulator with articulated bearing-steel knuckles.

**BOM:**
| Child | Description | Qty |
|-------------|---------------------------|----:|
| GND-RM-004 | Aluminium 7075-T6 Bar | 1 |
| GND-RM-007 | Bearing Steel 100Cr6 Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|----------------|----:|--------------------------------|
| 10 | CNC Milling | 55 | Mill palm body |
| 20 | CNC Turning | 35 | Turn finger knuckle pins |
| 30 | Assembly | 45 | Assemble finger linkages |
| 40 | Final finishing | 15 | |

---

### GND-C013 — Shoulder Armor

**BOM:**
| Child | Description | Qty |
|-------------|------------------------------|----:|
| GND-RM-003 | Gundarium-G Composite Plate | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------|----:|--------------|
| 10 | CNC Milling | 70 | Profile mill |
| 20 | Deburring | 10 | |
| 30 | Surface treatment | — | Topcoat (sub)|

---

### GND-C014 — Elbow Joint

Ground and hardened pivot pin set.

**BOM:**
| Child | Description | Qty |
|-------------|---------------------------|----:|
| GND-RM-007 | Bearing Steel 100Cr6 Bar | 2 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------------|----:|---------------------------|
| 10 | Sawing | 6 | |
| 20 | CNC Turning | 45 | Turn pivot pin and bushing|
| 30 | Grinding | 30 | Grind to h5 running fit |
| 40 | Intermediate inspection | 10 | |

---

### GND-C015 — Wrist Joint

**BOM:**
| Child | Description | Qty |
|-------------|--------------------------|----:|
| GND-RM-001 | Luna Titanium Alloy Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|--------------|----:|------------------------|
| 10 | Sawing | 6 | |
| 20 | CNC Turning | 40 | Turn yoke OD and bore |
| 30 | Grinding | 25 | Grind bearing seats P5 |

---

## Sub-Components — Legs (shared left/right)

### GND-C016 — Upper Leg Segment

Largest single milled part. Tests 100+ min operation time.

**BOM:**
| Child | Description | Qty |
|-------------|------------------------------|----:|
| GND-RM-005 | Aluminium 7075-T6 Sheet | 2 |
| GND-RM-003 | Gundarium-G Composite Plate | 0.5 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------|----:|------------------------------------|
| 10 | CNC Milling | 100 | Mill main tube — largest operation |
| 20 | CNC Milling | 70 | Mill overlay pockets, thruster boss|
| 30 | Deburring | 15 | |
| 40 | Surface treatment | — | Anodize (sub) |

---

### GND-C017 — Lower Leg Segment

**BOM:**
| Child | Description | Qty |
|-------------|--------------------------|----:|
| GND-RM-005 | Aluminium 7075-T6 Sheet | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|--------------|----:|------------------------------|
| 10 | CNC Milling | 80 | Mill shell profile |
| 20 | CNC Milling | 55 | Mill thruster mount bosses |
| 30 | Deburring | 12 | |

---

### GND-C018 — Foot Unit

**BOM:**
| Child | Description | Qty |
|-------------|--------------------------|----:|
| GND-RM-004 | Aluminium 7075-T6 Bar | 2 |
| GND-RM-009 | S355J2 Steel Plate | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|----------------|----:|-------------------------------|
| 10 | CNC Milling | 60 | Mill foot body |
| 20 | CNC Turning | 30 | Turn ankle mounting flange |
| 30 | Deburring | 10 | |
| 40 | Final finishing | 15 | |

---

### GND-C019 — Knee Joint

**BOM:**
| Child | Description | Qty |
|-------------|---------------------------|----:|
| GND-RM-007 | Bearing Steel 100Cr6 Bar | 2 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------------|----:|-------------------------|
| 10 | Sawing | 6 | |
| 20 | CNC Turning | 50 | Turn pivot components |
| 30 | Grinding | 35 | Grind to h5 |
| 40 | Intermediate inspection | 10 | |

---

### GND-C020 — Ankle Joint

3-axis yoke; CMM-verified after grinding.

**BOM:**
| Child | Description | Qty |
|-------------|---------------------------|----:|
| GND-RM-001 | Luna Titanium Alloy Bar | 1 |
| GND-RM-007 | Bearing Steel 100Cr6 Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|--------------|----:|-----------------------------|
| 10 | Sawing | 6 | |
| 20 | CNC Turning | 45 | Turn 3-axis yoke components |
| 30 | Grinding | 30 | Grind bearing seats |
| 40 | 3D Measuring | 20 | CMM verify all pivot axes |

---

## Sub-Components — Backpack

### GND-C021 — Thruster Nozzle

Inconel convergent-divergent nozzle. Stellite-coated throat. Used ×2 in Backpack Unit. Tests two subcontracted operations in one routing.

**BOM:**
| Child | Description | Qty |
|-------------|------------------|----:|
| GND-RM-006 | Inconel 718 Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|------------------------------|----:|------------------------------|
| 10 | Sawing | 8 | |
| 20 | CNC Turning | 70 | Rough convergent section |
| 30 | CNC Turning | 60 | Finish nozzle profile/throat |
| 40 | Grinding | 25 | Grind mounting flange |
| 50 | Stellite / carbide coating | — | Stellite throat coating (sub)|
| 60 | Final inspection | 20 | |

---

### GND-C022 — Fuel Tank

Welded titanium pressure vessel. Tests welding + hydrostatic inspection. Used ×2 in Backpack Unit.

**BOM:**
| Child | Description | Qty |
|-------------|-----------------------------------|----:|
| GND-RM-002 | Luna Titanium Alloy Sheet | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------------|----:|--------------------------------------|
| 10 | Sawing | 10 | Cut Ti sheet blanks |
| 20 | CNC Turning | 35 | Form end-cap profiles |
| 30 | Welding | 60 | TIG weld; full penetration required |
| 40 | Intermediate inspection | 20 | Hydrotest to 1.5× working pressure |

---

### GND-C023 — Vernier Thruster

Small attitude-control thruster. Used ×6 in Backpack Unit — tests high quantity in BOM.

**BOM:**
| Child | Description | Qty |
|-------------|--------------------------|----:|
| GND-RM-001 | Luna Titanium Alloy Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|--------------|----:|--------------------------|
| 10 | Sawing | 5 | |
| 20 | CNC Turning | 35 | Turn nozzle body |
| 30 | Grinding | 20 | Grind sealing seat |
| 40 | Marking | 5 | Engrave orientation mark |

---

### GND-C024 — Backpack Structural Frame

Welded S355 skeleton. Stress-relieved and datum-milled.

**BOM:**
| Child | Description | Qty |
|-------------|--------------------------|----:|
| GND-RM-009 | S355J2 Steel Plate | 2 |
| GND-RM-004 | Aluminium 7075-T6 Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|----------------|----:|----------------------------------|
| 10 | Sawing | 12 | Cut S355 sections |
| 20 | CNC Milling | 70 | Mill all weld prep and pockets |
| 30 | Welding | 75 | MIG weld structural frame |
| 40 | Heat treatment | — | Stress relief (sub) |
| 50 | CNC Milling | 45 | Datum mill all mounting faces |

---

## Sub-Components — Weapons

### GND-C025 — Shield Armor Panel

Gundarium composite impact plate. Used ×3 in Shield.

**BOM:**
| Child | Description | Qty |
|-------------|------------------------------|----:|
| GND-RM-003 | Gundarium-G Composite Plate | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------|----:|-------------------------|
| 10 | CNC Milling | 65 | Profile mill plate |
| 20 | Deburring | 10 | |
| 30 | Surface treatment | — | Anti-beam coating (sub) |

---

### GND-C026 — Shield Frame

Welded S355 skeleton with mounting hardpoints.

**BOM:**
| Child | Description | Qty |
|-------------|--------------------|----:|
| GND-RM-009 | S355J2 Steel Plate | 2 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|--------------|----:|----------------------------|
| 10 | Sawing | 10 | |
| 20 | CNC Milling | 55 | Mill mounting hardpoints |
| 30 | Welding | 50 | MIG weld frame skeleton |
| 40 | Deburring | 10 | |

---

### GND-C027 — Beam Rifle Body

Milled and turned receiver with broached E-CAP rail slot. Tests broaching operation.

**BOM:**
| Child | Description | Qty |
|-------------|---------------------------|----:|
| GND-RM-004 | Aluminium 7075-T6 Bar | 1 |
| GND-RM-007 | Bearing Steel 100Cr6 Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|------------------|----:|--------------------------|
| 10 | Sawing | 8 | |
| 20 | CNC Milling | 85 | Mill receiver profile |
| 30 | CNC Turning | 40 | Turn barrel bore and OD |
| 40 | Broaching | 15 | Broach E-CAP rail slot |
| 50 | Final inspection | 20 | |

---

### GND-C028 — Rifle Grip

**BOM:**
| Child | Description | Qty |
|-------------|-----------------------|----:|
| GND-RM-004 | Aluminium 7075-T6 Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-----------------|----:|------------------------|
| 10 | CNC Milling | 35 | Mill ergonomic profile |
| 20 | Final finishing | 15 | Knurl grip surface |

---

### GND-C029 — Trigger Mechanism

**BOM:**
| Child | Description | Qty |
|-------------|---------------------------|----:|
| GND-RM-010 | Stainless Steel 316L Bar | 1 |
| GND-RM-007 | Bearing Steel 100Cr6 Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|--------------|----:|-------------------------------------|
| 10 | CNC Milling | 40 | Mill trigger/sear from 316L |
| 20 | CNC Turning | 25 | Turn pivot pins from bearing steel |
| 30 | Assembly | 20 | Assemble and adjust pull weight |

---

### GND-C030 — Beam Saber Handle

Turned and surface-treated titanium grip. Tests PVD coating subcontract.

**BOM:**
| Child | Description | Qty |
|-------------|--------------------------|----:|
| GND-RM-001 | Luna Titanium Alloy Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------|----:|---------------------------------|
| 10 | Sawing | 6 | |
| 20 | CNC Turning | 45 | Turn grip body and emitter socket|
| 30 | Grinding | 20 | Grind emitter seat to H6 |
| 40 | Marking | 5 | Engrave UC markings |
| 50 | Surface treatment | — | PVD coating (sub) |

---

### GND-C031 — Saber Emitter

Inconel emitter tip with Stellite coating.

**BOM:**
| Child | Description | Qty |
|-------------|------------------|----:|
| GND-RM-006 | Inconel 718 Bar | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|----------------------------|----:|-------------------------------|
| 10 | Sawing | 6 | |
| 20 | CNC Turning | 50 | Rough Inconel emitter profile |
| 30 | CNC Turning | 40 | Finish emitter tip geometry |
| 40 | Stellite / carbide coating | — | Thermal resistance (sub) |

---

## Level-1 Assemblies

### GND-A001 — Head Assembly

**BOM:**
| Child | Description | Qty |
|-------------|----------------------|----:|
| GND-C001 | Helmet Outer Shell | 1 |
| GND-C002 | Visor Unit | 1 |
| GND-C003 | Vulcan Gun Barrel | 2 |
| GND-C004 | Neck Joint | 1 |
| GND-C005 | Sensor Array Housing | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------------|----:|-----------------------------------------|
| 10 | Assembly | 45 | Mount sensor, visor and neck to shell |
| 20 | Assembly | 30 | Install vulcan guns and harness |
| 30 | Intermediate inspection | 15 | |
| 40 | 3D Measuring | 25 | Verify eye/gun alignment |

---

### GND-A002 — Torso Assembly

**BOM:**
| Child | Description | Qty |
|-------------|----------------------|----:|
| GND-C006 | Core Fighter Frame | 1 |
| GND-C007 | Chest Armor Panel | 4 |
| GND-C008 | Cockpit Unit | 1 |
| GND-C009 | Power Reactor Housing| 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------------|----:|----------------------------------------|
| 10 | Assembly | 90 | Mount cockpit and reactor to frame |
| 20 | Assembly | 60 | Attach chest armor panels (×4) |
| 30 | Intermediate inspection | 20 | |
| 40 | 3D Measuring | 30 | CMM all limb attachment interfaces |

---

### GND-A003 — Right Arm Assembly

**BOM:**
| Child | Description | Qty |
|-------------|------------------|----:|
| GND-C010 | Upper Arm Segment| 1 |
| GND-C011 | Forearm Segment | 1 |
| GND-C012 | Hand Unit | 1 |
| GND-C013 | Shoulder Armor | 1 |
| GND-C014 | Elbow Joint | 1 |
| GND-C015 | Wrist Joint | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------------|----:|------------------------------------------|
| 10 | Assembly | 55 | Assemble shoulder armor, upper arm, elbow|
| 20 | Assembly | 45 | Attach forearm, wrist and hand |
| 30 | Intermediate inspection | 15 | Check range of motion all axes |

---

### GND-A004 — Left Arm Assembly

Identical BOM and routing to Right Arm Assembly. Tests the shared-component pattern.

**BOM:** same as GND-A003

**Operations:** same as GND-A003

---

### GND-A005 — Right Leg Assembly

**BOM:**
| Child | Description | Qty |
|-------------|-------------------|----:|
| GND-C016 | Upper Leg Segment | 1 |
| GND-C017 | Lower Leg Segment | 1 |
| GND-C018 | Foot Unit | 1 |
| GND-C019 | Knee Joint | 1 |
| GND-C020 | Ankle Joint | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------------|----:|------------------------------------|
| 10 | Assembly | 60 | Upper leg, knee joint, lower leg |
| 20 | Assembly | 40 | Ankle joint and foot unit |
| 30 | Intermediate inspection | 15 | Check range of motion, load bearing|

---

### GND-A006 — Left Leg Assembly

Identical BOM and routing to Right Leg Assembly.

**BOM:** same as GND-A005

**Operations:** same as GND-A005

---

### GND-A007 — Backpack Unit

**BOM:**
| Child | Description | Qty |
|-------------|--------------------------|----:|
| GND-C021 | Thruster Nozzle | 2 |
| GND-C022 | Fuel Tank | 2 |
| GND-C023 | Vernier Thruster | 6 |
| GND-C024 | Backpack Structural Frame| 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-------------------------|----:|--------------------------------------|
| 10 | Assembly | 50 | Mount nozzles and verniers to frame |
| 20 | Assembly | 35 | Install tanks and connect plumbing |
| 30 | Intermediate inspection | 15 | |
| 40 | Final inspection | 20 | Leak test all fuel connections |

---

### GND-A008 — Shield

**BOM:**
| Child | Description | Qty |
|-------------|--------------------|----:|
| GND-C025 | Shield Armor Panel | 3 |
| GND-C026 | Shield Frame | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-----------------|----:|-------------------------------|
| 10 | Assembly | 35 | Bond armor panels to frame |
| 20 | Final finishing | 15 | Edge seal and surface finish |
| 30 | Marking | 10 | Earth Federation markings |

---

### GND-A009 — Beam Rifle

**BOM:**
| Child | Description | Qty |
|-------------|-------------------|----:|
| GND-C027 | Beam Rifle Body | 1 |
| GND-C028 | Rifle Grip | 1 |
| GND-C029 | Trigger Mechanism | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|--------------------|----:|------------------------------------|
| 10 | Assembly | 40 | Install trigger and grip to body |
| 20 | Final inspection | 20 | |
| 30 | Measurement report | — | Function test certification (sub) |

---

### GND-A010 — Beam Saber

Used ×2 in top-level Gundam.

**BOM:**
| Child | Description | Qty |
|-------------|----------------|----:|
| GND-C030 | Saber Handle | 1 |
| GND-C031 | Saber Emitter | 1 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|-----------|----:|-------------------------------|
| 10 | Assembly | 20 | Press-fit emitter into handle |
| 20 | Marking | 10 | Serial number and UC marking |

---

## Top-Level — GND-0001 RX-78-2 Gundam

18m class general-purpose mobile suit. Full final assembly.

**BOM:**
| Child | Description | Qty |
|-------------|---------------------|----:|
| GND-A001 | Head Assembly | 1 |
| GND-A002 | Torso Assembly | 1 |
| GND-A003 | Right Arm Assembly | 1 |
| GND-A004 | Left Arm Assembly | 1 |
| GND-A005 | Right Leg Assembly | 1 |
| GND-A006 | Left Leg Assembly | 1 |
| GND-A007 | Backpack Unit | 1 |
| GND-A008 | Shield | 1 |
| GND-A009 | Beam Rifle | 1 |
| GND-A010 | Beam Saber | 2 |

**Operations:**
| Seq | Operation | Min | Notes |
|-----|--------------------|----:|------------------------------------------|
| 10 | Assembly | 120 | Mate torso to legs; structural fasteners |
| 20 | Assembly | 90 | Install arms and head; route harnesses |
| 30 | Assembly | 60 | Mount backpack and weapons systems |
| 40 | Final inspection | 45 | Full system dimensional check |
| 50 | 3D Measuring | 60 | CMM full-body at key datums |
| 60 | Measurement report | — | Certification and handover docs (sub) |

---

## Summary Statistics

| Category              |  Count |
| --------------------- | -----: |
| Raw materials         |     10 |
| Sub-components (L2)   |     31 |
| Major assemblies (L1) |     10 |
| Top-level product     |      1 |
| **Total articles**    | **52** |
| BOM lines             |    ~90 |
| Total operations      |   ~155 |
| Subcontracted ops     |     14 |
| BOM depth (levels)    |      4 |

---

## System Boundary Tests This Exercises

| Scenario                              | Where                                              |
| ------------------------------------- | -------------------------------------------------- |
| Multilevel BOM (4 levels deep)        | GND-0001 → GND-A001 → GND-C001 → GND-RM-002        |
| Shared components (left/right mirror) | GND-C010…C015 in both GND-A003 and GND-A004        |
| BOM qty > 1                           | Vulcan Gun ×2, Chest Armor ×4, Vernier ×6, etc.    |
| Top-level qty > 1                     | Beam Saber ×2 on GND-0001                          |
| Subcontract mid-routing               | GND-C006: Heat treatment between two milling ops   |
| Multiple subcontracts in one routing  | GND-C021: Stellite + final inspection              |
| Long operation (100+ min)             | GND-C016 op 10: CNC Milling 100 min                |
| CMM / 3D measuring ops                | GND-C004, GND-C009, GND-C020, GND-A001, GND-0001   |
| Welded pressure vessel                | GND-C022: weld + hydrotest sequence                |
| Stock movement chain depth            | Completing GND-0001 triggers movements 4 levels up |
| BOM explosion scale                   | 1 Gundam → 52 articles, ~90 BOM lines to resolve   |

