// Flight classes
abstract class Flight {
    public string flightNumber { get; set; }
    protected string origin { get; set; }
    protected string destination { get; set; }
    protected DateTime expectedTime { get; set; }
    public string status { get; set; }

    public Flight(string flightNumber, string origin, string destination, DateTime expectedTime, string status) {
        this.flightNumber = flightNumber;
        this.origin = origin;
        this.destination = destination;
        this.expectedTime = expectedTime;
        this.status = status;
    }

    public virtual double CalculateFees() {
        // boarding gate base fee
        double totalCost = 300;

        if (origin.Contains("SIN")) {
            // departing flight -> $800
            totalCost += 800;
        }
        if (destination.Contains("SIN")) {
            // arriving flight -> $500
            totalCost += 500;
        }

        // TODO: check for more than 5 flights (discount of 3% before any deductions below)

        // TODO: check for any 3 flights arriving / departing

        if (expectedTime.CompareTo(new TimeOnly(hour: 11, minute: 0)) < 0 || expectedTime.CompareTo(new TimeOnly(hour: 21, minute: 0)) > 0) {
            // for flights arriving / departing before 11am or after 9pm
            totalCost -= 110;
        }
        if (origin.Contains("DXB") || origin.Contains("BKK") || origin.Contains("NRT")) {
            totalCost -= 25;
        }
        return totalCost;
    }

    public override string ToString() {
        return $"{flightNumber,-16}{"3"}";
    }
}

// Inherited flights
class NORMFlight : Flight {
    public NORMFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        : base(flightNumber, origin, destination, expectedTime, status) { }

    public override double CalculateFees() {
        double totalCost = base.CalculateFees() - 50; // $50 off for no special codes
        return totalCost;
    }

    public override string ToString() {
        throw new NotImplementedException();
    }
}

class LWTTFlight : Flight {
    double requestFee { get; set; }

    public LWTTFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        : base(flightNumber, origin, destination, expectedTime, status) {
        this.requestFee = 500;
    }

    public override double CalculateFees() {
        double totalCost = requestFee + base.CalculateFees();
        return totalCost;
    }

    public override string ToString() {
        throw new NotImplementedException();
    }
}

class DDJBFlight : Flight {
    double requestFee { get; set; }

    public DDJBFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        : base(flightNumber, origin, destination, expectedTime, status) {
        this.requestFee = 300;
    }

    public override double CalculateFees() {
        double totalCost = requestFee + base.CalculateFees();
        return totalCost;
    }

    public override string ToString() {
        throw new NotImplementedException();
    }
}

class CFFTFlight : Flight {
    double requestFee { get; set; }

    public CFFTFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        : base(flightNumber, origin, destination, expectedTime, status) {
        this.requestFee = 150;
    }

    public override double CalculateFees() {
        double totalCost = requestFee + base.CalculateFees();
        return totalCost;
    }

    public override string ToString() {
        throw new NotImplementedException();
    }
}

// Airline class
class Airline {
    string name { get; set; }
    string code { get; set; }
    Dictionary<string, Flight> flights { get; set; } = new();
    
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
        throw new NotImplementedException();
    }

    public override string ToString() {
        throw new NotImplementedException();
    }
}

// BoardingGate class
class BoardingGate
{
    public string GateName { get; set; }
    public bool SupportsDDJB { get; set; } // Double-decker jet bridge
    public bool SupportsCFFT { get; set; } // Connecting flight fast transfer
    public bool SupportsLWTT { get; set; } // Longer waiting time
    public string? AssignedFlightNumber { get; set; } // Null if unassigned

    public BoardingGate(string gateName, bool supportsDDJB, bool supportsCFFT, bool supportsLWTT)
    {
        GateName = gateName;
        SupportsDDJB = supportsDDJB;
        SupportsCFFT = supportsCFFT;
        SupportsLWTT = supportsLWTT;
        AssignedFlightNumber = null;
    }

    public bool AssignFlight(string flightNumber)
    {
        if (AssignedFlightNumber == null)
        {
            AssignedFlightNumber = flightNumber;
            return true;
        }
        return false;
    }

    public void UnassignFlight()
    {
        AssignedFlightNumber = null;
    }

    public override string ToString()
    {
        return $"Gate: {GateName}, SupportsDDJB: {SupportsDDJB}, SupportsCFFT: {SupportsCFFT}, SupportsLWTT: {SupportsLWTT}, AssignedFlight: {AssignedFlightNumber ?? "None"}";
    }
}
// Terminal class
class Terminal
{
    public string TerminalName { get; set; }
    public Dictionary<string, BoardingGate> Gates { get; private set; } = new();

    public Terminal(string terminalName)
    {
        TerminalName = terminalName;
    }

    public void AddGate(BoardingGate gate)
    {
        Gates[gate.GateName] = gate;
    }

    public BoardingGate? GetUnassignedGate(Func<BoardingGate, bool> predicate)
    {
        foreach (var gate in Gates.Values)
        {
            if (gate.AssignedFlightNumber == null && predicate(gate))
            {
                return gate;
            }
        }
        return null;
    }

    public void ListGates()
    {
        foreach (var gate in Gates.Values)
        {
            Console.WriteLine(gate);
        }
    }
}
