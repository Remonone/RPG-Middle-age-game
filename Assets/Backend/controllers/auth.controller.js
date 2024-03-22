import {database} from "../server";
import bcrypt from "bcrypt";

export const fetchUser = async (req, res) => {
    const login = req.query['login'];
    const password = req.query['password'];

    const user = await database.collection('users').findOne({login});
    if(!!user) {
        if(bcrypt.compareSync(password, user.password)){
            res.status(200).send(user);
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
    const hashedPassword = bcrypt.hashSync(password);
    await database.collection('users').insertOne({_id: login, username, password: hashedPassword});
    res.status(200).send({result: "Success"});
}