describe('example to-do app', () => {
    beforeEach(() => {
        cy.visit('http://localhost:8080/')
    })

    it('displays two todo items by default', () => {
        cy.contains('Welcome to Your Vue.js App').should('be.visible');
    });
});