# Projektarbete
> Av: Jennifer Jakobsson, jeja2306@student.miun.se

## Projektarbete DT071G

### 💤 SleepApp 💤
SleepApp är en interaktiv applikation som hjälper dig att kartlägga dina sömnvanor. <br>Du svarar på fem snabba frågor för en uppskattning av din sömnkvalitet samt tips för att förbättra rutiner.
<br>
<br>

#### Användning av appen:
- Starta programmet
- Tryck Enter för att påbörja nytt test, R för att visa tidigare resultat eller X för att avsluta programmet.
- Svara på 5st frågor genom att skriva siffran för det alternativ som passar dig bäst och Enter.
- Följ instruktioner under programmets gång för att avsluta programmet.
- Ditt resultat visas med en färgkodad indikator och tips för att förbättra din sömn.
- Dagens testresultat sparas i sleepRecord.json

#### Funktioner
- 📊 Sömnregistrering - Du svarar på frågor kring sömntimmar, stress, koffeinintag och aktivitet.
- 🗓️ Historik - Se datum och resultat för tidigare test.
- 🗑️ Radera historik - Ta bort ett specifikt resultat.
- ⚠️ Varningssystem - Om ett test redan registrerats för idag får du en varning innan nytt test sparas.
- 🤖 Maskininlärning - Förutsägelse av sömnkvalitet baserat på dina svar.
- 🌙 Tips och rekommendationer - Feedback (poor, average, good) baserat på nivå av sömnvanor.
- 📝 Resultat - Se poäng per fråga samt totalpoäng med nivåindikation.

#### Teknisk information
- Skriven i C# med .Net
- Använder ML.NET för att förutsäga sömnkvalitet
- Konsolbaserad för att köras lokalt
- Använder JSON för lagring av testresultat