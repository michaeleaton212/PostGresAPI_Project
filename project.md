# Hotel Adula Projekt

**Autor:** Michael Eaton

## Beschreibung
Dieses Repository beinhaltet sowohl den Backend-Dienst als auch die Frontend-Anwendung für das **Hotel Adula Management System**.
Das Backend basiert auf **.NET** und stellt die notwendigen APIs zur Verfügung, während das Frontend mit **Angular** entwickelt wurde.

---

# Beispiel für eine kopierbare Box

Hier ist ein Text, den du kopieren kannst:

<div style="border: 1px solid #ccc; padding: 10px; margin: 10px 0; background-color: #f9f9f9; border-radius: 4px;">
  <p id="copyText">Dies ist der Text, der kopiert werden soll.</p>
  <button onclick="copyToClipboard()">Kopieren</button>
</div>

<script>
  function copyToClipboard() {
    const textToCopy = document.getElementById("copyText").innerText;
    navigator.clipboard.writeText(textToCopy)
      .then(() => {
        alert("Text wurde kopiert!");
      })
      .catch(err => {
        console.error("Fehler beim Kopieren: ", err);
      });
  }
</script>
