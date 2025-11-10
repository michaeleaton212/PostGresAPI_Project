import express from 'express';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// Path to the angular build
const dist = path.join(__dirname, 'dist', 'my-app', 'browser');

// static delivery without index but with fallthrough
const staticOptions = { index: false, fallthrough: true };

//apply App 
const app = express();


// 1) Statische Assets bereitstellen
//  German over /de/*
//  English over en-US /*
app.use('/de', express.static(path.join(dist, 'de-DE'), staticOptions));
app.use('/',    express.static(path.join(dist, 'en-US'), staticOptions));

// 2)Rewrites on the respective index.html

app.get(/^\/de(\/.*)?$/, (_req, res) => {
  res.sendFile(path.join(dist, 'de-DE', 'index.html'));
});

app.get(/^\/(.*)?$/, (_req, res) => {
  res.sendFile(path.join(dist, 'en-US', 'index.html'));
});

// Start
const PORT = process.env.PORT || 4200;
app.listen(PORT, () => {
  console.log(`Server läuft: http://localhost:${PORT}`);
  console.log(`  en-US: http://localhost:${PORT}/`);
  console.log(`  de-DE: http://localhost:${PORT}/de/`);
});
