# TestResult

Eine mithilfe von AngularJS, DevExtreme (Client) und ASP.NET (Server) geschrieben Anwendung zur Visualisierung der Unittest Ergebnisse der mps public solutions gmbh. Ausgelesen werden Logdateien im internen Format der BsGui Unittests.
Die Anwendung startet mit dem Dashboard, das eine Übersicht der letzten Ergebnisse gruppiert nach Datenbanktyp (MsSql und Oracle) und Version (2016 und 2017) darstellt.

![dashboard](https://user-images.githubusercontent.com/21066449/33218808-2239f8a2-d13f-11e7-8f26-02429af1d575.png)

Über das seitliche Menü werden alle Anwendungen angezeigt, die vertestet werden. Hierbei entspricht die Zahl in dem orangenen Badge
der Anzahl der Fehler im aktuellsten Testlauf unter MsSql in der Version 2017. Durch einen Klick werden die historischen Ergebnisse aller 
Tests dieser Anwendung in einem Graph visualiert. 

![apparea](https://user-images.githubusercontent.com/21066449/33218810-228ab35a-d13f-11e7-98ed-b8b8527f0eb9.png)

Man kann sowohl im Graphen als auch im Dashboard durch einen Klick auf die Detailansicht 
dieses Testlaufes abspringen. Dies funktioniert jedoch nur, wenn im entsprechenden Testlauf auch Fehler aufgetreten sind.

![run](https://user-images.githubusercontent.com/21066449/33218809-2262dcae-d13f-11e7-81ae-d4f6801cb5e0.png)
