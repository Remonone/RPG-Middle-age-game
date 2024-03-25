import {database} from "../server.js";
import bcrypt from "bcrypt";
import jwt from 'jsonwebtoken';
import {CRYPT_SALT, ENCRYPTION_KEY} from "../utils/secret.data.js";

export const fetchUser = async (req, res) => {
    const login = req.query['login'];
    const password = req.query['password'];

    const user = await database.collection('users').findOne({login});
    if(!!user) {
        if(bcrypt.compareSync(password, user.password)){
            const token = jwt.sign({username: user.player_id, login: user.login}, ENCRYPTION_KEY, { algorithm: 'RS256'});
            res.status(200).send(token);
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
    const hashedPassword = bcrypt.hashSync(password, parseInt(CRYPT_SALT));
    await database.collection('users').insertOne({_id: login, username, password: hashedPassword});
    const token = jwt.sign({username: username, login: login}, ENCRYPTION_KEY, { algorithm: 'RS256'});
    res.status(200).send(token);
}

export const saveUser = async (req, res) => {

}

export const loadUser = async (req, res) => {
    const token = req.body['jwt'];
    try {
        const userData = jwt.verify(token, ENCRYPTION_KEY);
        const user = await database.collection('users').findOne({_id: userData.player_id});
        res.status(200).send(user);
    } catch(e) {
        res.status(403).send("Invalid user credentials.");
    }
}