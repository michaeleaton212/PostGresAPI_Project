// local-i18n-server.mjs
// Einfacher Express-Server, der die gebaute Angular-App mit zwei Locales
// unter "/" (en-US) und "/de/" (de-DE) bereitstellt – inkl. SPA-Fallbacks.

import express from 'express';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// Pfad zum Angular-Build
const dist = path.join(__dirname, 'dist', 'my-app', 'browser');

// Statische Auslieferung ohne automatische index.html
const staticOptions = { index: false, fallthrough: true };

// App anlegen
const app = express();

// 1) Statische Assets bereitstellen
//    - Deutsch liegt im Build unter de-DE/*, erreichbar über /de/*
//    - Englisch liegt im Build unter en-US/*, erreichbar über /*

app.use('/de', express.static(path.join(dist, 'de-DE'), staticOptions));
app.use('/',    express.static(path.join(dist, 'en-US'), staticOptions));

// 2) SPA-Fallbacks (Rewrites auf die jeweilige index.html)
//    Reihenfolge ist wichtig: erst /de/*, dann Root.

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
