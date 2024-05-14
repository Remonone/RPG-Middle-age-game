import {createErrorMessage} from "../utils/error-convertor.js";
import {handleData} from "../services/content.services.js";
import {database} from "../server.js";
import {generateNewSession} from "../services/session.services.js";
import jwt from "jsonwebtoken";
import {ENCRYPTION_KEY} from "../utils/secret.data.js";

export const updateSession = async (req, res) => {
    const query = req.body;
    const session_id = query.session_id;
    const data = query.data;
    if(session_id === undefined) {
        res.status(400).send(createErrorMessage("Session Id was not provided"));
        return;
    }
    if(await database.collection('session').findOne({_id: session_id}) === undefined) {
        res.status(404).send(createErrorMessage("Session was not found"));
        return;
    }
    for(let player of data) {
            const player_id = player.id;
            const content = player.content;
            if(player_id === undefined) continue;
            await handleData(player_id, session_id, content);
    }
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