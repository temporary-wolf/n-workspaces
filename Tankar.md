# Norion kodtest

## Tolkning
- Det finns två sätt att tolka 
  "En bil som passerar flera betalstationer inom 60 minuter bara beskattas en gång.". 
Jag tolkar detta som att det går att starta flera fönster 60-minuters fönster per dag.
Men när jag läser koden som beräknar tullavgiften så verkar det som att detta bara gällerfrån det första fönstret på dagen. Jag hade hellre tolkat det såhär:

Om en förare inte startat ett passage-fönster de senaste 60 minuterna så kommer nästa passag starta ett 60-minuters fönster där föraren får köra igenom godtyckligt många passager men bara betala för den dyraste.

Explicit står det att alla passager startar ett fönster, men jag tycker en bättre tolkning är att vara maximalt snäll för bilföraren så passager som är gratis inte startar ett nytt 60-minuters fönster utan bara ignoreras.

Jag väljer att tolka algoritmen på detta sätt.

## Brister i koden
Jag har en personlig filosofi att man ska vara lite försiktig att skriva om kod som fungerar, då det ofta finns mer produktiva saker att lägga tiden på.
Med det sagt så finns det en hel del brister och antaganden i denna kod som kan ge upphovtill buggar som skulle kosta mycket till att hitta och ordna till.

### De jag uppfattar som störst är

- Antagande att datumen som går in i GetTollFee är i svensk lokal tid.
- Antagande att datumen som går in i GetTollFee är för en och samma dag.
- GetTollFee har en overload med omvänd ordning på argumenten och de gör olika saker. Extremt ondskefullt kodande.
- Classes med en funktion som bara returnerar en string som egentligen bör vara en enum.
- Göra om Enums till strängar istället för att jämföra mot enum-värden.
- Onödigt komplicerad business-logik för GetTollFee
- Komplicerade if else statements i IsTollFreeDate där det inte är uppenbart vad de olika datumen representerar
- Hårdkodade datum i IsTollFreeDate

### Nitpicks som inte inte påverkar på ett funktionellt plan
- TollCalculator har inget state och kan vara en statisk class


## Uppställning av problem
- Representera datan som en sorterad lista av passager genom betalstationer.
  Sorterad efter datum av passage i ökande ordning
- Gå igenom listan - den första passage som har kostnad > 0 startar ett fönster på en timme. Kostnaden för detta fönster är max(window(passager)).
 Nästa icke gratis passage efter detta fönster startar nästa fönster.
- Summera kostnaden för varje fönster per dag och sätt ett tak vid 60 kr.

- Det bör vara uppenbart vilken tidszon datumen är i.
Nu används datetime vilket innebär att sommartid inte räknas in... Detta kan bli mycket fel.

Vilket datum kommer kanske som UTC men bör checkas i svensk lokal tid.
- För att ta reda på kostnaden för en viss passage kan vi kolla 2 saker
1. Specifika dagar - Helger, dagar i Juli etc är undantagna
2. Tid på dygnet

