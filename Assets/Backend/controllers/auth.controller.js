import {database} from "../server.js";
import bcrypt from "bcrypt";
import jwt from 'jsonwebtoken';
import {CRYPT_SALT, ENCRYPTION_KEY} from "../utils/secret.data.js";
import {createErrorMessage, createErrorMessageWithType} from "../utils/error-convertor.js";

export const fetchUser = async (req, res) => {
    const login = req.query['login'];
    const password = req.query['password'];
    let ip = req.ip;
    if (ip.slice(0, 7) === "::ffff:") {
        ip = ip.slice(7)
    }
    if(ip === "::1") {
        ip = "127.0.0.1";
    }
    const user = await database.collection('users').findOne({_id: login});
    if(!!user) {
        if(bcrypt.compareSync(password, user.password)){
            const tokenToConvert = {username: user.username, login: user._id};
            const token = jwt.sign(tokenToConvert, ENCRYPTION_KEY, { algorithm: 'HS256'});
            res.status(200).send({token, user, ip, port: 7000});
        } else {
            res.status(403).send(createErrorMessageWithType("Password is incorrect", "Password"));
        }
    } else {
        res.status(404).send({type: "Login", message: "User was not found"});
    }
}

export const registerUser = async (req, res) => {
    const login = req.body['login'];
    const username = req.body['username'];
    const password = req.body['password'];

    const existingLogin = await database.collection('users').findOne({login});
    if(!!existingLogin){
        res.status(400).send(createErrorMessageWithType("This user is already existing!", "Login"));
        return;
    }
    const existingUsername = await database.collection('users').findOne({username});
    if(!!existingUsername){
        res.status(400).send(createErrorMessageWithType("Username", "This username is already used."));
        return;
    }
    let ip = req.ip;
    if (ip.slice(0, 7) === "::ffff:") ip = ip.slice(7)
    if(ip === "::1") ip = "127.0.0.1";
    
    const hashedPassword = bcrypt.hashSync(password, parseInt(CRYPT_SALT));
    const newUser = {_id: login, username, password: hashedPassword};
    await database.collection('users').insertOne({...newUser});
    const token = jwt.sign({username: username, login: login}, ENCRYPTION_KEY, { algorithm: 'HS256'});
    res.status(200).send({token, user: newUser, ip, port: 7000});
}

export const saveUser = async (req, res) => {
    const username = req.body.username;
    const userToken = req.query["token"];
    const userData = jwt.verify(userToken, ENCRYPTION_KEY);
    const session = await database.collection('session').findOne({host_id: userData.login, _id: req.query["session"]});
    if(!session) { 
        res.status(403).send(createErrorMessage("Wrong credentials!"));
        return;
    }
    await database.collection('player_data').updateOne({username}, {
        $set: {
            session_id: req.query["session"],
            username,
            content: req.body.content,
        }
    }, {upsert: true});
    res.status(200).send({message: "Successful"});
}