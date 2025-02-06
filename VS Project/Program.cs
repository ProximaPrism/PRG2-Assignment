// ---------------------------
//       Main function
// ---------------------------
using System.Text.RegularExpressions;
internal class Program {
    private static void Main(string[] args) {
        Terminal terminal = new("T5", new Dictionary<string, Airline>(), new Dictionary<string, Flight>());

        LoadAirlines("airlines.csv", terminal);
        LoadFlights("flights.csv", terminal);
        terminal.LoadGatesFromFile("boardinggates.csv");

        // Initalize flights to each airline
        foreach (Airline airline in terminal.airlines.Values) {
            foreach (Flight flight in terminal.flights.Values) {
                airline.AddFlight(flight);
            }
        }

        while (true) {
            Console.WriteLine("=============================================");
            Console.WriteLine("Welcome to Changi Airport Terminal 5");
            Console.WriteLine("=============================================");
            Console.WriteLine("1. List Flight Schedule");
            Console.WriteLine("2. List Boarding Gates");
            Console.WriteLine("3. Assign a Boarding Gate to a Flight");
            Console.WriteLine("4. Create Flight");
            Console.WriteLine("5. Display Airline Flights");
            Console.WriteLine("6. Modify Flight Details");
            Console.WriteLine("7. Bulk Assign Unassigned Flights to Gates");
            Console.WriteLine("8. Calculate Total Fees per Airline");
            Console.WriteLine("0. Exit");
            Console.WriteLine("=============================================");
            Console.Write("Please select your option: ");


            switch (Console.ReadLine()) {
                case "1":
                    ListFlights(terminal);
                    break;
                case "2":
                    terminal.ListGates();
                    break;
                case "3":
                    AssignBoardingGate(terminal);
                    break;
                case "4":
                    CreateFlight(terminal);
                    break;
                case "5":
                    DisplayAirlineFlights(terminal);
                    break;
                case "6":
                    ModifyFlightDetails(terminal);
                    break;
                case "7":
                    BulkAssignBoardingGates(terminal);
                    break;
                case "8":
                    CalculateTotalFeesPerAirline(terminal);
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

    // ---------------------------
    //       Basic Features
    // ---------------------------
    // Feature 1
    static void LoadAirlines(string filePath, Terminal terminal) {
        using StreamReader sr = new StreamReader(filePath);
        string? line;
        sr.ReadLine();
        while ((line = sr.ReadLine()) != null) {
            string[] airlineData = line.Split(',');
            string name = airlineData[0];
            string code = airlineData[1];
            Airline airline = new Airline(name, code, new Dictionary<string, Flight>());
            terminal.airlines.Add(code, airline);
        }
    }

    // Feature 2
    static void LoadFlights(string filePath, Terminal terminal) {
        using (StreamReader sr = new StreamReader(filePath)) {
            sr.ReadLine();
            string? s;
            while ((s = sr.ReadLine()) != null) {
                string[] flightArr = s.Split(',');
                // request code
                switch (flightArr[flightArr.Length - 1]) {
                    // flightNumber, origin, destination, expectedTime, Status (Scheduled on init)
                    case "LWTT": {
                            LWTTFlight flight = new(flightArr[0], flightArr[1], flightArr[2], DateTime.Parse(flightArr[3]), "Scheduled");
                            terminal.flights.Add(flightArr[0], flight);
                            break;
                        }
                    case "DDJB": {
                            DDJBFlight flight = new(flightArr[0], flightArr[1], flightArr[2], DateTime.Parse(flightArr[3]), "Scheduled");
                            terminal.flights.Add(flightArr[0], flight);
                            break;
                        }
                    case "CFFT": {
                            CFFTFlight flight = new(flightArr[0], flightArr[1], flightArr[2], DateTime.Parse(flightArr[3]), "Scheduled");
                            terminal.flights.Add(flightArr[0], flight);
                            break;
                        }
                    default: {
                            NORMFlight flight = new(flightArr[0], flightArr[1], flightArr[2], DateTime.Parse(flightArr[3]), "Scheduled");
                            terminal.flights.Add(flightArr[0], flight);
                            break;
                        }
                }
            }
        }
    }

    // Feature 3
    static void ListFlights(Terminal terminal) {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Flights for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine($"{"Flight Number",-16}{"Airline Name",-23}{"Origin",-23}{"Destination",-23}{"Expected Departure/Arrival Time",-36}{"Status"}");

        Dictionary<Flight, string> airlineNamePair = new();

        foreach (Flight flight in terminal.flights.Values) {
            foreach (Airline airline in terminal.airlines.Values) {
                if ((flight.flightNumber).Contains(airline.code)) {
                    airlineNamePair.Add(flight, airline.name);
                }
            }
        }
        List<Flight> sortedFlights = airlineNamePair.Keys.ToList();
        sortedFlights.Sort();

        foreach (Flight flight in sortedFlights) {
            Console.WriteLine($"{flight.flightNumber,-16}{airlineNamePair[flight],-23}{flight.origin,-23}{flight.destination,-23}{flight.expectedTime,-36}{flight.status}");
        }
    }

    // Feature 4
    //Should be under terminal, in classes

    // Feature 5
    static void AssignBoardingGate(Terminal terminal) {
        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine() ?? "";

        if (!terminal.flights.TryGetValue(flightNumber.Trim().ToUpper(), out var flight)) {
            Console.WriteLine("Flight not found.");
            return;
        }

        Console.WriteLine($"Flight Number: {flight.flightNumber}");
        Console.WriteLine($"Special Request Code: {flight.SpecialRequestCode ?? "None"}");
        Console.WriteLine($"Current Status: {flight.status}");

        BoardingGate? assignedGate = null;

        while (assignedGate == null) {
            Console.Write("Enter Boarding Gate: ");
            string gateName = Console.ReadLine() ?? "";

            if (!terminal.boardingGates.TryGetValue(gateName, out var selectedGate)) {
                Console.WriteLine("Invalid Gate. Please try again.");
                continue;
            }

            if (selectedGate.isAssigned) {
                Console.WriteLine("Gate is already assigned to another flight. Choose another.");
                continue;
            }
            if (!GateSupportsFlight(selectedGate, flight)) {
                Console.WriteLine("This gate does not support the selected flight type. Please select another gate.");
                continue;
            }
            assignedGate = selectedGate;
        }

        assignedGate.AssignFlight(flight.flightNumber);
        Console.WriteLine($"Successfully assigned {flight.flightNumber} to Gate {assignedGate.gateName}");
        Console.Write("Would you like to update the flight status? (Y/N): ");
        string response = Console.ReadLine()?.Trim().ToUpper() ?? "N";
        if (response == "Y") {
            Console.Write("Enter new status (Delayed, Boarding, On Time): ");
            string? newStatus = Console.ReadLine()?.Trim();

            while (true) {
                if (newStatus == "Delayed" || newStatus == "Boarding" || newStatus == "On Time") {
                    flight.status = newStatus;
                    break;
                }
                Console.WriteLine("Invalid status. Please try again.");
            }
        }
        else {
            flight.status = "Scheduled";
        }
        Console.WriteLine($"Flight {flight.flightNumber} is now marked as '{flight.status}' at Gate {assignedGate.gateName}");
    }
    // Method to check if a gate supports a flight type
    static bool GateSupportsFlight(BoardingGate gate, Flight flight) {
        return (flight.SpecialRequestCode == "DDJB" && gate.supportsDDJB) ||
               (flight.SpecialRequestCode == "CFFT" && gate.supportsCFFT) ||
               (flight.SpecialRequestCode == "LWTT" && gate.supportsLWTT) ||
               (flight.SpecialRequestCode == null); // This is for NORM flights
    }

    // Feature 6
    static void CreateFlight(Terminal terminal) {
        Regex validFlight = new Regex("@^[A-Z]{2} \\d{3}$");
        while (true) {
            string flightNumber;
            while (true) {
                Console.Write("Enter Flight Number: ");
                flightNumber = Console.ReadLine()?.Trim().ToUpper() ?? "";
                if (!string.IsNullOrEmpty(flightNumber) && !terminal.flights.ContainsKey(flightNumber) && validFlight.IsMatch(flightNumber)) {
                    break;
                }
                Console.WriteLine("Invalid or duplicate Flight Number. Please enter a unique flight number.");
            }

            string origin;
            while (true) {
                Console.Write("Enter Origin: ");
                origin = Console.ReadLine()?.Trim() ?? "";
                if (!string.IsNullOrEmpty(origin))
                    break;

                Console.WriteLine("Invalid origin location. Please try again.");
            }

            string destination;
            while (true) {
                Console.Write("Enter Destination: ");
                destination = Console.ReadLine()?.Trim() ?? "";
                if (!string.IsNullOrEmpty(destination))
                    break;

                Console.WriteLine("Invalid destination location. Please try again.");
            }

            DateTime expectedTime;
            while (true) {
                Console.Write("Enter Expected Departure/Arrival Time (dd/MM/yyyy HH:mm): ");
                if (DateTime.TryParse(Console.ReadLine(), out expectedTime))
                    break;

                Console.WriteLine("Invalid date format. Please enter again (dd/MM/yyyy HH:mm).");
            }
            string inputCode;
            while (true) {
                Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
                inputCode = Console.ReadLine()?.Trim().ToUpper() ?? "NONE";
                if (inputCode == "CFFT" || inputCode == "DDJB" || inputCode == "LWTT" || inputCode == "NONE")
                    break;

                Console.WriteLine("Invalid Special Request Code. Enter 'CFFT', 'DDJB', 'LWTT', or 'None'.");
            }
            Flight flight = inputCode switch {
                "LWTT" => new LWTTFlight(flightNumber, origin, destination, expectedTime, "Scheduled"),
                "DDJB" => new DDJBFlight(flightNumber, origin, destination, expectedTime, "Scheduled"),
                "CFFT" => new CFFTFlight(flightNumber, origin, destination, expectedTime, "Scheduled"),
                _ => new NORMFlight(flightNumber, origin, destination, expectedTime, "On Time"),
            };
            terminal.flights.Add(flightNumber, flight);
            string csvLine = $"{flightNumber},{origin},{destination},{expectedTime:dd/MM/yyyy HH:mm},{flight.status},{inputCode}";
            File.AppendAllText("flights.csv", csvLine + Environment.NewLine);
            Console.WriteLine($"\nFlight {flightNumber} has been successfully added!");

            Console.Write("\nWould you like to add another flight? (Y/N): ");
            string response = Console.ReadLine()?.Trim().ToUpper() ?? "N";
            if (response == "N") {
                Console.WriteLine("Returning to the main menu...");
                break;
            }
        }
    }
    // Feature 7
    static void DisplayAirlineFlights(Terminal terminal) {
        Console.WriteLine("==============================================");
        Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
        Console.WriteLine("==============================================");
        Console.WriteLine($"{"Airline Code",-18}{"Airline Name"}");
        foreach (Airline airline in terminal.airlines.Values) {
            Console.WriteLine($"{airline.code,-18}{airline.name}");
        }

        // Input validation is at the DisplayFlightSchedule() method
        Console.Write("Enter Airline Code (e.g. SQ, MH): ");
        string airlineCode = Console.ReadLine() ?? "";

        if (string.IsNullOrEmpty(airlineCode)) {
            Console.WriteLine("No valid airline selected");
            return;
        }
        Airline? selectedAirline = null;
        foreach (KeyValuePair<string, Airline> kvp in terminal.airlines) {
            if (airlineCode == kvp.Key) {
                selectedAirline = kvp.Value;
                break;
            }
            Console.WriteLine("No valid airline selected");
            return;
        }
        if (selectedAirline == null) {
            return;
        }

        List<Flight> airlineFlights = [.. selectedAirline.flights.Values];
        airlineFlights.Sort();

        Console.WriteLine($"\nFlights for {selectedAirline.name}:");
        foreach (Flight flight in airlineFlights) {
            Console.WriteLine($"{flight.flightNumber,-16}{selectedAirline.name,-23}{flight.origin,-23}{flight.destination,-23}{flight.expectedTime,-36}{flight.status}");
        }
    }

    // Feature 8
    static void ModifyFlightDetails(Terminal terminal) {
        Console.WriteLine("==============================================");
        Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
        Console.WriteLine("==============================================");
        Console.WriteLine($"{"Airline Code",-18}{"Airline Name"}");
        foreach (Airline airline in terminal.airlines.Values) {
            Console.WriteLine($"{airline.code,-18}{airline.name}");
        }

        // Input validation is at the DisplayFlightSchedule() method
        Console.Write("Enter Airline Code (e.g. SQ, MH): ");
        string airlineCode = Console.ReadLine() ?? "";

        if (string.IsNullOrEmpty(airlineCode)) {
            Console.WriteLine("No valid airline selected");
            return;
        }
        Airline? selectedAirline = null;
        foreach (KeyValuePair<string, Airline> kvp in terminal.airlines) {
            if (airlineCode == kvp.Key) {
                selectedAirline = kvp.Value;
                break;
            }
            Console.WriteLine("No valid airline selected");
            return;
        }
        if (selectedAirline == null) {
            return;
        }

        List<Flight> airlineFlights = [.. selectedAirline.flights.Values];
        airlineFlights.Sort();

        Console.WriteLine($"\nFlights for {selectedAirline.name}:");
        foreach (Flight flight in airlineFlights) {
            Console.WriteLine($"{flight.flightNumber,-16}{selectedAirline.name,-23}{flight.origin,-23}{flight.destination,-23}{flight.expectedTime,-36}{flight.status}");
        }

        string flightNumber = Console.ReadLine()?.Trim() ?? "";
        Flight? selectedFlight = airlineFlights.FirstOrDefault(f => f.flightNumber.Equals(flightNumber, StringComparison.OrdinalIgnoreCase));
        if (selectedFlight == null) {
            Console.WriteLine("Invalid Flight Number.");
            return;
        }

        BoardingGate? boardingGate = null;
        foreach (BoardingGate gate in terminal.boardingGates.Values) {
            if (gate.assignedFlightNumber != null &&
                gate.assignedFlightNumber.Trim().Equals(selectedFlight.flightNumber.Trim(), StringComparison.OrdinalIgnoreCase)) {
                boardingGate = gate;
                break;
            }
        }
        Console.WriteLine("\nCurrent Flight Details:");
        Console.WriteLine($"Flight Number: {selectedFlight.flightNumber}");
        Console.WriteLine($"Airline Name: {selectedAirline.name}");
        Console.WriteLine($"Origin: {selectedFlight.origin}");
        Console.WriteLine($"Destination: {selectedFlight.destination}");
        Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.expectedTime}");
        if (boardingGate != null) {
            Console.WriteLine($"Boarding Gate: {boardingGate.gateName}");
        }
        else {
            Console.WriteLine($"Boarding Gate: Unassigned");
        }

        Console.WriteLine("1. Modify Flight\n2. Delete Flight");
        Console.Write("Choose an option: ");
        string? option = Console.ReadLine();

        if (option == "1") {
            Console.WriteLine("What would you like to modify?");
            Console.WriteLine("1. Basic Information\n2. Status\n3. Special Request Code");
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

                    Console.Write("Enter new Destination: ");
                    string inputDest = Console.ReadLine() ?? "";
                    if (string.IsNullOrEmpty(inputDest)) {
                        Console.WriteLine("Invalid destination location");
                        break;
                    }
                    selectedFlight.destination = inputDest;

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
                case "2":
                    Console.Write("Enter new Status: ");
                    string inputStatus = Console.ReadLine() ?? "";
                    if (char.ToUpper(inputStatus[0]) + inputStatus[1] + inputStatus[2] + char.ToUpper(inputStatus[3]) + inputStatus.Substring(4) != "On Time" ||
                        char.ToUpper(inputStatus[0]) + inputStatus.Substring(1) != "Boarding" ||
                        char.ToUpper(inputStatus[0]) + inputStatus.Substring(1) != "Delayed" ||
                        char.ToUpper(inputStatus[0]) + inputStatus.Substring(1) != "Scheduled") {
                        Console.WriteLine("Invalid status");
                        break;
                    }
                    selectedFlight.status = inputStatus;
                    break;
                case "3":
                    Console.Write("Enter new Special Request Code (CFFT/DDJB/LWTT/None): ");
                    string inputCode = Console.ReadLine() ?? "";
                    if (inputCode.ToUpper() != "CFFT" || inputCode.ToUpper() != "DDJB" || inputCode.ToUpper() != "LWTT" || !string.IsNullOrEmpty(inputCode)) {
                        Console.WriteLine("Invalid Special request code");
                    }
                    switch (inputCode) {
                        case "CFFT":
                            selectedFlight = (CFFTFlight)selectedFlight;
                            break;
                        case "DDJB":
                            selectedFlight = (DDJBFlight)selectedFlight;
                            break;
                        case "LWTT":
                            selectedFlight = (LWTTFlight)selectedFlight;
                            break;
                        default:
                            selectedFlight = (NORMFlight)selectedFlight;
                            break;
                    }
                    UpdateFlightsCSV(terminal);
                    break;
                case "4":
                    AssignBoardingGate(terminal);
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    return;
            }
            Console.WriteLine("Flight details updated successfully.");
            Console.WriteLine("\nNew Flight Details:");
            Console.WriteLine($"Flight Number: {selectedFlight.flightNumber}");
            Console.WriteLine($"Airline Name: {selectedAirline.name}");
            Console.WriteLine($"Origin: {selectedFlight.origin}");
            Console.WriteLine($"Destination: {selectedFlight.destination}");
            Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.expectedTime}");
            if (boardingGate != null) {
                Console.WriteLine($"Boarding Gate: {boardingGate.gateName}");
            }
            else {
                Console.WriteLine($"Boarding Gate: Unassigned");
            }
        }
        else if (option == "2") {
            Console.Write("Are you sure you want to delete this flight? (Y/N): ");
            string confirmDelete = Console.ReadLine()?.Trim().ToUpper() ?? "N";
            if (confirmDelete == "Y") {
                if (boardingGate != null) {
                    boardingGate.UnassignFlight();
                }
                terminal.flights.Remove(flightNumber);
                UpdateFlightsCSV(terminal);
                Console.WriteLine($"Flight {flightNumber} deleted successfully.");
            }
            else {
                Console.WriteLine("Deletion cancelled.");
            }
        }
        else {
            Console.WriteLine("Invalid option.");
        }
    }
    //Method to save info  to csv file
    static void UpdateFlightsCSV(Terminal terminal) {
        string filePath = "flights.csv";
        using (StreamWriter writer = new StreamWriter(filePath)) {
            writer.WriteLine("FlightNumber,Origin,Destination,ExpectedTime,Status,SpecialRequestCode");
            foreach (var flight in terminal.flights.Values) {
                writer.WriteLine($"{flight.flightNumber},{flight.origin},{flight.destination},{flight.expectedTime:dd/MM/yyyy HH:mm},{flight.status},{flight.SpecialRequestCode ?? "None"}");
            }
        }
        Console.WriteLine("\nFlight records updated in flights.csv!");
    }

