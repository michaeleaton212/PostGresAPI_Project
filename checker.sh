#!/usr/bin/env bash
set -e

# Basic CI Security Checker (Backend + Frontend)

echo "===== CHECKER START ====="

# Backend: TestResults folder
# - What: check test output exists
# - Why: ensure dotnet tests really ran
echo "[Backend] Check TestResults..."
if [ ! -d "./TestResults" ]; then
  echo "FAIL: ./TestResults not found"
  exit 1
fi

if [ -z "$(ls -A ./TestResults 2>/dev/null)" ]; then
  echo "FAIL: ./TestResults is empty"
  exit 1
fi
echo "OK: TestResults exists and is not empty"

# Frontend path (edit if needed)
# - What: where package.json is
# - Why: only run npm checks if frontend exists
FRONTEND_DIR="Frontend/frontend/my-app"


# Frontend: detect frontend
# - What: check package.json exists
# - Why: skip npm checks if no frontend
if [ ! -f "${FRONTEND_DIR}/package.json" ]; then
  echo "[Frontend] No package.json -> skip frontend checks"
  echo "===== CHECKER SUCCESS ====="
  exit 0
fi

echo "[Frontend] Frontend found at: ${FRONTEND_DIR}"

# Frontend: lockfile required
# - What: package-lock.json must exist
# - Why: pinned deps / reproducible installs
if [ ! -f "${FRONTEND_DIR}/package-lock.json" ]; then
  echo "FAIL: package-lock.json missing"
  exit 1
fi
echo "OK: package-lock.json exists"

# Frontend: npm install forbidden in workflows
# - What: search workflows for 'npm install'
# - Why: enforce 'npm ci' in CI
if ls .github/workflows/*.yml >/dev/null 2>&1; then
  if grep -RIn --exclude-dir=.git "npm install" .github/workflows >/dev/null 2>&1; then
    echo "FAIL: 'npm install' found in workflow files (use 'npm ci')"
    grep -RIn --exclude-dir=.git "npm install" .github/workflows || true
    exit 1
  fi
fi
echo "OK: no 'npm install' in workflows"

# Frontend: security scan (npm audit)
# - What: run clean install + audit
# - Why: detect known vulnerable packages
if ! command -v npm >/dev/null 2>&1; then
  echo "FAIL: npm not available in runner (add actions/setup-node)"
  exit 1
fi

echo "[Frontend] Run npm ci + npm audit..."
cd "$FRONTEND_DIR"
npm ci --ignore-scripts
npm audit --audit-level=high
cd - >/dev/null
echo "OK: npm audit passed"

echo "===== CHECKER SUCCESS ====="
