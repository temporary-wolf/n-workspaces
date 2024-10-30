# Norion kodtest
Jag har skrivit detta dokument som beskriver vad jag gjort samt skapat en ny fil
`source/TollCalculatorsNewTollCalculator.cs` som implementerar dessa ändringar.

# Brister i koden
Jag har en personlig filosofi att man ska vara lite försiktig att skriva om kod som fungerar, då det ofta finns mer produktiva saker att lägga tiden på.
Med det sagt så finns det en hel del brister och antaganden i denna kod som kan ge upphovtill buggar som skulle kosta mycket till att hitta och ordna till.

Om jag skulle skriva om koden så som jag skulle uppskatta den så skulle jag ändra på nedanstående saker.

# Saker jag vill förbättra

- Problem: Antagande att datumen som går in i GetTollFee är i svensk lokal tid.
  Förbättring: Asserta att datumen är i svensk tid och kasta exception annars.

- Problem: Antagande att datumen som går in i GetTollFee är för en och samma dag.
  Förbättring: Asserta att datumen hör till samma dag och kasta exception annars.

- Problem: GetTollFee har en overload med omvänd ordning på argumenten och de gör olika saker. Extremt ondskefullt kodande.
  Lösning: Skriv inga onda overloads

- Problem: Classes med en funktion som bara returnerar en string som egentligen bör vara en enum.
  Lösning: Använd inte classer utan använd bara enumen direkt.

- Problem: Konstig ToString-Cast av enums istället för att använda enum-jämförelse.
  Lösning: Jämför enums mot varandra.

- Problem: Onödigt komplicerad business-logik för GetTollFee som också inte funkar som jag tycker den ska göra. See tolkning nedan för min tolkning.
  Lösning: Använd tidsfönster-algoritm som definierat nedan

- Problem: Komplicerade if else statements i IsTollFreeDate där det inte är uppenbart vad de olika datumen representerar och som också går out of date
- Lösning: Använd ett bibliotek som har koll på svenska helgdagar


## Uppställning av tollalgoritm problem
- Representera datan som en sorterad lista av passager genom betalstationer.
  Sorterad efter datum av passage i ökande ordning
- Gå igenom listan - den första passage som har kostnad > 0 startar ett fönster på en timme. Kostnaden för detta fönster är max(window(passager)).
 Nästa icke gratis passage efter detta fönster startar nästa fönster.
- Summera kostnaden för varje fönster per dag och sätt ett tak vid 60 kr.


## Tolkning
Det går att tolka tullavgifts-algoritmen på lite olika sätt, men när jag läser
"En bil som passerar flera betalstationer inom 60 minuter bara beskattas en gång.". 

Så tolkar jag detta som att det går att starta flera 60-minuters fönster per dag.
Men när jag läser koden som beräknar tullavgiften så verkar det som att detta bara gällerfrån det första fönstret på dagen. Jag hade hellre tolkat det såhär:

Om en förare inte startat ett passage-fönster de senaste 60 minuterna så kommer nästa passag starta ett 60-minuters fönster där föraren får köra igenom godtyckligt många passager men bara betala för den dyraste.

Explicit står det att alla passager startar ett fönster, men jag tycker en bättre tolkning är att vara maximalt snäll för bilföraren så passager som är gratis inte startar ett nytt 60-minuters fönster utan bara ignoreras.

Jag väljer att tolka algoritmen på det snällare sättet med flera fönster
