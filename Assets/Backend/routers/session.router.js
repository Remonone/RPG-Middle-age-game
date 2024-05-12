import {Router} from 'express';
import {
    generateSession,
    updateSession,
} from "../controllers/session.controller.js";

const router = Router();

router.get('/', updateSession);
router.get('/create', generateSession);

export default router;