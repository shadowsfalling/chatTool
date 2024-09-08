describe('API Test for Creating a Room', () => {
    it('should create a room successfully', () => {
        // Führe eine POST-Anfrage durch, um einen Raum zu erstellen
        cy.request('POST', 'http://localhost:5081/api/room', {
            name: 'Test Room'
        }).then((response) => {
            // Überprüfe, ob die Antwort erfolgreich war
            expect(response.status).to.eq(201);  // HTTP 201 Created
            expect(response.body).to.have.property('name', 'Test Room');
        });
    });
});