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
            totalCost -= (350 * (double)(Math.Floor((decimal)flights.Count / 3)));
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
        var flightStrings = flights.Select(kvp => $"{kvp.Key,-16}{name,-23}{kvp.Value.origin,-23}{kvp.Value.destination,-23}{kvp.Value.expectedTime,-36}{kvp.Value.status}");
        return string.Join("\n", flightStrings);
    }
}