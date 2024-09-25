describe('Room List - End-to-End Test', () => {
    beforeEach(() => {

        cy.loginDesktop();

        cy.visit('http://localhost:8080/rooms'); // Passe den Pfad an, um die Seite mit der Raumliste zu besuchen
    });

    it('should display the list of rooms fetched from the backend', () => {
        // Überprüfe, ob die Räume auf der Seite angezeigt werden
        cy.contains('Liste der Räume');

        // Warte darauf, dass die Räume geladen werden, z.B. durch eine Bedingung, die bestätigt, dass Räume angezeigt werden
        cy.get('.v-expansion-panel').should('exist').and('have.length.at.least', 1); // Überprüfen, ob mindestens ein Raum geladen ist
    });

    it('should navigate to the room page when clicking Enter Room button', () => {
        // Klicke auf den "Enter Room" Button für den ersten Raum in der Liste
        cy.get('.v-expansion-panel .v-btn').first().click();
        // Überprüfe, ob die URL korrekt auf die Raumnummer verweist
        cy.url().should('match', /\/room\/\d+/); // Überprüft, ob zur richtigen Raumseite navigiert wird
    });

    it('should join the room notification list and show success message', () => {
        // Öffne das Panel, um die Buttons sichtbar zu machen
        cy.get(':nth-child(1) > .v-expansion-panel-title > .v-expansion-panel-title__icon > .mdi-chevron-down').click();

        // Klicke auf den "Join Room-Notificationlist" Button
        cy.contains('Join Room-Notificationlist').first().click();

        // Warte auf das Erscheinen der Erfolgsmeldung
        cy.waitUntil(() =>
            cy.get('.v-alert__content').then(($el) => {
                return $el.is(':visible') && $el.text().includes('Du bist dem Raum beigetreten!');
            }),
            {
                timeout: 10000, // Erhöhe das Timeout, wenn die Netzwerkanfragen länger dauern
                interval: 500, // Intervalle, in denen geprüft wird, ob die Bedingung erfüllt ist
                errorMsg: 'Die Erfolgsmeldung wurde nicht rechtzeitig angezeigt.',
            }
        );

        // Überprüfe, ob die Erfolgsmeldung sichtbar ist und den korrekten Text enthält
        cy.get('.v-alert__content')
            .should('be.visible')
            .and('contain', 'Du bist dem Raum beigetreten!');
    });

    it('should show an error message if joining the room fails', () => {
        // Interceptiere den Request und erzwinge eine Fehlerantwort
        cy.intercept('POST', '**/api/Room/*/add-user', {
          statusCode: 400,
          body: { message: 'Fehler beim Beitreten des Raumes.' }, // Simulierte Fehlerantwort
        }).as('joinRoomError');
      
        // Stelle sicher, dass das erste Panel geöffnet wird
        cy.get('.v-expansion-panel').first().within(() => {
          cy.get('.v-expansion-panel-title').click(); // Öffne das Panel, falls es nicht geöffnet ist
        });
      
        // Klicke auf den "Join Room-Notificationlist" Button
        cy.contains('Join Room-Notificationlist').first().click();
      
        // Warte auf die simulierte Fehlerantwort
        cy.wait('@joinRoomError');
      
        // Überprüfe, ob die Fehlermeldung angezeigt wird
        cy.get('.v-alert__content')
          .should('be.visible')
          .and('contain', 'Fehler beim Beitreten des Raumes.');
      });
});