import {Router} from 'express';
import {
    fetchUser
} from "../controllers/auth.controller";

const router = Router();

router.get('get_user', fetchUser);

export default router;