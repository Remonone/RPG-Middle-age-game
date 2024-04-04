import {database} from "../server.js";
import bcrypt from "bcrypt";
import jwt from 'jsonwebtoken';
import {CRYPT_SALT, ENCRYPTION_KEY} from "../utils/secret.data.js";

export const fetchUser = async (req, res) => {
    const login = req.query['login'];
    const password = req.query['password'];

    const user = await database.collection('users').findOne({_id: login});
    if(!!user) {
        if(bcrypt.compareSync(password, user.password)){
            const tokenToConvert = {username: user.username, login: user._id};
            console.log(tokenToConvert);
            const token = jwt.sign(tokenToConvert, ENCRYPTION_KEY, { algorithm: 'HS256'});
            const serverCredentials = await database.collection('servers').findOne({serverName: user.server});
            res.status(200).send({token, ip: serverCredentials.ip, port: serverCredentials.port});
        } else {
            res.status(403).send({message: "Password is incorrect"});
        }
    } else {
        res.status(404).send({message: "User was not found"});
    }
}

export const registerUser = async (req, res) => {
    const login = req.body['login'];
    const username = req.body['username'];
    const password = req.body['password'];
    const server = req.body['server_name'];

    const existingLogin = await database.collection('users').findOne({login});
    if(!!existingLogin){
        res.status(400).send({error_message: "This user is already existing!"});
        return;
    }
    const existingUsername = await database.collection('users').findOne({username});
    if(!!existingUsername){
        res.status(400).send({error_message: "This username is already used."});
        return;
    }
    const serverCredentials = await database.collection('servers').findOne({serverName: server});
    const hashedPassword = bcrypt.hashSync(password, parseInt(CRYPT_SALT));
    await database.collection('users').insertOne({_id: login, username, password: hashedPassword, server});
    const token = jwt.sign({username: username, login: login}, ENCRYPTION_KEY, { algorithm: 'HS256'});
    res.status(200).send({token: token, ip: serverCredentials.ip, port: serverCredentials.port});
}

export const saveUser = async (req, res) => {
    const username = req.body.username;
    const userToken = req.query["token"];
    const userData = jwt.verify(userToken, ENCRYPTION_KEY);
    const user = await database.collection('users').findOne({_id: userData.login});
    console.log(req.body);
    if(user.username !== username) { 
        res.status(403).send({message: "Wrong credentials!"});
        return;
    }
    await database.collection('users').updateOne({username}, {
        $set: {
            content: req.body.content,
        }
    });
    res.status(200).send({message: "Successful"});
}

export const loadUser = async (req, res) => {
    const token = req.query['jwt'];
    try {
        const userData = jwt.verify(token, ENCRYPTION_KEY);
        console.log(userData);
        const user = await database.collection('users').findOne({_id: userData.login});
        console.log("Found user: ", user);
        res.status(200).send(user);
    } catch(e) {
        console.log(e);
        res.status(403).send("Invalid user credentials.");
    }
}