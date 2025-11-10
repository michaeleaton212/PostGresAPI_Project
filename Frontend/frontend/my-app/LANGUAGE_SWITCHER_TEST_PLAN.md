# Language Switcher Manual Test Plan

## Setup
1. Starten Sie den Server: `node local-i18n-server.mjs`
2. Öffnen Sie die Browser Developer Tools (F12) → Console Tab

## Test Cases

### Test 1: English to German Switch
**Steps:**
1. Navigate to `http://localhost:4200/`
2. Verify the page displays in English
3. Verify the "English" button is active (highlighted/disabled)
4. Click on the "Deutsch" button

**Expected Result:**
- URL changes to `http://localhost:4200/de`
- Page reloads with German content
- "Deutsch" button is now active (highlighted/disabled)
- "English" button is now clickable

**Status:** ☐ Pass ☐ Fail

---

### Test 2: German to English Switch
**Steps:**
1. Navigate to `http://localhost:4200/de`
2. Verify the page displays in German
3. Verify the "Deutsch" button is active (highlighted/disabled)
4. Click on the "English" button

**Expected Result:**
- URL changes to `http://localhost:4200/`
- Page reloads with English content
- "English" button is now active (highlighted/disabled)
- "Deutsch" button is now clickable

**Status:** ☐ Pass ☐ Fail

---

### Test 3: Deep Link - English
**Steps:**
1. Navigate to `http://localhost:4200/rooms`
2. Verify the page displays in English
3. Verify the "English" button is active
4. Click on the "Deutsch" button

**Expected Result:**
- URL changes to `http://localhost:4200/de/rooms`
- Page shows same content in German
- "Deutsch" button is now active

**Status:** ☐ Pass ☐ Fail

---

### Test 4: Deep Link - German
**Steps:**
1. Navigate to `http://localhost:4200/de/rooms`
2. Verify the page displays in German
3. Verify the "Deutsch" button is active
4. Click on the "English" button

**Expected Result:**
- URL changes to `http://localhost:4200/rooms`
- Page shows same content in English
- "English" button is now active

**Status:** ☐ Pass ☐ Fail

---

### Test 5: Query Parameters Preserved - German to English
**Steps:**
1. Navigate to `http://localhost:4200/de/rooms?id=123`
2. Click on the "English" button

**Expected Result:**
- URL changes to `http://localhost:4200/rooms?id=123`
- Query parameter `?id=123` is preserved

**Status:** ☐ Pass ☐ Fail

---

### Test 6: Query Parameters Preserved - English to German
**Steps:**
1. Navigate to `http://localhost:4200/rooms?id=456`
2. Click on the "Deutsch" button

**Expected Result:**
- URL changes to `http://localhost:4200/de/rooms?id=456`
- Query parameter `?id=456` is preserved

**Status:** ☐ Pass ☐ Fail

---

### Test 7: Double Click Prevention
**Steps:**
1. Navigate to `http://localhost:4200/`
2. Click on the "English" button (already active)

**Expected Result:**
- Nothing happens (button is disabled)
- URL remains `http://localhost:4200/`

**Status:** ☐ Pass ☐ Fail

---

### Test 8: Accessibility
**Steps:**
1. Navigate to `http://localhost:4200/`
2. Use Tab key to navigate to language buttons
3. Check `aria-pressed` attribute in Developer Tools

**Expected Result:**
- Both buttons are keyboard accessible
- Active button has `aria-pressed="true"`
- Inactive button has `aria-pressed="false"`

**Status:** ☐ Pass ☐ Fail

---

## Browser Console Checks

After each navigation, check the console for expected log messages that show:
- Current locale detected
- isGerman state
- Path conversion (from → to)

## Notes
- All tests should be performed in Chrome/Edge
- Clear browser cache if unexpected behavior occurs
- Check Network tab for any failed requests
