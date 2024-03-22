import {Router} from 'express';
import {
    fetchUser
} from "../controllers/auth.controller";

const router = Router();

router.get('get', fetchUser);
router.get('register', fetchUser);

export default router;