# README

#### 1. HOW TO BUILD THE CODE:

#### Compile and get nuget packages:
REBUILD SOLUTION IN VS ( Nuget packages should restore on build but if not click on RESTORE NUGET PACKAGES) 

#### Set the AppId and AppKey on the secrets:
GO TO CONSOLE APP PROJECT FOLDER IN Console or Powershell and run the following 2 commands:
dotnet user-secrets set TFLAccountDetails:AppId [YOUR_APPID]
dotnet user-secrets set TFLAccountDetails:AppKey [YOUR_APPKEY]

#### 2.HOW TO RUN THE APP/OUTPUT:

##### PUBLISH APP
Go to Console app project folder in console or Powershell and run the command: dotnet publish -c Release -r win10-x64

##### RUN APP
Then from the console app project folder, go to folder: \bin\Release\netcoreapp2.1\win10-x64 and run project by typing:  .\RoadStatusChecker.exe [RoadId]


#### 3.HOW TO RUN THE TESTS:

Go to Test explorer or Test menu and select Run all tests


#### 4. How I structured my project: 

##### Console App

I created a class that is used to return the system return code and that print data to the console. This is so is easier to test as we can inject the service into it.
The console app program just do DI configuration and call this printer class.


##### Service project

First I tested the api a bit with POSTMAN and used the VS functionality to paste Json objects as classes to get my API classes (RoadStatus and ApiErrorResponse). 
I did a decorator pattern to have an interface around httpclient so I could mock it on tests
Created main service


##### Test project

I set up a set of Unit tests for the restaurant service using Nunit and Moq. 
 Once the main service was tested, I added an integration test to test the real connection with the api. 
 finally I added one test to check the output from the console app and the status code returned.
 
 
#### 5. OTHER:

I think the only thing I missed to test was that the console app is calling RoadStatusPrinter PrintRoadStatusResult method once.
Decided to use .net core for the project 
Decided to use Microsoft.Extensions.DependencyInjection as well as other extensions options as seem to be best way to pass secrets 
On a production project I would have add other things like logging, better error handling, etc