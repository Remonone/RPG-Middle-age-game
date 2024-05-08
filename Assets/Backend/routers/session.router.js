import {Router} from 'express';
import {
    updateSession,
} from "../controllers/session.controller.js";

const router = Router();

router.get('/', updateSession);

export default router;