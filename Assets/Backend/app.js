import express from 'express';
import authRouter from "./routers/auth.router";

const app = express();

app.set('port', 8000);

app.use(express.json());
app.use('/api/v1/user', authRouter);


export default app;