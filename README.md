# What is?

SLiDS - Simple Lightweight Data Searching Library

SLiDS can be used for quick access to data. Data can be placed in an SQL database or memory (Runtime). It is also possible to cache data from an SQL database in memory.

## Dependecies

* .NET Statdard 2.1
* System.Data.SqlClient 4.8.1

## Interfaces

#### Main
* IRepository (IAsyncRepository) - Generic interface for the repository with the most common methods
* IDbRepository (IAsyncDbRepository) - Generic interface for the repository for placing data in a database, inherited from IRepository (IAsyncRepository)
* ICachadRepository - Generic interface for the repository for caching in-memory data from a database, inherited from IRepository
#### Common
* IObject - Generic interface for an object stored in the repository
* IDbObject - Generic interface for an object stored in the database repository, inherited IObject
* ICriteria - Main interface for describing conditions for selecting objects in the database

## Enums

* ColumnType - Enums of the available data types for defining the mapping of object properties to database columns

## Implementations

#### IRepository/IAsyncRepository (and inherited interfaces) implementations

* MemoryContext - ... TODO
* DbContext - ... TODO
* CachedDbContext - ... TODO

#### IObject & IDbObject implementations

The description of the implementations for the interfaces you want to place classes stored in the entity data model

#### ICriteria implimentations

* EmptyCriteria - Implimentation is empty criteria. The SQL WHERE-expression inserted expression "1 = 1". Simple access is performed as follows: var criteria = ICriteria.Empty
* OrCriteria - ... TODO
* AndCriteria - ... TODO
* EqCriteria - ... TODO
* InCriteria - ... TODO
* LikeCriteria - ... TODO
* NotLikeCriteria - ... TODO

## Other services

* DbWithoutObjectContext - Service for getting data from a database table of unknown structure

