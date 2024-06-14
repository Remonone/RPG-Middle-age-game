import {database} from "../server.js";

export const handleData = async (playerId, sessionId, content) => {
    const filter = {player_id: playerId, session_id: sessionId};
    await database.collection('player_data').updateOne(filter, {
        $set: {
            player_id: playerId,
            session_id: sessionId,
            content: content,
        },
    }, {upsert: true});
    return await database.collection('player_data').findOne(filter);
}