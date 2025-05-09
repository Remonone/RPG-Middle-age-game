import {lobbyCredentialsFilter, lobbyFilter} from "../services/lobby.services.js";
import {database} from "../server.js";
import bcrypt from "bcrypt";
import jwt from "jsonwebtoken";
import {ENCRYPTION_KEY} from "../utils/secret.data.js";
import {createErrorMessage} from "../utils/error-convertor.js";
import {generateID} from "../utils/helper.js";


export const fetchLobby = async(req, res) => {
    const rawLobbies = await database.collection('lobbies').find().toArray();
    const lobbies = rawLobbies.map(lobby => {
        return lobbyFilter(lobby);
    });
    res.status(200).send({lobbies});
}

export const getLobbyCredentials = async(req, res) => {
    console.log("Joining the lobby: " + req.query["id"]);
    const lobbyId = req.query["id"];
    const password = req.query["password"];
    const lobby = await database.collection('lobbies').findOne({_id: lobbyId});
    if(!lobby) {
        res.status(404).send(createErrorMessage("Lobby was not found..."));
        return;
    }
    if(lobby.password !== undefined) {
        if(!bcrypt.compareSync(password, lobby.password)) {
            res.status(403).send(createErrorMessage("Incorrect password"));
            return;
        }
    }
    res.status(200).send(lobbyCredentialsFilter(lobby));
}

export const createLobby = async(req, res) => {
    const { token, roomName, roomPassword, sessionId, ip, port } = req.body;
    let userData;
    if(roomName === undefined || roomName === '' || sessionId === '' || sessionId === undefined) {
        res.status(400).send(createErrorMessage("Room name was not provided"));
        return;
    }
    try {
        userData = jwt.verify(token, ENCRYPTION_KEY);
    } catch(e) {
        console.log(e);
        res.status(403).send(createErrorMessage(e));
        return;
    }
    const user = await database.collection('users').findOne({_id: userData.login});
    if(user === null) {
        console.log("Issue occurred with " + userData.login);
        res.status(404).send(createErrorMessage("User was not found."));
        return;
    }
    console.log(sessionId + " " + userData.login);
    let session = await database.collection('session').findOne({_id: sessionId, host_id: userData.login});

    if(session === null) {
        console.log("Session was not found. Aborting..");
        res.status(404).send(createErrorMessage("Session was not found."));
        return;
    }
    const room_id = generateID(9);
    const lobby = {
        _id: room_id,
        room_name: roomName,
        room_secured: roomPassword !== "",
        room_password: roomPassword || '',
        room_map: session.session_map,
        room_level: session.level,
        room_players: 1,
        room_host: user._id,
    };
    const lobbyToInsert = {
        ...lobby,
        server_ip: ip,
        server_port: port,
    }
    console.log({...lobbyToInsert});
    await database.collection('lobbies').insertOne({...lobbyToInsert});
    res.status(201).send({...lobby, session_id: session._id});
}

export const deleteLobby = async(req, res) => {
    const { token, roomId } = req.query;
    let userData;
    try {
        userData = jwt.verify(token, ENCRYPTION_KEY);
    } catch(e) {
        console.log(e);
        res.status(403).send(createErrorMessage(e));
        return;
    }
    const lobby = await database.collection('lobbies').findOne({_id: roomId});
    if(lobby === undefined) {
        res.status(404).send(createErrorMessage("Lobby was not found..."));
        return;
    }
    if(lobby.host !== userData.host) {
        res.status(403).send(createErrorMessage("Only host can delete the lobby!"))
    }
    console.log("Lobby to delete: " + lobby);
    await database.collection('lobbies').deleteOne(lobby);
    res.status(204).send();
}

export const updateLobby = async(req, res) => {
    const content = req.body;
    const { token, room_id } = req.query;
    let userData;
    try {
        userData = jwt.verify(token, ENCRYPTION_KEY);
    } catch(e) {
        res.status(403).send(createErrorMessage(e));
        return;
    }
    const lobby = await database.collection('lobbies').findOne({_id: room_id});
    if(lobby === undefined) {
        res.status(404).send(createErrorMessage("Lobby was not found..."));
        return;
    }
    if(lobby.host !== userData.host) {
        res.status(403).send(createErrorMessage("Only host can delete the lobby!"))
    }
    await database.collection('lobbies').updateOne({_id: room_id}, {
        $set: {...content}
    });

}