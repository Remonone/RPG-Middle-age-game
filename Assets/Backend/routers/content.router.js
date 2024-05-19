import {Router} from 'express';
import {
    getPlayerContent,
    handlePlayerContent,
} from "../controllers/content.controller.js";

const router = Router();

router.post('/update', handlePlayerContent);
router.get('/', getPlayerContent);

export default router;