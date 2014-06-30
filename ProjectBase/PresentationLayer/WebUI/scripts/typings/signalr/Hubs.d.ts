/// <reference path="..\jquery\jquery.d.ts" />

////////////////////
// available hubs //
////////////////////
//#region available hubs

interface SignalR {

    /**
      * The hub implemented by PB.Hubs.ChatHub
      */
    chatHub : ChatHub;
}
//#endregion available hubs

///////////////////////
// Service Contracts //
///////////////////////
//#region service contracts

//#region ChatHub hub

interface ChatHub {
    
    /**
      * This property lets you send messages to the ChatHub hub.
      */
    server : ChatHubServer;

    /**
      * The functions on this property should be replaced if you want to receive messages from the ChatHub hub.
      */
    client : ChatHubClient;
}

interface ChatHubServer {

    /** 
      * Sends a "joinGroup" message to the ChatHub hub.
      * Contract Documentation: ---
      * @param args {DCJoinGroupArgs} 
      * @return {JQueryPromise of void}
      */
    joinGroup(args : DCJoinGroupArgs) : JQueryPromise<void>;

    /** 
      * Sends a "broadcastToGroup" message to the ChatHub hub.
      * Contract Documentation: ---
      * @param args {DCBroadcastToGroupArgs} 
      * @return {JQueryPromise of void}
      */
    broadcastToGroup(args : DCBroadcastToGroupArgs) : JQueryPromise<void>;

    /** 
      * Sends a "leaveGroup" message to the ChatHub hub.
      * Contract Documentation: ---
      * @return {JQueryPromise of void}
      */
    leaveGroup() : JQueryPromise<void>;
}

interface ChatHubClient
{

    /**
      * Set this function with a "function(msg : DCMessage){}" to receive the "addMessage" message from the ChatHub hub.
      * Contract Documentation: ---
      * @param msg {DCMessage} 
      * @return {void}
      */
    addMessage : (msg : DCMessage) => void;

    /**
      * Set this function with a "function(msg : DCGroup){}" to receive the "addGroup" message from the ChatHub hub.
      * Contract Documentation: ---
      * @param msg {DCGroup} 
      * @return {void}
      */
    addGroup : (msg : DCGroup) => void;

    /**
      * Set this function with a "function(msg : DCGroup[]){}" to receive the "addGroups" message from the ChatHub hub.
      * Contract Documentation: ---
      * @param msg {DCGroup[]} 
      * @return {void}
      */
    addGroups : (msg : DCGroup[]) => void;
}

//#endregion ChatHub hub

//#endregion service contracts



////////////////////
// Data Contracts //
////////////////////
//#region data contracts


/**
  * Data contract for PB.Services.DataContracts.DCGroup
  */
interface DCGroup {
    Name : string;
    Count : number;
}


/**
  * Data contract for PB.Services.DataContracts.DCMessage
  */
interface DCMessage {
    Name : string;
    Message : string;
}


/**
  * Data contract for PB.Services.DataContracts.DCBroadcastToGroupArgs
  */
interface DCBroadcastToGroupArgs {
    Group : string;
    Message : string;
}


/**
  * Data contract for PB.Services.DataContracts.DCJoinGroupArgs
  */
interface DCJoinGroupArgs {
    Group : string;
}

//#endregion data contracts

