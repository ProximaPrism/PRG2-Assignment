// ---------------------------
//       Main function
// ---------------------------

//private static Dictionary<string, Airline> allAirlinesDict = new();
//private static Dictionary<string, Flight> allFlightsDict = new();
//private static Terminal terminal = new("T5");

static void Main(string[] args) {
    // key to store the code of the airline (Ex: "SQ", "MY", etc.)
    // value to store key details of airline
    Dictionary<string, Airline> allAirlinesDict = new();

    // key to store airline number of flight (Ex: "SQ 115", "EK 870", etc.)
    // value to store key details of flight
    Dictionary<string, Flight> allFlightsDict = new();
    Terminal terminal = new("T5");

    LoadAirlines("airlines.csv", allAirlinesDict);
    LoadFlights("flights.csv", allFlightsDict);
    terminal.LoadGatesFromFile("boardinggates.csv");

    // Initalize flights to each airline
    foreach (Airline airline in allAirlinesDict.Values) {
        foreach (Flight flight in allFlightsDict.Values) {
            airline.AddFlight(flight);
        }
    }

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
                ListFlights(allFlightsDict, allAirlinesDict);
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
                CalculateTotalFeesPerAirline(allFlightsDict, allAirlinesDict);
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
static void ListFlights(Dictionary<string, Flight> allFlightsDict, Dictionary<string, Airline> allAirlinesDict) {
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Flights for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine($"{"Flight Number",-16}{"Airline Name",-23}{"Origin",-23}{"Destination",-23}{"Expected Departure/Arrival Time"}");

    Dictionary<Flight, string> airlineNamePair = new();

    foreach (Flight flight in allFlightsDict.Values) {
        foreach (Airline airline in allAirlinesDict.Values) {
            if ((flight.flightNumber).Contains(airline.code)) {
                airlineNamePair.Add(flight, airline.name);
            }
        }
    }
    List<Flight> sortedFlights = airlineNamePair.Keys.ToList();
    sortedFlights.Sort();

    foreach (Flight flight in sortedFlights) {
        Console.WriteLine($"{flight.flightNumber,-16}{airlineNamePair[flight],-23}{flight.origin,-23}{flight.destination,-23}{flight.expectedTime}");
    }
}

// Feature 4
//Should be under terminal, in classes

// Feature 5
static void AssignBoardingGate(Dictionary<string, Flight> allFlightsDict, Terminal terminal)
{
    Console.Write("Enter Flight Number: ");
    string flightNumber = Console.ReadLine() ?? "";

    if (!allFlightsDict.TryGetValue(flightNumber, out var flight))
    {
        Console.WriteLine("Flight not found.");
        return;
    }

    Console.WriteLine($"Flight Number: {flight.flightNumber}");
    Console.WriteLine($"Special Request Code: {flight.SpecialRequestCode ?? "None"}");
    Console.WriteLine($"Current Status: {flight.status}");

    BoardingGate? assignedGate = null;

    while (assignedGate == null)
    {
        Console.Write("Enter Boarding Gate: ");
        string gateName = Console.ReadLine() ?? "";

        if (!terminal.Gates.TryGetValue(gateName, out var selectedGate))
        {
            Console.WriteLine("Invalid Gate. Please try again.");
            continue;
        }

        if (selectedGate.IsAssigned)
        {
            Console.WriteLine("Gate is already assigned to another flight. Choose another.");
            continue;
        }
        if (!GateSupportsFlight(selectedGate, flight))
        {
            Console.WriteLine("This gate does not support the flight type. Please select another gate.");
            continue;
        }

        assignedGate = selectedGate;
    }

    assignedGate.AssignFlight(flight.flightNumber);
    Console.WriteLine($"Successfully assigned {flight.flightNumber} to Gate {assignedGate.GateName}");
    Console.Write("Would you like to update the flight status? (Y/N): ");
    string response = Console.ReadLine()?.Trim().ToUpper() ?? "N";
    if (response == "Y")
    {
        Console.Write("Enter new status (Delayed, Boarding, On Time): ");
        string newStatus = Console.ReadLine()?.Trim();

        if (newStatus == "Delayed" || newStatus == "Boarding" || newStatus == "On Time")
        {
            flight.status = newStatus;
        }
        else
        {
            Console.WriteLine("Invalid status. Setting to default: On Time.");
            flight.status = "On Time";
        }
    }
    else
    {
        flight.status = "On Time";
    }

    Console.WriteLine($"Flight {flight.flightNumber} is now marked as '{flight.status}' at Gate {assignedGate.GateName}");
}
// Method to check if a gate supports a flight type
static bool GateSupportsFlight(BoardingGate gate, Flight flight)
{
    return (flight.SpecialRequestCode == "DDJB" && gate.SupportsDDJB) ||
           (flight.SpecialRequestCode == "CFFT" && gate.SupportsCFFT) ||
           (flight.SpecialRequestCode == "LWTT" && gate.SupportsLWTT) ||
           (flight.SpecialRequestCode == null); // This is for NORM flights
}

// Feature 6
static void CreateFlight(Dictionary<string, Flight> allFlightsDict)
{
    while (true)
    {
        Console.Write("Enter Flight Number: ");
        string flightNumber;
        while (true)
        {
            flightNumber = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(flightNumber) && !allFlightsDict.ContainsKey(flightNumber))
                break;

            Console.WriteLine("Invalid or duplicate Flight Number. Please enter a unique flight number.");
        }

        Console.Write("Enter Origin: ");
        string origin;
        while (true)
        {
            origin = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(origin))
                break;

            Console.WriteLine("Invalid origin location. Please try again.");
        }

        Console.Write("Enter Destination: ");
        string destination;
        while (true)
        {
            destination = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(destination))
                break;

            Console.WriteLine("Invalid destination location. Please try again.");
        }

        Console.Write("Enter Expected Departure/Arrival Time (dd/MM/yyyy HH:mm): ");
        DateTime expectedTime;
        while (true)
        {
            if (DateTime.TryParse(Console.ReadLine(), out expectedTime))
                break;

            Console.WriteLine("Invalid date format. Please enter again (dd/MM/yyyy HH:mm).");
        }

        Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
        string inputCode;
        while (true)
        {
            inputCode = Console.ReadLine()?.Trim().ToUpper() ?? "NONE";
            if (inputCode == "CFFT" || inputCode == "DDJB" || inputCode == "LWTT" || inputCode == "NONE")
                break;

            Console.WriteLine("Invalid Special Request Code. Enter 'CFFT', 'DDJB', 'LWTT', or 'None'.");
        }
        Flight flight = inputCode switch
        {
            "LWTT" => new LWTTFlight(flightNumber, origin, destination, expectedTime, "On Time"),
            "DDJB" => new DDJBFlight(flightNumber, origin, destination, expectedTime, "On Time"),
            "CFFT" => new CFFTFlight(flightNumber, origin, destination, expectedTime, "On Time"),
            _ => new NORMFlight(flightNumber, origin, destination, expectedTime, "On Time"),
        };
        allFlightsDict[flightNumber] = flight;
        string csvLine = $"{flightNumber},{origin},{destination},{expectedTime:dd/MM/yyyy HH:mm},{flight.status},{inputCode}";
        File.AppendAllText("flights.csv", csvLine + Environment.NewLine);
        Console.WriteLine($"\nFlight {flightNumber} has been successfully added!");
        Console.Write("\nWould you like to add another flight? (Y/N): ");
        string response = Console.ReadLine()?.Trim().ToUpper() ?? "N";
        if (response != "N")
        {
            Console.WriteLine("Returning to the main menu...");
            break;
        }
    }
}
// Feature 7
static void DisplayFlightSchedule(Dictionary<string, Flight> allFlightsDict, Dictionary<string, Airline> allAirlinesDict, Terminal terminal) {
    Airline? selectedAirline = HandleAirlineCode(allAirlinesDict);
    if (selectedAirline == null) {
        Console.WriteLine("Invalid airline code");
        return;
    } 
    List<Flight> airlineFlights = [.. selectedAirline.flights.Values];

    Console.WriteLine($"\nFlights for {selectedAirline.name}:");
    foreach (var flight in airlineFlights) {
        Console.WriteLine($"{flight.flightNumber}: {flight.origin} -> {flight.destination}");
    }
    Console.Write("\nEnter Flight Number: ");
    string flightNumber = Console.ReadLine()?.Trim() ?? "";
    Flight? selectedFlight = airlineFlights.FirstOrDefault(f => f.flightNumber.Equals(flightNumber, StringComparison.OrdinalIgnoreCase));
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

static Airline? HandleAirlineCode(Dictionary<string, Airline> allAirlinesDict) {
    Console.WriteLine("Available Airlines:");
    foreach (var airline in allAirlinesDict.Values) {
        Console.WriteLine($"{airline.code}: {airline.name}");
    }
    // Input validation is at the DisplayFlightSchedule() method
    Console.Write("Enter Airline Code (e.g. SQ, MH): ");
    string airlineCode = Console.ReadLine() ?? "";

    List<Flight> flights = new();
    foreach (KeyValuePair<string, Airline> kvp in allAirlinesDict) {
        if (kvp.Key.Trim().ToUpper().Contains(airlineCode)) {
            flights = kvp.Value.flights.Values.ToList();
            flights.Sort();
            foreach (Flight flight in flights) {
                Console.WriteLine($"{flight.flightNumber,-16}{kvp.Value.name,-23}{flight.origin,-23}{flight.destination,-23}{flight.expectedTime}");
            }
            return kvp.Value;
        }
    }
    return null;
}


// Feature 8
static void ModifyFlightDetails(Dictionary<string, Airline> allAirlinesDict) {
    Console.WriteLine("Available Airlines:");
    foreach (var airline in allAirlinesDict.Values) {
        Console.WriteLine($"{airline.code}: {airline.name}");
    }

    Console.Write("Enter Airline Code: ");
    string airlineCode = Console.ReadLine() ?? "";

    if (string.IsNullOrEmpty(airlineCode) || !allAirlinesDict.TryGetValue(airlineCode, out var selectedAirline)) {
        Console.WriteLine("Invalid Airline Code.");
        return;
    }

    Console.WriteLine($"Flights for {selectedAirline.name}:");
    foreach (var flight in selectedAirline.flights.Values) {
        Console.WriteLine($"{flight.flightNumber}: {flight.origin} -> {flight.destination}");
    }

    Console.Write("Enter Flight Number to modify or delete: ");
    string flightNumber = Console.ReadLine() ?? "";

    if (string.IsNullOrEmpty(flightNumber) || !selectedAirline.flights.TryGetValue(flightNumber, out var selectedFlight)) {
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
                string inputOrigin = Console.ReadLine() ?? "";
                if (string.IsNullOrEmpty(inputOrigin)) {
                    Console.WriteLine("Invalid origin location");
                    break;
                }
                selectedFlight.origin = inputOrigin;
                break;
            case "2":
                Console.Write("Enter new Destination: ");
                string inputDest = Console.ReadLine() ?? "";
                if (string.IsNullOrEmpty(inputDest)) {
                    Console.WriteLine("Invalid destination location");
                    break;
                }
                selectedFlight.destination = inputDest;
                break;
            case "3":
                Console.Write("Enter new Expected Time (dd/mm/yyyy hh:mm): ");
                while (true) {
                    try {
                        selectedFlight.expectedTime = DateTime.Parse(Console.ReadLine() ?? "");
                        break;
                    }
                    catch (FormatException) {
                        Console.WriteLine("Invalid expected time value");
                    }
                }
                break;
            case "4":
                Console.Write("Enter new Status: ");
                string inputStatus = Console.ReadLine() ?? "";
                if (char.ToUpper(inputStatus[0]) + inputStatus[1] + inputStatus[2] + char.ToUpper(inputStatus[3]) + inputStatus.Substring(4) != "On Time" ||
                    char.ToUpper(inputStatus[0]) + inputStatus.Substring(1) != "Boarding" ||
                    char.ToUpper(inputStatus[0]) + inputStatus.Substring(1) != "Delayed") {
                    Console.WriteLine("Invalid status");
                }
                selectedFlight.status = inputStatus;
                break;
            case "5":
                Console.Write("Enter new Special Request Code (CFFT/DDJB/LWTT/None): ");
                string inputCode = Console.ReadLine() ?? "";
                if (inputCode.ToUpper() != "CFFT" || inputCode.ToUpper() != "DDJB" || inputCode.ToUpper() != "LWTT" || !string.IsNullOrEmpty(inputCode)) {
                    Console.WriteLine("Invalid Special request code");
                }
                switch (inputCode) {
                    case "CFFT":
                        selectedFlight = (CFFTFlight) selectedFlight;
                        break;
                    case "DDJB":
                        selectedFlight = (DDJBFlight) selectedFlight;
                        break;
                    case "LWTT":
                        selectedFlight = (LWTTFlight) selectedFlight;
                        break;
                    default:
                        selectedFlight = (NORMFlight) selectedFlight;
                        break;
                }
                break;
            default:
                Console.WriteLine("Invalid option.");
                return;
        }

        Console.WriteLine("Flight details updated successfully.");
    }
    else if (option == "2") {
        Console.Write("Are you sure you want to delete this flight? (Y/N): ");
        string inputOption = Console.ReadLine() ?? "";
        while (true) {
            if (inputOption.ToUpper() == "Y") {
                if (selectedAirline.RemoveFlight(selectedFlight)) {
                    Console.WriteLine("Flight deleted successfully.");
                    break;
                }
                Console.WriteLine("Deletion unsuccessful. Flight not found");
                break;
            }
            else if (inputOption.ToUpper() == "N") {
                Console.WriteLine("Deletion cancelled");
                break;
            }
            else {
                Console.WriteLine("Invalid option");
            }
        }
    }
    else {
        Console.WriteLine("Invalid option.");
    }
}

