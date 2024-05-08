import {createErrorMessage} from "../utils/error-convertor.js";
import {handleData} from "../services/content.services.js";
import {database} from "../server.js";

export const updateSession = async (req, res) => {
    const query = req.body;
    const session_id = query.session_id;
    const data = query.data;
    if(session_id === undefined) {
        res.status(400).send(createErrorMessage("Session Id was not provided"));
        return;
    }
    if(await database.collection('sessions').findOne({_id: session_id}) === undefined) {
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