    // Feature 9
    // Implemented as interfaces in the classes

    // ---------------------------
    //      Advanced Features
    // ---------------------------
    // Advanced Feature 1: Bulk Assign Unassigned Flights to Boarding Gates
    static void BulkAssignBoardingGates(Terminal terminal) {
        Queue<Flight> unassignedFlights = new Queue<Flight>();

        // Identify unassigned flights
        foreach (var flight in terminal.flights.Values) {
            if (!terminal.boardingGates.Values.Any(g => g.assignedFlightNumber == flight.flightNumber)) {
                unassignedFlights.Enqueue(flight);
            }
        }

        Console.WriteLine($"Total Unassigned Flights: {unassignedFlights.Count}");

        // Assign flights to available boardingGates
        while (unassignedFlights.Count > 0) {
            Flight flight = unassignedFlights.Dequeue();
            BoardingGate? assignedGate = terminal.GetUnassignedGate(g =>
                flight is DDJBFlight && g.supportsDDJB ||
                flight is CFFTFlight && g.supportsCFFT ||
                flight is LWTTFlight && g.supportsLWTT ||
                flight is NORMFlight);

            if (assignedGate != null) {
                assignedGate.AssignFlight(flight.flightNumber);
                Console.WriteLine($"Assigned {flight.flightNumber} to Gate {assignedGate.gateName}");
            }
            else {
                Console.WriteLine($"No available gate for {flight.flightNumber}");
            }
        }
    }

    // Advanced Feature 2: Calculate Total Fees per Airline
    static void CalculateTotalFeesPerAirline(Terminal terminal) {
        double grandTotal = 0;
        double totalDiscounts = 0;
        foreach (KeyValuePair<string, Airline> kvp in terminal.airlines) {
            double baseCost = 0;
            double totalCost = 0;
            foreach (Flight flight in terminal.flights.Values) {
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
}
