class Terminal {
    public string terminalName { get; set; }
    public Dictionary<string, Airline> airlines { get; set; } = new();
    // key to store the code of the airline (Ex: "SQ", "MY", etc.)
    // value to store key details of airline
    public Dictionary<string, Flight> flights { get; set; } = new();
    // key to store airline number of flight (Ex: "SQ 115", "EK 870", etc.)
    // value to store key details of flight
    public Dictionary<string, BoardingGate> boardingGates { get; private set; } = new();

    public Terminal(string terminalName, Dictionary<string, Airline> airlines, Dictionary<string, Flight> flights) {
        this.terminalName = terminalName;
        this.airlines = airlines;
        this.flights = flights;
    }

    public void AddGate(BoardingGate gate) {
        boardingGates[gate.gateName] = gate;
    }

    public BoardingGate? GetUnassignedGate(Func<BoardingGate, bool> predicate) {
        foreach (var gate in boardingGates.Values) {
            if (gate.assignedFlightNumber == null && predicate(gate)) {
                return gate;
            }
        }
        return null;
    }

    public void ListGates() {
        Console.WriteLine("====================================================");
        Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
        Console.WriteLine("====================================================");
        Console.WriteLine("{0,-10} {1,-15} {2,-15} {3,-15} {4,-20}", "Gate", "Supports DDJB", "Supports CFFT", "Supports LWTT", "Assigned Flight");

        foreach (var gate in boardingGates.Values) {
            Console.WriteLine("{0,-10} {1,-15} {2,-15} {3,-15} {4,-20}",
                gate.gateName,
                gate.supportsDDJB ? "Yes" : "No",
                gate.supportsCFFT ? "Yes" : "No",
                gate.supportsLWTT ? "Yes" : "No",
                gate.assignedFlightNumber ?? "None");
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
