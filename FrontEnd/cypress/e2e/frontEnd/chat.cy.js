describe('Chat Room Tests', () => {
  beforeEach(() => {

      cy.resetDb()
      cy.loginDesktop();
      cy.visit('http://localhost:8080/room/1');
  });

  it('should load the chat room and display existing messages', () => {
      cy.contains('Chat (Room: 1)').should('be.visible');
      cy.contains('Welcome,').should('be.visible');
      cy.get('.chat-sheet').within(() => {
          cy.get('.message-content').should('exist');
      });
  });

  it('should send a message and display it in the chat', () => {
      cy.get('input[placeholder="Schreibe eine Nachricht..."]').type('Hallo, dies ist ein Test!');
      cy.get('.v-container > .v-btn').click();
      cy.get('.chat-sheet').within(() => {
          cy.contains('Hallo, dies ist ein Test!').should('be.visible');
      });
  });


  it('should send a notification to all clients', () => {
    cy.intercept('POST', '**/api/auth/login').as('loginRequest');
    cy.visit('http://localhost:8080/login');

    cy.get('#input-8').type('testuser');
    cy.get('#input-10').type('password123');
    cy.get('.v-form > .v-btn').click();

    cy.window().then((win) => {
      const storedTokenRaw = win.localStorage.getItem('user');
      console.log("Stored Token Raw:", storedTokenRaw);

      const storedToken = storedTokenRaw ? JSON.parse(storedTokenRaw) : null;
      console.log("Parsed Token Object:", storedToken);

      if (!storedToken || !storedToken.token) {
        throw new Error('Token konnte nicht aus dem Speicher extrahiert werden.');
      }

      cy.wrap(storedToken.token).as('authToken');
    });


    cy.visit('http://localhost:8080/room/1');

    cy.wait(1000);

    cy.get('@authToken').then((authToken) => {
      const headers = {
        'Authorization': `Bearer ${authToken}`,
        'Content-Type': 'application/json',
        'My-Custom-Header': 'Test header'
      };

      console.log('Headers being set:', headers);

      cy.request({
        method: 'POST',
        url: 'http://localhost:5176/api/room/send',
        headers: headers,
        body: '"Hello, this is a test notification."',
        failOnStatusCode: false
      }).then((response) => {
        console.log('Response from server:', response); 
        if (response.status === 401) {
          console.error('Unauthorized error:', response.headers['www-authenticate']);
        }
        expect(response.status).to.eq(200);


        cy.contains("Hello, this is a test notification.").should('be.visible');

      });
    });
  });
});