import {createErrorMessage} from "../utils/error-convertor.js";
import {handleData} from "../services/content.services.js";

export const handlePlayerContent = (req, res) => {
    const query = req.body;
    const player_id = query.player_id;
    const session_id = query.session_id;
    const content = query.content;

    if(player_id === undefined) {
        res.status(400).send(createErrorMessage("Player Id was not provided"));
        return;
    }
    if(session_id === undefined) {
        res.status(400).send(createErrorMessage("Session Id was not provided"));
        return;
    }
    handleData(player_id, session_id, content);
    res.status(201).send();
}