// Feature 9
// Implemented as interfaces in the classes

// ---------------------------
//      Advanced Features
// ---------------------------
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
            flight is DDJBFlight && g.SupportsDDJB ||
            flight is CFFTFlight && g.SupportsCFFT ||
            flight is LWTTFlight && g.SupportsLWTT ||
            flight is NORMFlight);

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
static void CalculateTotalFeesPerAirline(Dictionary<string, Flight> allFlightsDict, Dictionary<string, Airline> allAirlinesDict) {
    double grandTotal = 0;
    double totalDiscounts = 0;
    foreach (KeyValuePair<string, Airline> kvp in allAirlinesDict) {
        double baseCost = 0;
        double totalCost = 0;
        foreach (Flight flight in allFlightsDict.Values) {
            if (flight.flightNumber.Contains(kvp.Key)) {
                baseCost += flight.CalculateFees();
            }
        }
        totalCost += kvp.Value.CalculateFees();
        
        grandTotal += totalCost;
        totalDiscounts += (baseCost - totalCost);
        Console.WriteLine($"{kvp.Value.name,-20}: Original Fee: ${baseCost:N0}, Discount: ${baseCost - totalCost:N0}, Final Fee: ${totalCost:N0}");
    }
    Console.WriteLine("-------------------------------------------------------------------------------");
    Console.WriteLine($"Grand Total: ${grandTotal:N2}, Discounts over Grand Total: {(totalDiscounts / grandTotal) * 100:F2}% (${totalDiscounts:N2})");
}
