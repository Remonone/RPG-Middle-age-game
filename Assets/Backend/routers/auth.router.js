import {Router} from 'express';
import {
    fetchUser,
    registerUser,
    saveUser,
    loadUser,
} from "../controllers/auth.controller.js";

const router = Router();

router.get('/get', fetchUser);
router.post('/register', registerUser);
router.put('/save', saveUser);
router.get('/load', loadUser);

export default router;