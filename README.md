Lista uwag

1 - Argument metody ImportAndPrintData zawierał błędną ścieżkę do pliku z danymi, zmieniłem ją na poprawną.

2 - Wprowadziłem instrukcję warunkową do pętli importującej linie danych z pliku .csv, tak aby pomijać puste linie gdyż powodowały one błąd IndexOutOfRange, lub niepotrzebnie zajmowały miejsce i czas operacji programu.

3 - W tym samym segmencie dodałem kolejną instrukcję warunkową która wypełnia puste pola rekordów wartością string będącą "", program mógł wziąć pod uwagę również te niepełne linie danych.

4 - Zmieniłem operator warunku kończącego tej samej pętli z <= na <, aby nie pobierać ostatnich pustych lini w kodzie, ponieważ wtedy powstawał error IndexOutOfRange.

5 - Usunąłem niepotrzebną inicjalizację jednego pustego obiektu dla listy ImportedObjects, ponieważ niepotrzebnie zajmowało to pamięć i powodowało problemy gdy pętla czyszcząca dane odczytywała ten obiekt.

6 - Dokonałem refaktoryzacji klasy ImportedObject, aby jej właściwości i pola miały taką samą konwencję - na przykład jedna właściwość miała swoje get i set zadeklarowane w linijce niżej, a inna w tej samej, więc po prostu to ujednoliciłem.

7 - Złączyłem te instrukcje warunkowe w klasie DataReader, które były niepotrzebnie w sobie zagnieżdżone, a wymagały tylko warunku logicznego AND w nadrzędnym ifie. Poprawia to czytelność kodu.

8 - Dodałem metodę ToUpper() do ParentType, aby nie musieć wywoływać tej metody później parę razy w porównaniach.

9 - Dodałem metodę rozszerzającą typ string która czyści i poprawia importowane linie danych, tak aby móc używać jej zamiast kopiowania tego samego kodu wielokrotnie jak miało to miejsce w pętli foreach o komentarzu "clear and correct imported data".

10 - Przeniosłem funkcjonalności pętli foreach odpowiedzialnej za korekcję i czyszczenie lini danych do pętli która te dane wczytuje i zapisuje do obiektów. Tym samym kod został zoptymalizowany poprzez usunięcie jednej zbędnej pętli.

11 - Zmieniłem nazwę zmiennych ImportedObjects, impObj na kolejno objectList i obj, aby po pierwsze zróżnicować te nazwy gdyż mogą się one mylić, oraz aby w przypadku ImportedObjects aby kierować się konwencją nazewniczą gdzie zmienne lokalne są nazywane według lowerCamelCase

Dodatkowo sugerowałbym zmianę właściwości IsNullable na typ boolean, ponieważ stanowiłoby to dodatkową walidację danych w razie gdyby któryś rekord posiadał polę IsNullable które nie pasuje ani do True ani False. Powoduje to problemy gdyż baza danych otrzyma rekordy które mogą być potencjalnie wadliwe i zawierać błędne wartości IsNullable (na przykład hipotetycznie pole z wartością "trrrrrue"). Ponadto boolean jest zwyczajnie o wiele lżejszym typem od stringa jeśli chodzi o zajmowaną pamięć, co też trzeba wziąć pod uwagę. Nie dokonałem tej zmiany samemu, ponieważ sposób obsługi błędnych lub pustych danych zależałby od potrzeb klienta i/lub konsensusu z kolegą tworzącym kod.

Podobnie jest z właściwościami ParentType i DataType, te zmienne również mogłyby być innego typu niż string, na przykład enum, gdzie podobnie eliminujemy możliwe nieścisłości w bazie danych i używamy mniej zasobożernego typu danych.
