import {Router} from 'express';
import {
    createLobby,
    deleteLobby,
    updateLobby,
    fetchLobby,
    getLobbyCredentials,
} from "../controllers/lobby.controller.js";

const router = Router();

router.get('/', fetchLobby);
router.get('/receive', getLobbyCredentials);
router.get('/create', createLobby);
router.delete('/delete', deleteLobby);
router.put('/update', updateLobby);

export default router;