// ---------------------------
//       Main function
// ---------------------------

//private static Dictionary<string, Airline> allAirlinesDict = new();
//private static Dictionary<string, Flight> allFlightsDict = new();
//private static Terminal terminal = new("T5");

static void Main(string[] args) {
    Dictionary<string, Airline> allAirlinesDict = new();
    Dictionary<string, Flight> allFlightsDict = new();
    Terminal terminal = new("T5");

    LoadAirlines("airlines.csv", allAirlinesDict);
    LoadFlights("flights.csv", allFlightsDict);
    terminal.LoadGatesFromFile("boardinggates.csv");

    while (true) {
        Console.WriteLine("=============================================");
        Console.WriteLine("Welcome to Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine("1. List All Flights");
        Console.WriteLine("2. List Boarding Gates");
        Console.WriteLine("3. Assign a Boarding Gate to a Flight");
        Console.WriteLine("4. Create Flight");
        Console.WriteLine("5. Display Flight Schedule");
        Console.WriteLine("6. Modify Flight Details");
        Console.WriteLine("7. Bulk Assign Unassigned Flights to Gates");
        Console.WriteLine("8. Calculate Total Fees per Airline");
        Console.WriteLine("0. Exit");
        Console.WriteLine("=============================================");
        Console.Write("Please select your option: ");


        switch (Console.ReadLine()) {
            case "1":
                ListFlights(allFlightsDict);
                break;
            case "2":
                terminal.ListGates();
                break;
            case "3":
                AssignBoardingGate(allFlightsDict, terminal);
                break;
            case "4":
                CreateFlight(allFlightsDict);
                break;
            case "5":
                DisplayFlightSchedule(allFlightsDict, allAirlinesDict, terminal);
                break;
            case "6":
                ModifyFlightDetails(allAirlinesDict);
                break;
            case "7":
                BulkAssignBoardingGates(allFlightsDict, terminal);
                break;
            case "8":
                CalculateTotalFeesPerAirline(allFlightsDict);
                break;
            case "0":
                Console.WriteLine("Now exiting...");
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }
}
// Required for the main function to initialize
Main(args);

// ---------------------------
//       Basic Features
// ---------------------------
// Feature 1
static void LoadAirlines(string filePath, Dictionary<string, Airline> allAirlinesDict) {
    using StreamReader sr = new StreamReader(filePath);
    string? line;
    sr.ReadLine();
    while ((line = sr.ReadLine()) != null) {
        string[] airlineData = line.Split(',');
        string name = airlineData[0];
        string code = airlineData[1];
        Airline airline = new Airline(name, code, new Dictionary<string, Flight>());
        allAirlinesDict.Add(code, airline);
    }
}

// Feature 2
// key to store airline number of flight (Ex: "SQ 115", "EK 870", etc.)
// value to store key details of flight
static void LoadFlights(string filePath, Dictionary<string, Flight> allFlightsDict) {
    using (StreamReader sr = new StreamReader(filePath)) {
        sr.ReadLine();
        string? s;
        while ((s = sr.ReadLine()) != null) {
            string[] flightArr = s.Split(',');
            // request code
            switch (flightArr[flightArr.Length - 1]) {
                // flightNumber, origin, destination, expectedTime, Status (on time on init)
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
//Should be under terminal, in classes

// Feature 5
static void AssignBoardingGate(Dictionary<string, Flight> allFlightsDict, Terminal terminal) {
    Console.Write("Enter Flight Number: ");
    string flightNumber = Console.ReadLine() ?? "";

    if (!allFlightsDict.TryGetValue(flightNumber, out var flight)) {
        Console.WriteLine("Flight not found.");
        return;
    }

    BoardingGate? assignedGate = terminal.GetUnassignedGate(g =>
        (flight is DDJBFlight && g.SupportsDDJB) ||
        (flight is CFFTFlight && g.SupportsCFFT) ||
        (flight is LWTTFlight && g.SupportsLWTT) ||
        (flight is NORMFlight));

    if (assignedGate != null) {
        assignedGate.AssignFlight(flight.flightNumber);
        Console.WriteLine($"Assigned {flight.flightNumber} to Gate {assignedGate.GateName}");
    }
    else {
        Console.WriteLine("No available gate for this flight.");
    }
}
// Feature 6
static void CreateFlight(Dictionary<string, Flight> allFlightsDict) {
    Console.Write("Enter Flight Number: ");
    string flightNumber = Console.ReadLine() ?? "";
    Console.Write("Enter Origin: ");
    string origin = Console.ReadLine() ?? "";
    Console.Write("Enter Destination: ");
    string destination = Console.ReadLine() ?? "";
    Console.Write("Enter Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
    DateTime expectedTime = DateTime.Parse(Console.ReadLine() ?? "01/01/2000 00:00");
    Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
    string specialCode = Console.ReadLine() ?? "None";

    Flight flight = specialCode switch {
        "LWTT" => new LWTTFlight(flightNumber, origin, destination, expectedTime, "On Time"),
        "DDJB" => new DDJBFlight(flightNumber, origin, destination, expectedTime, "On Time"),
        "CFFT" => new CFFTFlight(flightNumber, origin, destination, expectedTime, "On Time"),
        _ => new NORMFlight(flightNumber, origin, destination, expectedTime, "On Time"),
    };
    //Side note: ?? is for the default input if input is invalid
    allFlightsDict[flightNumber] = flight;
    Console.WriteLine($"Flight {flightNumber} has been added!");
}

// Feature 7
static void DisplayFlightSchedule(Dictionary<string, Flight> allFlightsDict, Dictionary<string, Airline> allAirlinesDict, Terminal terminal) {
    Console.WriteLine("Available Airlines:");
    foreach (var airline in allAirlinesDict.Values) {
        Console.WriteLine($"{airline.code}: {airline.name}");
    }
    Console.Write("Enter Airline Code (e.g. SQ, MH): ");
    string airlineCode = Console.ReadLine()?.Trim().ToUpper() ?? "";
    if (!allAirlinesDict.TryGetValue(airlineCode, out Airline selectedAirline)) {
        Console.WriteLine("Invalid Airline Code.");
        return;
    }
    List<Flight> airlineFlights = new List<Flight>();
    foreach (var flight in allFlightsDict.Values) {
        if (flight.flightNumber.Trim().StartsWith(airlineCode, StringComparison.OrdinalIgnoreCase))
            airlineFlights.Add(flight);
    }

    if (airlineFlights.Count == 0) {
        Console.WriteLine("No flights found for this airline.");
        return;
    }
    Console.WriteLine($"\nFlights for {selectedAirline.name}:");
    foreach (var flight in airlineFlights) {
        Console.WriteLine($"{flight.flightNumber}: {flight.origin} -> {flight.destination}");
    }
    Console.Write("\nEnter Flight Number: ");
    string flightNumber = Console.ReadLine()?.Trim() ?? "";
    Flight selectedFlight = airlineFlights.FirstOrDefault(f => f.flightNumber.Equals(flightNumber, StringComparison.OrdinalIgnoreCase));
    if (selectedFlight == null) {
        Console.WriteLine("Invalid Flight Number.");
        return;
    }
    Console.WriteLine("\nFlight Details:");
    Console.WriteLine($"Flight Number: {selectedFlight.flightNumber}");
    Console.WriteLine($"Airline Name: {selectedAirline.name}");
    Console.WriteLine($"Origin: {selectedFlight.origin}");
    Console.WriteLine($"Destination: {selectedFlight.destination}");
    Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.expectedTime}");
    string specialRequest = "";
    if (selectedFlight is NORMFlight)
        specialRequest = "NORM";
    else if (selectedFlight is LWTTFlight)
        specialRequest = "LWTT";
    else if (selectedFlight is DDJBFlight)
        specialRequest = "DDJB";
    else if (selectedFlight is CFFTFlight)
        specialRequest = "CFFT";
    Console.WriteLine($"Special Request Code: {specialRequest}");
    string boardingGate = "None";
    foreach (var gate in terminal.Gates.Values) {
        if (gate.AssignedFlightNumber != null &&
            gate.AssignedFlightNumber.Trim().Equals(selectedFlight.flightNumber.Trim(), StringComparison.OrdinalIgnoreCase)) {
            boardingGate = gate.GateName;
            break;
        }
    }
    Console.WriteLine($"Boarding Gate: {boardingGate}");
}


// Feature 8
static void ModifyFlightDetails(Dictionary<string, Airline> allAirlinesDict) {
    Console.WriteLine("Available Airlines:");
    foreach (var airline in allAirlinesDict.Values) {
        Console.WriteLine($"{airline.code}: {airline.name}");
    }

    Console.Write("Enter Airline Code: ");
    string airlineCode = Console.ReadLine() ?? "";

    if (string.IsNullOrEmpty(airlineCode) || (!allAirlinesDict.TryGetValue(airlineCode, out var selectedAirline))) {
        Console.WriteLine("Invalid Airline Code.");
        return;
    }

    Console.WriteLine($"Flights for {selectedAirline.name}:");
    foreach (var flight in selectedAirline.flights.Values) {
        Console.WriteLine($"{flight.flightNumber}: {flight.origin} -> {flight.destination}");
    }

    Console.Write("Enter Flight Number to modify or delete: ");
    string flightNumber = Console.ReadLine() ?? "";

    if (string.IsNullOrEmpty(flightNumber) || (!selectedAirline.flights.TryGetValue(flightNumber, out var selectedFlight))) {
        Console.WriteLine("Invalid Flight Number.");
        return;
    }

    Console.WriteLine("1. Modify Flight\n2. Delete Flight");
    Console.Write("Choose an option: ");
    string? option = Console.ReadLine();

    if (option == "1") {
        Console.WriteLine("What would you like to modify?");
        Console.WriteLine("1. Origin\n2. Destination\n3. Expected Time\n4. Status\n5. Special Request Code");
        Console.Write("Choose an option: ");
        string? modifyOption = Console.ReadLine();

        switch (modifyOption) {
            case "1":
                Console.Write("Enter new Origin: ");
                selectedFlight.origin = Console.ReadLine();
                break;
            case "2":
                Console.Write("Enter new Destination: ");
                selectedFlight.destination = Console.ReadLine();
                break;
            case "3":
                Console.Write("Enter new Expected Time (dd/mm/yyyy hh:mm): ");
                selectedFlight.expectedTime = DateTime.Parse(Console.ReadLine());
                break;
            case "4":
                Console.Write("Enter new Status: ");
                selectedFlight.status = Console.ReadLine();
                break;
            case "5":
                Console.Write("Enter new Special Request Code (CFFT/DDJB/LWTT/None): ");
                selectedFlight.status = Console.ReadLine();
                break;
            default:
                Console.WriteLine("Invalid option.");
                return;
        }

        Console.WriteLine("Flight details updated successfully.");
    }
    else if (option == "2") {
        Console.Write("Are you sure you want to delete this flight? (Y/N): ");
        if (Console.ReadLine()?.ToUpper() == "Y") {
            selectedAirline.flights.Remove(flightNumber);
            Console.WriteLine("Flight deleted successfully.");
        }
        else {
            Console.WriteLine("Deletion cancelled.");
        }
    }
    else {
        Console.WriteLine("Invalid option.");
    }
}

// Feature 9

// Advanced Feature 1: Bulk Assign Unassigned Flights to Boarding Gates
static void BulkAssignBoardingGates(Dictionary<string, Flight> allFlightsDict, Terminal terminal) {
    Queue<Flight> unassignedFlights = new Queue<Flight>();

    // Identify unassigned flights
    foreach (var flight in allFlightsDict.Values) {
        if (!terminal.Gates.Values.Any(g => g.AssignedFlightNumber == flight.flightNumber)) {
            unassignedFlights.Enqueue(flight);
        }
    }

    Console.WriteLine($"Total Unassigned Flights: {unassignedFlights.Count}");

    // Assign flights to available gates
    while (unassignedFlights.Count > 0) {
        Flight flight = unassignedFlights.Dequeue();
        BoardingGate? assignedGate = terminal.GetUnassignedGate(g =>
            (flight is DDJBFlight && g.SupportsDDJB) ||
            (flight is CFFTFlight && g.SupportsCFFT) ||
            (flight is LWTTFlight && g.SupportsLWTT) ||
            (flight is NORMFlight));

        if (assignedGate != null) {
            assignedGate.AssignFlight(flight.flightNumber);
            Console.WriteLine($"Assigned {flight.flightNumber} to Gate {assignedGate.GateName}");
        }
        else {
            Console.WriteLine($"No available gate for {flight.flightNumber}");
        }
    }
}

// Advanced Feature 2: Calculate Total Fees per Airline
static void CalculateTotalFeesPerAirline(Dictionary<string, Flight> allFlightsDict) {
    Dictionary<string, double> airlineFees = new();
    Dictionary<string, int> flightCounts = new();

    foreach (var flight in allFlightsDict.Values) {
        double fee = 300; // Base gate fee

        if (flight.destination.Contains("SIN")) fee += 500;
        if (flight.origin.Contains("SIN")) fee += 800;
        if (flight is DDJBFlight) fee += 300;
        if (flight is CFFTFlight) fee += 150;
        if (flight is LWTTFlight) fee += 500;

        if (!airlineFees.ContainsKey(flight.flightNumber.Substring(0, 2))) {
            airlineFees[flight.flightNumber.Substring(0, 2)] = 0;
            flightCounts[flight.flightNumber.Substring(0, 2)] = 0;
        }

        airlineFees[flight.flightNumber.Substring(0, 2)] += fee;
        flightCounts[flight.flightNumber.Substring(0, 2)]++;
    }

    foreach (var airline in airlineFees.Keys) {
        double totalFee = airlineFees[airline];
        int flights = flightCounts[airline];
        double discount = (flights / 3) * 350; // Discount for every 3 flights
        if (flights > 5) discount += totalFee * 0.03;
        Console.WriteLine($"{airline}: Original Fee: ${totalFee}, Discount: ${discount}, Final Fee: ${totalFee - discount}");
    }
}
