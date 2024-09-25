describe('API Tests for RoomController', () => {
    let authToken;
    let firstTest = true;

    beforeEach(() => {
        if (firstTest) {
            cy.resetDb();
            firstTest = false;
        }

        cy.request('POST', 'http://localhost:5175/api/auth/login', {
            username: 'testuser',
            password: 'password123'
        }).then((response) => {
            authToken = response.body.token;
        });
    });

    it('should create a room successfully', () => {
        cy.request({
            method: 'POST',
            url: 'http://localhost:5177/api/room',
            headers: {
                Authorization: `Bearer ${authToken}`
            },
            body: {
                name: 'Test Room'
            }
        }).then((response) => {
            expect(response.status).to.eq(201);
            expect(response.body).to.have.property('name', 'Test Room');
        });
    });

    it('should return conflict when creating a room with existing name', () => {
        cy.request({
            method: 'POST',
            url: 'http://localhost:5177/api/room',
            headers: {
                Authorization: `Bearer ${authToken}`
            },
            body: {
                name: 'New Test Room'
            }
        });

        cy.request({
            method: 'POST',
            url: 'http://localhost:5177/api/room',
            headers: {
                Authorization: `Bearer ${authToken}`
            },
            body: {
                name: 'New Test Room'
            },
            failOnStatusCode: false
        }).then((response) => {
            expect(response.status).to.eq(409);
        });
    });

    it('should get a room by ID successfully', () => {
        cy.request({
            method: 'POST',
            url: 'http://localhost:5177/api/room',
            headers: {
                Authorization: `Bearer ${authToken}`
            },
            body: {
                name: 'Another Test Room'
            }
        }).then((createResponse) => {
            const roomId = createResponse.body.id;

            cy.request({
                method: 'GET',
                url: `http://localhost:5177/api/room/${roomId}`,
                headers: {
                    Authorization: `Bearer ${authToken}`
                }
            }).then((response) => {
                expect(response.status).to.eq(200);
                expect(response.body).to.have.property('name', 'Another Test Room');
            });
        });
    });

    it('should return 404 for non-existent room', () => {
        cy.request({
            method: 'GET',
            url: 'http://localhost:5177/api/room/9999', // Ungültige ID
            headers: {
                Authorization: `Bearer ${authToken}`
            },
            failOnStatusCode: false
        }).then((response) => {
            expect(response.status).to.eq(404);
        });
    });


    it('should get all rooms', () => {
        cy.request({
            method: 'GET',
            url: 'http://localhost:5177/api/room/all',
            headers: {
                Authorization: `Bearer ${authToken}`
            }
        }).then((response) => {
            expect(response.status).to.eq(200);
            expect(response.body).to.be.an('array');
        });
    });


    it('should add a user to a room', () => {
        cy.request({
            method: 'POST',
            url: 'http://localhost:5177/api/room',
            headers: {
                Authorization: `Bearer ${authToken}`
            },
            body: {
                name: 'Room for User Addition'
            }
        }).then((createResponse) => {
            const roomId = createResponse.body.id;

            cy.request({
                method: 'POST',
                url: `http://localhost:5177/api/room/${roomId}/add-user`,
                headers: {
                    Authorization: `Bearer ${authToken}`
                },
                body: {
                    userId: 1
                }
            }).then((response) => {
                expect(response.status).to.eq(200);
                expect(response.body).to.eq('Benutzer wurde dem Raum hinzugefügt.');
            });
        });
    });
});