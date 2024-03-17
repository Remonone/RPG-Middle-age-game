import {MongoClient} from "mongodb";
import app from "./app";

const url = "";

const client = new MongoClient(url);

const database = startServer();

function startServer() {
    try {
        const db = client.db("admin");
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