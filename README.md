# icmarkets-web-api
ICMarkets WEB API .NET Developer Project

Project structure:
1. IcMarkets.Domain
2. IcMarkets.Infrastructure
3. IcMarkets.UseCases
4. IcMarkets.WebApi
5. IcMarkets.UnitTests

Database: 
1. Install and connect to PostgreSQL instance (via WSL2, for example). 
2. Manually create database: IcMarkets.
3. Run migrations from IcMarkets.Infrastructure project to populate database with tables: essentially, just one table for this project - public.Blockchains.

Start solution with IcMarkets.WebApi being set a startup project.
