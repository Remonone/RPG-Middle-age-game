import {database} from "../server";
import bcrypt from "bcrypt";

export const fetchUser = async (req, res, next) => {
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