describe('API Tests for UserService AuthController', () => {
    let authToken;
    let firstTest = true;

    beforeEach(() => {
        if (firstTest) {
            cy.resetDb();
            firstTest = false;
        }
    });

    it('should register a new user successfully', () => {
        cy.request({
            method: 'POST',
            url: 'http://localhost:5175/api/auth/register',
            body: {
                username: 'foreignuser',
                email: 'foreignuser@example.com',
                password: 'Password123',
                PasswordRepeat: 'Password123'
            }
        }).then((response) => {
            expect(response.status).to.eq(200);
            expect(response.body).to.have.property('token');
            authToken = response.body.token;
        });
    });

    it('should log in a user successfully', () => {
        cy.request({
            method: 'POST',
            url: 'http://localhost:5175/api/auth/login',
            body: {
                username: 'foreignuser',
                password: 'Password123'
            }
        }).then((response) => {
            expect(response.status).to.eq(200);
            expect(response.body).to.have.property('token');
            authToken = response.body.token; 
        });
    });

    it('should return unauthorized for incorrect login', () => {
        cy.request({
            method: 'POST',
            url: 'http://localhost:5175/api/auth/login',
            body: {
                username: 'wronguser',
                password: 'wrongpassword'
            },
            failOnStatusCode: false
        }).then((response) => {
            expect(response.status).to.eq(401);
        });
    });

    
    it('should get user information successfully', () => {
        cy.request({
            method: 'GET',
            url: 'http://localhost:5175/api/auth/me',
            headers: {
                Authorization: `Bearer ${authToken}`
            }
        }).then((response) => {
            expect(response.status).to.eq(200);
            expect(response.body).to.have.property('username', 'foreignuser'); 
            expect(response.body).to.have.property('id');
        });
    });

    
    it('should return unauthorized when getting user info without token', () => {
        cy.request({
            method: 'GET',
            url: 'http://localhost:5175/api/auth/me',
            failOnStatusCode: false
        }).then((response) => {
            expect(response.status).to.eq(401);
        });
    });

    it('should return not found if user does not exist', () => {
            const manipulatedToken = authToken.slice(0, -1) + 'x';

        cy.request({
            method: 'GET',
            url: 'http://localhost:5175/api/auth/me',
            headers: {
                Authorization: `Bearer ${manipulatedToken}`,
            },
            failOnStatusCode: false
        }).then((response) => {
            expect(response.status).to.eq(401); 
        });
    });
});