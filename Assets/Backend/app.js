import express from 'express';
import authRouter from "./routers/auth.router.js";
import lobbyRouter from "./routers/lobby.router.js";
import sessionRouter from "./routers/session.router.js";
import contentRouter from "./routers/content.router.js";
import requestIp from 'request-ip';

const app = express();

app.set('port', 8000);

app.set('trust proxy', true);
app.use(requestIp.mw());
app.use(express.json());
app.use('/api/v1/user', authRouter);
app.use('/api/v1/lobby', lobbyRouter);
app.use('/api/v1/session', sessionRouter);
app.use('/api/v1/playerContent', contentRouter);

export default app;