import {createErrorMessage} from "../utils/error-convertor.js";
import {database} from "../server.js";
import {generateNewSession} from "../services/session.services.js";
import jwt from "jsonwebtoken";
import {ENCRYPTION_KEY} from "../utils/secret.data.js";

export const updateSession = async (req, res) => {
    const token = req.query.token
    const session_id = req.query.session;
    const data = req.body;
    let userData;
    if(token === undefined) { 
        res.status(400).send(createErrorMessage("User token is empty"));
        return;
    }
    if(session_id === undefined) {
        res.status(400).send(createErrorMessage("Session Id was not provided"));
        return;
    }
    try {
        userData = jwt.verify(token, ENCRYPTION_KEY);
    } catch(e) {
        console.log(e);
        res.status(403).send(createErrorMessage(e));
        return;
    }
    if(await database.collection('session').findOne({_id: session_id, host_id: userData.login}) === undefined) {
        res.status(404).send(createErrorMessage("Session was not found"));
        return;
    }
    
    await database.collection("session").updateOne({_id: session_id, host_id: userData.login}, {
        $set: {
            session_map: data.map,
            level: data.level,
        }
    }, {upsert: true});
    res.status(200).send();
}

export const generateSession = async (req, res) => {
    const { token, name } = req.query;

    const userData = jwt.verify(token, ENCRYPTION_KEY);

    const session = await generateNewSession(userData.login, name);
    console.log("New world were created: " + session);

    res.status(200).send(session);
}

export const getSessions = async (req, res) => {
    const { token } = req.query;
    let userData;
    try {
        userData = jwt.verify(token, ENCRYPTION_KEY);
    } catch(e) {
        console.log(e);
        res.status(403).send(createErrorMessage(e));
        return;
    }
    
    const sessions = await database.collection('session').find({host_id: userData.login}).toArray();
    console.log(sessions);
    res.status(200).send({sessions});
}