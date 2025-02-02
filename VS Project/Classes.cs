// Flight classes
abstract class Flight : IComparable<Flight> {
    public string flightNumber { get; set; }
    public string origin { get; set; }
    public string destination { get; set; }
    public DateTime expectedTime { get; set; }
    public string status { get; set; }
    public string? SpecialRequestCode { get; set; }

    
    public Flight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, string? specialRequestCode = null) {
        this.flightNumber = flightNumber;
        this.origin = origin;
        this.destination = destination;
        this.expectedTime = expectedTime;
        this.status = status;
        this.SpecialRequestCode = specialRequestCode;
    }

    public virtual double CalculateFees() {
        // costs
        // boarding gate base fee
        double baseCost = 300;

        if (origin.Contains("SIN")) {
            // departing flight -> $800
            baseCost += 800;
        }
        if (destination.Contains("SIN")) {
            // arriving flight -> $500
            baseCost += 500;
        }
        return baseCost;
    }

    public override string ToString() {
        return $"A new flight with {flightNumber} has been added";
    }

    public int CompareTo(Flight other) { 
        if (other == null) return 1;
        return expectedTime.CompareTo(other.expectedTime);
    }
}

// Inherited flights
class NORMFlight : Flight {
    public NORMFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        : base(flightNumber, origin, destination, expectedTime, status, null) { }

    public override double CalculateFees() {
        double baseCost = base.CalculateFees();
        return baseCost;
    }

    public override string ToString() {
        return base.ToString();
    }
}

class LWTTFlight : Flight {
    double requestFee { get; set; }

    public LWTTFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        : base(flightNumber, origin, destination, expectedTime, status, "LWTT") {
        requestFee = 500;
    }

    public override double CalculateFees() {
        double baseCost = requestFee + base.CalculateFees();
        return baseCost;
    }

    public override string ToString() {
        return base.ToString();
    }
}

class DDJBFlight : Flight {
    double requestFee { get; set; }

    public DDJBFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        : base(flightNumber, origin, destination, expectedTime, status, "DDJB") {
        requestFee = 300;
    }

    public override double CalculateFees() {
        double baseCost = requestFee + base.CalculateFees();
        return baseCost;
    }

    public override string ToString() {
        return base.ToString();
    }
}

class CFFTFlight : Flight {
    double requestFee { get; set; }

    public DDJBFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        : base(flightNumber, origin, destination, expectedTime, status, "DDJB") {
        requestFee = 150;
    }

    public override double CalculateFees() {
        double baseCost = requestFee + base.CalculateFees();
        return baseCost;
    }

    public override string ToString() {
        return base.ToString();
    }
}

// Airline class
class Airline {
    public string name { get; set; }
    public string code { get; set; }
    public Dictionary<string, Flight> flights { get; set; } = new();

    public Airline(string name, string code, Dictionary<string, Flight> flights) {
        this.name = name;
        this.code = code;
        this.flights = flights;
    }

    public bool AddFlight(Flight flight) {
        if ((flight.flightNumber).Contains(code)) {
            flights.Add(flight.flightNumber, flight);
            return true;
        }
        return false;
    }

    public bool RemoveFlight(Flight flight) {
        foreach (KeyValuePair<string, Flight> kvp in flights) {
            if (kvp.Value.flightNumber == flight.flightNumber) {
                flights.Remove(kvp.Key);
                return true;
            }
        }
        return false;
    }

