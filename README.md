# NativaGlobalUsers
Play role Create internal CRUD for users.

## Context:
A bot requires consulting and storing customer data.
For this, a REST API is being considered that implements a CRUD in the database to query, insert and update records. The fields to store are: Name, e-mail, and password. All fields are required and password must be validated to a minimum of 8 characters, alphanumeric, upper and lower case, and at least one special character.
- A) Develop the architecture of the solution, including the necessary database and endpoints.
- B) Justify the architecture, endpoint structure, and database.
    
## Disclaimer
This code implements a basic CRUD for users, my asumson was this code will be used in an internal environment like a client cloud and not include a security and encriptation layer,
all endpoints do not require a bearer token and the password are saved as plain text on the database. 

## Overview
Application was created on a Windows Technologies API Net Core 6.0 / Identity Framework (SQL Implementation) / Repository Pattern and MVC this is the most common set of tools and pattern used on the industry.

Implemented EndPoint:

- [x] GET api/User : Retuns a list of Users Wrapped on a JSon response model
- [x] PUT api/User : Request a User model in Json format to insert user in database
- [x] GET api/User/{id} : Return a user by ID
- [x] DELETE api/User/{id} : To Remove a User on the Database
- [x] PUT api/User/{id} : To Update a User by ID
- [x] PATCH api/User/{id} : To Update just a property of a user model

- [x] Database was included on the project as Migration

This this last case use a JSON Patch document any reference can validate on [jsonpatch](https://jsonpatch.com/). 
Sample for this implementation:
>[
  {
    "path": "/Name",
    "op": "replace",
    "value": "FrankMarin"
  }
]

