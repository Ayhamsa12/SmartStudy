"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(() => {
    console.log("Connection started!");
}).catch(err => console.error(err));



console.log("JavaScript is working!");