    public double CalculateFees() {
        double totalCost = 0;
        foreach (Flight flight in flights.Values) {
            // Base cost
            totalCost += flight.CalculateFees();

            // check for more than 5 flights (discount of 3% before any deductions below)
            if (flights.Count > 5) {
                totalCost *= 0.97;
            }

            // discounts
            // check for every 3 flights arriving / departing
            totalCost -= (350 * (double)(Math.Floor((decimal) flights.Count / 3)));
            // for flights arriving / departing before 11am or after 9pm
            if (flight.expectedTime.TimeOfDay.CompareTo(new TimeOnly(hour: 11, minute: 0).ToTimeSpan()) < 0 || 
                flight.expectedTime.TimeOfDay.CompareTo(new TimeOnly(hour: 21, minute: 0).ToTimeSpan()) > 0) {
                totalCost -= 110;
            }
            // for each flight with the Origin of Dubai(DXB), Bangkok(BKK) or Tokyo(NRT
            if (flight.origin.Contains("DXB") || flight.origin.Contains("BKK") || flight.origin.Contains("NRT")) {
                totalCost -= 25;
            }
            // TODO: add $50 off for no special codes -> to check if the flight is a NORM flight
            if (flight is NORMFlight) {
                totalCost -= 50;
            }
        }
        return totalCost;
    }

    public override string ToString() {
        var flightStrings = flights.Select(kvp => $"{kvp.Key,-16}{name,-23}{kvp.Value.origin,-23}{kvp.Value.destination,-23}{kvp.Value.expectedTime}");
        return string.Join("\n", flightStrings);
    }
}

// BoardingGate class
class BoardingGate {
    public string GateName { get; set; }
    public bool SupportsDDJB { get; set; }
    public bool SupportsCFFT { get; set; }
    public bool SupportsLWTT { get; set; }
    public string? AssignedFlightNumber { get; set; }

    public bool IsAssigned => AssignedFlightNumber != null;
    
    public BoardingGate(string gateName, bool supportsDDJB, bool supportsCFFT, bool supportsLWTT) {
        GateName = gateName;
        SupportsDDJB = supportsDDJB;
        SupportsCFFT = supportsCFFT;
        SupportsLWTT = supportsLWTT;
        AssignedFlightNumber = null;
    }

    public bool AssignFlight(string flightNumber) {
        if (AssignedFlightNumber == null) {
            AssignedFlightNumber = flightNumber;
            return true;
        }
        return false;
    }

    public void UnassignFlight() {
        AssignedFlightNumber = null;
    }

    public override string ToString() {
        return $"Gate: {GateName}, SupportsDDJB: {SupportsDDJB}, SupportsCFFT: {SupportsCFFT}, SupportsLWTT: {SupportsLWTT}, AssignedFlight: {AssignedFlightNumber ?? "None"}";
    }
}
// Terminal class
class Terminal {
    public string TerminalName { get; set; }
    public Dictionary<string, BoardingGate> Gates { get; private set; } = new();

    public Terminal(string terminalName) {
        TerminalName = terminalName;
    }

    public void AddGate(BoardingGate gate) {
        Gates[gate.GateName] = gate;
    }

    public BoardingGate? GetUnassignedGate(Func<BoardingGate, bool> predicate) {
        foreach (var gate in Gates.Values) {
            if (gate.AssignedFlightNumber == null && predicate(gate)) {
                return gate;
            }
        }
        return null;
    }

    public void ListGates() {
        Console.WriteLine("{0,-10} {1,-15} {2,-15} {3,-15} {4,-20}", "Gate", "Supports DDJB", "Supports CFFT", "Supports LWTT", "Assigned Flight");

        foreach (var gate in Gates.Values) {
            Console.WriteLine("{0,-10} {1,-15} {2,-15} {3,-15} {4,-20}",
                gate.GateName,
                gate.SupportsDDJB ? "Yes" : "No",
                gate.SupportsCFFT ? "Yes" : "No",
                gate.SupportsLWTT ? "Yes" : "No",
                gate.AssignedFlightNumber ?? "None");
        }
    }
    public void LoadGatesFromFile(string filePath) {
        using StreamReader sr = new StreamReader(filePath);
        string? line;
        sr.ReadLine();
        while ((line = sr.ReadLine()) != null) {
            string[] gateInfo = line.Split(',');
            string gateName = gateInfo[0];
            bool supportsDDJB = bool.Parse(gateInfo[1]);
            bool supportsCFFT = bool.Parse(gateInfo[2]);
            bool supportsLWTT = bool.Parse(gateInfo[3]);

            AddGate(new BoardingGate(gateName, supportsDDJB, supportsCFFT, supportsLWTT));
        }
    }
}
