# MarketFellow

(Work in Progress - Not Finished)

A proxy for gather trade values from multiple sources.

Projects:


## Web Api ######################################

A .Net Core application responsible for gathering trading information from multiple sources. 
The sources definition is registered at appsettings.json, making the project agnostic of them.

The communication with the sources is being made through HttpClient for the trades metadata (eg. trade products) 
and with ClientWebSocket for gathering trades live. 
The trades information gets propagated to the Api live feed listeners.
Additionally, information is meanted to be stored in a mongoDB, as it is received.

The Api communicates with the outside world using GraphQL Api middleware.

## Web Client ######################################

An angular 7 application responsible for consuming the Web Api and display trades.
Communication with the GraphQL Api of the server is being made using Apollo library.

The UI is written with the help of material design library.

The app gives users the ability to pool the API for sources using HTTP requests.
Based on their criteria, they can either listen to a live feed using web socket communication or ask for historical data using HTTP.


## Run Instructions ##############################

Prerequisites : Latest Visual Studio or Visual Studio code, node.js (npm) and an internet connection :)

Web Api: Simply open the solution and run using IIS Express.
 
Web Client: With a command prompt navigate to MarketFellowApi/ClientApp and run: "npm install" and then "ng serve"
 
 ##### Open a browser at http://localhost:4200/home
