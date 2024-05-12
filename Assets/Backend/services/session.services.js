import {DEFAULT_SCENE} from "../utils/secret.data.js";
import {database} from "../server.js";
import {generateID} from "../utils/helper.js";

export const generateNewSession = async (hostId) => {
    const session = {
        _id: generateID(24),
        host_id: hostId,
        session_map: DEFAULT_SCENE,
        level: 1,
    };
    await database.collection('session').insertOne({...session});
    return session;
}
