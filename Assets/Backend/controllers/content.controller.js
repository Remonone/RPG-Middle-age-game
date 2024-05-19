import {createErrorMessage} from "../utils/error-convertor.js";
import {handleData} from "../services/content.services.js";
import {isNullOrEmpty} from "../utils/string.utils.js";

export const handlePlayerContent = (req, res) => {
    const query = req.body;
    const player_id = query.player_id;
    const session_id = query.session_id;
    const content = query.content;

    if(isNullOrEmpty(player_id)) {
        res.status(400).send(createErrorMessage("Player Id was not provided"));
        return;
    }
    if(isNullOrEmpty(session_id)) {
        res.status(400).send(createErrorMessage("Session Id was not provided"));
        return;
    }
    handleData(player_id, session_id, content);
    res.status(201).send();
}

export const getPlayerContent = async (req, res) => {
    const {playerId, sessionId} = req.query;
    if(isNullOrEmpty(playerId)) {
        res.status(400).send(createErrorMessage("Player ID was not provided."));
        return;
    }
    if(isNullOrEmpty(sessionId)) {
        res.status(400).send(createErrorMessage("Session ID was not provided"));
        return;
    }
    const data = await database.collection('content').findOne({player_id: playerId, session_id: sessionId});
    if(data === undefined) {
        res.status(404).send(createErrorMessage("Client is not existing in that session"));
    }
    res.status(200).send({content: data.content});
}