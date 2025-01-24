// ---------------------------
//       Basic Features
// ---------------------------
// Feature 1
//Dictionary<string, Airline> allAirlinesDict = new();

//static void LoadAirlines(Dictionary<string, Airline> allAirlinesDict, Dictionary<string, Flight> allFlightsDict) {
    //using (StreamReader sr = new StreamReader("airlines.csv")) {
        //sr.ReadLine();
        //string? s;
        //while ((s = sr.ReadLine()) != null) {
            //string[] airlineNameCodePair = s.Split(',');
            //throw new NotImplementedException();
        //}
    //}
//}
static Dictionary<string, Airline> allAirlinesDict = new();
static Dictionary<string, Flight> allFlightsDict = new();
static Terminal terminal = new("T5");

static void LoadAirlines(string filePath)
{
    using StreamReader sr = new StreamReader(filePath);
    string? line;
    sr.ReadLine();
    while ((line = sr.ReadLine()) != null)
    {
        string[] airlineData = line.Split(',');
        string name = airlineData[0];
        string code = airlineData[1];
        Airline airline = new Airline(name, code, new Dictionary<string, Flight>());
        allAirlinesDict[code] = airline;
    }
}

static void LoadFlights(string filePath)
{
    using StreamReader sr = new StreamReader(filePath);
    string? line;
    sr.ReadLine();
    while ((line = sr.ReadLine()) != null)
    {
        string[] flightData = line.Split(',');
        string flightNumber = flightData[0];
        string origin = flightData[1];
        string destination = flightData[2];
        DateTime expectedTime = DateTime.Parse(flightData[3]);
        string status = "On Time";

        Flight flight;
        string requestCode = flightData[^1]?.ToUpper();
        if (requestCode == "LWTT")
            flight = new LWTTFlight(flightNumber, origin, destination, expectedTime, status);
        else if (requestCode == "DDJB")
            flight = new DDJBFlight(flightNumber, origin, destination, expectedTime, status);
        else if (requestCode == "CFFT")
            flight = new CFFTFlight(flightNumber, origin, destination, expectedTime, status);
        else
            flight = new NORMFlight(flightNumber, origin, destination, expectedTime, status);

        allFlightsDict[flightNumber] = flight;
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
static void CreateFlight()
    {
        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine();
        Console.Write("Enter Origin: ");
        string origin = Console.ReadLine();
        Console.Write("Enter Destination: ");
        string destination = Console.ReadLine();
        Console.Write("Enter Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
        DateTime expectedTime = DateTime.Parse(Console.ReadLine());
        Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
        string specialCode = Console.ReadLine();

        Flight flight = specialCode switch
        {
            "LWTT" => new LWTTFlight(flightNumber, origin, destination, expectedTime, "On Time"),
            "DDJB" => new DDJBFlight(flightNumber, origin, destination, expectedTime, "On Time"),
            "CFFT" => new CFFTFlight(flightNumber, origin, destination, expectedTime, "On Time"),
            _ => new NORMFlight(flightNumber, origin, destination, expectedTime, "On Time"),
        };

        allFlightsDict[flightNumber] = flight;
        Console.WriteLine($"Flight {flightNumber} has been added!");
    }
// Feature 5

// Feature 6

// Feature 7
static void DisplayFlightSchedule()
    {
        foreach (var flight in allFlightsDict.Values)
        {
            Console.WriteLine(flight);
        }
    }

    static void Main(string[] args)
    {
        LoadAirlines("airlines.csv");
        LoadFlights("flights.csv");
        terminal.LoadGatesFromFile("boardinggates.csv");

        bool running = true;
        while (running)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("Welcome to Changi Airport Terminal 5");
            Console.WriteLine("=============================================");
            Console.WriteLine("1. List All Flights");
            Console.WriteLine("2. List Boarding Gates");
            Console.WriteLine("3. Assign a Boarding Gate to a Flight");
            Console.WriteLine("4. Create Flight");
            Console.WriteLine("7. Display Flight Schedule");
            Console.WriteLine("0. Exit");
            Console.Write("Please select your option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    DisplayFlightSchedule();
                    break;
                case "2":
                    terminal.ListGates();
                    break;
                case "4":
                    CreateFlight();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}

// Feature 8

// Feature 9

// Call stack
LoadFlights(allFlightsDict);
LoadAirlines(allAirlinesDict, allFlightsDict);
ListFlights(allFlightsDict);
