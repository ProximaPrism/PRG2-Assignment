class BoardingGate {
    public string gateName { get; set; }
    public bool supportsDDJB { get; set; }
    public bool supportsCFFT { get; set; }
    public bool supportsLWTT { get; set; }
    public string? assignedFlightNumber { get; set; }

    public bool isAssigned => assignedFlightNumber != null;

    public BoardingGate(string gateName, bool supportsDDJB, bool supportsCFFT, bool supportsLWTT) {
        this.gateName = gateName;
        this.supportsDDJB = supportsDDJB;
        this.supportsCFFT = supportsCFFT;
        this.supportsLWTT = supportsLWTT;
        assignedFlightNumber = null;
    }

    public bool AssignFlight(string flightNumber) {
        if (assignedFlightNumber == null) {
            assignedFlightNumber = flightNumber;
            return true;
        }
        return false;
    }

    public void UnassignFlight() {
        assignedFlightNumber = null;
    }

    public override string ToString() {
        return $"Gate: {gateName}, SupportsDDJB: {supportsDDJB}, SupportsCFFT: {supportsCFFT}, SupportsLWTT: {supportsLWTT}, AssignedFlight: {assignedFlightNumber ?? "None"}";
    }
}