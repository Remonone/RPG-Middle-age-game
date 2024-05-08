import {Router} from 'express';
import {
    handlePlayerContent,
} from "../controllers/content.controller.js";

const router = Router();

router.get('/', handlePlayerContent);

export default router;