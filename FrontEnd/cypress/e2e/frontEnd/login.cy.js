describe('Login Form', () => {
    beforeEach(() => {
        // Besuche die Seite mit dem Login-Formular
        cy.visit('http://localhost:8080/login'); // Passe den Pfad an, je nachdem, wo das Login-Formular gerendert wird
    });

    it('should display the login form', () => {
        // Überprüfe, ob das Formular geladen wird
        cy.get('#input-7').should('exist');
        cy.get('#input-9').should('exist');
        cy.get('.v-form > .v-btn').should('exist');
    });

    it('should show an error message for invalid credentials', () => {
        // Gib ungültige Anmeldedaten ein und klicke auf Login
        cy.get('#input-7').type('invaliduser');
        cy.get('#input-9').type('wrongpassword');
        cy.get('.v-form > .v-btn').click();

        // Überprüfe, ob die Fehlermeldung angezeigt wird
        cy.get('.v-alert').should('contain', 'Invalid credentials');
    });

    it('should log in successfully with correct credentials', () => {

        cy.loginDesktop();
        // Überprüfe, ob die Seite nach erfolgreicher Anmeldung umgeleitet wird
        cy.url().should('eq', `http://localhost:8080/`); // Passe die URL entsprechend der Weiterleitung an
    });
});