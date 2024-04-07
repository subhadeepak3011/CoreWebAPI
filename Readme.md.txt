# C#.Net Core Product API

## Features

Operations to Create, Read, Update and Delete the Product data have been implemeted in C# using .Net Core.

## Building the solution

To use this project, follow these steps:

1. Open the “GHDWebAPI.sln” from the Root folder "GHDWebAPI".
2. Right click on the “GHDWebAPI” project and click on “Build”.
3. Once build is successful, run the Web API with https selected.
4. This should launch the SwaggerUI and bring up the index.html page.

## Running the Unit Tests

Follow these steps to execute the unit tests:

1. Right click on GHDWebAPI.Tests project and click on "Run Tests" to execute the unit test cases.
2. Alternatively click on Test -> Run All Tests in the toolbar to execute the tests.

## Interacting with the application

Expand each node and click on "Try it out" and "Execute" to run the corresponding operation.
1. GET     : This method returns all the products from the database.
2. GET {id}: This method returns the product corresponding to the given id.
3. POST    : This method creates and adds a new product to the database.
4. PUT     : This method updates modifications to the product with the given id to the database.
5. DELETE  : This method deletes the product with the given id from the database.

Input data is validated at every end point and appropriate error messages are presented to the user.