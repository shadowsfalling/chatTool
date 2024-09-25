// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add('login', (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })
import 'cypress-wait-until';

Cypress.Commands.add('login', () => {
    cy.visit('/login'); // Die Login-Seite deiner Anwendung
    cy.get('input[name="username"]').type('deinBenutzername'); // Passe die Selektoren und Eingaben an
    cy.get('input[name="password"]').type('deinPasswort');
    cy.get('button[type="submit"]').click(); // Passe den Button-Selektor an
    // Warte ggf. auf ein Element nach dem erfolgreichen Login, z.B. die Navigation zur Startseite
    cy.url().should('include', '/dashboard'); // Beispiel: Prüfung, ob die URL nach dem Login korrekt ist
});

Cypress.Commands.add('resetDb', () => {
    cy.request({
        method: 'POST',
        url: 'http://localhost:5175/api/Database/reset',
    }).then(() => {
        cy.request({
            method: 'POST',
            url: 'http://localhost:5175/api/Database/seed',
        });

        cy.request({
            method: 'POST',
            url: 'http://localhost:5176/api/Database/seed',
        });

        cy.request({
            method: 'POST',
            url: 'http://localhost:5177/api/Database/seed',
        });

    });
});

Cypress.Commands.add('loginDesktop', () => {

    cy.visit('http://localhost:8080'); // Passe den Pfad an die URL an, auf der die Raumliste gerendert wird

    cy.get('#input-7').type('testuser');
    cy.get('#input-9').type('password123');
    cy.get('.v-form > .v-btn').click();
});