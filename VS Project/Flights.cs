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
        SpecialRequestCode = specialRequestCode;
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

    public CFFTFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        : base(flightNumber, origin, destination, expectedTime, status, "CFFT") {
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
