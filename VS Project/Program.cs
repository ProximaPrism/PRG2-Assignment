// ---------------------------
//       Basic Features
// ---------------------------
// Feature 1
Dictionary<string, Airline> allAirlinesDict = new();

static void LoadAirlines(Dictionary<string, Airline> allAirlinesDict, Dictionary<string, Flight> allFlightsDict) {
    using (StreamReader sr = new StreamReader("airlines.csv")) {
        sr.ReadLine();
        string? s;
        while ((s = sr.ReadLine()) != null) {
            string[] airlineNameCodePair = s.Split(',');
            throw new NotImplementedException();
        }
    }
}


// Feature 2
// key to store airline number of flight (Ex: "SQ 115", "EK 870", etc.)
// value to store key details of flight
Dictionary<string, Flight> allFlightsDict = new();

static void LoadFlights(Dictionary<string, Flight> allFlightsDict) {
    using (StreamReader sr = new StreamReader("flights.csv")) {
        sr.ReadLine();
        string? s;
        while ((s = sr.ReadLine()) != null) { 
            string[] flightArr = s.Split(',');
            switch (flightArr[flightArr.Length - 1]) {
                case "LWTT": {
                        LWTTFlight flight = new(flightArr[0], flightArr[1], flightArr[2], DateTime.Parse(flightArr[3]), "On Time");
                        allFlightsDict.Add(flightArr[0], flight);
                        break;
                    }
                case "DDJB": {
                        DDJBFlight flight = new(flightArr[0], flightArr[1], flightArr[2], DateTime.Parse(flightArr[3]), "On Time");
                        allFlightsDict.Add(flightArr[0], flight);
                        break;
                    }
                case "CFFT": {
                        CFFTFlight flight = new(flightArr[0], flightArr[1], flightArr[2], DateTime.Parse(flightArr[3]), "On Time");
                        allFlightsDict.Add(flightArr[0], flight);
                        break;
                    }
                default: {
                        NORMFlight flight = new(flightArr[0], flightArr[1], flightArr[2], DateTime.Parse(flightArr[3]), "On Time");
                        allFlightsDict.Add(flightArr[0], flight);
                        break;
                    }
            }
        }
    }
}

// Feature 3
static void ListFlights(Dictionary<string, Flight> allFlightsDict) {
    foreach (KeyValuePair<string, Flight> kvp in allFlightsDict) {
        Console.WriteLine(kvp.Key);
    }
}

// Feature 4

// Feature 5

// Feature 6

// Feature 7

// Feature 8

// Feature 9

// Call stack
LoadFlights(allFlightsDict);
LoadAirlines(allAirlinesDict, allFlightsDict);
ListFlights(allFlightsDict);
