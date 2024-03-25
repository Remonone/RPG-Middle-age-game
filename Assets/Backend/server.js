import {MongoClient} from "mongodb";
import app from "./app.js";
import {MONGO_DB_URL} from "./utils/secret.data.js";

const client = new MongoClient(MONGO_DB_URL);

const database = startServer();

function startServer() {
    try {
        const db = client.db("GameplayDB");
        app.listen(app.get('port'), () => {
            console.log("Server started successfully...");
        });
        return db;
    } catch(e) {
        console.log(e);
        return undefined;
    }
}

export {database};