// Flight classes
abstract class Flight {
    protected string flightNumber { get; set; }
    protected string origin { get; set; }
    protected string destination { get; set; }
    protected DateTime expectedTime { get; set; }
    protected string status { get; set; }

    public Flight(string flightNumber, string origin, string destination, DateTime expectedTime, string status) {
        this.flightNumber = flightNumber;
        this.origin = origin;
        this.destination = destination;
        this.expectedTime = expectedTime;
        this.status = status;
    }

    public abstract double CalculateFees();

    public override string ToString() {
        throw new NotImplementedException();
    }
}

// Inherited flights
class NORMFlight : Flight {
    public NORMFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        : base(flightNumber, origin, destination, expectedTime, status) { }

    public override double CalculateFees() {
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
        return totalCost - 50; // for no special req. codes
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
        // boarding gate base fee + LWTT code req.
        double totalCost = 300 + 500;

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
        // boarding gate base fee + DDJB code req.
        double totalCost = 300 + 300;

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
        // boarding gate base fee + CFFT code req.
        double totalCost = 300 + 150;

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
        throw new NotImplementedException();
    }
}

// Airline class

// BoardingGate class

// Terminal class