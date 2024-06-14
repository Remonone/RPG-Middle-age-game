import {Router} from 'express';
import {
    fetchUser,
    registerUser,
    saveUser,
} from "../controllers/auth.controller.js";

const router = Router();

router.get('/get', fetchUser);
router.post('/register', registerUser);
router.put('/save', saveUser);

export default router;