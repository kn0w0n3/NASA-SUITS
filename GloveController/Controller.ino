struct InputPacket {
    unsigned short x;
    unsigned short y;
    bool button1;
    bool button2;
    // Maybe eventually include battery %

    bool operator!=(const InputPacket& rhs) {
        return (x != rhs.x) || (y != rhs.y) ||
               (button1 != rhs.button1) || (button2 != rhs.button2);
    }
};

const int RATE = 10; // Delay between packets in ms

// Pins for the inputs
const int PIN_X = A0;
const int PIN_Y = A1;
const int PIN_B1 = 2;
const int PIN_B2 = 4;

// Packet byte codes
const char START = 17;
const char END = 18;
const char ESCAPE = 19;

InputPacket prevPack; // The last packet sent

void setup() {
    // Initialize the Serial
    Serial.begin(115200);

    // Set up the pins
    pinMode(PIN_B1, INPUT);
    pinMode(PIN_B2, INPUT);
    pinMode(PIN_X, INPUT);
    pinMode(PIN_Y, INPUT);
}

void loop() {
    // Create the packet
    InputPacket pack;
    pack.x = analogRead(PIN_X);
    pack.y = analogRead(PIN_Y);
    pack.button1 = digitalRead(PIN_B1);
    pack.button2 = digitalRead(PIN_B2);

    // Only send if there are changes
    if (pack != prevPack) {
        prevPack = pack;

        // Cast the packet to its byte form
        char *data = reinterpret_cast<char *>(&pack);

        Serial.write(START);
        for (int i = 0; i < 6; i++) {
            if (data[i] == START || data[i] == END || data[i] == ESCAPE) {
                // If the data happen to be a special code, send an escape code before
                Serial.write(ESCAPE);
            }
            Serial.write(data[i]);
        }
        Serial.write(END);
    }
    delay(RATE);
}
