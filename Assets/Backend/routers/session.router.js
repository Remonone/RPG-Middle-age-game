import {Router} from 'express';
import {
    generateSession,
    updateSession,
    getSessions,
} from "../controllers/session.controller.js";

const router = Router();

router.get('/', updateSession);
router.get('/create', generateSession);
router.get('/fetch', getSessions)

export default router;