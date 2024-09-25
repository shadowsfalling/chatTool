describe('API Tests for ChatService RoomController', () => {
    let authToken;

    beforeEach(() => {
        // Authentifiziere den Benutzer und speichere das Token
        cy.request('POST', 'http://localhost:5175/api/auth/login', {
            username: 'testuser',
            password: 'password123'
        }).then((response) => {
            authToken = response.body.token;
        });
    });

    // Test: Abrufen eines Raumes nach ID (GET: api/room?id={id})
    it('should get room by ID successfully', () => {
        cy.request({
            method: 'GET',
            url: 'http://localhost:5176/api/room',
            qs: { id: 1 }, // Beispielhafte Raum-ID, passe diese je nach Testdaten an
            headers: {
                Authorization: `Bearer ${authToken}`
            }
        }).then((response) => {
            expect(response.status).to.eq(200);
            expect(response.body).to.eq('works'); // Erwartete Antwort basierend auf dem Controller
        });
    });

// Test: Senden einer Benachrichtigung (POST: api/room/send)
it('should send a notification to all clients', () => {
    cy.request({
        method: 'POST',
        url: 'http://localhost:5176/api/room/send',
        headers: {
            Authorization: `Bearer ${authToken}`,
            'Content-Type': 'application/json' // Content-Type muss JSON sein
        },
        // Der Body muss ein einfacher String sein, keine JSON-Objekt-Struktur
        body: '"Hello, this is a test notification."' // Die Nachricht als reiner JSON-String
    }).then((response) => {
        expect(response.status).to.eq(200);
        expect(response.body).to.have.property('message', 'Notification sent!'); // Überprüfen der Antwortnachricht
    });
});

    // Test: Abrufen der Nachrichten nach Raum-ID (GET: api/room/{roomId}/messages)
    it('should get messages by room ID', () => {
        const roomId = 1; // Beispielhafte Raum-ID, anpassen nach Bedarf

        cy.request({
            method: 'GET',
            url: `http://localhost:5176/1/messages`,
            headers: {
                Authorization: `Bearer ${authToken}`
            }
        }).then((response) => {
            expect(response.status).to.eq(200);
            expect(response.body).to.be.an('array'); // Erwartung einer Liste von Nachrichten
            // Optional: Überprüfe, ob die Nachrichten bestimmte Eigenschaften haben
            if (response.body.length > 0) {
                expect(response.body[0]).to.have.property('content'); // Beispielhafte Überprüfung
            }
        });
    });

    // Test: Abrufen der Nachrichten mit nicht existierender Raum-ID (GET: api/room/{roomId}/messages)
    it('should return not found for non-existent room ID', () => {
        const invalidRoomId = 9999; // Beispielhafte ungültige Raum-ID

        cy.request({
            method: 'GET',
            url: `http://localhost:5176/room/${invalidRoomId}/messages`,
            headers: {
                Authorization: `Bearer ${authToken}`
            },
            failOnStatusCode: false // Erlaubt den Test bei 404 Fehlern fortzusetzen
        }).then((response) => {
            expect(response.status).to.eq(404); // Überprüfen auf 404 Not Found
        });
    });
});