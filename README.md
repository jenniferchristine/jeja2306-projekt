# Projektarbete
> Av: Jennifer Jakobsson, jeja2306@student.miun.se

## Projektarbete DT071G

### 💤 SleepApp 💤
SleepApp är en interaktiv applikation som hjälper dig att kartlägga dina sömnvanor. Du svarar på fem snabba frågor för en uppskattning av din sömnkvalitet samt tips för att förbättra rutiner.
<br>
<br>

#### Användning av appen:
- Starta programmet
- Tryck Enter för att påbörja nytt test, Y för att visa tidigare resultat eller X för att avsluta programmet.
- Svara på 5st frågor genom att skriva siffran för det alternativ som passar dig bäst och Enter.
- Följ instruktioner under programmets gång för att avsluta programmet.
- Ditt resultat visas med en färgkodad indikator och tips för att förbättra din sömn.
- Dagens testresultat sparas i sleepRecord.json
<br>
<br>

#### Funktioner
📊 Sömnregistrering - Du svarar på frågor kring sömntimmar, stress, koffeinintag och aktivitet.
<br>
🗓️ Historik - Se datim och resultat för tidigare test.
<br>
⚠️ Varningssystem - Om ett test redan registrerats för idag får du en varning innan nytt test sparas.
<br>
🤖 Maskininlärning - Prediktion av sömnkvalitet baserat på dina svar.
<br>
🌙 Tips och rekommendationer - Feedback (poor, average, good) baserat på nivå av sömnvanor.
<br>
📝 Resultat - Se poäng per fråga samt totalpoäng med nivåindikation.
<br>
<br>

#### Teknisk information
- Skriven i C# med .Net
- Använder ML.NET för att prediktera sömnkvalitet
- Konsolbaserad för att köras lokalt
- Använder JSON för lagring av testresultat