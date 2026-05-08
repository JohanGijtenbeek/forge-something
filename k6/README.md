# k6 Load Tests

## Volgorde van uitvoering

### 1. Smoke test — altijd eerst
Verifieert dat de API werkt voordat je zwaardere tests draait.
```powershell
k6 run tests/smoke.js
```

### 2. Load test — normale load
Simuleert 50 gelijktijdige gebruikers gedurende 4 minuten.
```powershell
k6 run tests/load.js
```

### 3. Stress test — breekpunt zoeken
Bouwt op naar 300 gelijktijdige gebruikers.
```powershell
k6 run tests/stress.js
```

## Andere omgeving

```powershell
k6 run tests/load.js -e BASE_URL=http://jouw-server:5272
```

## Output opslaan

```powershell
k6 run tests/load.js --out json=results/load-$(Get-Date -Format 'yyyyMMdd-HHmmss').json
```
