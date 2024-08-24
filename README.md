# TvShowsAndCastScraper

C# and EF Core

# Projects

## TvMaze.ScraperWorker

A Console.App project with a BackgroundService that will call the service to scraper the API https://www.tvmaze.com/api while the CancelationToken is active.

## TvMaze.Repository

Layer responsible for accessing the database for the CRUD operations  
The Migrations are also generated in this layer  
Isolates all access to the data on this layer

## TvMaze.Service

This project contains all the logic for the ScraperWorker and for the local API

- class ScraperService.cs
  - Polly Retry and Rate Limit Policies
  - Parallel tasks
  - Refit
- class TvShowService.cs
  - AutoMapper
  - Pagination

## TvMaze.Api

Provide endpoint to fetch all the TvShows Paginated offering the option to choose items per page

# How to run the project locally

- Install the .NET 8 SDK.
- Clone the repository
- Navigate to the root folder in a terminal and run `dotnet tool restore`.
- Build the solution with `dotnet build` and run all the tests with `dotnet test`.

## To Run the API and create the database follow the steps in a terminal:

- Navigate to TvMaze.Api and run `dotnet ef database update` to create the database.
  In case the command is not found run before then repeat previous step `dotnet tool install --global dotnet-ef`
- Run `dotnet run` to launch the application - https://localhost:5001/swagger/index.html - You can request the data passing the page number and also the amount of items per page.
- You can also use the class `TvMazeScraper.http` to make the requests once the application is running

## To run the actual scraping run the following steps in a second terminal

- Navigate to the `TvMaze.ScraperWorker` folder.
- Run `dotnet run` to launch the application.

## Given more time I would make the following improvements:

- Add integration tests for the API and the Scraper
- Add extra logging and monitoring
- Parameterize the Polly Retry and Rate Limit configurations and other configs
- Add CI/CD pipeline
- Add security such as to prevent DDOS on the API I'm exposing
